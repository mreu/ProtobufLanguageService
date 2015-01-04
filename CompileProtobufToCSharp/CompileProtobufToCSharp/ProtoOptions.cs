#region Copyright © 2015 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtoOptions.cs" company="Michael Reukauff">
//   Copyright © 2015 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace CompileProtobufToCSharp
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    /// The proto options.
    /// </summary>
    internal class ProtoOptions
    {
        /// <summary>
        /// Get the options from the proto files.
        /// </summary>
        /// <param name="protoFile">
        /// The path to the proto File.
        /// </param>
        /// <remarks>
        /// Options in the proto files must be the first lines in the file and must begin with "//#". The options must follow separated by semicolons.
        /// Options can be spread over more then one line. The first line that does not begin with "//#" stops reading the options.
        /// Unknown options are ignored. NO error or waring is thrown. todo: change this behaviour and display error in Error List
        /// Options for the protoc.exe must be entered in one line beginning with "//#protoc=". The text following the = is sent to the proc.exe as paramaeter.
        /// </remarks>
        internal void ParseOptions(string protoFile)
        {
            using (var file = File.OpenText(protoFile))
            {
                while (true)
                {
                    var line = file.ReadLine();
                    if (line == null)
                    {
                        break;
                    }

                    if (!line.StartsWith("//#"))
                    {
                        break;
                    }

                    if (line.StartsWith("//#protoc="))
                    {
                        ProtocParms = line.Substring(10);
                        continue;
                    }

                    var opts = line.Substring(3).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var opt in opts)
                    {
                        switch (opt.Trim().ToLower())
                        {
                            case "xml":
                                Xml = true;
                                break;
                            case "binary":
                                Binary = true;
                                break;
                            case "datacontract":
                                DataContract = true;
                                break;
                            case "protorpc": // this option is obsolete
                                ProtoRpc = true;
                                break;
                            case "observable":
                                Observable = true;
                                break;
                            case "preobservable":
                                PreObservable = true;
                                break;
                            case "partialmethods":
                                PartialMethods = true;
                                break;
                            case "detectmissing":
                                DetectMissing = true;
                                break;
                            case "lightframework":
                                LightFramework = true;
                                break;
                            case "asynchronous":
                                Asynchronous = true;
                                break;
                            case "clientproxy":
                                ClientProxy = true;
                                break;
                            case "import":
                                ////Import = true;
                                break;
                            case "fixcase":
                                FixCase = true;
                                break;
                            case "noconstructor":
                                NoConstructor = true;
                                break;
                            case "treatobsoleteaserror":
                                TreatObsoleteAsError = true;
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get the options as code comments.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>. The options.
        /// </returns>
        internal string GetOptionsAsComment()
        {
            var sb = new StringBuilder();

            sb.AppendLine("// Code generated with the following options:");

            if (Xml)
            {
                sb.AppendLine("//  Xml");
            }

            if (DataContract)
            {
                sb.AppendLine("//  DataContract");
            }

            if (Binary)
            {
                sb.AppendLine("//  Binary");
            }

            if (ProtoRpc)
            {
                sb.AppendLine("//  ProtoRpc --> This option is obsolete and not supported anymore by protobuf-net.");
            }

            if (Observable)
            {
                sb.AppendLine("//  Observable");
            }

            if (PreObservable)
            {
                sb.AppendLine("//  PreObservable");
            }

            if (PartialMethods)
            {
                sb.AppendLine("//  PartialMethods");
            }

            if (DetectMissing)
            {
                sb.AppendLine("//  DetectMissing");
            }

            if (LightFramework)
            {
                sb.AppendLine("//  LightFramework");
            }

            if (Asynchronous)
            {
                sb.AppendLine("//  Asynchronous");
            }

            if (ClientProxy)
            {
                sb.AppendLine("//  ClientProxy");
            }

            if (FixCase)
            {
                sb.AppendLine("//  FixCase");
            }

            if (NoConstructor)
            {
                sb.AppendLine("//  NoConstructor");
            }

            if (TreatObsoleteAsError)
            {
                sb.AppendLine("//  TreatObsoleteAsError");
            }

            if (!string.IsNullOrEmpty(ProtocParms))
            {
                sb.AppendLine("//  ProtocParms=" + ProtocParms);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets a value indicating whether explicit xml support is enabled (XmlSerializer).
        /// </summary>
        /// <remarks>
        /// Used by protobuf-net of Marc Gravell.
        /// </remarks>
        internal bool Xml { get; private set; }

        /// <summary>
        /// Gets a value indicating whether data-contract support (DataContractSerializer; requires .NET 3.0) is enabled.
        /// </summary>
        /// <remarks>
        /// Used by protobuf-net of Marc Gravell.
        /// </remarks>
        internal bool DataContract { get; private set; }

        /// <summary>
        /// Gets a value indicating whether binary support (BinaryFormatter; not supported on Silverlight) is enabled.
        /// </summary>
        /// <remarks>
        /// Used by protobuf-net of Marc Gravell.
        /// </remarks>
        internal bool Binary { get; private set; }

        /// <summary>
        /// Gets a value indicating whether proto-rpc client is enabled.
        /// </summary>
        /// <remarks>
        /// Used by protobuf-net of Marc Gravell.
        /// </remarks>
        internal bool ProtoRpc { get; private set; }

        /// <summary>
        /// Gets a value indicating whether change notification (observer pattern) support is emitted.
        /// </summary>
        /// <remarks>
        /// Used by protobuf-net of Marc Gravell.
        /// </remarks>
        internal bool Observable { get; private set; }

        /// <summary>
        /// Gets a value indicating whether pre-change notification (observer pattern) support (requires .NET 3.5) is emitted.
        /// </summary>
        /// <remarks>
        /// Used by protobuf-net of Marc Gravell.
        /// </remarks>
        internal bool PreObservable { get; private set; }

        /// <summary>
        /// Gets a value indicating whether partial methods for changes (requires C# 3.0) are provided.
        /// </summary>
        /// <remarks>
        /// Used by protobuf-net of Marc Gravell.
        /// </remarks>
        internal bool PartialMethods { get; private set; }

        /// <summary>
        /// Gets a value indicating whether *Specified properties to indicate whether fields are present is provided.
        /// </summary>
        /// <remarks>
        /// Used by protobuf-net of Marc Gravell.
        /// </remarks>
        internal bool DetectMissing { get; private set; }

        /// <summary>
        /// Gets a value indicating whether additional attributes not included in CF/Silverlight are omitted.
        /// </summary>
        /// <remarks>
        /// Used by protobuf-net of Marc Gravell.
        /// </remarks>
        internal bool LightFramework { get; private set; }

        /// <summary>
        /// Gets a value indicating whether an asynchronous methods for use with WCF should be emitted.
        /// </summary>
        /// <remarks>
        /// Used by protobuf-net of Marc Gravell.
        /// </remarks>
        internal bool Asynchronous { get; private set; }

        /// <summary>
        /// Gets a value indicating whether an asynchronous client proxy class is emitted.
        /// </summary>
        /// <remarks>
        /// Used by protobuf-net of Marc Gravell.
        /// </remarks>
        internal bool ClientProxy { get; private set; }

        /// <summary>
        /// Gets a value indicating whether Additional namespaces to import (semicolon delimited). todo: ???? how to do this
        /// </summary>
        /// <remarks>
        /// Used by protobuf-net of Marc Gravell.
        /// </remarks>
        ////internal bool Import { get; private set; }

        /// <summary>
        /// Gets a value indicating whether types/properties become PascalCase; fields become camelCase.
        /// </summary>
        /// <remarks>
        /// Used by protobuf-net of Marc Gravell.
        /// </remarks>
        internal bool FixCase { get; private set; }

        /// <summary>
        /// Gets a value indicating whether a constructor should be build or not.
        /// </summary>
        internal bool NoConstructor { get; private set; }

        /// <summary>
        /// Gets a value indicating whether obsolence is treated as error.
        /// </summary>
        internal bool TreatObsoleteAsError { get; private set; }

        /// <summary>
        /// Gets the protoc parms.
        /// </summary>
        /// <remarks>
        /// Used by protobuf-net of Marc Gravell and protobuf-csharp of Jon Skeet.
        /// </remarks>
        internal string ProtocParms { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the whole protobuf is obsolete.
        /// </summary>
        internal bool IsObsolete { get; set; }
    }
}
