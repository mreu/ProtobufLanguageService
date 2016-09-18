// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtobufContentType.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MichaelReukauff.Protobuf
{
    using System.ComponentModel.Composition;

    using Microsoft.VisualStudio.Utilities;

    /// <summary>
    /// The protobuf language class.
    /// </summary>
    internal static class ProtobufLanguage
    {
        /// <summary>
        /// The content type (const). Value: "protobuf".
        /// </summary>
        public const string ContentType = "protobuf";

        /// <summary>
        /// The file extension (const). Value: ".proto".
        /// </summary>
        public const string FileExtension = ".proto";

        /// <summary>
        /// The protobuf content type definition.
        /// </summary>
        [Export]
        [Name(ContentType)]
        [BaseDefinition("code")]
        internal static ContentTypeDefinition ProtobufContentTypeDefinition;

        /// <summary>
        /// The protobuf syntax file extension definition.
        /// </summary>
        [Export]
        [FileExtension(FileExtension)]
        [ContentType(ContentType)]
        internal static FileExtensionToContentTypeDefinition ProtobufSyntaxFileExtensionDefinition;
    }
}
