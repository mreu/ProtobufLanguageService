// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HighlightWordTagger.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace
namespace MichaelReukauff.Protobuf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Operations;
    using Microsoft.VisualStudio.Text.Tagging;

    /// <summary>
    /// The highlight word tagger.
    /// </summary>
    public class HighlightWordTagger : ITagger<HighlightWordTag>
    {
        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        public ITextView View { get; set; }

        /// <summary>
        /// Gets or sets the source buffer.
        /// </summary>
        public ITextBuffer SourceBuffer { get; set; }

        /// <summary>
        /// Gets or sets the text search service.
        /// </summary>
        public ITextSearchService TextSearchService { get; set; }

        /// <summary>
        /// Gets or sets the text structure navigator.
        /// </summary>
        public ITextStructureNavigator TextStructureNavigator { get; set; }

        /// <summary>
        /// Gets or sets the word spans.
        /// </summary>
        public NormalizedSnapshotSpanCollection WordSpans { get; set; }

        /// <summary>
        /// Gets or sets the current word.
        /// </summary>
        public SnapshotSpan? CurrentWord { get; set; }

        /// <summary>
        /// Gets or sets the requested point.
        /// </summary>
        public SnapshotPoint RequestedPoint { get; set; }

#pragma warning disable SA1401 // Fields must be private
        /// <summary>
        /// The update lock.
        /// </summary>
        public readonly object UpdateLock = new object();
#pragma warning restore SA1401 // Fields must be private

        /// <summary>
        /// The tags changed event.
        /// </summary>
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="HighlightWordTagger"/> class.
        /// The constructor initializes the properties listed earlier and adds LayoutChanged and PositionChanged event handlers.
        /// </summary>
        /// <param name="view">
        /// The text view.
        /// </param>
        /// <param name="sourceBuffer">
        /// The source buffer.
        /// </param>
        /// <param name="textSearchService">
        /// The text search service.
        /// </param>
        /// <param name="textStructureNavigator">
        /// The text structure navigator.
        /// </param>
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
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TextViewLayoutChangedEventArgs"/>.</param>
        public void ViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
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
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="CaretPositionChangedEventArgs"/>.</param>
        public void CaretPositionChanged(object sender, CaretPositionChangedEventArgs e)
        {
            UpdateAtCaretPosition(e.NewPosition);
        }

        /// <summary>
        /// This method finds every word in the text buffer that is identical to the word
        /// where the cursor is positioned and constructs a list of SnapshotSpan objects
        /// that correspond to the occurrences of the word. It then calls SynchronousUpdate, which raises the TagsChanged event.
        /// </summary>
        /// <param name="caretPosition">The caret position.</param>
        public void UpdateAtCaretPosition(CaretPosition caretPosition)
        {
            var point = caretPosition.Point.GetPoint(SourceBuffer, caretPosition.Affinity);

            if (!point.HasValue)
            {
                return;
            }

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

        /// <summary>
        /// The update word adornments.
        /// </summary>
        public void UpdateWordAdornments()
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

                    // If the word still isn't valid, we're done
                    if (!WordExtentIsValid(currentRequest, word))
                    {
                        foundWord = false;
                    }
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
            {
                return;
            }

            // Find the new spans
            var findData = new FindData(currentWord.GetText(), currentWord.Snapshot)
            {
                FindOptions = FindOptions.WholeWord | FindOptions.MatchCase
            };

            wordSpans.AddRange(TextSearchService.FindAll(findData));

            // If another change hasn't happened, do a real update
            if (currentRequest == RequestedPoint)
            {
                SynchronousUpdate(currentRequest, new NormalizedSnapshotSpanCollection(wordSpans), currentWord);
            }
        }

        /// <summary>
        /// The word extent is valid.
        /// </summary>
        /// <param name="currentRequest">The currentRequest.</param>
        /// <param name="word">The word.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool WordExtentIsValid(SnapshotPoint currentRequest, TextExtent word)
        {
            return word.IsSignificant && currentRequest.Snapshot.GetText(word.Span).Any(char.IsLetter);
        }

        /// <summary>
        /// The SynchronousUpdate performs a synchronous update on the WordSpans and CurrentWord properties, and raises the TagsChanged event.
        /// </summary>
        /// <param name="currentRequest">The current request.</param>
        /// <param name="newSpans">The new spans.</param>
        /// <param name="newCurrentWord">The new current word.</param>
        public void SynchronousUpdate(SnapshotPoint currentRequest, NormalizedSnapshotSpanCollection newSpans, SnapshotSpan? newCurrentWord)
        {
            lock (UpdateLock)
            {
                if (currentRequest != RequestedPoint)
                {
                    return;
                }

                WordSpans = newSpans;
                CurrentWord = newCurrentWord;

                var tempEvent = TagsChanged;
                tempEvent?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(SourceBuffer.CurrentSnapshot, 0, SourceBuffer.CurrentSnapshot.Length)));
            }
        }

        /// <summary>
        /// This method takes a collection of SnapshotSpan objects and returns an enumeration of tag spans.
        /// Implement this method as a yield iterator,
        /// which enables lazy evaluation (that is, evaluation of the set only when individual items are accessed) of the tags.
        /// Here the method returns a TagSpan&lt;T&gt; object that has a "blue" TextMarkerTag, which provides a blue background.
        /// </summary>
        /// <param name="spans">The spans.</param>
        /// <returns>List of tags.</returns>
        public IEnumerable<ITagSpan<HighlightWordTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (CurrentWord == null)
            {
                yield break;
            }

            // Hold on to a "snapshot" of the word spans and current word, so that we maintain the same
            // collection throughout
            var currentWord = CurrentWord.Value;
            var wordSpans = WordSpans;

            if (spans.Count == 0 || wordSpans.Count == 0)
            {
                yield break;
            }

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
            {
                yield return new TagSpan<HighlightWordTag>(currentWord, new HighlightWordTag());
            }

            // Second, yield all the other words in the file
            foreach (var span in NormalizedSnapshotSpanCollection.Overlap(spans, wordSpans))
            {
                yield return new TagSpan<HighlightWordTag>(span, new HighlightWordTag());
            }
        }
    }
}
