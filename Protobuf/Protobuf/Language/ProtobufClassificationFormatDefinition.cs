// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtobufClassificationFormatDefinition.cs" company="Michael Reukauff, Germany">
//   Copyright © 2016 Michael Reukauff, Germany. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace
namespace MichaelReukauff.Protobuf
{
    using System.ComponentModel.Composition;
    using System.Windows.Media;

    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;

    /// <summary>
    /// The protobuf enum.
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ProtobufFormatDefinitions.Enum)]
    [Name(ProtobufFormatDefinitions.Enum)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)] // set the priority to be after the default classifiers
    internal sealed class ProtobufEnum : ClassificationFormatDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProtobufEnum"/> class.
        /// </summary>
        public ProtobufEnum()
        {
            IsBold = false;
            ForegroundColor = Colors.Olive;
            DisplayName = "Protobuf Enum";
        }
    }

    /// <summary>
    /// The protobuf field rule.
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ProtobufFormatDefinitions.FieldRule)]
    [Name(ProtobufFormatDefinitions.FieldRule)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)] // set the priority to be after the default classifiers
    internal sealed class ProtobufFieldRule : ClassificationFormatDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProtobufFieldRule"/> class.
        /// </summary>
        public ProtobufFieldRule()
        {
            IsBold = false;
            ForegroundColor = Colors.Orchid;
            DisplayName = "Protobuf Field Rule";
        }
    }

    /// <summary>
    /// The protobuf top level cmd.
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ProtobufFormatDefinitions.TopLevelCmd)]
    [Name(ProtobufFormatDefinitions.TopLevelCmd)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)] // set the priority to be after the default classifiers
    internal sealed class ProtobufTopLevelCmd : ClassificationFormatDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProtobufTopLevelCmd"/> class.
        /// </summary>
        public ProtobufTopLevelCmd()
        {
            IsBold = false;
            ForegroundColor = Colors.DodgerBlue;
            DisplayName = "Protobuf Top Level Cmd";
        }
    }
}
