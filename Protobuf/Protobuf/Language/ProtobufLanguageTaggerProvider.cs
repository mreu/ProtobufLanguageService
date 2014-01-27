#region Copyright © 2013 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtobufLanguageProvider.cs" company="Michael Reukauff">
//   Copyright © 2013 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace MichaelReukauff.Protobuf
{
  using System;
  using System.ComponentModel.Composition;

  using Microsoft.VisualStudio.Text;
  using Microsoft.VisualStudio.Text.Tagging;
  using Microsoft.VisualStudio.Utilities;

  [Export(typeof(ITaggerProvider))]
  [TagType(typeof(ProtobufTokenTag))]
  [ContentType(ProtobufLanguage.ContentType)]
  internal sealed class ProtobufLanguagTaggerProvider : ITaggerProvider
  {
    public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
    {
      // create a single tagger for each buffer.
      Func<ITagger<T>> sc = () => new ProtobufLanguageTagger(buffer) as ITagger<T>;
      return buffer.Properties.GetOrCreateSingletonProperty(sc);
    }
  }
}
