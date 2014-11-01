#region Copyright © 2013 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Error.cs" company="Michael Reukauff">
//   Copyright © 2013 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

// ReSharper disable once CheckNamespace
namespace MichaelReukauff.Lexer
{
    using System.Diagnostics;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The error class.
    /// </summary>
    [DebuggerDisplay("Text = {Text}, Pos = {Position}, Len = {Length}, Offset = {Offset}, Error = {Message}")]
    public class Error : Token
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Error"/> class.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="position">The position.</param>
        /// <param name="length">The length.</param>
        /// <param name="text">The text.</param>
        /// <param name="codeType">The code type.</param>
        /// <param name="message">The message.</param>
        public Error(int line, int position, int length, string text, CodeType codeType, string message)
            : base(line, position, length, text, codeType)
        {
            Message = message;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Error"/> class.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="match">The match.</param>
        /// <param name="codeType">The code type.</param>
        /// <param name="message">The message.</param>
        public Error(int line, Match match, CodeType codeType, string message)
            : this(line, match.Index, match.Length, match.Value, codeType, message)
        {
        }
    }
}
