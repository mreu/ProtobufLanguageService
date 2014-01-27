#region Copyright © 2013 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HighlightWordTag.cs" company="Michael Reukauff">
//   Copyright © 2013 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace MichaelReukauff.Protobuf
{
  using Microsoft.VisualStudio.Text.Tagging;

  internal class HighlightWordTag : TextMarkerTag
  {
    /// <summary>
    /// In the constructor, pass in the name of the format definition.
    /// </summary>
    public HighlightWordTag()
      : base(ProtobufFormatDefinitions.Highlight)
    {
    }
  }
}
