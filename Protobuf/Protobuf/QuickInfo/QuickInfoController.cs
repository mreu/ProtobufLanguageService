// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuickInfoController.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace
namespace MichaelReukauff.Protobuf
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;

    /// <summary>
    /// The quick info controller.
    /// </summary>
    internal class QuickInfoController : IIntellisenseController
    {
        /// <summary>
        /// The _text view.
        /// </summary>
        private ITextView textView;

        /// <summary>
        /// The _subject buffers.
        /// </summary>
        private readonly IList<ITextBuffer> subjectBuffers;

        /// <summary>
        /// The _component context.
        /// </summary>
        private readonly QuickInfoControllerProvider componentContext;

        /// <summary>
        /// The _session.
        /// </summary>
        private IQuickInfoSession session;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuickInfoController"/> class.
        /// The constructor sets the fields and adds the mouse hover event handler.
        /// </summary>
        /// <param name="textView">
        /// The text view.
        /// </param>
        /// <param name="subjectBuffers">
        /// The text buffer.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        internal QuickInfoController(ITextView textView, IList<ITextBuffer> subjectBuffers, QuickInfoControllerProvider provider)
        {
            this.textView = textView;
            this.subjectBuffers = subjectBuffers;
            componentContext = provider;

            this.textView.MouseHover += OnTextViewMouseHover;
        }

        /// <summary>
        /// Add the mouse hover event handler that triggers the QuickInfo session.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="MouseHoverEventArgs"/>.
        /// </param>
        private void OnTextViewMouseHover(object sender, MouseHoverEventArgs e)
        {
            // find the mouse position by mapping down to the subject buffer
            var point = GetMousePosition(new SnapshotPoint(textView.TextSnapshot, e.Position));

            if (point != null)
            {
                var triggerPoint = point.Value.Snapshot.CreateTrackingPoint(point.Value.Position, PointTrackingMode.Positive);

                // Find the broker for this buffer
                if (!componentContext.QuickInfoBroker.IsQuickInfoActive(textView))
                {
                    session = componentContext.QuickInfoBroker.CreateQuickInfoSession(textView, triggerPoint, true);
                    session.Start();
                }
            }
        }

        /// <summary>
        /// Get mouse position.
        /// </summary>
        /// <param name="topPosition">
        /// The top position.
        /// </param>
        /// <returns>
        /// The <see cref="SnapshotPoint"/>.
        /// </returns>
        private SnapshotPoint? GetMousePosition(SnapshotPoint topPosition)
        {
            // Map this point down to the appropriate subject buffer.
            return textView.BufferGraph.MapDownToFirstMatch(
                topPosition,
                PointTrackingMode.Positive,
                snapshot => subjectBuffers.Contains(snapshot.TextBuffer),
                PositionAffinity.Predecessor);
        }

        /// <summary>
        /// Implement the Detach method so that it removes the mouse hover event handler when the controller is detached from the text view.
        /// </summary>
        /// <param name="view">
        /// The text view.
        /// </param>
        public void Detach(ITextView view)
        {
            if (textView == view)
            {
                textView.MouseHover -= OnTextViewMouseHover;
                textView = null;
            }
        }

        /// <summary>
        /// Implement the ConnectSubjectBuffer method as empty method here.
        /// </summary>
        /// <param name="subjectBuffer">The text buffer.</param>
        public void ConnectSubjectBuffer(ITextBuffer subjectBuffer)
        {
        }

        /// <summary>
        /// Implement the DisconnectSubjectBuffer method as empty method here.
        /// </summary>
        /// <param name="subjectBuffer">The text buffer.</param>
        public void DisconnectSubjectBuffer(ITextBuffer subjectBuffer)
        {
        }
    }
}
