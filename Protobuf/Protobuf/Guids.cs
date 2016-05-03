// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Guids.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MichaelReukauff.Protobuf
{
    // Guids.cs
    // MUST match guids.h
    using System;

    /// <summary>
    /// The guid list class.
    /// </summary>
    static class GuidList
    {
#pragma warning disable SA1303 // Const field names must begin with upper-case letter
        /// <summary>
        /// The guid protobuf pkg string (const). Value: "5d89ec18-2175-4dc0-861a-9718edfa0be1".
        /// </summary>
        public const string guidProtobufPkgString = "5d89ec18-2175-4dc0-861a-9718edfa0be1";

        /// <summary>
        /// The guid protobuf cmd set string (const). Value: "5048df81-256d-4c07-bfe0-ebd0c4cfa657".
        /// </summary>
        public const string guidProtobufCmdSetString = "5048df81-256d-4c07-bfe0-ebd0c4cfa657";

        /// <summary>
        /// The guid protobuf cmd set (readonly). Value: new Guid(guidProtobufCmdSetString).
        /// </summary>
        public static readonly Guid guidProtobufCmdSet = new Guid(guidProtobufCmdSetString);
#pragma warning restore SA1303 // Const field names must begin with upper-case letter
    }
}