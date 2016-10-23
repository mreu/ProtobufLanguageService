// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BraceMatchingTaggerProvider.cs" company="Michael Reukauff, Germany">
//   Copyright © 2016 Michael Reukauff, Germany. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MichaelReukauff.Protobuf
{
    using System.ComponentModel.Composition;

    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    /// <summary>
    /// The brace matching tagger provider.
    /// </summary>
    [Export(typeof(IViewTaggerProvider))]
    [ContentType(ProtobufLanguage.ContentType)]
    [TagType(typeof(BraceMatchingTag))]
    internal class BraceMatchingTaggerProvider : IViewTaggerProvider
    {
        /// <summary>
        /// The create tagger.
        /// </summary>
        /// <param name="textView">The text view.</param>
        /// <param name="buffer">The buffer.</param>
        /// <typeparam name="T">The tagger.</typeparam>
        /// <returns>The <see cref="ITagger{T}"/>.</returns>
        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer)
            where T : ITag
        {
            if (textView == null)
            {
                return null;
            }

            // provide highlighting only on the top-level buffer
            if (textView.TextBuffer != buffer)
            {
                return null;
            }

            return new BraceMatchingTagger(textView, buffer) as ITagger<T>;
        }
    }
}
