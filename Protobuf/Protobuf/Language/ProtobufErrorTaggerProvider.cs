#region Copyright © 2013 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtobufErrorTaggerProvider.cs" company="Michael Reukauff">
//   Copyright © 2013 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace MichaelReukauff.Protobuf
{
  using System;
  using System.ComponentModel.Composition;

  using Microsoft.VisualStudio.Shell;
  using Microsoft.VisualStudio.Text;
  using Microsoft.VisualStudio.Text.Tagging;
  using Microsoft.VisualStudio.Utilities;

  [Export(typeof(ITaggerProvider))]
  [ContentType("protobuf")]
  [TagType(typeof(ErrorTag))]
  internal sealed class ProtobufErrorTaggerProvider : ITaggerProvider
  {
    [Import(typeof(SVsServiceProvider))]
    internal IServiceProvider _serviceProvider { get; set; }

    [Import]
    internal IBufferTagAggregatorFactoryService _aggregatorFactory { get; set; }

    [Import]
    ITextDocumentFactoryService _textDocumentFactory { get; set; }

    public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
    {
      // create a single tagger for each buffer.
      Func<ITagger<T>> sc = () => new ErrorTagger(buffer, _aggregatorFactory, _serviceProvider, _textDocumentFactory) as ITagger<T>;
      return buffer.Properties.GetOrCreateSingletonProperty(sc);
    }
  }
}
