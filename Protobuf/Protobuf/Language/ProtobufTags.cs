#region Copyright © 2013 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtobufTags.cs" company="Michael Reukauff">
//   Copyright © 2013 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace MichaelReukauff.Protobuf
{
  using Lexer;

  using Microsoft.VisualStudio.Text.Tagging;

  /// <summary>
  /// The normal Tag to classify the protobuf
  /// </summary>
  public class ProtobufTokenTag : ITag
  {
    public CodeType CodeType { get; private set; }

    public ProtobufTokenTag(CodeType type)
    {
      CodeType = type;
    }
  }

  /// <summary>
  /// The error Tag to show the red squiggle line and hold the error message
  /// </summary>
  public class ProtobufErrorTag : ProtobufTokenTag
  {
    public string Message { get; private set; }

    public ProtobufErrorTag(string message)
      : base(CodeType.Error)
    {
      Message = message;
    }
  }
}
