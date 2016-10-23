// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HighlightWordTaggerProvider.cs" company="Michael Reukauff, Germany">
//   Copyright © 2016 Michael Reukauff, Germany. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace
namespace MichaelReukauff.Protobuf
{
    using System.ComponentModel.Composition;

    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Operations;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    /// <summary>
    /// The highlight word tagger provider.
    /// </summary>
    [Export(typeof(IViewTaggerProvider))]
    [ContentType(ProtobufLanguage.ContentType)]
    [TagType(typeof(TextMarkerTag))]
    internal class HighlightWordTaggerProvider : IViewTaggerProvider
    {
        /// <summary>
        /// Gets or sets the text search service.
        /// </summary>
        [Import]
        internal ITextSearchService TextSearchService { get; set; }

        /// <summary>
        /// Gets or sets the text structure navigator selector.
        /// </summary>
        [Import]
        internal ITextStructureNavigatorSelectorService TextStructureNavigatorSelector { get; set; }

        /// <summary>
        /// Implement the CreateTagger method to return an instance of HighlightWordTagger.
        /// </summary>
        /// <typeparam name="T">The tagger.</typeparam>
        /// <param name="textView">The text view.</param>
        /// <param name="buffer">The text buffer.</param>
        /// <returns>An ITagger.</returns>
        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer)
            where T : ITag
        {
            // provide highlighting only on the top buffer
            if (textView.TextBuffer != buffer)
            {
                return null;
            }

            var textStructureNavigator = TextStructureNavigatorSelector.GetTextStructureNavigator(buffer);

            return new HighlightWordTagger(textView, buffer, TextSearchService, textStructureNavigator) as ITagger<T>;
        }
    }
}
