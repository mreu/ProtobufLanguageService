// Guids.cs
// MUST match guids.h
using System;


namespace MichaelReukauff.Protobuf
{
    static class GuidList
    {
        public const string guidProtobufPkgString = "5d89ec18-2175-4dc0-861a-9718edfa0be1";
        public const string guidProtobufCmdSetString = "5048df81-256d-4c07-bfe0-ebd0c4cfa657";

        public static readonly Guid guidProtobufCmdSet = new Guid(guidProtobufCmdSetString);
    };
}