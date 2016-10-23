// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuickInfoSourceProvider.cs" company="Michael Reukauff, Germany">
//   Copyright © 2016 Michael Reukauff, Germany. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace
namespace MichaelReukauff.Protobuf
{
    using System.ComponentModel.Composition;

    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    /// <summary>
    /// The quick info source provider.
    /// </summary>
    [Export(typeof(IQuickInfoSourceProvider))]
    [Name("Protobuf ToolTip QuickInfo Source")]
    [Order(Before = "Default Quick Info Presenter")]
    [ContentType(ProtobufLanguage.ContentType)]
    internal class QuickInfoSourceProvider : IQuickInfoSourceProvider
    {
        /// <summary>
        /// Gets or sets the agg service.
        /// </summary>
        [Import]
        public IBufferTagAggregatorFactoryService AggService { get; set; }

        /// <summary>
        /// Implement TryCreateQuickInfoSource to return a new QuickInfoSource.
        /// </summary>
        /// <param name="textBuffer">
        /// The text buffer.
        /// </param>
        /// <returns>
        /// The new IQuickInfoSource.
        /// </returns>
        public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
        {
            return new QuickInfoSource(textBuffer, AggService.CreateTagAggregator<ProtobufTokenTag>(textBuffer));
        }
    }
}
