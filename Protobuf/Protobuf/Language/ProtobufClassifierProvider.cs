// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtobufClassifierProvider.cs" company="Michael Reukauff, Germany">
//   Copyright © 2016 Michael Reukauff, Germany. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace
namespace MichaelReukauff.Protobuf
{
    using System;
    using System.ComponentModel.Composition;

    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    /// <summary>
    /// The protobuf classifier provider.
    /// </summary>
    [Export(typeof(ITaggerProvider))]
    [ContentType(ProtobufLanguage.ContentType)]
    [Name("ProtobufSyntaxProvider")]
    [TagType(typeof(ClassificationTag))]
    internal sealed class ProtobufClassifierProvider : ITaggerProvider
    {
        /// <summary>
        /// Gets or sets the ClassificationTypeRegistry.
        /// Import the classification registry to be used for getting a reference
        /// to the custom classification type later.
        /// </summary>
        [Import]
        internal IClassificationTypeRegistryService ClassificationTypeRegistry { get; set; }

        /// <summary>
        /// Gets or sets the aggregator factory.
        /// </summary>
        [Import]
        internal IBufferTagAggregatorFactoryService AggregatorFactory { get; set; }

        /// <summary>
        /// Gets or sets the content type registry service.
        /// </summary>
        [Import]
        internal IContentTypeRegistryService ContentTypeRegistryService { get; set; }

        /// <summary>
        /// The create tagger.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <typeparam name="T">The ITagger.</typeparam>
        /// <returns>The <see cref="ITagger{T}"/>.</returns>
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer)
            where T : ITag
        {
            // create a single tagger for each buffer.
            Func<ITagger<T>> sc = () => new ProtobufClassifier(buffer, AggregatorFactory, ClassificationTypeRegistry) as ITagger<T>;

            // ReSharper disable once UnusedVariable
            var ct = ContentTypeRegistryService.ContentTypes;

            return buffer.Properties.GetOrCreateSingletonProperty(sc);
        }
    }
}
