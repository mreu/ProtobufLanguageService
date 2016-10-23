// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutliningTaggerProvider.cs" company="Michael Reukauff, Germany">
//   Copyright © 2016 Michael Reukauff, Germany. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MichaelReukauff.Protobuf.Outlining
{
    using System;
    using System.ComponentModel.Composition;

    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    /// <summary>
    /// The outlining tagger provider.
    /// </summary>
    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IOutliningRegionTag))]
    [ContentType(ProtobufLanguage.ContentType)]
    internal sealed class OutliningTaggerProvider : ITaggerProvider
    {
        /// <summary>
        /// Implement the CreateTagger&lt;T&gt; method by adding an OutliningTagger to the properties of the buffer.
        /// </summary>
        /// <typeparam name="T">The ITagger.</typeparam>
        /// <param name="buffer">The buffer.</param>
        /// <returns>The <see cref="ITagger{t}"/>.</returns>
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer)
            where T : ITag
        {
            // create a single tagger for each buffer.
            Func<ITagger<T>> sc = () => new OutliningTagger(buffer) as ITagger<T>;
            return buffer.Properties.GetOrCreateSingletonProperty(sc);
        }
    }
}
