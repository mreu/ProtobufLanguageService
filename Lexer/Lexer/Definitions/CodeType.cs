#region Copyright © 2013 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeType.cs" company="Michael Reukauff">
//   Copyright © 2013 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

// ReSharper disable once CheckNamespace
namespace MichaelReukauff.Lexer
{
    /// <summary>
    /// The CodeType.
    /// </summary>
    public enum CodeType
    {
        /// <summary>
        /// Simply text.
        /// </summary>
        Text = 0,

        /// <summary>
        /// A keyword.
        /// </summary>
        Keyword = 1,

        /// <summary>
        /// A comment.
        /// </summary>
        Comment = 2,

        /// <summary>
        /// An identifier.
        /// </summary>
        Identifier = 3,

        /// <summary>
        /// A string.
        /// </summary>
        String = 4,

        /// <summary>
        /// A number.
        /// </summary>
        Number = 5,

        /// <summary>
        /// Enum fields.
        /// </summary>
        Enums = 6,

        /// <summary>
        /// Symbol definition.
        /// </summary>
        SymDef = 7,

        /// <summary>
        /// Symbol reference.
        /// </summary>
        SymRef = 8,

        /// <summary>
        /// Required, optional, repeated.
        /// </summary>
        FieldRule = 9,

        /// <summary>
        /// Package, import, enum, message, option, service.
        /// </summary>
        TopLevelCmd = 10,

        /// <summary>
        /// Name of the package.
        /// </summary>
        Namespace = 11,

        /// <summary>
        /// Error tag.
        /// </summary>
        Error = 12
    }
}
