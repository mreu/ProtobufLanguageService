// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtobufClassificationTypes.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MichaelReukauff.Protobuf.Language
{
    using System.ComponentModel.Composition;
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;

    /// <summary>
    /// The protobuf classification types.
    /// </summary>
    internal static class ProtobufClassificationTypes
    {
        /// <summary>
        /// Defines the "Protobuf Enum" classification type.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ProtobufFormatDefinitions.Enum)]
        internal static ClassificationTypeDefinition ProtobufEnum;

        /// <summary>
        /// Defines the "Protobuf FieldRule" classification type.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ProtobufFormatDefinitions.FieldRule)]
        internal static ClassificationTypeDefinition ProtobufFieldRule;

        /// <summary>
        /// Defines the "Protobuf TopLevelCmd" classification type.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ProtobufFormatDefinitions.TopLevelCmd)]
        internal static ClassificationTypeDefinition ProtobufTopLevelCmd;
    }
}

