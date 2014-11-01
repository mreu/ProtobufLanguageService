#region Copyright © 2014 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BraceMatchingTag.cs" company="Michael Reukauff">
//   Copyright © 2014 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace MichaelReukauff.Protobuf
{
    using Microsoft.VisualStudio.Text.Tagging;

    /// <summary>
    /// The brace matching tag.
    /// </summary>
    internal class BraceMatchingTag : TextMarkerTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BraceMatchingTag"/> class. 
        /// In the constructor, pass in the name of the format definition.
        /// </summary>
        public BraceMatchingTag()
            : base(ProtobufFormatDefinitions.BraceMatching)
        {
        }
    }
}
