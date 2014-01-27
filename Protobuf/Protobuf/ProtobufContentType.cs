#region Copyright © 2013 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtobufContentType.cs" company="Michael Reukauff">
//   Copyright © 2013 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace MichaelReukauff.Protobuf
{
  using System.ComponentModel.Composition;

  using Microsoft.VisualStudio.Utilities;

  internal static class ProtobufLanguage
  {
    public const string ContentType = "protobuf";

    public const string FileExtension = ".proto";

    [Export]
    [Name(ContentType)]
    [BaseDefinition("code")]
    internal static ContentTypeDefinition ProtobufContentTypeDefinition = null;

    [Export]
    [FileExtension(FileExtension)]
    [ContentType(ContentType)]
    internal static FileExtensionToContentTypeDefinition ProtobufSyntaxFileExtensionDefinition = null;
  }
}
