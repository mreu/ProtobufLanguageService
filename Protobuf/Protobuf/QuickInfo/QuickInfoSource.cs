#region Copyright © 2014 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuickInfoSource.cs" company="Michael Reukauff">
//   Copyright © 2014 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

// ReSharper disable once CheckNamespace
namespace MichaelReukauff.Protobuf
{
    using System.Collections.Generic;
    using System.Linq;

    using MichaelReukauff.Lexer;

    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;

    /// <summary>
    /// The quick info source.
    /// </summary>
    internal class QuickInfoSource : IQuickInfoSource
    {
        /// <summary>
        /// The _aggregator.
        /// </summary>
        private readonly ITagAggregator<ProtobufTokenTag> aggregator;

        /// <summary>
        /// The _buffer.
        /// </summary>
        private readonly ITextBuffer buffer;

        /// <summary>
        /// The _keywords.
        /// </summary>
        private readonly IDictionary<string, string> keywords;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuickInfoSource"/> class. 
        /// The constructor sets the QuickInfo source provider and the text buffer, and populates the set of method names, and method signatures and descriptions.
        /// </summary>
        /// <param name="buffer">
        /// The text buffer.
        /// </param>
        /// <param name="aggregator">
        /// The aggregator.
        /// </param>
        public QuickInfoSource(ITextBuffer buffer, ITagAggregator<ProtobufTokenTag> aggregator)
        {
            this.aggregator = aggregator;
            this.buffer = buffer;
            keywords = new ProtobufWordListProvider().GetWordsWithDescription();
        }

        /// <summary>
        /// Implement the AugmentQuickInfoSession method.
        /// Here the method finds the current word, or the previous word if the cursor is at the end of a line or a text buffer.
        /// If the word is one of the method names, the description for that method name is added to the QuickInfo content.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="quickInfoContent">
        /// The quick info content.
        /// </param>
        /// <param name="applicableToSpan">
        /// The tracking span.
        /// </param>
        public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> quickInfoContent, out ITrackingSpan applicableToSpan)
        {
            applicableToSpan = null;

            var snapshotPoint = session.GetTriggerPoint(buffer.CurrentSnapshot);
            if (snapshotPoint != null)
            {
                var triggerPoint = (SnapshotPoint)snapshotPoint;

                // find each span that looks like a token and look it up in the dictionary
                foreach (IMappingTagSpan<ProtobufTokenTag> curTag in aggregator.GetTags(new SnapshotSpan(triggerPoint, triggerPoint)))
                {
                    if (curTag.Tag.CodeType == CodeType.Keyword)
                    {
                        SnapshotSpan tagSpan = curTag.Span.GetSpans(buffer).First();
                        if (keywords.Keys.Contains(tagSpan.GetText()))
                        {
                            applicableToSpan = buffer.CurrentSnapshot.CreateTrackingSpan(tagSpan, SpanTrackingMode.EdgeExclusive);
                            quickInfoContent.Add(keywords[tagSpan.GetText()]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
