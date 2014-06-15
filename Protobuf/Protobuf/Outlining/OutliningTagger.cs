#region Copyright © 2013 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutliningTagger.cs" company="Michael Reukauff">
//   Copyright © 2013 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace MichaelReukauff.Protobuf
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  using Microsoft.VisualStudio.Text;
  using Microsoft.VisualStudio.Text.Tagging;

  internal sealed class OutliningTagger : ITagger<IOutliningRegionTag>
  {
    /// <summary>
    /// the characters that start the outlining region
    /// </summary>
    private const string StartHide = "{";

    /// <summary>
    /// the characters that end the outlining region
    /// </summary>
    private const string EndHide = "}";

    /// <summary>
    /// the characters that are displayed when the region is collapsed
    /// </summary>
    private const string Ellipsis = "...";

    private int _ix;

    /// <summary>
    /// the contents of the tooltip for the collapsed span
    /// </summary>
    private string _hoverText = string.Empty;

    readonly ITextBuffer _buffer;
    ITextSnapshot _snapshot;
    List<Region> _regions;

    public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

    /// <summary>
    /// The tagger constructor that initializes the fields, parses the buffer, and adds an event handler to the Changed event.
    /// </summary>
    /// <param name="buffer">The TextBuffer</param>
    public OutliningTagger(ITextBuffer buffer)
    {
      _buffer = buffer;
      _snapshot = buffer.CurrentSnapshot;
      _regions = new List<Region>();
      ReParse();
      _buffer.Changed += BufferChanged;
    }

    /// <summary>
    /// Implement the GetTags method, which instantiates the tag spans.
    /// This assumes that the spans in the NormalizedSpanCollection passed in to the method are contiguous,
    /// although this may not always be the case. This method instantiates a new tag span for each of the outlining regions.
    /// </summary>
    /// <param name="spans"></param>
    /// <returns></returns>
    public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
    {
      if (spans.Count == 0)
        yield break;

      List<Region> currentRegions = _regions;
      ITextSnapshot currentSnapshot = _snapshot;
      SnapshotSpan entire = new SnapshotSpan(spans[0].Start, spans[spans.Count - 1].End).TranslateTo(currentSnapshot, SpanTrackingMode.EdgeExclusive);
      int startLineNumber = entire.Start.GetContainingLine().LineNumber;
      int endLineNumber = entire.End.GetContainingLine().LineNumber;

      foreach (var region in currentRegions)
      {
        if (region.StartLine <= endLineNumber && region.EndLine >= startLineNumber)
        {
          var startLine = currentSnapshot.GetLineFromLineNumber(region.StartLine);
          var endLine = currentSnapshot.GetLineFromLineNumber(region.EndLine);

          var snapshot = new SnapshotSpan(startLine.Start + region.StartOffset, endLine.End);

          _hoverText = snapshot.GetText();
          _hoverText = _hoverText.Substring(1, _hoverText.Length - 2).Trim(new[] { '\r', '\n' });

          //the region starts at the beginning of the "{", and goes until the *end* of the line that contains the "}".
          yield return new TagSpan<IOutliningRegionTag>(snapshot, new OutliningRegionTag(false, false, Ellipsis + _ix++, _hoverText));
        }
      }
    }

    /// <summary>
    /// Add a BufferChanged event handler that responds to Changed events by parsing the text buffer.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void BufferChanged(object sender, TextContentChangedEventArgs e)
    {
      // If this isn't the most up-to-date version of the buffer, then ignore it for now (we'll eventually get another change event). 
      if (e.After != _buffer.CurrentSnapshot)
        return;

      ReParse();
    }

    /// <summary>
    /// Parse the text
    /// </summary>
    void ReParse()
    {
      ITextSnapshot newSnapshot = _buffer.CurrentSnapshot;
      var newRegions = new List<Region>();

      // keep the current (deepest) partial region, which will have
      // references to any parent partial regions.
      PartialRegion currentRegion = null;

      foreach (var line in newSnapshot.Lines)
      {
        int regionStart;
        string text = line.GetText();

        // lines that contain a "{" denote the start of a new region.
        if ((regionStart = text.IndexOf(StartHide, StringComparison.Ordinal)) != -1)
        {
          int currentLevel = (currentRegion != null) ? currentRegion.Level : 1;
          int newLevel;

          if (!TryGetLevel(text, regionStart, out newLevel))
            newLevel = currentLevel + 1;

          // levels are the same and we have an existing region; 
          // end the current region and start the next 
          if (currentLevel == newLevel && currentRegion != null)
          {
            newRegions.Add(new Region
            {
              Level = currentRegion.Level,
              StartLine = currentRegion.StartLine,
              StartOffset = currentRegion.StartOffset,
              EndLine = line.LineNumber
            });

            currentRegion = new PartialRegion
            {
              Level = newLevel,
              StartLine = line.LineNumber,
              StartOffset = regionStart,
              PartialParent = currentRegion.PartialParent
            };
          }
          else
          {
            // this is a new (sub)region 
            currentRegion = new PartialRegion
            {
              Level = newLevel,
              StartLine = line.LineNumber,
              StartOffset = regionStart,
              PartialParent = currentRegion
            };
          }
        }

          // lines that contain "}" denote the end of a region
        else
        {
          if ((regionStart = text.IndexOf(EndHide, StringComparison.Ordinal)) != -1)
          {
            int currentLevel = (currentRegion != null) ? currentRegion.Level : 1;
            int closingLevel;

            if (!TryGetLevel(text, regionStart, out closingLevel))
              closingLevel = currentLevel;

            // the regions match
            if (currentRegion != null && currentLevel == closingLevel)
            {
              newRegions.Add(new Region
              {
                Level = currentLevel,
                StartLine = currentRegion.StartLine,
                StartOffset = currentRegion.StartOffset,
                EndLine = line.LineNumber
              });

              currentRegion = currentRegion.PartialParent;
            }
          }
        }
      }

      // determine the changed span, and send a changed event with the new spans
      var oldSpans = new List<Span>(_regions.Select(r => AsSnapshotSpan(r, _snapshot)
                                                                  .TranslateTo(newSnapshot, SpanTrackingMode.EdgeExclusive)
                                                                  .Span));
      var newSpans = new List<Span>(newRegions.Select(r => AsSnapshotSpan(r, newSnapshot).Span));

      var oldSpanCollection = new NormalizedSpanCollection(oldSpans);
      var newSpanCollection = new NormalizedSpanCollection(newSpans);

      // the changed regions are regions that appear in one set or the other, but not both.
      NormalizedSpanCollection removed = NormalizedSpanCollection.Difference(oldSpanCollection, newSpanCollection);

      int changeStart = int.MaxValue;
      int changeEnd = -1;

      if (removed.Count > 0)
      {
        changeStart = removed[0].Start;
        changeEnd = removed[removed.Count - 1].End;
      }

      if (newSpans.Count > 0)
      {
        changeStart = Math.Min(changeStart, newSpans[0].Start);
        changeEnd = Math.Max(changeEnd, newSpans[newSpans.Count - 1].End);
      }

      _snapshot = newSnapshot;
      _regions = newRegions;

      if (changeStart <= changeEnd)
      {
        ITextSnapshot snap = _snapshot;
        if (TagsChanged != null)
          TagsChanged(this, new SnapshotSpanEventArgs(new SnapshotSpan(snap, Span.FromBounds(changeStart, changeEnd))));
      }
    }

    /// <summary>
    /// The helper method gets an integer that represents the level of the outlining, such that 1 is the leftmost brace pair.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="startIndex"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    static bool TryGetLevel(string text, int startIndex, out int level)
    {
      level = -1;
      if (text.Length > startIndex + 3)
      {
        if (int.TryParse(text.Substring(startIndex + 1), out level))
          return true;
      }

      return false;
    }

    /// <summary>
    /// The helper method translates a Region (defined later in this topic) into a SnapshotSpan.
    /// </summary>
    /// <param name="region"></param>
    /// <param name="snapshot"></param>
    /// <returns></returns>
    static SnapshotSpan AsSnapshotSpan(Region region, ITextSnapshot snapshot)
    {
      var startLine = snapshot.GetLineFromLineNumber(region.StartLine);
      var endLine = (region.StartLine == region.EndLine) ? startLine : snapshot.GetLineFromLineNumber(region.EndLine);

      return new SnapshotSpan(startLine.Start + region.StartOffset, endLine.End);
    }

    /// <summary>
    /// The following code is for illustration only.
    /// It defines a PartialRegion class that contains the line number and offset of the start of an outlining region,
    /// and also a reference to the parent region (if any).
    /// This enables the parser to set up nested outlining regions.
    /// A derived Region class contains a reference to the line number of the end of an outlining region.
    /// </summary>
    class PartialRegion
    {
      public int StartLine { get; set; }
      public int StartOffset { get; set; }
      public int Level { get; set; }
      public PartialRegion PartialParent { get; set; }
    }

    class Region : PartialRegion
    {
      public int EndLine { get; set; }
    }
  }
}
