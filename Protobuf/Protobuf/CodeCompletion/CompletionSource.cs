// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompletionSource.cs" company="Michael Reukauff, Germany">
//   Copyright © 2016 Michael Reukauff, Germany. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace
namespace MichaelReukauff.Protobuf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Operations;

    /// <summary>
    /// The completion source.
    /// </summary>
    internal class CompletionSource : ICompletionSource
    {
        /// <summary>
        /// The _buffer.
        /// </summary>
        private readonly ITextBuffer buffer;

        /// <summary>
        /// The _navigator selector service.
        /// </summary>
        private readonly ITextStructureNavigatorSelectorService navigatorSelectorService;

        /// <summary>
        /// The _keywords.
        /// </summary>
        private readonly IDictionary<string, string> keywords;

        /// <summary>
        /// The is disposed.
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompletionSource"/> class.
        /// </summary>
        /// <param name="navigatorSelectorService">The navigator selector service.</param>
        /// <param name="buffer">The text buffer.</param>
        public CompletionSource(ITextStructureNavigatorSelectorService navigatorSelectorService, ITextBuffer buffer)
        {
            this.buffer = buffer;
            this.navigatorSelectorService = navigatorSelectorService;
            keywords = new ProtobufWordListProvider().GetWordsWithDescription(' ');
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        void IDisposable.Dispose()
        {
            if (!isDisposed)
            {
                GC.SuppressFinalize(this);
                isDisposed = true;
            }
        }

        /// <summary>
        /// Implement the AugmentCompletionSession method by adding a completion set that contains the completions you want to provide in the context.
        /// Each completion set contains a set of Completion completions, and corresponds to a tab of the completion window.
        /// The FindTokenSpanAtPosition method is defined in the next step.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="completionSets">The completion sets.</param>
        void ICompletionSource.AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            // create a list of completions from the dictionary of valid tokens
            var completions = new List<Completion>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var token in keywords)
            {
                completions.Add(new Completion(token.Key, token.Key, token.Value, null, null));
            }

            ////ITextSnapshot snapshot = buffer.CurrentSnapshot;
            ////var snapshotPoint = session.GetTriggerPoint(snapshot);
            ////if (snapshotPoint != null)
            ////{
            ////    var triggerPoint = (SnapshotPoint)snapshotPoint;
            ////    var line = triggerPoint.GetContainingLine();
            ////    SnapshotPoint start = triggerPoint - 1;
            ////}

            var res = FindTokenSpanAtPosition(session);

            completionSets.Add(new CompletionSet("Protobuf keywords", "Protobuf keywords", res, completions, Enumerable.Empty<Completion>()));
        }

        /// <summary>
        /// Find the current word from the position of the cursor.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns>The tracking span.</returns>
        private ITrackingSpan FindTokenSpanAtPosition(ICompletionSession session)
        {
            var currentPoint = session.TextView.Caret.Position.BufferPosition - 1;
            var navigator = navigatorSelectorService.GetTextStructureNavigator(buffer);
            var extent = navigator.GetExtentOfWord(currentPoint);
            return currentPoint.Snapshot.CreateTrackingSpan(extent.Span, SpanTrackingMode.EdgeInclusive);
        }
    }
}
