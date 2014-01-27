#region Copyright © 2013 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompletionSource.cs" company="Michael Reukauff">
//   Copyright © 2013 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace MichaelReukauff.Protobuf
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  using Microsoft.VisualStudio.Language.Intellisense;
  using Microsoft.VisualStudio.Text;
  using Microsoft.VisualStudio.Text.Operations;

  internal class CompletionSource : ICompletionSource
  {
    private readonly ITextBuffer _buffer;
    private readonly ITextStructureNavigatorSelectorService _navigatorSelectorService;
    private readonly IDictionary<string, string> _keywords;
    private bool _isDisposed;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="navigatorSelectorService"></param>
    /// <param name="buffer">The text buffer</param>
    public CompletionSource(ITextStructureNavigatorSelectorService navigatorSelectorService, ITextBuffer buffer)
    {
      _buffer = buffer;
      _navigatorSelectorService = navigatorSelectorService;
      _keywords = new ProtobufWordListProvider().GetWordsWithDescription();
    }

    void IDisposable.Dispose()
    {
      if (!_isDisposed)
      {
        GC.SuppressFinalize(this);
        _isDisposed = true;
      }
    }

    /// <summary>
    /// Implement the AugmentCompletionSession method by adding a completion set that contains the completions you want to provide in the context.
    /// Each completion set contains a set of Completion completions, and corresponds to a tab of the completion window.
    /// The FindTokenSpanAtPosition method is defined in the next step.
    /// </summary>
    /// <param name="session"></param>
    /// <param name="completionSets"></param>
    void ICompletionSource.AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
    {
      // create a list of completions from the dictionary of valid tokens
      List<Completion> completions = new List<Completion>();
      foreach (KeyValuePair<string, string> token in _keywords)
      {
        completions.Add(new Completion(token.Key, token.Key, token.Value, null, null));
      }

      ITextSnapshot snapshot = _buffer.CurrentSnapshot;
      var triggerPoint = (SnapshotPoint)session.GetTriggerPoint(snapshot);

      //var line = triggerPoint.GetContainingLine();
      SnapshotPoint start = triggerPoint - 1;

      var res = FindTokenSpanAtPosition(session);

      completionSets.Add(new CompletionSet("Protobuf keywords", "Protobuf keywords", res, completions, Enumerable.Empty<Completion>()));
    }

    /// <summary>
    /// Find the current word from the position of the cursor
    /// </summary>
    /// <param name="session"></param>
    /// <returns></returns>
    private ITrackingSpan FindTokenSpanAtPosition(ICompletionSession session)
    {
      SnapshotPoint currentPoint = (session.TextView.Caret.Position.BufferPosition) - 1;
      ITextStructureNavigator navigator = _navigatorSelectorService.GetTextStructureNavigator(_buffer);
      TextExtent extent = navigator.GetExtentOfWord(currentPoint);
      return currentPoint.Snapshot.CreateTrackingSpan(extent.Span, SpanTrackingMode.EdgeInclusive);
    }
  }
}
