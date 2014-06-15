#region Copyright © 2013 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HighlightWordTagger.cs" company="Michael Reukauff">
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
  using Microsoft.VisualStudio.Text.Editor;
  using Microsoft.VisualStudio.Text.Operations;
  using Microsoft.VisualStudio.Text.Tagging;

  class HighlightWordTagger : ITagger<HighlightWordTag>
  {
    ITextView View { get; set; }
    ITextBuffer SourceBuffer { get; set; }
    ITextSearchService TextSearchService { get; set; }
    ITextStructureNavigator TextStructureNavigator { get; set; }
    NormalizedSnapshotSpanCollection WordSpans { get; set; }
    SnapshotSpan? CurrentWord { get; set; }
    SnapshotPoint RequestedPoint { get; set; }
    readonly object _updateLock = new object();

    public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

    /// <summary>
    /// The constructor initializes the properties listed earlier and adds LayoutChanged and PositionChanged event handlers.
    /// </summary>
    /// <param name="view"></param>
    /// <param name="sourceBuffer"></param>
    /// <param name="textSearchService"></param>
    /// <param name="textStructureNavigator"></param>
    public HighlightWordTagger(ITextView view, ITextBuffer sourceBuffer, ITextSearchService textSearchService, ITextStructureNavigator textStructureNavigator)
    {
      View = view;
      SourceBuffer = sourceBuffer;
      TextSearchService = textSearchService;
      TextStructureNavigator = textStructureNavigator;

      WordSpans = new NormalizedSnapshotSpanCollection();
      CurrentWord = null;

      View.Caret.PositionChanged += CaretPositionChanged;
      View.LayoutChanged += ViewLayoutChanged;
    }

    /// <summary>
    /// The event handlers both call the UpdateAtCaretPosition method.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void ViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
    {
      // If a new snapshot wasn't generated, then skip this layout
      if (e.NewSnapshot != e.OldSnapshot)
      {
        UpdateAtCaretPosition(View.Caret.Position);
      }
    }

    /// <summary>
    /// The event handlers both call the UpdateAtCaretPosition method.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void CaretPositionChanged(object sender, CaretPositionChangedEventArgs e)
    {
      UpdateAtCaretPosition(e.NewPosition);
    }

    /// <summary>
    /// This method finds every word in the text buffer that is identical to the word
    /// where the cursor is positioned and constructs a list of SnapshotSpan objects
    /// that correspond to the occurrences of the word. It then calls SynchronousUpdate, which raises the TagsChanged event.
    /// </summary>
    /// <param name="caretPosition"></param>
    void UpdateAtCaretPosition(CaretPosition caretPosition)
    {
      var point = caretPosition.Point.GetPoint(SourceBuffer, caretPosition.Affinity);

      if (!point.HasValue)
        return;

      // If the new caret position is still within the current word (and on the same snapshot), we don't need to check it
      if (CurrentWord.HasValue
          && CurrentWord.Value.Snapshot == View.TextSnapshot
          && point.Value >= CurrentWord.Value.Start
          && point.Value <= CurrentWord.Value.End)
      {
        return;
      }

      RequestedPoint = point.Value;

      UpdateWordAdornments();
    }

    void UpdateWordAdornments()
    {
      var currentRequest = RequestedPoint;
      var wordSpans = new List<SnapshotSpan>();

      // Find all words in the buffer like the one the caret is on
     var word = TextStructureNavigator.GetExtentOfWord(currentRequest);
      var foundWord = true;

      // If we've selected something not worth highlighting, we might have missed a "word" by a little bit
      if (!WordExtentIsValid(currentRequest, word))
      {
        // Before we retry, make sure it is worthwhile
        if (word.Span.Start != currentRequest
             || currentRequest == currentRequest.GetContainingLine().Start
             || char.IsWhiteSpace((currentRequest - 1).GetChar()))
        {
          foundWord = false;
        }
        else
        {
          // Try again, one character previous.
          // If the caret is at the end of a word, pick up the word.
          word = TextStructureNavigator.GetExtentOfWord(currentRequest - 1);

          //If the word still isn't valid, we're done 
          if (!WordExtentIsValid(currentRequest, word))
            foundWord = false;
        }
      }

      if (!foundWord)
      {
        // If we couldn't find a word, clear out the existing markers
        SynchronousUpdate(currentRequest, new NormalizedSnapshotSpanCollection(), null);
        return;
      }

      var currentWord = word.Span;
      // If this is the current word, and the caret moved within a word, we're done.
      if (CurrentWord.HasValue && currentWord == CurrentWord)
        return;

      // Find the new spans
      var findData = new FindData(currentWord.GetText(), currentWord.Snapshot)
      {
        FindOptions = FindOptions.WholeWord | FindOptions.MatchCase
      };

      wordSpans.AddRange(TextSearchService.FindAll(findData));

      // If another change hasn't happened, do a real update
      if (currentRequest == RequestedPoint)
        SynchronousUpdate(currentRequest, new NormalizedSnapshotSpanCollection(wordSpans), currentWord);
    }

    static bool WordExtentIsValid(SnapshotPoint currentRequest, TextExtent word)
    {
      return word.IsSignificant && currentRequest.Snapshot.GetText(word.Span).Any(char.IsLetter);
    }

    /// <summary>
    /// The SynchronousUpdate performs a synchronous update on the WordSpans and CurrentWord properties, and raises the TagsChanged event.
    /// </summary>
    /// <param name="currentRequest"></param>
    /// <param name="newSpans"></param>
    /// <param name="newCurrentWord"></param>
    void SynchronousUpdate(SnapshotPoint currentRequest, NormalizedSnapshotSpanCollection newSpans, SnapshotSpan? newCurrentWord)
    {
      lock (_updateLock)
      {
        if (currentRequest != RequestedPoint)
          return;

        WordSpans = newSpans;
        CurrentWord = newCurrentWord;

        var tempEvent = TagsChanged;
        if (tempEvent != null)
          tempEvent(this, new SnapshotSpanEventArgs(new SnapshotSpan(SourceBuffer.CurrentSnapshot, 0, SourceBuffer.CurrentSnapshot.Length)));
      }
    }

    /// <summary>
    /// This method takes a collection of SnapshotSpan objects and returns an enumeration of tag spans.
    /// Implement this method as a yield iterator,
    /// which enables lazy evaluation (that is, evaluation of the set only when individual items are accessed) of the tags.
    /// Here the method returns a TagSpan&lt;T&gt; object that has a "blue" TextMarkerTag, which provides a blue background.
    /// </summary>
    /// <param name="spans"></param>
    /// <returns></returns>
    public IEnumerable<ITagSpan<HighlightWordTag>> GetTags(NormalizedSnapshotSpanCollection spans)
    {
      if (CurrentWord == null)
        yield break;

      // Hold on to a "snapshot" of the word spans and current word, so that we maintain the same
      // collection throughout
      var currentWord = CurrentWord.Value;
      var wordSpans = WordSpans;

      if (spans.Count == 0 || wordSpans.Count == 0)
        yield break;

      // If the requested snapshot isn't the same as the one our words are on, translate our spans to the expected snapshot
      if (spans[0].Snapshot != wordSpans[0].Snapshot)
      {
        wordSpans = new NormalizedSnapshotSpanCollection(wordSpans.Select(span => span.TranslateTo(spans[0].Snapshot, SpanTrackingMode.EdgeExclusive)));

        currentWord = currentWord.TranslateTo(spans[0].Snapshot, SpanTrackingMode.EdgeExclusive);
      }

      // First, yield back the word the cursor is under (if it overlaps)
      // Note that we'll yield back the same word again in the wordspans collection;
      // the duplication here is expected.
      if (spans.OverlapsWith(new NormalizedSnapshotSpanCollection(currentWord)))
        yield return new TagSpan<HighlightWordTag>(currentWord, new HighlightWordTag());

      // Second, yield all the other words in the file
      foreach (var span in NormalizedSnapshotSpanCollection.Overlap(spans, wordSpans))
      {
        yield return new TagSpan<HighlightWordTag>(span, new HighlightWordTag());
      }
    }
  }
}
