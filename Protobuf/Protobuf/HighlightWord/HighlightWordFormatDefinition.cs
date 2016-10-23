// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HighlightWordFormatDefinition.cs" company="Michael Reukauff, Germany">
//   Copyright © 2016 Michael Reukauff, Germany. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace
namespace MichaelReukauff.Protobuf
{
    using System.ComponentModel.Composition;
    using System.Windows.Media;

    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;

    /// <summary>
    /// The highlight word format definition.
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [Name(ProtobufFormatDefinitions.Highlight)]
    [UserVisible(true)]
    internal class HighlightWordFormatDefinition : MarkerFormatDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HighlightWordFormatDefinition"/> class.
        /// In the constructor for HighlightWordFormatDefinition, define its display name and appearance.
        /// The Background property defines the fill color, while the Foreground property defines the border color.
        /// </summary>
        public HighlightWordFormatDefinition()
        {
            BackgroundColor = Color.FromRgb(14, 69, 131);
            ForegroundColor = Color.FromRgb(173, 192, 211);
            DisplayName = "ProtoBuf Highlight Word";
            ZOrder = 5;
        }
    }
}
