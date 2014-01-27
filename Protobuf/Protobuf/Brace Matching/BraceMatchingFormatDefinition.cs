#region Copyright © 2013 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BraceMatchingFormatDefinition.cs" company="Michael Reukauff">
//   Copyright © 2013 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace MichaelReukauff.Protobuf
{
  using System.ComponentModel.Composition;
  using System.Windows.Media;

  using Microsoft.VisualStudio.Text.Classification;
  using Microsoft.VisualStudio.Utilities;

  [Export(typeof(EditorFormatDefinition))]
  [Name(ProtobufFormatDefinitions.BraceMatching)]
  [UserVisible(true)]
  internal class BraceMatchingFormatDefinition : MarkerFormatDefinition
  {
    /// <summary>
    /// In the constructor for HighlightWordFormatDefinition, define its display name and appearance.
    /// The Background property defines the fill color, while the Foreground property defines the border color.
    /// </summary>
    public BraceMatchingFormatDefinition()
    {
      BackgroundColor = Color.FromRgb(14, 69, 131);
      ForegroundColor = Color.FromRgb(173, 192, 211);
      DisplayName = "ProtoBuf Brace Matching";
      ZOrder = 5;
    }
  }
}
