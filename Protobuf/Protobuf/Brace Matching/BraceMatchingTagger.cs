#region Copyright © 2014 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BraceMatchingTagger.cs" company="Michael Reukauff">
//   Copyright © 2014 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace MichaelReukauff.Protobuf
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;

    /// <summary>
    /// The brace matching tagger.
    /// </summary>
    internal class BraceMatchingTagger : ITagger<TextMarkerTag>
    {
        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        private ITextView View { get; set; }

        /// <summary>
        /// Gets or sets the source buffer.
        /// </summary>
        private ITextBuffer SourceBuffer { get; set; }

        /// <summary>
        /// Gets or sets the current char.
        /// </summary>
        private SnapshotPoint? CurrentChar { get; set; }

        /// <summary>
        /// The brace list.
        /// </summary>
        private readonly Dictionary<char, char> braceList;

        /// <summary>
        /// The tags changed event.
        /// </summary>
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="BraceMatchingTagger"/> class.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="sourceBuffer">The source buffer.</param>
        internal BraceMatchingTagger(ITextView view, ITextBuffer sourceBuffer)
        {
            // here the keys are the open braces, and the values are the close braces
            braceList = new Dictionary<char, char> { { '{', '}' }, { '[', ']' }, { '(', ')' } };

            View = view;
            SourceBuffer = sourceBuffer;
            CurrentChar = null;

            View.Caret.PositionChanged += CaretPositionChanged;
            View.LayoutChanged += ViewLayoutChanged;
        }

        /// <summary>
        /// The event handler updates the current caret position of the CurrentChar property and raise the TagsChanged event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TextViewLayoutChangedEventArgs"/>.</param>
        private void ViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            // make sure that there has really been a change
            if (e.NewSnapshot != e.OldSnapshot)
            {
                UpdateAtCaretPosition(View.Caret.Position);
            }
        }

        /// <summary>
        /// The event handler updates the current caret position of the CurrentChar property and raise the TagsChanged event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="CaretPositionChangedEventArgs"/>.</param>
        private void CaretPositionChanged(object sender, CaretPositionChangedEventArgs e)
        {
            UpdateAtCaretPosition(e.NewPosition);
        }

        /// <summary>
        /// The update at caret position.
        /// </summary>
        /// <param name="caretPosition">The caret position.</param>
        private void UpdateAtCaretPosition(CaretPosition caretPosition)
        {
            CurrentChar = caretPosition.Point.GetPoint(SourceBuffer, caretPosition.Affinity);

            if (!CurrentChar.HasValue)
            {
                return;
            }

            var tempEvent = TagsChanged;
            if (tempEvent != null)
            {
                tempEvent(this, new SnapshotSpanEventArgs(new SnapshotSpan(SourceBuffer.CurrentSnapshot, 0, SourceBuffer.CurrentSnapshot.Length)));
            }
        }

        /// <summary>
        /// Implements the GetTags method to match braces either when the current character is an open brace
        /// or when the previous character is a close brace, as in Visual Studio.
        /// When the match is found, this method instantiates two tags, one for the open brace and one for the close brace.
        /// </summary>
        /// <param name="spans">The spans.</param>
        /// <returns>The list of text marker.</returns>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1108:BlockStatementsMustNotContainEmbeddedComments", Justification = "Reviewed. Suppression is OK here.")]
        public IEnumerable<ITagSpan<TextMarkerTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            // there is no content in the buffer
            if (spans.Count == 0)
            {
                yield break;
            }

            // don't do anything if the current SnapshotPoint is not initialized or at the end of the buffer
            if (!CurrentChar.HasValue) //// || CurrentChar.Value.Position >= CurrentChar.Value.Snapshot.Length)
            {
                yield break;
            }

            // hold on to a snapshot of the current character
            SnapshotPoint currentChar = CurrentChar.Value;

            // if no text return;
            if (currentChar.Snapshot.Length == 0)
            {
                yield break;
            }

            // if the requested snapshot isn't the same as the one the brace is on, translate our spans to the expected snapshot 
            if (spans[0].Snapshot != currentChar.Snapshot)
            {
                currentChar = currentChar.TranslateTo(spans[0].Snapshot, PointTrackingMode.Positive);
            }

            // get the current char and the previous char 
            char currentText = CurrentChar.Value.Position >= CurrentChar.Value.Snapshot.Length ? (currentChar - 1).GetChar() : currentChar.GetChar();

            // if currentChar is 0 (beginning of buffer), don't move it back
            SnapshotPoint lastChar = currentChar == 0 ? currentChar : currentChar - 1;
            char lastText = lastChar.GetChar();
            SnapshotSpan pairSpan;

            // the key is the open brace
            if (braceList.ContainsKey(currentText))
            {
                char closeChar;
                braceList.TryGetValue(currentText, out closeChar);
                if (FindMatchingCloseChar(currentChar, currentText, closeChar, View.TextViewLines.Count, out pairSpan))
                {
                    yield return new TagSpan<TextMarkerTag>(new SnapshotSpan(currentChar, 1), new BraceMatchingTag());
                    yield return new TagSpan<TextMarkerTag>(pairSpan, new BraceMatchingTag());
                }
            }
            else
            {
                // the value is the close brace, which is the *previous* character
                if (braceList.ContainsValue(lastText))
                {
                    var open = from n in braceList where n.Value.Equals(lastText) select n.Key;

                    if (FindMatchingOpenChar(lastChar, open.ElementAt(0), lastText, View.TextViewLines.Count, out pairSpan))
                    {
                        yield return new TagSpan<TextMarkerTag>(new SnapshotSpan(lastChar, 1), new BraceMatchingTag());
                        yield return new TagSpan<TextMarkerTag>(pairSpan, new BraceMatchingTag());
                    }
                }
            }
        }

        /// <summary>
        /// This method finds the matching brace at any level of nesting.
        /// This method finds the close character that matches the open character.
        /// </summary>
        /// <param name="startPoint">The start point.</param>
        /// <param name="open">Opening character.</param>
        /// <param name="close">Closing character.</param>
        /// <param name="maxLines">Maximum lines.</param>
        /// <param name="pairSpan">The shnap shot span.</param>
        /// <returns>True if found otherwise false.</returns>
        private static bool FindMatchingCloseChar(SnapshotPoint startPoint, char open, char close, int maxLines, out SnapshotSpan pairSpan)
        {
            pairSpan = new SnapshotSpan(startPoint.Snapshot, 1, 1);
            ITextSnapshotLine line = startPoint.GetContainingLine();
            string lineText = line.GetText();
            int lineNumber = line.LineNumber;
            int offset = startPoint.Position - line.Start.Position + 1;

            int stopLineNumber = startPoint.Snapshot.LineCount - 1;
            if (maxLines > 0)
            {
                stopLineNumber = Math.Min(stopLineNumber, lineNumber + maxLines);
            }

            int openCount = 0;
            while (true)
            {
                // walk the entire line
                while (offset < line.Length)
                {
                    char currentChar = lineText[offset];

                    // found the close character
                    if (currentChar == close)
                    {
                        if (openCount > 0)
                        {
                            openCount--;
                        }
                        else
                        {
                            // found the matching close
                            pairSpan = new SnapshotSpan(startPoint.Snapshot, line.Start + offset, 1);
                            return true;
                        }
                    }
                    else
                    {
                        // this is another open
                        if (currentChar == open)
                        {
                            openCount++;
                        }
                    }

                    offset++;
                }

                // move on to the next line
                if (++lineNumber > stopLineNumber)
                {
                    break;
                }

                line = line.Snapshot.GetLineFromLineNumber(lineNumber);
                lineText = line.GetText();
                offset = 0;
            }

            return false;
        }

        /// <summary>
        /// This method finds the open character that matches a close character.
        /// </summary>
        /// <param name="startPoint">The start point.</param>
        /// <param name="open">Opening character.</param>
        /// <param name="close">Closing character.</param>
        /// <param name="maxLines">Maximum lines.</param>
        /// <param name="pairSpan">The shnap shot span.</param>
        /// <returns>True if found otherwise false.</returns>
        private static bool FindMatchingOpenChar(SnapshotPoint startPoint, char open, char close, int maxLines, out SnapshotSpan pairSpan)
        {
            pairSpan = new SnapshotSpan(startPoint, startPoint);

            ITextSnapshotLine line = startPoint.GetContainingLine();

            int lineNumber = line.LineNumber;
            int offset = startPoint - line.Start - 1; // move the offset to the character before this one

            // if the offset is negative, move to the previous line
            if (offset < 0)
            {
                line = line.Snapshot.GetLineFromLineNumber(--lineNumber);
                offset = line.Length - 1;
            }

            string lineText = line.GetText();

            int stopLineNumber = 0;
            if (maxLines > 0)
            {
                stopLineNumber = Math.Max(stopLineNumber, lineNumber - maxLines);
            }

            int closeCount = 0;

            while (true)
            {
                // Walk the entire line
                while (offset >= 0)
                {
                    char currentChar = lineText[offset];

                    if (currentChar == open)
                    {
                        if (closeCount > 0)
                        {
                            closeCount--;
                        }
                        else
                        {
                            // We've found the open character
                            // we just want the character itself
                            pairSpan = new SnapshotSpan(line.Start + offset, 1);

                            return true;
                        }
                    }
                    else if (currentChar == close)
                    {
                        closeCount++;
                    }

                    offset--;
                }

                // Move to the previous line
                if (--lineNumber < stopLineNumber)
                {
                    break;
                }

                line = line.Snapshot.GetLineFromLineNumber(lineNumber);
                lineText = line.GetText();
                offset = line.Length - 1;
            }

            return false;
        }
    }
}