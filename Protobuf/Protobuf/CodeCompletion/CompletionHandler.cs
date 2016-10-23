// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompletionHandler.cs" company="Michael Reukauff, Germany">
//   Copyright © 2016 Michael Reukauff, Germany. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace
namespace MichaelReukauff.Protobuf
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;

    using Lexer;

    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.OLE.Interop;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.TextManager.Interop;

    /// <summary>
    /// The completion handler.
    /// </summary>
    internal class CompletionHandler : IOleCommandTarget
    {
        /// <summary>
        /// The _next command handler.
        /// </summary>
        private readonly IOleCommandTarget nextCommandHandler;

        /// <summary>
        /// The _text view.
        /// </summary>
        private readonly ITextView textView;

        /// <summary>
        /// The _provider.
        /// </summary>
        private readonly CompletionHandlerProvider provider;

        /// <summary>
        /// The _session.
        /// </summary>
        private ICompletionSession session;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompletionHandler"/> class.
        /// </summary>
        /// <param name="textViewAdapter">The text view adapter.</param>
        /// <param name="textView">The text viewer.</param>
        /// <param name="provider">The provider.</param>
        internal CompletionHandler(IVsTextView textViewAdapter, ITextView textView, CompletionHandlerProvider provider)
        {
            this.textView = textView;
            this.provider = provider;

            // add the command to the command chain
            textViewAdapter.AddCommandFilter(this, out nextCommandHandler);
        }

        /// <summary>
        /// Query status.
        /// </summary>
        /// <param name="pguidCmdGroup">The guid.</param>
        /// <param name="cmds">The cmd.</param>
        /// <param name="prgCmds">The program cmd.</param>
        /// <param name="cmdText">The cmd text.</param>
        /// <returns>The status.</returns>
        public int QueryStatus(ref Guid pguidCmdGroup, uint cmds, OLECMD[] prgCmds, IntPtr cmdText)
        {
            return nextCommandHandler.QueryStatus(ref pguidCmdGroup, cmds, prgCmds, cmdText);
        }

        /// <summary>
        /// The exec.
        /// </summary>
        /// <param name="pguidCmdGroup">The cmd group guid.</param>
        /// <param name="cmdId">The cmd id.</param>
        /// <param name="cmdExecOpt">The cmd exec opt.</param>
        /// <param name="pvaIn">The in pointer.</param>
        /// <param name="pvaOut">The out pointer.</param>
        /// <returns>The return value.</returns>
        public int Exec(ref Guid pguidCmdGroup, uint cmdId, uint cmdExecOpt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (VsShellUtilities.IsInAutomationFunction(provider.ServiceProvider))
            {
                return nextCommandHandler.Exec(ref pguidCmdGroup, cmdId, cmdExecOpt, pvaIn, pvaOut);
            }

            // make a copy of this so we can look at it after forwarding some commands
            var commandId = cmdId;
            var typedChar = char.MinValue;

            // make sure the input is a char before getting it
            if (pguidCmdGroup == VSConstants.VSStd2K && cmdId == (uint)VSConstants.VSStd2KCmdID.TYPECHAR)
            {
                typedChar = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
            }

            // check for a commit character
            if (cmdId == (uint)VSConstants.VSStd2KCmdID.RETURN
                || cmdId == (uint)VSConstants.VSStd2KCmdID.TAB
                || (char.IsWhiteSpace(typedChar) || char.IsPunctuation(typedChar)))
            {
                // check for a selection
                if (session != null && !session.IsDismissed)
                {
                    // if the selection is fully selected, commit the current session
                    if (session.SelectedCompletionSet.SelectionStatus.IsSelected)
                    {
                        session.Commit();

                        // also, don't add the character to the buffer
                        return VSConstants.S_OK;
                    }

                    // if there is no selection, dismiss the session
                    session.Dismiss();
                }
            }

            // pass along the command so the char is added to the buffer
            var retVal = nextCommandHandler.Exec(ref pguidCmdGroup, cmdId, cmdExecOpt, pvaIn, pvaOut);
            var handled = false;

            if (!typedChar.Equals(char.MinValue) && char.IsLetterOrDigit(typedChar))
            {
                // If there is no active session, bring up completion
                if (session == null || session.IsDismissed)
                {
                    TriggerCompletion();

                    if (session != null)
                    {
                        session.Filter();
                    }
                }
                else
                {
                    // the completion session is already active, so just filter
                    session.Filter();
                }

                handled = true;
            }
            else
            {
                // redo the filter if there is a deletion
                if (commandId == (uint)VSConstants.VSStd2KCmdID.BACKSPACE
                    || commandId == (uint)VSConstants.VSStd2KCmdID.DELETE)
                {
                    if (session != null && !session.IsDismissed)
                    {
                        session.Filter();
                    }

                    handled = true;
                }
            }

            if (handled)
            {
                return VSConstants.S_OK;
            }

            return retVal;
        }

        /// <summary>
        /// Trigger completion.
        /// </summary>
        private void TriggerCompletion()
        {
            // the caret must be in a non-projection location
            var caretPoint = textView.Caret.Position.Point.GetPoint(textBuffer => (!textBuffer.ContentType.IsOfType("projection")), PositionAffinity.Predecessor);
            if (!caretPoint.HasValue)
            {
                return;
            }

            var cp = (SnapshotPoint)caretPoint;
            var position = cp.Position;

            // check for "//" before the current cursor position in this line
            var line = ((SnapshotPoint)caretPoint).Snapshot.GetLineFromPosition(((SnapshotPoint)caretPoint).Position);

            // get the text of this line
            var text = line.GetText();

            // get only the text portion which is left of the actual position
            text = text.Substring(0, position - line.Start.Position);

            // if there is a comment starting, don't do auto completion
            if (text.IndexOf("//", StringComparison.CurrentCultureIgnoreCase) != -1)
            {
                return;
            }

            var lex = new Lexer(cp.Snapshot.GetText());
            lex.AnalyzeForCommentsOnly();

            var res = lex.Tokens.FirstOrDefault(x => x.Position < cp.Position && x.Position + x.Length >= cp.Position);

            if (res != null)
            {
                return;
            }

            session = provider.CompletionBroker.CreateCompletionSession(textView, caretPoint.Value.Snapshot.CreateTrackingPoint(caretPoint.Value.Position, PointTrackingMode.Positive), true);

            // subscribe to the Dismissed event on the session
            session.Dismissed += OnSessionDismissed;
            session.Start();
        }

        /// <summary>
        /// The on session dismissed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void OnSessionDismissed(object sender, EventArgs e)
        {
            session.Dismissed -= OnSessionDismissed;
            session = null;
        }
    }
}
