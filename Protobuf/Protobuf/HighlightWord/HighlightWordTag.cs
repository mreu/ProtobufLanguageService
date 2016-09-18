// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HighlightWordTag.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace
namespace MichaelReukauff.Protobuf
{
  using Microsoft.VisualStudio.Text.Tagging;

    /// <summary>
    /// The highlight word tag.
    /// </summary>
    public class HighlightWordTag : TextMarkerTag
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="HighlightWordTag"/> class.
    /// In the constructor, pass in the name of the format definition.
    /// </summary>
    public HighlightWordTag()
      : base(ProtobufFormatDefinitions.Highlight)
    {
    }
  }
}
