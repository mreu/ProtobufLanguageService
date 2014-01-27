#region Copyright © 2013 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtobufClassifier.cs" company="Michael Reukauff">
//   Copyright © 2013 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace MichaelReukauff.Protobuf
{
  using System;
  using System.Collections.Generic;

  using MichaelReukauff.Lexer;

  using Microsoft.VisualStudio.Shell;
  using Microsoft.VisualStudio.Shell.Interop;
  using Microsoft.VisualStudio.Text;
  using Microsoft.VisualStudio.Text.Classification;
  using Microsoft.VisualStudio.Text.Tagging;

  internal sealed class ProtobufClassifier : ITagger<ClassificationTag>
  {
    readonly ITextBuffer _buffer;

    readonly ITagAggregator<ProtobufTokenTag> _aggregator;

    private readonly Guid _outputPaneGuid = new Guid("{7451EC7F-98F4-48F3-9600-78DDFD826BBC}");
    private const string _outputWindowName = "Protobuf";

    public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

    readonly IDictionary<CodeType, IClassificationType> _protobufTypes;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="buffer">The buffer</param>
    /// <param name="aggregatorFactory"></param>
    /// <param name="typeService"></param>
    internal ProtobufClassifier(ITextBuffer buffer, IBufferTagAggregatorFactoryService aggregatorFactory, IClassificationTypeRegistryService typeService)
    {
      IVsOutputWindowPane myOutputPane;
      _buffer = buffer;
      _aggregator = aggregatorFactory.CreateTagAggregator<ProtobufTokenTag>(buffer);

      _aggregator.TagsChanged += _aggregator_TagsChanged;

      IVsOutputWindow outputWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;

      if (outputWindow != null)
      {
        outputWindow.CreatePane(ref _outputPaneGuid, _outputWindowName, 1, 1);
        outputWindow.GetPane(ref _outputPaneGuid, out myOutputPane);
      }

      // create mapping from token types to classification types
      _protobufTypes = new Dictionary<CodeType, IClassificationType>();
      _protobufTypes[CodeType.Text] = typeService.GetClassificationType(ProtobufFormatDefinitions.Text);
      _protobufTypes[CodeType.Keyword] = typeService.GetClassificationType(ProtobufFormatDefinitions.Keyword);
      _protobufTypes[CodeType.Comment] = typeService.GetClassificationType(ProtobufFormatDefinitions.Comment);
      _protobufTypes[CodeType.Identifier] = typeService.GetClassificationType(ProtobufFormatDefinitions.Identifier);
      _protobufTypes[CodeType.String] = typeService.GetClassificationType(ProtobufFormatDefinitions.String);
      _protobufTypes[CodeType.Number] = typeService.GetClassificationType(ProtobufFormatDefinitions.Number);
      _protobufTypes[CodeType.Error] = typeService.GetClassificationType(ProtobufFormatDefinitions.Text);

      _protobufTypes[CodeType.Enums] = typeService.GetClassificationType(ProtobufFormatDefinitions.Enum);
      _protobufTypes[CodeType.SymDef] = typeService.GetClassificationType(ProtobufFormatDefinitions.SymDef);
      _protobufTypes[CodeType.SymRef] = typeService.GetClassificationType(ProtobufFormatDefinitions.SymRef);
      _protobufTypes[CodeType.FieldRule] = typeService.GetClassificationType(ProtobufFormatDefinitions.FieldRule);
      _protobufTypes[CodeType.TopLevelCmd] = typeService.GetClassificationType(ProtobufFormatDefinitions.TopLevelCmd);
      _protobufTypes[CodeType.Namespace] = typeService.GetClassificationType(ProtobufFormatDefinitions.Keyword);
    }

    void _aggregator_TagsChanged(object sender, TagsChangedEventArgs e)
    {
      var temp = TagsChanged;
      if (temp != null)
      {
        NormalizedSnapshotSpanCollection spans = e.Span.GetSpans(_buffer.CurrentSnapshot);
        if (spans.Count > 0)
        {
          SnapshotSpan span = new SnapshotSpan(spans[0].Start, spans[spans.Count - 1].End);
          temp(this, new SnapshotSpanEventArgs(span));
        }
      }
    }

    /// <summary>
    /// Translate each TokenColor to an appropriate ClassificationTag
    /// </summary>
    public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
    {
      foreach (var tagSpan in _aggregator.GetTags(spans))
      {
        var tagSpans = tagSpan.Span.GetSpans(spans[0].Snapshot);
        yield return new TagSpan<ClassificationTag>(tagSpans[0], new ClassificationTag(_protobufTypes[tagSpan.Tag._type]));
      }
    }
  }
}
