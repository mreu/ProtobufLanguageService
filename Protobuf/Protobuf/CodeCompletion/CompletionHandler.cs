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

  internal class CompletionHandler : IOleCommandTarget
  {
    private readonly IOleCommandTarget _nextCommandHandler;

    private readonly ITextView _textView;

    private readonly CompletionHandlerProvider _provider;

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
    /// <param name="nCmdId"></param>
    /// <param name="nCmdexecopt"></param>
    /// <param name="pvaIn"></param>
    /// <param name="pvaOut"></param>
    /// <returns></returns>
    public int Exec(ref Guid pguidCmdGroup, uint nCmdId, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
    {
      if (VsShellUtilities.IsInAutomationFunction(_provider.ServiceProvider))
        return _nextCommandHandler.Exec(ref pguidCmdGroup, nCmdId, nCmdexecopt, pvaIn, pvaOut);

      // make a copy of this so we can look at it after forwarding some commands
      uint commandId = nCmdId;
      char typedChar = char.MinValue;

      // make sure the input is a char before getting it
      if (pguidCmdGroup == VSConstants.VSStd2K && nCmdId == (uint)VSConstants.VSStd2KCmdID.TYPECHAR)
        typedChar = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);

      // check for a commit character
      if (nCmdId == (uint)VSConstants.VSStd2KCmdID.RETURN
          || nCmdId == (uint)VSConstants.VSStd2KCmdID.TAB
          || (char.IsWhiteSpace(typedChar) || char.IsPunctuation(typedChar)))
      {
        // check for a selection
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
      int retVal = _nextCommandHandler.Exec(ref pguidCmdGroup, nCmdId, nCmdexecopt, pvaIn, pvaOut);
      bool handled = false;

      if (!typedChar.Equals(char.MinValue) && char.IsLetterOrDigit(typedChar))
      {
        if (_session == null || _session.IsDismissed) // If there is no active session, bring up completion
        {
          TriggerCompletion();

          if (_session != null)
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
        if (commandId == (uint)VSConstants.VSStd2KCmdID.BACKSPACE //redo the filter if there is a deletion
            || commandId == (uint)VSConstants.VSStd2KCmdID.DELETE)
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

      var cp = (SnapshotPoint)caretPoint;
      int position = cp.Position;

      // check for "//" before the current cursor position in this line
      var line = ((SnapshotPoint)caretPoint).Snapshot.GetLineFromPosition(((SnapshotPoint)caretPoint).Position);

      // get the text of this line
      var text = line.GetText();
      // get only the text portion which is left of the actual position
      text = text.Substring(0,position - line.Start.Position);

      // if there is a comment starting, don't do auto completion
      if (text.IndexOf("//", StringComparison.CurrentCultureIgnoreCase) != -1)
        return;

      var lex = new Lexer(cp.Snapshot.GetText());
      lex.AnalyzeForCommentsOnly();

      var res = lex.Tokens.FirstOrDefault(x => x.Position < cp.Position && x.Position + x.Length >= cp.Position);

      if (res != null)
        return;

      _session = _provider.CompletionBroker.CreateCompletionSession(_textView, caretPoint.Value.Snapshot.CreateTrackingPoint(caretPoint.Value.Position, PointTrackingMode.Positive), true);

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
