#region Copyright © 2014 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtobufClassifier.cs" company="Michael Reukauff">
//   Copyright © 2014 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

// ReSharper disable once CheckNamespace
namespace MichaelReukauff.Protobuf
{
    using System;
    using System.Collections.Generic;

    using Lexer;

    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Tagging;

    /// <summary>
    /// The protobuf classifier.
    /// </summary>
    internal sealed class ProtobufClassifier : ITagger<ClassificationTag>
    {
        /// <summary>
        /// The _buffer.
        /// </summary>
        public readonly ITextBuffer Buffer;

        /// <summary>
        /// The _aggregator.
        /// </summary>
        public readonly ITagAggregator<ProtobufTokenTag> Aggregator;

        /// <summary>
        /// The _output pane guid.
        /// </summary>
        private readonly Guid outputPaneGuid = new Guid("{7451EC7F-98F4-48F3-9600-78DDFD826BBC}");

        /// <summary>
        /// The output window name.
        /// </summary>
        private const string OutputWindowName = "Protobuf";

        /// <summary>
        /// The tags changed.
        /// </summary>
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        /// <summary>
        /// The _protobuf types.
        /// </summary>
        public readonly IDictionary<CodeType, IClassificationType> ProtobufTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtobufClassifier"/> class. 
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="aggregatorFactory">The aggregate factory.</param>
        /// <param name="typeService">The type service.</param>
        internal ProtobufClassifier(ITextBuffer buffer, IBufferTagAggregatorFactoryService aggregatorFactory, IClassificationTypeRegistryService typeService)
        {
            Buffer = buffer;
            Aggregator = aggregatorFactory.CreateTagAggregator<ProtobufTokenTag>(buffer);

            Aggregator.TagsChanged += AggregatorTagsChanged;

            var outputWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;

            if (outputWindow != null)
            {
                outputWindow.CreatePane(ref outputPaneGuid, OutputWindowName, 1, 1);
                IVsOutputWindowPane myOutputPane;
                outputWindow.GetPane(ref outputPaneGuid, out myOutputPane);
            }

            // create mapping from token types to classification types
            ProtobufTypes = new Dictionary<CodeType, IClassificationType>();
            ProtobufTypes[CodeType.Text] = typeService.GetClassificationType(ProtobufFormatDefinitions.Text);
            ProtobufTypes[CodeType.Keyword] = typeService.GetClassificationType(ProtobufFormatDefinitions.Keyword);
            ProtobufTypes[CodeType.Comment] = typeService.GetClassificationType(ProtobufFormatDefinitions.Comment);
            ProtobufTypes[CodeType.Identifier] = typeService.GetClassificationType(ProtobufFormatDefinitions.Identifier);
            ProtobufTypes[CodeType.String] = typeService.GetClassificationType(ProtobufFormatDefinitions.String);
            ProtobufTypes[CodeType.Number] = typeService.GetClassificationType(ProtobufFormatDefinitions.Number);
            ProtobufTypes[CodeType.Error] = typeService.GetClassificationType(ProtobufFormatDefinitions.Text);

            ProtobufTypes[CodeType.Enums] = typeService.GetClassificationType(ProtobufFormatDefinitions.Enum);
            ProtobufTypes[CodeType.SymDef] = typeService.GetClassificationType(ProtobufFormatDefinitions.SymDef);
            ProtobufTypes[CodeType.SymRef] = typeService.GetClassificationType(ProtobufFormatDefinitions.SymRef);
            ProtobufTypes[CodeType.FieldRule] = typeService.GetClassificationType(ProtobufFormatDefinitions.FieldRule);
            ProtobufTypes[CodeType.TopLevelCmd] = typeService.GetClassificationType(ProtobufFormatDefinitions.TopLevelCmd);
            ProtobufTypes[CodeType.Namespace] = typeService.GetClassificationType(ProtobufFormatDefinitions.Keyword);
        }

        /// <summary>
        /// The aggregator tags changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TagsChangedEventArgs"/>.</param>
        private void AggregatorTagsChanged(object sender, TagsChangedEventArgs e)
        {
            var temp = TagsChanged;
            if (temp != null)
            {
                NormalizedSnapshotSpanCollection spans = e.Span.GetSpans(Buffer.CurrentSnapshot);
                if (spans.Count > 0)
                {
                    var span = new SnapshotSpan(spans[0].Start, spans[spans.Count - 1].End);
                    temp(this, new SnapshotSpanEventArgs(span));
                }
            }
        }

        /// <summary>
        /// Translate each TokenColor to an appropriate ClassificationTag.
        /// </summary>
        /// <param name="spans">The spans.</param>
        /// <returns>The <see cref="IEnumerable{ITagSpan}"/>.</returns>
        public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var tagSpan in Aggregator.GetTags(spans))
            {
                var tagSpans = tagSpan.Span.GetSpans(spans[0].Snapshot);
                yield return new TagSpan<ClassificationTag>(tagSpans[0], new ClassificationTag(ProtobufTypes[tagSpan.Tag.CodeType]));
            }
        }
    }
}
