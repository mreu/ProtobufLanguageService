#region Copyright © 2013 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuickInfoController.cs" company="Michael Reukauff">
//   Copyright © 2013 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace MichaelReukauff.Protobuf
{
  using System.Collections.Generic;

  using Microsoft.VisualStudio.Language.Intellisense;
  using Microsoft.VisualStudio.Text;
  using Microsoft.VisualStudio.Text.Editor;

  internal class QuickInfoController : IIntellisenseController
  {
    private ITextView _textView;
    private readonly IList<ITextBuffer> _subjectBuffers;
    private readonly QuickInfoControllerProvider _componentContext;
    private IQuickInfoSession _session;

    /// <summary>
    /// The constructor sets the fields and adds the mouse hover event handler.
    /// </summary>
    /// <param name="textView"></param>
    /// <param name="subjectBuffers"></param>
    /// <param name="provider"></param>
    internal QuickInfoController(ITextView textView, IList<ITextBuffer> subjectBuffers, QuickInfoControllerProvider provider)
    {
      _textView = textView;
      _subjectBuffers = subjectBuffers;
      _componentContext = provider;

      _textView.MouseHover += OnTextViewMouseHover;
    }

    /// <summary>
    /// Add the mouse hover event handler that triggers the QuickInfo session.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnTextViewMouseHover(object sender, MouseHoverEventArgs e)
    {
      //find the mouse position by mapping down to the subject buffer
      SnapshotPoint? point = GetMousePosition(new SnapshotPoint(_textView.TextSnapshot, e.Position));

      if (point != null)
      {
        ITrackingPoint triggerPoint = point.Value.Snapshot.CreateTrackingPoint(point.Value.Position, PointTrackingMode.Positive);

        // Find the broker for this buffer
        if (!_componentContext.QuickInfoBroker.IsQuickInfoActive(_textView))
        {
          _session = _componentContext.QuickInfoBroker.CreateQuickInfoSession(_textView, triggerPoint, true);
          _session.Start();
        }
      }
    }

    /// <summary>
    /// Get mouse position
    /// </summary>
    /// <param name="topPosition"></param>
    /// <returns></returns>
    private SnapshotPoint? GetMousePosition(SnapshotPoint topPosition)
    {
      // Map this point down to the appropriate subject buffer.
      return _textView.BufferGraph.MapDownToFirstMatch
          (
          topPosition,
          PointTrackingMode.Positive,
          snapshot => _subjectBuffers.Contains(snapshot.TextBuffer),
          PositionAffinity.Predecessor
          );
    }

    /// <summary>
    /// Implement the Detach method so that it removes the mouse hover event handler when the controller is detached from the text view.
    /// </summary>
    /// <param name="textView"></param>
    public void Detach(ITextView textView)
    {
      if (_textView == textView)
      {
        _textView.MouseHover -= OnTextViewMouseHover;
        _textView = null;
      }
    }

    /// <summary>
    /// Implement the ConnectSubjectBuffer method as empty method here.
    /// </summary>
    /// <param name="subjectBuffer"></param>
    public void ConnectSubjectBuffer(ITextBuffer subjectBuffer)
    {
    }

    /// <summary>
    /// Implement the DisconnectSubjectBuffer method as empty method here.
    /// </summary>
    /// <param name="subjectBuffer"></param>
    public void DisconnectSubjectBuffer(ITextBuffer subjectBuffer)
    {
    }
  }
}
