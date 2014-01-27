#region Copyright © 2013 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Error.cs" company="Michael Reukauff">
//   Copyright © 2013 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace MichaelReukauff.Lexer
{
  using System.Diagnostics;
  using System.Text.RegularExpressions;

  [DebuggerDisplay("Text = {Text}, Pos = {Position}, Len = {Length}, Offset = {Offset}, Error = {Message}")]
  public class Error : Token
  {
    public string Message { get; set; }

    public int Offset { get; set; }

    public Error(int line, int position, int length, string text, CodeType codeType, string message)
      : base(line, position, length, text, codeType)
    {
      Message = message;
    }

    public Error(int line, Match match, CodeType codeType, string message)
      : this(line, match.Index, match.Length, match.Value, codeType, message)
    {
    }
  }
}
