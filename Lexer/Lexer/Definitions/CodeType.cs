#region Copyright © 2013 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeType.cs" company="Michael Reukauff">
//   Copyright © 2013 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace MichaelReukauff.Lexer
{
  public enum CodeType
  {
    Text = 0,         // simply text
    Keyword = 1,      // a keyword
    Comment = 2,      // a comment
    Identifier = 3,   // an identifier
    String = 4,       // a string
    Number = 5,       // a number
    Enums = 6,        // Enum fields
    SymDef = 7,       // symbol definition
    SymRef = 8,       // symbol reference
    FieldRule = 9,    // required, optional, repeated
    TopLevelCmd = 10, // package, import, enum, message, option, service
    Namespace = 11,   // name of the package
    Error = 12,       // error tag
  }
}
