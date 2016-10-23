// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutliningTagger.cs" company="Michael Reukauff, Germany">
//   Copyright © 2016 Michael Reukauff, Germany. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace
namespace MichaelReukauff.Protobuf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;

    /// <summary>
    /// The outlining tagger.
    /// </summary>
    internal sealed class OutliningTagger : ITagger<IOutliningRegionTag>
    {
        /// <summary>
        /// The characters that start the outlining region.
        /// </summary>
        private const string StartHide = "{";

        /// <summary>
        /// The characters that end the outlining region.
        /// </summary>
        private const string EndHide = "}";

        /// <summary>
        /// The characters that are displayed when the region is collapsed.
        /// </summary>
        private const string Ellipsis = "...";

        /// <summary>
        /// The index.
        /// </summary>
        private int ix;

        /// <summary>
        /// The contents of the tooltip for the collapsed span.
        /// </summary>
        private string hoverText = string.Empty;

        /// <summary>
        /// The _buffer.
        /// </summary>
        private readonly ITextBuffer buffer;

        /// <summary>
        /// The _snapshot.
        /// </summary>
        private ITextSnapshot snapshot;

        /// <summary>
        /// The _regions.
        /// </summary>
        private List<Region> regions;

        /// <summary>
        /// The tags changed.
        /// </summary>
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutliningTagger"/> class.
        /// The tagger constructor that initializes the fields, parses the buffer, and adds an event handler to the Changed event.
        /// </summary>
        /// <param name="buffer">
        /// The TextBuffer.
        /// </param>
        public OutliningTagger(ITextBuffer buffer)
        {
            this.buffer = buffer;
            snapshot = buffer.CurrentSnapshot;
            regions = new List<Region>();
            ReParse();
            this.buffer.Changed += BufferChanged;
        }

        /// <summary>
        /// Implement the GetTags method, which instantiates the tag spans.
        /// This assumes that the spans in the NormalizedSpanCollection passed in to the method are contiguous,
        /// although this may not always be the case. This method instantiates a new tag span for each of the outlining regions.
        /// </summary>
        /// <param name="spans">The spans.</param>
        /// <returns>A list of outlining region tags.</returns>
        public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)
            {
                yield break;
            }

            var currentRegions = regions;
            var currentSnapshot = snapshot;
            var entire = new SnapshotSpan(spans[0].Start, spans[spans.Count - 1].End).TranslateTo(currentSnapshot, SpanTrackingMode.EdgeExclusive);
            var startLineNumber = entire.Start.GetContainingLine().LineNumber;
            var endLineNumber = entire.End.GetContainingLine().LineNumber;

            foreach (var region in currentRegions)
            {
                if (region.StartLine <= endLineNumber && region.EndLine >= startLineNumber)
                {
                    var startLine = currentSnapshot.GetLineFromLineNumber(region.StartLine);
                    var endLine = currentSnapshot.GetLineFromLineNumber(region.EndLine);

                    var snapshotspan = new SnapshotSpan(startLine.Start + region.StartOffset, endLine.End);

                    hoverText = snapshotspan.GetText();
                    hoverText = hoverText.Substring(1, hoverText.Length - 2).Trim(new[] { '\r', '\n' });

                    // the region starts at the beginning of the "{", and goes until the *end* of the line that contains the "}".
                    yield return new TagSpan<IOutliningRegionTag>(snapshotspan, new OutliningRegionTag(false, false, Ellipsis + ix++, hoverText));
                }
            }
        }

        /// <summary>
        /// Add a BufferChanged event handler that responds to Changed events by parsing the text buffer.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TextContentChangedEventArgs"/>.</param>
        private void BufferChanged(object sender, TextContentChangedEventArgs e)
        {
            // If this isn't the most up-to-date version of the buffer, then ignore it for now (we'll eventually get another change event).
            if (e.After != buffer.CurrentSnapshot)
            {
                return;
            }

            ReParse();
        }

        /// <summary>
        /// Parse the text.
        /// </summary>
        private void ReParse()
        {
            var newSnapshot = buffer.CurrentSnapshot;
            var newRegions = new List<Region>();

            // keep the current (deepest) partial region, which will have
            // references to any parent partial regions.
            PartialRegion currentRegion = null;

            foreach (var line in newSnapshot.Lines)
            {
                int regionStart;
                var text = line.GetText();

                // lines that contain a "{" denote the start of a new region.
                if ((regionStart = text.IndexOf(StartHide, StringComparison.Ordinal)) != -1)
                {
                    var currentLevel = (currentRegion != null) ? currentRegion.Level : 1;
                    int newLevel;

                    if (!TryGetLevel(text, regionStart, out newLevel))
                    {
                        newLevel = currentLevel + 1;
                    }

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
                else
                {
                    // lines that contain "}" denote the end of a region
                    if ((regionStart = text.IndexOf(EndHide, StringComparison.Ordinal)) != -1)
                    {
                        var currentLevel = (currentRegion != null) ? currentRegion.Level : 1;
                        int closingLevel;

                        if (!TryGetLevel(text, regionStart, out closingLevel))
                        {
                            closingLevel = currentLevel;
                        }

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
            var oldSpans = new List<Span>(regions.Select(r => AsSnapshotSpan(r, snapshot)
                                                                        .TranslateTo(newSnapshot, SpanTrackingMode.EdgeExclusive)
                                                                        .Span));
            var newSpans = new List<Span>(newRegions.Select(r => AsSnapshotSpan(r, newSnapshot).Span));

            var oldSpanCollection = new NormalizedSpanCollection(oldSpans);
            var newSpanCollection = new NormalizedSpanCollection(newSpans);

            // the changed regions are regions that appear in one set or the other, but not both.
            var removed = NormalizedSpanCollection.Difference(oldSpanCollection, newSpanCollection);

            var changeStart = int.MaxValue;
            var changeEnd = -1;

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

            snapshot = newSnapshot;
            regions = newRegions;

            if (changeStart <= changeEnd)
            {
                var snap = snapshot;
                TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(snap, Span.FromBounds(changeStart, changeEnd))));
            }
        }

        /// <summary>
        /// The helper method gets an integer that represents the level of the outlining, such that 1 is the leftmost brace pair.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="level">The level.</param>
        /// <returns>True if parse was successful otherwise false.</returns>
        public static bool TryGetLevel(string text, int startIndex, out int level)
        {
            level = -1;
            if (text.Length > startIndex + 3)
            {
                if (int.TryParse(text.Substring(startIndex + 1), out level))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// The helper method translates a Region (defined later in this topic) into a SnapshotSpan.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="snapshot">The snapshot.</param>
        /// <returns>The <see cref="SnapshotSpan"/>.</returns>
        public static SnapshotSpan AsSnapshotSpan(Region region, ITextSnapshot snapshot)
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
        public class PartialRegion
        {
            /// <summary>
            /// Gets or sets the start line.
            /// </summary>
            public int StartLine { get; set; }

            /// <summary>
            /// Gets or sets the start offset.
            /// </summary>
            public int StartOffset { get; set; }

            /// <summary>
            /// Gets or sets the level.
            /// </summary>
            public int Level { get; set; }

            /// <summary>
            /// Gets or sets the partial parent.
            /// </summary>
            public PartialRegion PartialParent { get; set; }
        }

        /// <summary>
        /// The region.
        /// </summary>
        public class Region : PartialRegion
        {
            /// <summary>
            /// Gets or sets the end line.
            /// </summary>
            public int EndLine { get; set; }
        }
    }
}
