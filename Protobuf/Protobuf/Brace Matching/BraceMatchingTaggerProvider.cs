#region Copyright © 2013 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BraceMatchingTaggerProvider.cs" company="Michael Reukauff">
//   Copyright © 2013 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace MichaelReukauff.Protobuf
{
  using System.ComponentModel.Composition;

  using Microsoft.VisualStudio.Text;
  using Microsoft.VisualStudio.Text.Editor;
  using Microsoft.VisualStudio.Text.Tagging;
  using Microsoft.VisualStudio.Utilities;

  [Export(typeof(IViewTaggerProvider))]
  [ContentType(ProtobufLanguage.ContentType)]
  [TagType(typeof(BraceMatchingTag))]
  internal class BraceMatchingTaggerProvider : IViewTaggerProvider
  {
    public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
    {
      if (textView == null)
        return null;

      // provide highlighting only on the top-level buffer
      if (textView.TextBuffer != buffer)
        return null;

      return new BraceMatchingTagger(textView, buffer) as ITagger<T>;
    }
  }
}
