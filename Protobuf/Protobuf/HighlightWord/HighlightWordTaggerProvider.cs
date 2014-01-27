#region Copyright © 2013 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HighlightWordTaggerProvider.cs" company="Michael Reukauff">
//   Copyright © 2013 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace MichaelReukauff.Protobuf
{
  using System.ComponentModel.Composition;

  using Microsoft.VisualStudio.Text;
  using Microsoft.VisualStudio.Text.Editor;
  using Microsoft.VisualStudio.Text.Operations;
  using Microsoft.VisualStudio.Text.Tagging;
  using Microsoft.VisualStudio.Utilities;

  [Export(typeof(IViewTaggerProvider))]
  [ContentType(ProtobufLanguage.ContentType)]
  [TagType(typeof(TextMarkerTag))]
  internal class HighlightWordTaggerProvider : IViewTaggerProvider
  {
    [Import]
    internal ITextSearchService TextSearchService { get; set; }

    [Import]
    internal ITextStructureNavigatorSelectorService TextStructureNavigatorSelector { get; set; }

    /// <summary>
    /// Implement the CreateTagger method to return an instance of HighlightWordTagger.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="textView"></param>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
    {
      // provide highlighting only on the top buffer
      if (textView.TextBuffer != buffer)
        return null;

      ITextStructureNavigator textStructureNavigator = TextStructureNavigatorSelector.GetTextStructureNavigator(buffer);

      return new HighlightWordTagger(textView, buffer, TextSearchService, textStructureNavigator) as ITagger<T>;
    }
  }
}
