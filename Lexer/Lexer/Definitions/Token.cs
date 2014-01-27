#region Copyright © 2013 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Token.cs" company="Michael Reukauff">
//   Copyright © 2013 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace MichaelReukauff.Lexer
{
  using System.Diagnostics;
  using System.Text.RegularExpressions;

  [DebuggerDisplay("Text = {Text}, Pos = {Position}, Len = {Length}, Type = {CodeType}")]
  public class Token
  {
    /// <summary>
    /// Line number where the token was found (beginning with line 0)
    /// </summary>
    public int Line { get; set; }

    /// <summary>
    /// Position of the token in the line (starting with 0)
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// The length of the text
    /// </summary>
    public int Length { get; set; }

    /// <summary>
    /// Found text
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// The type
    /// </summary>
    public CodeType CodeType { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="line">Line number where the token was found (beginning with line 0)</param>
    /// <param name="position">Position of the token in the line (starting with 0)</param>
    /// <param name="length">The length of the text</param>
    /// <param name="text">Found text</param>
    /// <param name="codeType">The type of the token</param>
    public Token(int line, int position, int length, string text, CodeType codeType)
    {
      Line = line;
      Position = position;
      Length = length;
      Text = text;
      CodeType = codeType;
    }

    public Token(int line, Match match, CodeType codeType)
      : this(line, match.Index, match.Length, match.Value, codeType)
    {
    }
  }
}
