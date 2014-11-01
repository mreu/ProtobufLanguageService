#region Copyright © 2014 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompletionSourceProvider.cs" company="Michael Reukauff">
//   Copyright © 2014 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

// ReSharper disable once CheckNamespace
namespace MichaelReukauff.Protobuf
{
    using System.ComponentModel.Composition;

    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Operations;
    using Microsoft.VisualStudio.Utilities;

    /// <summary>
    /// The completion source provider.
    /// </summary>
    [Export(typeof(ICompletionSourceProvider))]
    [ContentType(ProtobufLanguage.ContentType)]
    [Name("protobuf token completion")]
    internal class CompletionSourceProvider : ICompletionSourceProvider
    {
        /// <summary>
        /// Gets or sets the navigator service.
        /// </summary>
        [Import]
        internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }

        /// <summary>
        /// The try create completion source.
        /// </summary>
        /// <param name="textBuffer">
        /// The text buffer.
        /// </param>
        /// <returns>
        /// The <see cref="ICompletionSource"/>.
        /// </returns>
        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            return new CompletionSource(NavigatorService, textBuffer);
        }
    }
}
