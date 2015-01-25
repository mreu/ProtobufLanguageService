#region Copyright © 2015 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtoDescriptor.cs" company="Michael Reukauff">
//   Copyright © 2015 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace CompileProtobufToCSharp
{
    using System.Collections.Generic;

    /// <summary>
    /// The file descriptor set.
    /// </summary>
    public class FileDescriptorSet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileDescriptorSet"/> class.
        /// </summary>
        public FileDescriptorSet()
        {
            Files = new List<FileDescriptor>();
        }

        /// <summary>
        /// Gets the file descriptors.
        /// </summary>
        public List<FileDescriptor> Files { get; private set; }
    }

    /// <summary>
    /// The file descriptor.
    /// </summary>
    public class FileDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileDescriptor"/> class.
        /// </summary>
        public FileDescriptor()
        {
            Name = string.Empty;
            Package = string.Empty;
            Dependencies = new List<string>();
            PublicDependencies = new List<int>();
            WeakDependencies = new List<int>();
            MessageTypes = new List<MessageDescriptor>();
            EnumTypes = new List<EnumDescriptor>();
            Services = new List<ServiceDescriptor>();
            Extensions = new List<FieldDescriptor>();
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the package.
        /// </summary>
        public string Package { get; set; }

        /// <summary>
        /// Gets the dependencies.
        /// </summary>
        public List<string> Dependencies { get; private set; }

        /// <summary>
        /// Gets the public dependencies.
        /// </summary>
        public List<int> PublicDependencies { get; private set; }

        /// <summary>
        /// Gets the weak dependencies.
        /// </summary>
        public List<int> WeakDependencies { get; private set; }

        /// <summary>
        /// Gets the message types.
        /// </summary>
        public List<MessageDescriptor> MessageTypes { get; private set; }

        /// <summary>
        /// Gets the enum types.
        /// </summary>
        public List<EnumDescriptor> EnumTypes { get; private set; }

        /// <summary>
        /// Gets the services.
        /// </summary>
        public List<ServiceDescriptor> Services { get; private set; }

        /// <summary>
        /// Gets the extensions.
        /// </summary>
        public List<FieldDescriptor> Extensions { get; private set; }

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        public FileOptions Options { get; set; }

        /// <summary>
        /// Gets or sets the source code info.
        /// </summary>
        public SourceCodeInfo SourceCodeInfo { get; set; }
    }

    /// <summary>
    /// The message descriptor.
    /// </summary>
    public class MessageDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageDescriptor"/> class.
        /// </summary>
        public MessageDescriptor()
        {
            Name = string.Empty;
            Fields = new List<FieldDescriptor>();
            Extensions = new List<FieldDescriptor>();
            NestedTypes = new List<MessageDescriptor>();
            EnumTypes = new List<EnumDescriptor>();
            ExtensionRanges = new List<ExtensionRange>();
            OneofDecl = new List<OneofDescriptor>();
            Options = null;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the fields.
        /// </summary>
        public List<FieldDescriptor> Fields { get; private set; }

        /// <summary>
        /// Gets the extensions.
        /// </summary>
        public List<FieldDescriptor> Extensions { get; private set; }

        /// <summary>
        /// Gets the nested types.
        /// </summary>
        public List<MessageDescriptor> NestedTypes { get; private set; }

        /// <summary>
        /// Gets the enum types.
        /// </summary>
        public List<EnumDescriptor> EnumTypes { get; private set; }

        /// <summary>
        /// Gets the extension ranges.
        /// </summary>
        public List<ExtensionRange> ExtensionRanges { get; private set; }

        /// <summary>
        /// Gets the oneof decl.
        /// </summary>
        public List<OneofDescriptor> OneofDecl { get; private set; }

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        public MessageOptions Options { get; set; }

        /// <summary>
        /// The extension range.
        /// </summary>
        public class ExtensionRange
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ExtensionRange"/> class.
            /// </summary>
            public ExtensionRange()
            {
                Start = default(int);
                End = default(int);
            }

            /// <summary>
            /// Gets or sets the start.
            /// </summary>
            public int Start { get; set; }

            /// <summary>
            /// Gets or sets the end.
            /// </summary>
            public int End { get; set; }
        }
    }

    /// <summary>
    /// The field descriptor.
    /// </summary>
    public class FieldDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldDescriptor"/> class.
        /// </summary>
        public FieldDescriptor()
        {
            Name = string.Empty;
            Number = default(int);
            Label = Labels.LABEL_OPTIONAL;
            Type = Types.TYPE_DOUBLE;
            TypeName = string.Empty;
            Extendee = string.Empty;
            DefaultValue = string.Empty;
            OneofIndex = null;
            Options = null;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public Labels Label { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public Types Type { get; set; }

        /// <summary>
        /// Gets or sets the type name.
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Gets or sets the extendee.
        /// </summary>
        public string Extendee { get; set; }

        /// <summary>
        /// Gets or sets the default value.
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the oneof index.
        /// </summary>
        public int? OneofIndex { get; set; }

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        public FieldOptions Options { get; set; }

        /// <summary>
        /// The types.
        /// </summary>
        public enum Types
        {
            TYPE_DOUBLE = 1,
            TYPE_FLOAT = 2,
            TYPE_INT64 = 3,
            TYPE_UINT64 = 4,
            TYPE_INT32 = 5,
            TYPE_FIXED64 = 6,
            TYPE_FIXED32 = 7,
            TYPE_BOOL = 8,
            TYPE_STRING = 9,
            TYPE_GROUP = 10,
            TYPE_MESSAGE = 11,
            TYPE_BYTES = 12,
            TYPE_UINT32 = 13,
            TYPE_ENUM = 14,
            TYPE_SFIXED32 = 15,
            TYPE_SFIXED64 = 16,
            TYPE_SINT32 = 17,
            TYPE_SINT64 = 18
        }

        /// <summary>
        /// The labels.
        /// </summary>
        public enum Labels
        {
            LABEL_OPTIONAL = 1,
            LABEL_REQUIRED = 2,
            LABEL_REPEATED = 3
        }
    }

    /// <summary>
    /// The oneof descriptor.
    /// </summary>
    public class OneofDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OneofDescriptor"/> class.
        /// </summary>
        public OneofDescriptor()
        {
            Name = string.Empty;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// The enum descriptor.
    /// </summary>
    public class EnumDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumDescriptor"/> class.
        /// </summary>
        public EnumDescriptor()
        {
            Name = string.Empty;
            Values = new List<EnumValueDescriptor>();
            Options = null;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the enum values.
        /// </summary>
        public List<EnumValueDescriptor> Values { get; private set; }

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        public EnumOptions Options { get; set; }
    }

    /// <summary>
    /// The enum value descriptor.
    /// </summary>
    public class EnumValueDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumValueDescriptor"/> class.
        /// </summary>
        public EnumValueDescriptor()
        {
            Name = string.Empty;
            Number = default(int);
            Options = null;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        public EnumValueOptions Options { get; set; }
    }

    /// <summary>
    /// The service descriptor.
    /// </summary>
    public class ServiceDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceDescriptor"/> class.
        /// </summary>
        public ServiceDescriptor()
        {
            Name = string.Empty;
            Methods = new List<MethodDescriptor>();
            Options = null;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the methods.
        /// </summary>
        public List<MethodDescriptor> Methods { get; private set; }

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        public ServiceOptions Options { get; set; }
    }

    /// <summary>
    /// The method descriptor.
    /// </summary>
    public class MethodDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MethodDescriptor"/> class.
        /// </summary>
        public MethodDescriptor()
        {
            Name = string.Empty;
            InputType = string.Empty;
            OutputType = string.Empty;
            Options = null;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the input type.
        /// </summary>
        public string InputType { get; set; }

        /// <summary>
        /// Gets or sets the output type.
        /// </summary>
        public string OutputType { get; set; }

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        public MethodOptions Options { get; set; }
    }

    /// <summary>
    /// The file options.
    /// </summary>
    public class FileOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileOptions"/> class.
        /// </summary>
        public FileOptions()
        {
            JavaPackage = string.Empty;
            JavaOuterClassname = string.Empty;
            JavaMultipleFiles = false;
            JavaGenerateEqualsAndHash = false;
            JavaStringCheckUtf8 = false;
            OptimizeFor = OptimizeMode.SPEED;
            GoPackage = string.Empty;
            CcGenericServices = false;
            JavaGenericServices = false;
            PyGenericServices = false;
            Deprecated = false;
            UninterpretedOptions = new List<UninterpretedOption>();
        }

        /// <summary>
        /// Gets or sets the java package.
        /// </summary>
        public string JavaPackage { get; set; }

        /// <summary>
        /// Gets or sets the java outer classname.
        /// </summary>
        public string JavaOuterClassname { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether java multiple files.
        /// </summary>
        public bool JavaMultipleFiles { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether java generate equals and hash.
        /// </summary>
        public bool JavaGenerateEqualsAndHash { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether java string check utf 8.
        /// </summary>
        public bool JavaStringCheckUtf8 { get; set; }

        /// <summary>
        /// Gets or sets the optimize for.
        /// </summary>
        public OptimizeMode OptimizeFor { get; set; }

        /// <summary>
        /// Gets or sets the go package.
        /// </summary>
        public string GoPackage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether cc generic services.
        /// </summary>
        public bool CcGenericServices { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether java generic services.
        /// </summary>
        public bool JavaGenericServices { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether py generic services.
        /// </summary>
        public bool PyGenericServices { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the file is deprecated.
        /// </summary>
        public bool Deprecated { get; set; }

        /// <summary>
        /// Gets the uninterpreted options.
        /// </summary>
        public List<UninterpretedOption> UninterpretedOptions { get; private set; }

        /// <summary>
        /// The optimize mode.
        /// </summary>
        public enum OptimizeMode
        {
            SPEED = 1,
            CODE_SIZE = 2,
            LITE_RUNTIME = 3
        }
    }

    /// <summary>
    /// The message options.
    /// </summary>
    public class MessageOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageOptions"/> class.
        /// </summary>
        public MessageOptions()
        {
            MessageSetWireFormat = false;
            NoStandardDescriptorAccessor = false;
            Deprecated = false;
            UninterpretedOptions = new List<UninterpretedOption>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether message set wire format.
        /// </summary>
        public bool MessageSetWireFormat { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether no standard descriptor accessor.
        /// </summary>
        public bool NoStandardDescriptorAccessor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the message is deprecated.
        /// </summary>
        public bool Deprecated { get; set; }

        /// <summary>
        /// Gets the uninterpreted options.
        /// </summary>
        public List<UninterpretedOption> UninterpretedOptions { get; private set; }
    }

    /// <summary>
    /// The field options.
    /// </summary>
    public class FieldOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldOptions"/> class.
        /// </summary>
        public FieldOptions()
        {
            Ctype = CType.STRING;
            Packed = default(bool);
            Lazy = false;
            Deprecated = false;
            ExperimentalMapKey = string.Empty;
            Weak = false;
            UninterpretedOptions = new List<UninterpretedOption>();
        }

        /// <summary>
        /// Gets or sets the ctype.
        /// </summary>
        public CType Ctype { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether packed.
        /// </summary>
        public bool Packed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether lazy.
        /// </summary>
        public bool Lazy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the field is deprecated.
        /// </summary>
        public bool Deprecated { get; set; }

        /// <summary>
        /// Gets or sets the experimental map key.
        /// </summary>
        public string ExperimentalMapKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether weak.
        /// </summary>
        public bool Weak { get; set; }

        /// <summary>
        /// Gets the uninterpreted options.
        /// </summary>
        public List<UninterpretedOption> UninterpretedOptions { get; private set; }

        /// <summary>
        /// The c type.
        /// </summary>
        public enum CType
        {
            STRING = 0,
            CORD = 1,
            STRING_PIECE = 2
        }
    }

    /// <summary>
    /// The enum options.
    /// </summary>
    public class EnumOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumOptions"/> class.
        /// </summary>
        public EnumOptions()
        {
            AllowAlias = default(bool);
            Deprecated = false;
            UninterpretedOptions = new List<UninterpretedOption>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether allow alias.
        /// </summary>
        public bool AllowAlias { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the enum is deprecated.
        /// </summary>
        public bool Deprecated { get; set; }

        /// <summary>
        /// Gets the uninterpreted options.
        /// </summary>
        public List<UninterpretedOption> UninterpretedOptions { get; private set; }
    }

    /// <summary>
    /// The enum value options.
    /// </summary>
    public class EnumValueOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumValueOptions"/> class.
        /// </summary>
        public EnumValueOptions()
        {
            UninterpretedOptions = new List<UninterpretedOption>();
            Deprecated = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the enum value is deprecated.
        /// </summary>
        public bool Deprecated { get; set; }

        /// <summary>
        /// Gets the uninterpreted options.
        /// </summary>
        public List<UninterpretedOption> UninterpretedOptions { get; private set; }
    }

    /// <summary>
    /// The service options.
    /// </summary>
    public class ServiceOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceOptions"/> class.
        /// </summary>
        public ServiceOptions()
        {
            UninterpretedOptions = new List<UninterpretedOption>();
            Deprecated = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the service is deprecated.
        /// </summary>
        public bool Deprecated { get; set; }

        /// <summary>
        /// Gets the uninterpreted options.
        /// </summary>
        public List<UninterpretedOption> UninterpretedOptions { get; private set; }
    }

    /// <summary>
    /// The method options.
    /// </summary>
    public class MethodOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MethodOptions"/> class.
        /// </summary>
        public MethodOptions()
        {
            UninterpretedOptions = new List<UninterpretedOption>();
            Deprecated = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the method is deprecated.
        /// </summary>
        public bool Deprecated { get; set; }

        /// <summary>
        /// Gets the uninterpreted options.
        /// </summary>
        public List<UninterpretedOption> UninterpretedOptions { get; private set; }
    }

    /// <summary>
    /// The uninterpreted option.
    /// </summary>
    public class UninterpretedOption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UninterpretedOption"/> class.
        /// </summary>
        public UninterpretedOption()
        {
            NegativeIntValue = default(long);
            PositiveIntValue = default(ulong);
            IdentifierValue = string.Empty;
            Names = new List<NamePart>();
            DoubleValue = default(double);
            StringValue = null;
            AggregateValue = string.Empty;
        }

        /// <summary>
        /// Gets the names.
        /// </summary>
        public List<NamePart> Names { get; private set; }

        /// <summary>
        /// Gets or sets the identifier value.
        /// </summary>
        public string IdentifierValue { get; set; }

        /// <summary>
        /// Gets or sets the positive int value.
        /// </summary>
        public ulong PositiveIntValue { get; set; }

        /// <summary>
        /// Gets or sets the negative int value.
        /// </summary>
        public long NegativeIntValue { get; set; }

        /// <summary>
        /// Gets or sets the double value.
        /// </summary>
        public double DoubleValue { get; set; }

        /// <summary>
        /// Gets or sets the string value.
        /// </summary>
        public byte[] StringValue { get; set; }

        /// <summary>
        /// Gets or sets the aggregate value.
        /// </summary>
        public string AggregateValue { get; set; }

        /// <summary>
        /// The name part.
        /// </summary>
        public class NamePart
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="NamePart"/> class.
            /// </summary>
            public NamePart()
            {
                Namepart = string.Empty;
                IsExtension = null;
            }

            /// <summary>
            /// Gets or sets the namepart.
            /// </summary>
            public string Namepart { get; set; }

            /// <summary>
            /// Gets or sets the is extension.
            /// </summary>
            public bool? IsExtension { get; set; }
        }
    }

    /// <summary>
    /// The source code info.
    /// </summary>
    public class SourceCodeInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceCodeInfo"/> class.
        /// </summary>
        public SourceCodeInfo()
        {
            Locations = new List<Location>();
        }

        /// <summary>
        /// Gets the locations.
        /// </summary>
        public List<Location> Locations { get; private set; }

        /// <summary>
        /// The location.
        /// </summary>
        public class Location
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Location"/> class.
            /// </summary>
            public Location()
            {
                Path = new List<int>();
                Span = new List<int>();
                LeadingComments = string.Empty;
                TrailingComments = string.Empty;
            }

            /// <summary>
            /// Gets the path.
            /// </summary>
            public List<int> Path { get; private set; }

            /// <summary>
            /// Gets the span.
            /// </summary>
            public List<int> Span { get; private set; }

            /// <summary>
            /// Gets or sets the leading comments.
            /// </summary>
            public string LeadingComments { get; set; }

            /// <summary>
            /// Gets or sets the trailing comments.
            /// </summary>
            public string TrailingComments { get; set; }
        }
    }
}
