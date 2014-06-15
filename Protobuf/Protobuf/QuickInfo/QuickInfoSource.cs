#region Copyright © 2013 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuickInfoSource.cs" company="Michael Reukauff">
//   Copyright © 2013 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace MichaelReukauff.Protobuf
{
  using System.Collections.Generic;
  using System.Linq;

  using MichaelReukauff.Lexer;

  using Microsoft.VisualStudio.Language.Intellisense;
  using Microsoft.VisualStudio.Text;
  using Microsoft.VisualStudio.Text.Tagging;

  internal class QuickInfoSource : IQuickInfoSource
  {
    private readonly ITagAggregator<ProtobufTokenTag> _aggregator;
    private readonly ITextBuffer _buffer;

    private readonly IDictionary<string, string> _keywords;

    /// <summary>
    /// The constructor sets the QuickInfo source provider and the text buffer, and populates the set of method names, and method signatures and descriptions.
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="aggregator"></param>
    public QuickInfoSource(ITextBuffer buffer, ITagAggregator<ProtobufTokenTag> aggregator)
    {
      _aggregator = aggregator;
      _buffer = buffer;
      _keywords = new ProtobufWordListProvider().GetWordsWithDescription();
    }

    /// <summary>
    /// Implement the AugmentQuickInfoSession method.
    /// Here the method finds the current word, or the previous word if the cursor is at the end of a line or a text buffer.
    /// If the word is one of the method names, the description for that method name is added to the QuickInfo content.
    /// </summary>
    /// <param name="session"></param>
    /// <param name="quickInfoContent"></param>
    /// <param name="applicableToSpan"></param>
    public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> quickInfoContent, out ITrackingSpan applicableToSpan)
    {
      applicableToSpan = null;

      var triggerPoint = (SnapshotPoint)session.GetTriggerPoint(_buffer.CurrentSnapshot);

      // find each span that looks like a token and look it up in the dictionary
      foreach (IMappingTagSpan<ProtobufTokenTag> curTag in _aggregator.GetTags(new SnapshotSpan(triggerPoint, triggerPoint)))
      {
        if (curTag.Tag.CodeType == CodeType.Keyword)
        {
          SnapshotSpan tagSpan = curTag.Span.GetSpans(_buffer).First();
          if (_keywords.Keys.Contains(tagSpan.GetText()))
          {
            applicableToSpan = _buffer.CurrentSnapshot.CreateTrackingSpan(tagSpan, SpanTrackingMode.EdgeExclusive);
            quickInfoContent.Add(_keywords[tagSpan.GetText()]);
          }
        }
      }
    }

    public void Dispose()
    {
    }
  }
}
