#region Copyright © 2013 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompletionSourceProvider.cs" company="Michael Reukauff">
//   Copyright © 2013 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace MichaelReukauff.Protobuf
{
  using System.ComponentModel.Composition;

  using Microsoft.VisualStudio.Language.Intellisense;
  using Microsoft.VisualStudio.Text;
  using Microsoft.VisualStudio.Text.Operations;
  using Microsoft.VisualStudio.Utilities;

  [Export(typeof(ICompletionSourceProvider))]
  [ContentType(ProtobufLanguage.ContentType)]
  [Name("protobuf token completion")]
  internal class CompletionSourceProvider : ICompletionSourceProvider
  {
    [Import]
    internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }
    
    public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
    {
      return new CompletionSource(NavigatorService, textBuffer);
    }
  }
}
