#region Copyright © 2014 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtobufLanguageTaggerProvider.cs" company="Michael Reukauff">
//   Copyright © 2014 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

// ReSharper disable once CheckNamespace
namespace MichaelReukauff.Protobuf
{
    using System;
    using System.ComponentModel.Composition;

    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    /// <summary>
    /// The protobuf languag tagger provider.
    /// </summary>
    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(ProtobufTokenTag))]
    [ContentType(ProtobufLanguage.ContentType)]
    internal sealed class ProtobufLanguagTaggerProvider : ITaggerProvider
    {
        /// <summary>
        /// The create tagger.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <typeparam name="T">The ITagger.</typeparam>
        /// <returns>
        /// The <see cref="ITagger{T}"/>.
        /// </returns>
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            // create a single tagger for each buffer.
            Func<ITagger<T>> sc = () => new ProtobufLanguageTagger(buffer) as ITagger<T>;
            return buffer.Properties.GetOrCreateSingletonProperty(sc);
        }
    }
}
