#region Copyright © 2014 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtobufTags.cs" company="Michael Reukauff">
//   Copyright © 2014 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

// ReSharper disable once CheckNamespace
namespace MichaelReukauff.Protobuf
{
    using Lexer;

    using Microsoft.VisualStudio.Text.Tagging;

    /// <summary>
    /// The normal Tag to classify the protobuf.
    /// </summary>
    public class ProtobufTokenTag : ITag
    {
        /// <summary>
        /// Gets the code type.
        /// </summary>
        public CodeType CodeType { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtobufTokenTag"/> class.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        public ProtobufTokenTag(CodeType type)
        {
            CodeType = type;
        }
    }

    /// <summary>
    /// The error Tag to show the red squiggle line and hold the error message.
    /// </summary>
    public class ProtobufErrorTag : ProtobufTokenTag
    {
        /// <summary>
        /// Gets the message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtobufErrorTag"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public ProtobufErrorTag(string message)
            : base(CodeType.Error)
        {
            Message = message;
        }
    }
}
