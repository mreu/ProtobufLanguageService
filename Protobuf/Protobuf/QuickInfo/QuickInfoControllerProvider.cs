// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuickInfoControllerProvider.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace
namespace MichaelReukauff.Protobuf
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Utilities;

    /// <summary>
    /// The quick info controller provider.
    /// </summary>
    [Export(typeof(IIntellisenseControllerProvider))]
    [Name("Protobuf QuickInfo Controller")]
    [ContentType(ProtobufLanguage.ContentType)]
    internal sealed class QuickInfoControllerProvider : IIntellisenseControllerProvider
    {
        /// <summary>
        /// Gets or sets the quick info broker.
        /// </summary>
        [Import]
        internal IQuickInfoBroker QuickInfoBroker { get; set; }

        /// <summary>
        /// Implement the TryCreateIntellisenseController method by instantiating the QuickInfo controller.
        /// </summary>
        /// <param name="textView">The text view.</param>
        /// <param name="subjectBuffers">The text buffer.</param>
        /// <returns>The new QuickInfoController.</returns>
        public IIntellisenseController TryCreateIntellisenseController(ITextView textView, IList<ITextBuffer> subjectBuffers)
        {
            return new QuickInfoController(textView, subjectBuffers, this);
        }
    }
}
