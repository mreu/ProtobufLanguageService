// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtobufWordListProvider.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace
namespace MichaelReukauff.Protobuf
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The protobuf word list provider.
    /// </summary>
    internal class ProtobufWordListProvider
    {
        /// <summary>
        /// The _list.
        /// </summary>
        private readonly List<ProtobufToken> list = new List<ProtobufToken>
    {
      // first level keywords (+ service below)
      new ProtobufToken("package", "Use package to prevent name clashes between protocol message types."),

      new ProtobufToken("import", "Importing another protobuf"),
      new ProtobufToken("message", "A protobuf message"),
      new ProtobufToken("enum", "An enumeration"),
      new ProtobufToken("extend", "Extend a message"),
      new ProtobufToken("oneof", "OneOf"),

      // field rules
      new ProtobufToken("required", "A well-formed message must have exactly one of this field."),
      new ProtobufToken("optional", "A well-formed message can have zero or one of this field (but not more than one)."),
      new ProtobufToken("repeated", "This field can be repeated any number of times (including zero) in a well-formed message. The order of the repeated values will be preserved."),

      // data types
      new ProtobufToken("double", "A double field"),
      new ProtobufToken("float", "A float field"),
      new ProtobufToken("uint32", "Uses variable-length encoding. 32 bit."),
      new ProtobufToken("uint64", "Uses variable-length encoding. 64 bit."),
      new ProtobufToken("sint32", "Uses variable-length encoding. Signed int value. These more efficiently encode negative numbers than regular int32s."),
      new ProtobufToken("sint64", "Uses variable-length encoding. Signed int value. These more efficiently encode negative numbers than regular int64s."),
      new ProtobufToken("int32", "Uses variable-length encoding. Inefficient for encoding negative numbers – if your field is likely to have negative values, use sint32 instead."),
      new ProtobufToken("int64", "Uses variable-length encoding. Inefficient for encoding negative numbers – if your field is likely to have negative values, use sint64 instead."),
      new ProtobufToken("sfixed32", "Always four bytes."),
      new ProtobufToken("sfixed64", "Always eight bytes."),
      new ProtobufToken("fixed32", "Always four bytes. More efficient than uint32 if values are often greater than 2^28."),
      new ProtobufToken("fixed64", "Always eight bytes. More efficient than uint64 if values are often greater than 2^56."),
      new ProtobufToken("bool", "A boolean field."),
      new ProtobufToken("string", "A string must always contain UTF-8 encoded or 7-bit ASCII text."),
      new ProtobufToken("bytes", "May contain any arbitrary sequence of bytes."),

      // service keywords
      new ProtobufToken("service", "Defines a RPC service."),
      new ProtobufToken("rpc", "Defines a RPC service."),
      new ProtobufToken("returns", "Defines the return message."),

      // keywords
      new ProtobufToken("default", "Default value"),
      new ProtobufToken("public", "This import is public"),
      new ProtobufToken("extensions", "This extend a message"),
      new ProtobufToken("syntax", "The syntax of the protobuf language used. Must be \"prot2\" or \"proto3\""),

      // Literals
      new ProtobufToken("true", "A boolean"),
      new ProtobufToken("false", "A boolean"),

      // options
      new ProtobufToken("option", "An option"),
      new ProtobufToken("allow_alias", "Allow alias in enums"),

      new ProtobufToken("java_package", "The package you want to use for your generated Java classes"),
      new ProtobufToken("java_outer_classname", "The class name for the outermost Java class."),
      new ProtobufToken("optimize_for", "This affects the code generators."),
      new ProtobufToken("SPEED", "The protocol buffer compiler will generate code for serializing, parsing, and performing other common operations on your message types. This code is extremely highly optimized"),
      new ProtobufToken("CODE_SIZE", "The protocol buffer compiler will generate minimal classes and will rely on shared, reflection-based code to implement serialialization, parsing, and various other operations."),
      new ProtobufToken("LITE_RUNTIME", " The protocol buffer compiler will generate classes that depend only on the \"lite\" runtime library."),
      new ProtobufToken("cc_generic_services", "Whether or not the protocol buffer compiler should generate abstract service code."),
      new ProtobufToken("java_generic_services", "Whether or not the protocol buffer compiler should generate abstract service code."),
      new ProtobufToken("py_generic_services", "Whether or not the protocol buffer compiler should generate abstract service code."),
      new ProtobufToken("message_set_wire_format", "If set to true, the message uses a different binary format intended to be compatible with an old format used inside Google called MessageSet. Users outside Google will probably never need to use this option."),
      new ProtobufToken("packed", "If set to true on a repeated field of a basic integer type, a more compact encoding will be used."),
      new ProtobufToken("deprecated", " If set to true, indicates that the files, service, enum, message, method or enum value is deprecated and should not be used by new code"),
    };

        #region internal methods
        /// <summary>
        /// The get words with description.
        /// </summary>
        /// <param name="append">The append.</param>
        /// <returns>The <see cref="IDictionary{TKey,TValue}"/>.</returns>
        internal IDictionary<string, string> GetWordsWithDescription(char append)
        {
            return list.ToDictionary(x => x.Name + append, x => x.Description);
        }

        /// <summary>
        /// The get words with description.
        /// </summary>
        /// <returns>
        /// The <see cref="IDictionary{TKey,TValue}"/>.
        /// </returns>
        internal IDictionary<string, string> GetWordsWithDescription()
        {
            return list.ToDictionary(x => x.Name, x => x.Description);
        }
        #endregion internal methods
    }

    #region ProtobufToken
    /// <summary>
    /// The Protobuf Token with description.
    /// </summary>
    internal class ProtobufToken
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        internal string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        internal string Description { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtobufToken"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        internal ProtobufToken(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
    #endregion ProtobufToken
}
