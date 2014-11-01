#region Copyright © 2014 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtobufErrorTaggerProvider.cs" company="Michael Reukauff">
//   Copyright © 2014 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

// ReSharper disable once CheckNamespace
namespace MichaelReukauff.Protobuf
{
    using System;
    using System.ComponentModel.Composition;

    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    /// <summary>
    /// The protobuf error tagger provider.
    /// </summary>
    [Export(typeof(ITaggerProvider))]
    [ContentType("protobuf")]
    [TagType(typeof(ErrorTag))]
    internal sealed class ProtobufErrorTaggerProvider : ITaggerProvider
    {
        /// <summary>
        /// Gets or sets the service provider.
        /// </summary>
        [Import(typeof(SVsServiceProvider))]
        internal IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Gets or sets the aggregator factory.
        /// </summary>
        [Import]
        internal IBufferTagAggregatorFactoryService AggregatorFactory { get; set; }

        /// <summary>
        /// Gets or sets the text document factory.
        /// </summary>
        [Import]
        internal ITextDocumentFactoryService TextDocumentFactory { get; set; }

        /// <summary>
        /// The create tagger.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <typeparam name="T">The ITagger.</typeparam>
        /// <returns>The <see cref="ITagger{T}"/>.</returns>
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            // create a single tagger for each buffer.
            Func<ITagger<T>> sc = () => new ErrorTagger(buffer, AggregatorFactory, ServiceProvider, TextDocumentFactory) as ITagger<T>;
            return buffer.Properties.GetOrCreateSingletonProperty(sc);
        }
    }
}
