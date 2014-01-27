#region Copyright © 2013 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuickInfoControllerProvider.cs" company="Michael Reukauff">
//   Copyright © 2013 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace MichaelReukauff.Protobuf
{
  using System.Collections.Generic;
  using System.ComponentModel.Composition;

  using Microsoft.VisualStudio.Language.Intellisense;
  using Microsoft.VisualStudio.Text;
  using Microsoft.VisualStudio.Text.Editor;
  using Microsoft.VisualStudio.Utilities;

  [Export(typeof(IIntellisenseControllerProvider))]
  [Name("Protobuf QuickInfo Controller")]
  [ContentType(ProtobufLanguage.ContentType)]
  internal sealed class QuickInfoControllerProvider : IIntellisenseControllerProvider
  {
    [Import]
    internal IQuickInfoBroker QuickInfoBroker { get; set; }

    /// <summary>
    /// Implement the TryCreateIntellisenseController method by instantiating the QuickInfo controller.
    /// </summary>
    /// <param name="textView"></param>
    /// <param name="subjectBuffers"></param>
    /// <returns>The new QuickInfoController</returns>
    public IIntellisenseController TryCreateIntellisenseController(ITextView textView, IList<ITextBuffer> subjectBuffers)
    {
      return new QuickInfoController(textView, subjectBuffers, this);
    }
  }
}
