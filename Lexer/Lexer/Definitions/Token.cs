// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Token.cs" company="Michael Reukauff, Germany">
//   Copyright © 2016 Michael Reukauff, Germany. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace
namespace MichaelReukauff.Lexer
{
    using System.Diagnostics;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The token class.
    /// </summary>
    [DebuggerDisplay("Text = {Text}, Pos = {Position}, Len = {Length}, Type = {CodeType}")]
    public class Token
    {
        /// <summary>
        /// Gets or sets the line number where the token was found (beginning with line 0).
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// Gets or sets the position of the token in the line (starting with 0).
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Gets or sets the length of the text.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Gets or sets the found text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the code type.
        /// </summary>
        public CodeType CodeType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class.
        /// </summary>
        /// <param name="line">Line number where the token was found (beginning with line 0).</param>
        /// <param name="position">Position of the token in the line (starting with 0).</param>
        /// <param name="length">The length of the text.</param>
        /// <param name="text">Found text.</param>
        /// <param name="codeType">The type of the token.</param>
        public Token(int line, int position, int length, string text, CodeType codeType)
        {
            Line = line;
            Position = position;
            Length = length;
            Text = text;
            CodeType = codeType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="match">The match.</param>
        /// <param name="codeType">The code type.</param>
        public Token(int line, Match match, CodeType codeType)
            : this(line, match.Index, match.Length, match.Value, codeType)
        {
        }
    }
}
