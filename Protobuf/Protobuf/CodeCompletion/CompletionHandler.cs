#region Copyright © 2013 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompletionHandler.cs" company="Michael Reukauff">
//   Copyright © 2013 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace MichaelReukauff.Protobuf
{
  using System;
  using System.Runtime.InteropServices;

  using Microsoft.VisualStudio;
  using Microsoft.VisualStudio.Language.Intellisense;
  using Microsoft.VisualStudio.OLE.Interop;
  using Microsoft.VisualStudio.Shell;
  using Microsoft.VisualStudio.Text;
  using Microsoft.VisualStudio.Text.Editor;
  using Microsoft.VisualStudio.TextManager.Interop;

  internal class CompletionHandler : IOleCommandTarget
  {
    private readonly IOleCommandTarget _nextCommandHandler;

    private ITextView _textView;

    private CompletionHandlerProvider _provider;

    private ICompletionSession _session;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="textViewAdapter"></param>
    /// <param name="textView"></param>
    /// <param name="provider"></param>
    internal CompletionHandler(IVsTextView textViewAdapter, ITextView textView, CompletionHandlerProvider provider)
    {
      _textView = textView;
      _provider = provider;

      //add the command to the command chain
      textViewAdapter.AddCommandFilter(this, out _nextCommandHandler);
    }

    /// <summary>
    /// Query status
    /// </summary>
    /// <param name="pguidCmdGroup"></param>
    /// <param name="cCmds"></param>
    /// <param name="prgCmds"></param>
    /// <param name="pCmdText"></param>
    /// <returns></returns>
    public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
    {
      return _nextCommandHandler.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
    }

    /// <summary>
    /// Exec
    /// </summary>
    /// <param name="pguidCmdGroup"></param>
    /// <param name="nCmdID"></param>
    /// <param name="nCmdexecopt"></param>
    /// <param name="pvaIn"></param>
    /// <param name="pvaOut"></param>
    /// <returns></returns>
    public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
    {
      if (VsShellUtilities.IsInAutomationFunction(_provider.ServiceProvider))
        return _nextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);

      // make a copy of this so we can look at it after forwarding some commands
      uint commandID = nCmdID;
      char typedChar = char.MinValue;

      // make sure the input is a char before getting it
      if (pguidCmdGroup == VSConstants.VSStd2K && nCmdID == (uint)VSConstants.VSStd2KCmdID.TYPECHAR)
        typedChar = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);

      // check for a commit character
      if (nCmdID == (uint)VSConstants.VSStd2KCmdID.RETURN
          || nCmdID == (uint)VSConstants.VSStd2KCmdID.TAB
          || (char.IsWhiteSpace(typedChar) || char.IsPunctuation(typedChar)))
      {
        // check for a a selection
        if (_session != null && !_session.IsDismissed)
        {
          //if the selection is fully selected, commit the current session
          if (_session.SelectedCompletionSet.SelectionStatus.IsSelected)
          {
            _session.Commit();
            // also, don't add the character to the buffer
            return VSConstants.S_OK;
          }

            // if there is no selection, dismiss the session
            _session.Dismiss();
        }
      }

      // pass along the command so the char is added to the buffer
      int retVal = _nextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
      bool handled = false;

      if (!typedChar.Equals(char.MinValue) && char.IsLetterOrDigit(typedChar))
      {
        if (_session == null || _session.IsDismissed) // If there is no active session, bring up completion
        {
          TriggerCompletion();
          _session.Filter();
        }
        else // the completion session is already active, so just filter
        {
          _session.Filter();
        }

        handled = true;
      }
      else
      {
        if (commandID == (uint)VSConstants.VSStd2KCmdID.BACKSPACE //redo the filter if there is a deletion
            || commandID == (uint)VSConstants.VSStd2KCmdID.DELETE)
        {
          if (_session != null && !_session.IsDismissed)
            _session.Filter();
          handled = true;
        }
      }

      if (handled)
        return VSConstants.S_OK;

      return retVal;
    }

    /// <summary>
    /// Trigger completion
    /// </summary>
    /// <returns></returns>
    private void TriggerCompletion()
    {
      //the caret must be in a non-projection location 
      SnapshotPoint? caretPoint = _textView.Caret.Position.Point.GetPoint(textBuffer => (!textBuffer.ContentType.IsOfType("projection")), PositionAffinity.Predecessor);
      if (!caretPoint.HasValue)
        return;

      _session = _provider.CompletionBroker.CreateCompletionSession(_textView,caretPoint.Value.Snapshot.CreateTrackingPoint(caretPoint.Value.Position, PointTrackingMode.Positive),true);

      //subscribe to the Dismissed event on the session 
      _session.Dismissed += OnSessionDismissed;
      _session.Start();
    }

    private void OnSessionDismissed(object sender, EventArgs e)
    {
      _session.Dismissed -= OnSessionDismissed;
      _session = null;
    }
  }
}
