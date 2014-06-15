#region Copyright © 2013 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompletionHandlerProvider.cs" company="Michael Reukauff">
//   Copyright © 2013 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

using Microsoft.VisualStudio.Text.Tagging;

namespace MichaelReukauff.Protobuf
{
  using System;
  using System.ComponentModel.Composition;

  using Microsoft.VisualStudio.Editor;
  using Microsoft.VisualStudio.Language.Intellisense;
  using Microsoft.VisualStudio.Shell;
  using Microsoft.VisualStudio.Text.Editor;
  using Microsoft.VisualStudio.TextManager.Interop;
  using Microsoft.VisualStudio.Utilities;

  [Export(typeof(IVsTextViewCreationListener))]
  [Name("protobuf token completion handler")]
  [ContentType(ProtobufLanguage.ContentType)]
  [TextViewRole(PredefinedTextViewRoles.Interactive)]
  internal class CompletionHandlerProvider : IVsTextViewCreationListener
  {
    [Import]
    internal IVsEditorAdaptersFactoryService AdapterService { get; set; }

    [Import]
    internal ICompletionBroker CompletionBroker { get; set; }

    [Import]
    internal SVsServiceProvider ServiceProvider { get; set; }

    [Import]
    internal IBufferTagAggregatorFactoryService AggregatorFactory { get; set; }

    /// <summary>
    /// Implement the VsTextViewCreated method to instantiate the command handler.
    /// </summary>
    /// <param name="textViewAdapter"></param>
    public void VsTextViewCreated(IVsTextView textViewAdapter)
    {
      ITextView textView = AdapterService.GetWpfTextView(textViewAdapter);
      if (textView == null)
        return;

      Func<CompletionHandler> createCommandHandler = () => new CompletionHandler(textViewAdapter, textView, this);
      textView.Properties.GetOrCreateSingletonProperty(createCommandHandler);
    }
  }
}
