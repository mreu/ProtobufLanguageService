// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtobufLanguageTagger.cs" company="Michael Reukauff, Germany">
//   Copyright © 2016 Michael Reukauff, Germany. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace
namespace MichaelReukauff.Protobuf
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Lexer;

    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;

    /// <summary>
    /// ProtobufLanguageTagger is the core parser for the protobuf 'language'. It handles continuation lines
    /// and finds both language elements and errors within the given SnapshotSpan(s).
    /// </summary>
    internal sealed class ProtobufLanguageTagger : ITagger<ProtobufTokenTag>, IDisposable
    {
        /// <summary>
        /// The _buffer.
        /// </summary>
        private readonly ITextBuffer buffer;

        /// <summary>
        /// The _task.
        /// </summary>
        private Task task;

        /// <summary>
        /// The _lexer.
        /// </summary>
        private Lexer lexer;

        /// <summary>
        /// The _tag list.
        /// </summary>
        private readonly List<ITagSpan<ProtobufTokenTag>> tagList = new List<ITagSpan<ProtobufTokenTag>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtobufLanguageTagger"/> class.
        /// </summary>
        /// <param name="buffer">The text buffer.</param>
        internal ProtobufLanguageTagger(ITextBuffer buffer)
        {
            this.buffer = buffer;

            Parse();

            this.buffer.Changed += BufferChanged;
        }

        /// <summary>
        /// Task to reparse the whole text and fires a TagsChanged event.
        /// </summary>
        /// <param name="snapshot">
        /// The snapshot.
        /// </param>
        private void Parse(ITextSnapshot snapshot)
        {
            Parse();

            var startPoint = new SnapshotPoint(snapshot, 0);
            var endPoint = new SnapshotPoint(snapshot, snapshot.Length);
            var expandedSpan = new SnapshotSpan(startPoint, endPoint);
            var args = new SnapshotSpanEventArgs(expandedSpan);

            if (TagsChanged != null)
            {
                TagsChanged(this, args);
            }
        }

        /// <summary>
        /// Parse the text and build error and token lists.
        /// </summary>
        private void Parse()
        {
            lexer = new Lexer(buffer.CurrentSnapshot.GetText());
            lexer.Analyze();

            tagList.Clear();

            // copy the lexer.Token list to an array, because it can be changed by the parser in the background
            // this should not happen, but under some circumstandings it happens (and I don't know when and why)
            foreach (var message in lexer.Errors.ToArray())
            {
                // check the length of the new span, should not be longer than the current text
                var length = message.Length;
                if (message.Position + message.Length > buffer.CurrentSnapshot.Length)
                {
                    length = buffer.CurrentSnapshot.Length - message.Position;
                }

                if (length > 0)
                {
                    var newSpan = new Span(message.Position, length);
                    tagList.Add(new TagSpan<ProtobufTokenTag>(new SnapshotSpan(buffer.CurrentSnapshot, newSpan), new ProtobufErrorTag(message.Message)));
                }
            }

            // copy the lexer.Token list to an array, because it can be changed by the parser in the background
            // this should not happen, but under some circumstandings it happens (and I don't know when and why)
            foreach (var token in lexer.Tokens.ToArray())
            {
                // check the length of the new span, should not be longer than the current text
                var length = token.Length;
                if (token.Position + token.Length > buffer.CurrentSnapshot.Length)
                {
                    length = buffer.CurrentSnapshot.Length - token.Position;
                }

                if (length > 0)
                {
                    var newSpan = new Span(token.Position, length);
                    tagList.Add(new TagSpan<ProtobufTokenTag>(new SnapshotSpan(buffer.CurrentSnapshot, newSpan), new ProtobufTokenTag(token.CodeType)));
                }
            }
        }

        /// <summary>
        /// When the buffer changes, we reparse all in a separate task.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TextContentChangedEventArgs"/>.</param>
        private void BufferChanged(object sender, TextContentChangedEventArgs e)
        {
            var temp = TagsChanged;
            if (temp != null)
            {
                Action<object> action = obj => Parse((ITextSnapshot)obj);
                task = new Task(action, e.After.TextBuffer.CurrentSnapshot); // start background parsing
                task.Start();
            }
        }

        /// <summary>
        /// The tags changed.
        /// </summary>
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        /// <summary>
        /// Parse the given span(s) and return all the tags that intersect the specified spans.
        /// </summary>
        /// <param name="spans">Ordered collection of non-overlapping spans.</param>
        /// <returns>Unordered enumeration of tags.</returns>
        public IEnumerable<ITagSpan<ProtobufTokenTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            return tagList;
        }

        #region IDisposable
        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            task.Dispose();
        }
        #endregion IDisposable
    }
}
