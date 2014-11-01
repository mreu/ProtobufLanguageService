#region Copyright © 2014 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompletionHandlerProvider.cs" company="Michael Reukauff">
//   Copyright © 2014 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

// ReSharper disable once CheckNamespace
namespace MichaelReukauff.Protobuf
{
    using System;
    using System.ComponentModel.Composition;

    using Microsoft.VisualStudio.Editor;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Microsoft.VisualStudio.Utilities;

    /// <summary>
    /// The completion handler provider.
    /// </summary>
    [Export(typeof(IVsTextViewCreationListener))]
    [Name("protobuf token completion handler")]
    [ContentType(ProtobufLanguage.ContentType)]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    internal class CompletionHandlerProvider : IVsTextViewCreationListener
    {
        /// <summary>
        /// Gets or sets the adapter service.
        /// </summary>
        [Import]
        internal IVsEditorAdaptersFactoryService AdapterService { get; set; }

        /// <summary>
        /// Gets or sets the completion broker.
        /// </summary>
        [Import]
        internal ICompletionBroker CompletionBroker { get; set; }

        /// <summary>
        /// Gets or sets the service provider.
        /// </summary>
        [Import]
        internal SVsServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Gets or sets the aggregator factory.
        /// </summary>
        [Import]
        internal IBufferTagAggregatorFactoryService AggregatorFactory { get; set; }

        /// <summary>
        /// Implement the VsTextViewCreated method to instantiate the command handler.
        /// </summary>
        /// <param name="textViewAdapter">The text view adapter.</param>
        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            ITextView textView = AdapterService.GetWpfTextView(textViewAdapter);
            if (textView == null)
            {
                return;
            }

            Func<CompletionHandler> createCommandHandler = () => new CompletionHandler(textViewAdapter, textView, this);
            textView.Properties.GetOrCreateSingletonProperty(createCommandHandler);
        }
    }
}
