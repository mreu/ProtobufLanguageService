#region Copyright © 2013 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuickInfoSourceProvider.cs" company="Michael Reukauff">
//   Copyright © 2013 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace MichaelReukauff.Protobuf
{
  using System.ComponentModel.Composition;

  using Microsoft.VisualStudio.Language.Intellisense;
  using Microsoft.VisualStudio.Text;
  using Microsoft.VisualStudio.Text.Tagging;
  using Microsoft.VisualStudio.Utilities;

  [Export(typeof(IQuickInfoSourceProvider))]
  [Name("Protobuf ToolTip QuickInfo Source")]
  [Order(Before = "Default Quick Info Presenter")]
  [ContentType(ProtobufLanguage.ContentType)]
  internal class QuickInfoSourceProvider : IQuickInfoSourceProvider
  {
    [Import]
    IBufferTagAggregatorFactoryService AggService { get; set; }

    /// <summary>
    /// Implement TryCreateQuickInfoSource to return a new QuickInfoSource.
    /// </summary>
    /// <param name="textBuffer"></param>
    /// <returns>The new IQuickInfoSource</returns>
    public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
    {
      return new QuickInfoSource(textBuffer, AggService.CreateTagAggregator<ProtobufTokenTag>(textBuffer));
    }
  }
}
