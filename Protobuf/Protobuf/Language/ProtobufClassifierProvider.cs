#region Copyright © 2013 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtobufClassifierProvider.cs" company="Michael Reukauff">
//   Copyright © 2013 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace MichaelReukauff.Protobuf
{
  using System;
  using System.ComponentModel.Composition;

  using Microsoft.VisualStudio.Text;
  using Microsoft.VisualStudio.Text.Classification;
  using Microsoft.VisualStudio.Text.Tagging;
  using Microsoft.VisualStudio.Utilities;

  [Export(typeof(ITaggerProvider))]
  [ContentType(ProtobufLanguage.ContentType)]
  [Name("ProtobufSyntaxProvider")]
  [TagType(typeof(ClassificationTag))]
  internal sealed class ProtobufClassifierProvider : ITaggerProvider
  {
    /// <summary>
    /// Import the classification registry to be used for getting a reference
    /// to the custom classification type later.
    /// </summary>
    [Import]
    internal IClassificationTypeRegistryService ClassificationTypeRegistry { get; set; }

    [Import]
    internal IBufferTagAggregatorFactoryService AggregatorFactory { get; set; }

    [Import]
    internal IContentTypeRegistryService ContentTypeRegistryService { get; set; }

    public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
    {
      // create a single tagger for each buffer.
      Func<ITagger<T>> sc = () => new ProtobufClassifier(buffer, AggregatorFactory, ClassificationTypeRegistry) as ITagger<T>;

      var ct = ContentTypeRegistryService.ContentTypes;

      return buffer.Properties.GetOrCreateSingletonProperty(sc);
    }
  }
}
