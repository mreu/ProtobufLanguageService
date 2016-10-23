// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtobufErrorTagger.cs" company="Michael Reukauff, Germany">
//   Copyright © 2016 Michael Reukauff, Germany. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace
namespace MichaelReukauff.Protobuf
{
    using System;
    using System.Collections.Generic;

    using Lexer;

    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;

    /// <summary>
    /// Translate Token into ErrorTags and Error List items.
    /// </summary>
    internal sealed class ErrorTagger : ITagger<ErrorTag>, IDisposable
    {
#pragma warning disable SA1401 // Fields must be private
        /// <summary>
        /// The _aggregator.
        /// </summary>
        public readonly ITagAggregator<ProtobufTokenTag> Aggregator;

        /// <summary>
        /// The _buffer.
        /// </summary>
        public readonly ITextBuffer Buffer;

        /// <summary>
        /// The _error provider.
        /// </summary>
        public readonly ErrorListProvider ErrorProvider;

        /// <summary>
        /// The _document.
        /// </summary>
        public readonly ITextDocument Document;
#pragma warning restore SA1401 // Fields must be private

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorTagger"/> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="aggregatorFactory">The aggregator factory.</param>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="textDocumentFactory">The text document factory.</param>
        internal ErrorTagger(ITextBuffer buffer, IBufferTagAggregatorFactoryService aggregatorFactory, IServiceProvider serviceProvider, ITextDocumentFactoryService textDocumentFactory)
        {
            Buffer = buffer;

            Aggregator = aggregatorFactory.CreateTagAggregator<ProtobufTokenTag>(buffer);

            if (!textDocumentFactory.TryGetTextDocument(Buffer, out Document))
            {
                Document = null;
            }

            ErrorProvider = new ErrorListProvider(serviceProvider);

            ReparseFile(null, EventArgs.Empty);

            BufferIdleEventUtil.AddBufferIdleEventListener(Buffer, ReparseFile);
        }

        /// <summary>
        /// The Dispose.
        /// </summary>
        public void Dispose()
        {
            if (ErrorProvider != null)
            {
                ErrorProvider.Tasks.Clear();
                ErrorProvider.Dispose();
            }

            BufferIdleEventUtil.RemoveBufferIdleEventListener(Buffer, ReparseFile);
        }

        /// <summary>
        /// Find the Error tokens in the set of all tokens and create an ErrorTag for each.
        /// </summary>
        /// <param name="spans">The spans.</param>
        /// <returns>The <see cref="IEnumerable{ITagSpan}"/>.</returns>
        public IEnumerable<ITagSpan<ErrorTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var tagSpan in Aggregator.GetTags(spans))
            {
                if (tagSpan.Tag.CodeType == CodeType.Error)
                {
                    var tagSpans = tagSpan.Span.GetSpans(spans[0].Snapshot);
                    var tag = tagSpan.Tag as ProtobufErrorTag;
                    if (tag != null)
                    {
                        yield return new TagSpan<ErrorTag>(tagSpans[0], new ErrorTag("error", tag.Message));
                    }
                }
            }
        }

#pragma warning disable 67
        /// <summary>
        /// The Classifier tagger is translating buffer change events into TagsChanged events, so we don't have to.
        /// </summary>
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
#pragma warning restore

        /// <summary>
        /// Updates the Error List by clearing our items and adding any errors found in the current set of tags.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="EventArgs"/>.</param>
        private void ReparseFile(object sender, EventArgs args)
        {
            var snapshot = Buffer.CurrentSnapshot;
            var spans = new NormalizedSnapshotSpanCollection(new SnapshotSpan(snapshot, 0, snapshot.Length));

            ErrorProvider.Tasks.Clear();

            foreach (var tagSpan in Aggregator.GetTags(spans))
            {
                if (tagSpan.Tag.CodeType == CodeType.Error)
                {
                    var tagSpans = tagSpan.Span.GetSpans(spans[0].Snapshot);
                    var tag = tagSpan.Tag as ProtobufErrorTag;
                    AddErrorTask(tagSpans[0], tag);
                }
            }
        }

        /// <summary>
        /// Add one task to the Error List based on the given tag.
        /// </summary>
        /// <param name="span">The span.</param>
        /// <param name="tag">The tag.</param>
        private void AddErrorTask(SnapshotSpan span, ProtobufErrorTag tag)
        {
            if (ErrorProvider != null)
            {
                var task = new ErrorTask { CanDelete = true };
                if (Document != null)
                {
                    task.Document = Document.FilePath;
                }

                task.ErrorCategory = TaskErrorCategory.Error;
                task.Text = tag.Message;
                task.Line = span.Start.GetContainingLine().LineNumber;
                task.Column = span.Start.Position - span.Start.GetContainingLine().Start.Position;

                task.Navigate += TaskNavigate;

                ErrorProvider.Tasks.Add(task);
            }
        }

        /// <summary>
        /// Callback method attached to each of our tasks in the Error List.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> .</param>
        private void TaskNavigate(object sender, EventArgs e)
        {
            var error = sender as ErrorTask;

            if (error != null)
            {
                error.Line += 1;
                error.Column += 1;
                ErrorProvider.Navigate(error, new Guid(EnvDTE.Constants.vsViewKindCode));
                error.Column -= 1;
                error.Line -= 1;
            }
        }
    }
}