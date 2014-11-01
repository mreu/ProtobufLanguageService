#region Copyright © 2014 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtobufClassificationTypes.cs" company="Michael Reukauff">
//   Copyright © 2014 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

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
        internal static ClassificationTypeDefinition ProtobufEnum = null;

        /// <summary>
        /// Defines the "Protobuf FieldRule" classification type.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ProtobufFormatDefinitions.FieldRule)]
        internal static ClassificationTypeDefinition ProtobufFieldRule = null;

        /// <summary>
        /// Defines the "Protobuf TopLevelCmd" classification type.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ProtobufFormatDefinitions.TopLevelCmd)]
        internal static ClassificationTypeDefinition ProtobufTopLevelCmd = null;
    }
}

