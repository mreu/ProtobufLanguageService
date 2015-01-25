#region Copyright © 2014 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Compile.cs" company="Michael Reukauff">
//   Copyright © 2014 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace CompileProtobufToCSharp
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// The compile.
    /// </summary>
    internal class Compile
    {
        /// <summary>
        /// Googles protobuf compiler.
        /// </summary>
        private const string Protoc = "protoc.exe";

        /// <summary>
        /// The path to the proto file in the visual studio project.
        /// </summary>
        private string protoFile;

        /// <summary>
        /// The temporary folder to copy the protobuf file to and the protobuf compiler.
        /// </summary>
        private string tempFolder;

        /// <summary>
        /// The errormessage.
        /// </summary>
        private string errormessage;

        /// <summary>
        /// The infomessage.
        /// </summary>
        private string infomessage;

        /// <summary>
        /// The protoc version.
        /// </summary>
        private string protocVersion;

        /// <summary>
        /// The intend level.
        /// </summary>
        private int intend;

        /// <summary>
        /// The current proto.
        /// </summary>
        private FileDescriptor currentProto;

        /// <summary>
        /// The options of the proto file.
        /// </summary>
        private readonly ProtoOptions options = new ProtoOptions();

        /// <summary>
        /// Run the whole process.
        /// </summary>
        /// <param name="proto">
        /// The path to the protbuf file.
        /// </param>
        public void Run(string proto)
        {
            protoFile = proto;

            try
            {
                CopyFile();
                options.ParseOptions(protoFile);
                if (PrepareProtoc())
                {
                    GetProtocVersion();
                    if (CompileProto())
                    {
                        // GenerateXml();
                        GenerateCode();
                    }
                }
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.Message, "protobuf");
            }
            finally
            {
                if (Directory.Exists(tempFolder))
                {
                    Directory.Delete(tempFolder, true);
                }
            }
        }

        /// <summary>
        /// Generate the xml file.
        /// </summary>
        ////private void GenerateXml()
        ////{
        ////    var protoBin = Path.Combine(tempFolder, "output.txt");

        ////    var files = new FileDescriptorSet();

        ////    using (var stream = File.OpenRead(protoBin))
        ////    {
        ////        Serializer.Merge(stream, files);
        ////    }

        ////    var xser = new XmlSerializer(typeof(FileDescriptorSet));
        ////    var settings = new XmlWriterSettings { Indent = true, IndentChars = "  ", NewLineHandling = NewLineHandling.Entitize };
        ////    var sb = new StringBuilder();
        ////    using (var writer = XmlWriter.Create(sb, settings))
        ////    {
        ////        xser.Serialize(writer, files);
        ////    }

        ////    File.WriteAllText(Path.Combine(tempFolder, "output.xml"), sb.ToString());
        ////}

        /// <summary>
        /// Copy the protobuf to the temp folder and convert it to UTF-8 without BOF, otherwise protov cannot read the file.
        /// </summary>
        private void CopyFile()
        {
            ////tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            tempFolder = Path.Combine(Path.GetTempPath(), "protobuf");
            Directory.CreateDirectory(tempFolder);

            // ReSharper disable once AssignNullToNotNullAttribute
            var tempFile = Path.Combine(tempFolder, Path.GetFileName(protoFile));

            // copy file as UTF-8 file without BOM (needed by protoc.exe)
            var text = File.ReadAllText(protoFile, Encoding.Default);
            File.WriteAllText(tempFile, text, new UTF8Encoding(false));
        }

        /// <summary>
        /// Get protoc from the resources and copy it to the temp folder.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>. True if protoc could be copyied otherwise false.
        /// </returns>
        private bool PrepareProtoc()
        {
            // with this I get all resource names
            //// string[] names = GetType().Assembly.GetManifestResourceNames();

            var name = typeof(Compile).Namespace + '.' + Protoc;

            using (var resourceStream = GetType().Assembly.GetManifestResourceStream(name))
            {
                if (resourceStream != null)
                {
                    var protocFile = Path.Combine(tempFolder, Protoc);

                    using (Stream outFile = File.OpenWrite(protocFile))
                    {
                        long length = 0;
                        int bytesRead;
                        var buffer = new byte[4096];
                        while ((bytesRead = resourceStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            outFile.Write(buffer, 0, bytesRead);
                            length += bytesRead;
                        }

                        outFile.SetLength(length);

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Get the protoc version number.
        /// </summary>
        private void GetProtocVersion()
        {
            infomessage = string.Empty;
            errormessage = string.Empty;
            RunProtoc("--version");

            var x = infomessage.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (x.Count() > 1)
            {
                protocVersion = x[1];
            }
        }

        /// <summary>
        /// Compile the proto file.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>. True if protoc compiles the proto file otherwise false.
        /// </returns>
        private bool CompileProto()
        {
            var parms = string.Format(
                    "\"--descriptor_set_out={0}\" \"--proto_path={1}\" --error_format=gcc \"{2}\"",
                    Path.Combine(tempFolder, "output.txt"),
                    tempFolder,
                    // ReSharper disable once AssignNullToNotNullAttribute
                    Path.Combine(tempFolder, Path.GetFileName(protoFile)));

            // todo: get protoc parms from proto file and add them here
            ////parms += string.Join(" ", protocParms) // extra parms

            return RunProtoc(parms);
        }

        /// <summary>
        /// Run protoc to compile the proto file.
        /// </summary>
        /// <param name="parms">
        /// The parmeters for the protoc.exe.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>. True if protoc runs ok otherwise false.
        /// </returns>
        private bool RunProtoc(string parms)
        {
            try
            {
                var protocFile = Path.Combine(tempFolder, Protoc);

                using (var proc = new Process())
                {
                    var psi = new ProcessStartInfo(protocFile, parms);
                    Debug.WriteLine(psi.FileName + " " + psi.Arguments, "protobuf");

                    psi.CreateNoWindow = true;
                    psi.WindowStyle = ProcessWindowStyle.Hidden;
                    psi.WorkingDirectory = Environment.CurrentDirectory;
                    psi.UseShellExecute = false;
                    psi.RedirectStandardOutput = true;
                    psi.RedirectStandardError = true;

                    proc.StartInfo = psi;

                    proc.OutputDataReceived += OutputDataReceived;
                    proc.ErrorDataReceived += ErrorDataReceived;

                    proc.Start();

                    proc.BeginOutputReadLine();
                    proc.BeginErrorReadLine();

                    proc.WaitForExit();

                    if (proc.ExitCode == 0)
                    {
                        return true;
                    }
                }
            }
            catch (Exception exp)
            {
                errormessage = exp.Message;
                Debug.WriteLine(exp.Message, "protobuf");
            }

            return false;
        }

        /// <summary>
        /// Receive the error messages.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data))
            {
                return;
            }

            if (!string.IsNullOrEmpty(errormessage))
            {
                errormessage += Environment.NewLine;
            }

            errormessage += e.Data;
            Debug.WriteLine(e.Data, "protobuf");
        }

        /// <summary>
        /// Receive the info messages.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data))
            {
                return;
            }

            if (!string.IsNullOrEmpty(infomessage))
            {
                infomessage += Environment.NewLine;
            }

            infomessage += e.Data;
            Debug.WriteLine(e.Data, "protobuf");
        }

        /// <summary>
        /// Generate the code file.
        /// </summary>
        private void GenerateCode()
        {
            var protoBin = Path.Combine(tempFolder, "output.txt");

            var analyze = new AnalyzeProtobuf();
            var files = analyze.Run(protoBin);

            ////var files = new FileDescriptorSet();

            ////using (var stream = File.OpenRead(protoBin))
            ////{
            ////    Serializer.Merge(stream, files);
            ////}

            ////var ver = Assembly.GetAssembly(typeof(Serializer)).GetName().Version.ToString();

            var newFile = Path.Combine(tempFolder, Path.GetFileName(protoFile) + ".cs");
            using (var file = File.CreateText(newFile))
            {
                file.WriteLine("// ---------------------------------------------------------------------------------------------------");
                file.WriteLine("// <auto-generated>");
                file.WriteLine("//     This code was generated by the Visual Studio Protobuf Language Service.");
                file.WriteLine("//     https://visualstudiogallery.msdn.microsoft.com/4bc0f38c-b058-4e05-ae38-155e053c19c5");
                file.WriteLine("//");
                file.WriteLine("//     Protobuf-net from Marc Gravell is needed to use this code.");
                file.WriteLine("//     https://code.google.com/p/protobuf-net/");
                file.WriteLine("//");
                file.WriteLine("//     This code was generated by the help of the google protoc.exe (protobuf compiler) version " + protocVersion);
                ////file.WriteLine("//     and protobuf-net version " + ver);
                file.WriteLine("//");
                file.WriteLine("//     Changes to this file may cause incorrect behaviour and will be lost if the code is regenerated.");
                file.WriteLine("// </auto-generated>");
                file.WriteLine("// ---------------------------------------------------------------------------------------------------");

                file.WriteLine();
                file.WriteLine("// generated from " + Path.GetFileName(protoFile));
                file.WriteLine();

                file.WriteLine(options.GetOptionsAsComment());

                foreach (var proto in files.Files)
                {
                    currentProto = proto;

                    var ns = string.IsNullOrEmpty(proto.Package) ? "projectname" : proto.Package;
                    WriteLineIntended(file, string.Format("namespace {0}", options.FixCase ? ToPascalCase(ns) : ns));

                    WriteLineIntended(file, "{");

                    intend++;

                    if (proto.Options != null)
                    {
                        if (proto.Options.Deprecated)
                        {
                            options.IsObsolete = true;
                        }
                    }

                    WriteMessages(file, proto.MessageTypes, true);
                    WriteEnums(file, proto.EnumTypes, !proto.MessageTypes.Any());
                    WriteServices(file, proto.Services, !proto.MessageTypes.Any() || !proto.EnumTypes.Any());

                    intend--;
                    file.Write("}");
                }

                file.Flush();
            }
        }

        /// <summary>
        /// Generate ann interface for each service.
        /// </summary>
        /// <param name="file">
        /// The file stream.
        /// </param>
        /// <param name="services">
        /// The services.
        /// </param>
        /// <param name="first">
        /// If not first write a blank line.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed. Suppression is OK here.")]
        // ReSharper disable once FunctionComplexityOverflow
        private void WriteServices(StreamWriter file, IEnumerable<ServiceDescriptor> services, bool first)
        {
            foreach (var item in services)
            {
                if (!first)
                {
                    file.WriteLine();
                }

                var obsolete = false;

                if (options.IsObsolete)
                {
                    obsolete = true;
                }
                else
                {
                    if (item.Options != null)
                    {
                        if (item.Options.Deprecated)
                        {
                            obsolete = true;
                        }
                    }
                }

                if (obsolete)
                {
                    WriteLineIntended(
                        file,
                        string.Format("[global::System.Obsolete(\"Service {1} is obsolete.\", {0})]", options.TreatObsoleteAsError ? "true" : "false", item.Name));
                }

                if (options.DataContract)
                {
                    WriteLineIntended(file, string.Format("[global::System.ServiceModel.ServiceContract(Name = @\"{0}\")]", item.Name));
                }

                WriteLineIntended(file, "public interface I" + (options.FixCase ? ToPascalCase(item.Name) : item.Name));
                WriteLineIntended(file, "{");
                intend++;

                first = true;
                foreach (var method in item.Methods)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        file.WriteLine();
                    }

                    if (!obsolete)
                    {
                        if (item.Options != null)
                        {
                            if (item.Options.Deprecated)
                            {
                                WriteLineIntended(
                                    file,
                                    string.Format("[global::System.Obsolete(\"Service {1} is obsolete.\", {0})]", options.TreatObsoleteAsError ? "true" : "false", item.Name));
                            }
                        }
                    }

                    if (options.DataContract)
                    {
                        WriteLineIntended(file, string.Format("[global::System.ServiceModel.OperationContract(Name = @\"{0}\")]", method.Name));

                        if (!options.LightFramework)
                        {
                            WriteLineIntended(file, "[global::ProtoBuf.ServiceModel.ProtoBehavior]");
                        }
                    }

                    var input = method.InputType.StartsWith(".") ? method.InputType.Substring(1) : method.InputType;
                    input = options.FixCase ? ToPascalCase(input) : input;
                    var output = method.OutputType.StartsWith(".") ? method.OutputType.Substring(1) : method.OutputType;
                    output = options.FixCase ? ToPascalCase(output) : output;
                    var name = options.FixCase ? ToPascalCase(method.Name) : method.Name;

                    WriteLineIntended(file, string.Format("{0} {1}({2} request);", output, name, input));

                    if (options.Asynchronous && (options.DataContract || options.ClientProxy))
                    {
                        file.WriteLine();

                        WriteLineIntended(file, string.Format("[global::System.ServiceModel.OperationContract(AsyncPattern = true, Name = @\"{0}\")]", name));
                        WriteLineIntended(
                            file,
                            string.Format(
                                "global::System.IAsyncResult Begin{0}({1} request, global::System.AsyncCallback callback, object state);",
                                name,
                                input));
                        WriteLineIntended(file, string.Format("{0} End{1}(global::System.IAsyncResult ar);", output, name));
                    }
                }

                intend--;
                WriteLineIntended(file, "}");

                if (options.Asynchronous && options.DataContract && options.ClientProxy)
                {
                    foreach (var method in item.Methods)
                    {
                        ////var input = method.input_type.StartsWith(".") ? method.input_type.Substring(1) : method.input_type;
                        ////input = options.FixCase ? ToPascalCase(input) : input;
                        var output = method.OutputType.StartsWith(".") ? method.OutputType.Substring(1) : method.OutputType;
                        output = options.FixCase ? ToPascalCase(output) : output;

                        file.WriteLine();

                        WriteLineIntended(
                            file,
                            string.Format(
                                "public partial class {0}CompletedEventArgs : global::System.ComponentModel.AsyncCompletedEventArgs",
                                options.FixCase ? ToPascalCase(method.Name) : method.Name));
                        WriteLineIntended(file, "{");
                        intend++;

                        WriteLineIntended(file, "private readonly object[] results;");
                        file.WriteLine();

                        WriteLineIntended(
                            file,
                            string.Format(
                                "public {0}CompletedEventArgs(object[] results, global::System.Exception exception, bool cancelled, object userState)",
                                options.FixCase ? ToPascalCase(method.Name) : method.Name));
                        intend++;
                        WriteLineIntended(file, ": base(exception, cancelled, userState)");
                        intend--;

                        WriteLineIntended(file, "{");
                        intend++;
                        WriteLineIntended(file, "this.results = results;");
                        intend--;
                        WriteLineIntended(file, "}");

                        file.WriteLine();

                        WriteLineIntended(file, string.Format("public {0} Result", output));
                        WriteLineIntended(file, "{");
                        intend++;
                        WriteLineIntended(file, "get");
                        WriteLineIntended(file, "{");
                        intend++;
                        WriteLineIntended(file, "base.RaiseExceptionIfNecessary();");
                        WriteLineIntended(file, string.Format("return ({0})(this.results[0]);", output));
                        intend--;
                        WriteLineIntended(file, "}");
                        intend--;
                        WriteLineIntended(file, "}");
                        intend--;
                        WriteLineIntended(file, "}");
                    }

                    file.WriteLine();

                    WriteLineIntended(file, "[global::System.Diagnostics.DebuggerStepThroughAttribute()]");
                    WriteLineIntended(file, string.Format("public partial class {0}Client : global::System.ServiceModel.ClientBase<I{0}>, I{0}", options.FixCase ? ToPascalCase(item.Name) : item.Name));
                    WriteLineIntended(file, "{");
                    intend++;
                    WriteLineIntended(file, string.Format("public {0}Client()", options.FixCase ? ToPascalCase(item.Name) : item.Name));
                    WriteLineIntended(file, "{ }");
                    WriteLineIntended(file, string.Format("public {0}Client(string endpointConfigurationName)", options.FixCase ? ToPascalCase(item.Name) : item.Name));
                    intend++;
                    WriteLineIntended(file, ": base(endpointConfigurationName)");
                    intend--;
                    WriteLineIntended(file, "{ }");

                    WriteLineIntended(file, string.Format("public {0}Client(string endpointConfigurationName, string remoteAddress)", options.FixCase ? ToPascalCase(item.Name) : item.Name));
                    intend++;
                    WriteLineIntended(file, ": base(endpointConfigurationName, remoteAddress)");
                    intend--;
                    WriteLineIntended(file, "{ }");

                    WriteLineIntended(file, string.Format("public {0}Client(string endpointConfigurationName, global::System.ServiceModel.EndpointAddress remoteAddress)", options.FixCase ? ToPascalCase(item.Name) : item.Name));
                    intend++;
                    WriteLineIntended(file, ": base(endpointConfigurationName, remoteAddress)");
                    intend--;
                    WriteLineIntended(file, "{ }");

                    WriteLineIntended(file, string.Format("public {0}Client(global::System.ServiceModel.Channels.Binding binding, global::System.ServiceModel.EndpointAddress remoteAddress)", options.FixCase ? ToPascalCase(item.Name) : item.Name));
                    intend++;
                    WriteLineIntended(file, ": base(binding, remoteAddress)");
                    intend--;
                    WriteLineIntended(file, "{ }");

                    foreach (var method in item.Methods)
                    {
                        var input = method.InputType.StartsWith(".") ? method.InputType.Substring(1) : method.InputType;
                        input = options.FixCase ? ToPascalCase(input) : input;
                        var output = method.OutputType.StartsWith(".") ? method.OutputType.Substring(1) : method.OutputType;
                        output = options.FixCase ? ToPascalCase(output) : output;

                        file.WriteLine();

                        WriteLineIntended(file, string.Format("private BeginOperationDelegate onBegin{0}Delegate;", options.FixCase ? ToPascalCase(method.Name) : method.Name));
                        WriteLineIntended(file, string.Format("private EndOperationDelegate onEnd{0}Delegate;", options.FixCase ? ToPascalCase(method.Name) : method.Name));
                        WriteLineIntended(file, string.Format("private global::System.Threading.SendOrPostCallback on{0}CompletedDelegate;", options.FixCase ? ToPascalCase(method.Name) : method.Name));
                        WriteLineIntended(file, string.Format("public event global::System.EventHandler<{0}CompletedEventArgs> {0}Completed;", options.FixCase ? ToPascalCase(method.Name) : method.Name));
                        file.WriteLine();

                        WriteLineIntended(file, string.Format("public {1} {0}({2} request)", options.FixCase ? ToPascalCase(method.Name) : method.Name, output, input));
                        WriteLineIntended(file, "{");
                        intend++;
                        WriteLineIntended(file, string.Format("return base.Channel.{0}(request);", options.FixCase ? ToPascalCase(method.Name) : method.Name));
                        intend--;
                        WriteLineIntended(file, "}");
                        file.WriteLine();

                        WriteLineIntended(file, "[global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]");
                        WriteLineIntended(file, string.Format("public global::System.IAsyncResult Begin{0}({1} request, global::System.AsyncCallback callback, object asyncState)", options.FixCase ? ToPascalCase(method.Name) : method.Name, input));
                        WriteLineIntended(file, "{");
                        intend++;
                        WriteLineIntended(file, string.Format("return base.Channel.Begin{0}(request, callback, asyncState);", options.FixCase ? ToPascalCase(method.Name) : method.Name));
                        intend--;
                        WriteLineIntended(file, "}");
                        file.WriteLine();

                        WriteLineIntended(file, "[global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]");
                        WriteLineIntended(file, string.Format("public {1} End{0}(global::System.IAsyncResult result)", options.FixCase ? ToPascalCase(method.Name) : method.Name, output));
                        WriteLineIntended(file, "{");
                        intend++;
                        WriteLineIntended(file, string.Format("return base.Channel.End{0}(result);", options.FixCase ? ToPascalCase(method.Name) : method.Name));
                        intend--;
                        WriteLineIntended(file, "}");
                        file.WriteLine();

                        WriteLineIntended(file, string.Format("private global::System.IAsyncResult OnBegin{0}(object[] inValues, global::System.AsyncCallback callback, object asyncState)", options.FixCase ? ToPascalCase(method.Name) : method.Name));
                        WriteLineIntended(file, "{");
                        intend++;
                        WriteLineIntended(file, string.Format("{0} request = ({0})(inValues[0]);", input));
                        WriteLineIntended(file, string.Format("return this.Begin{0}(request, callback, asyncState);", options.FixCase ? ToPascalCase(method.Name) : method.Name));
                        intend--;
                        WriteLineIntended(file, "}");
                        file.WriteLine();

                        WriteLineIntended(file, string.Format("private object[] OnEnd{0}(global::System.IAsyncResult result)", options.FixCase ? ToPascalCase(method.Name) : method.Name));
                        WriteLineIntended(file, "{");
                        intend++;
                        WriteLineIntended(file, string.Format("{1} retVal = this.End{0}(result);", options.FixCase ? ToPascalCase(method.Name) : method.Name, output));
                        WriteLineIntended(file, "return new object[] { retVal };");
                        intend--;
                        WriteLineIntended(file, "}");
                        file.WriteLine();

                        WriteLineIntended(file, string.Format("private void On{0}Completed(object state)", options.FixCase ? ToPascalCase(method.Name) : method.Name));
                        WriteLineIntended(file, "{");
                        intend++;
                        WriteLineIntended(file, string.Format("if (this.{0}Completed != null)", options.FixCase ? ToPascalCase(method.Name) : method.Name));
                        WriteLineIntended(file, "{");
                        intend++;
                        WriteLineIntended(file, "InvokeAsyncCompletedEventArgs e = (InvokeAsyncCompletedEventArgs)(state);");
                        WriteLineIntended(file, string.Format("this.{0}Completed(this, new {0}CompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));", options.FixCase ? ToPascalCase(method.Name) : method.Name));
                        intend--;
                        WriteLineIntended(file, "}");
                        intend--;
                        WriteLineIntended(file, "}");
                        file.WriteLine();

                        WriteLineIntended(file, string.Format("public void {0}Async({1} request)", options.FixCase ? ToPascalCase(method.Name) : method.Name, input));
                        WriteLineIntended(file, "{");
                        intend++;
                        WriteLineIntended(file, string.Format("this.{0}Async(request, null);", options.FixCase ? ToPascalCase(method.Name) : method.Name));
                        intend--;
                        WriteLineIntended(file, "}");
                        file.WriteLine();

                        WriteLineIntended(file, string.Format("public void {0}Async({1} request, object userState)", options.FixCase ? ToPascalCase(method.Name) : method.Name, input));
                        WriteLineIntended(file, "{");
                        intend++;
                        WriteLineIntended(file, string.Format("if ((this.onBegin{0}Delegate == null))", options.FixCase ? ToPascalCase(method.Name) : method.Name));
                        intend++;
                        WriteLineIntended(file, string.Format("this.onBegin{0}Delegate = new BeginOperationDelegate(this.OnBegin{0});", options.FixCase ? ToPascalCase(method.Name) : method.Name));
                        intend--;
                        file.WriteLine();
                        WriteLineIntended(file, string.Format("if ((this.onEnd{0}Delegate == null))", options.FixCase ? ToPascalCase(method.Name) : method.Name));
                        intend++;
                        WriteLineIntended(file, string.Format("this.onEnd{0}Delegate = new EndOperationDelegate(this.OnEnd{0});", options.FixCase ? ToPascalCase(method.Name) : method.Name));
                        intend--;
                        file.WriteLine();

                        WriteLineIntended(file, string.Format("if ((this.on{0}CompletedDelegate == null))", options.FixCase ? ToPascalCase(method.Name) : method.Name));
                        intend++;
                        WriteLineIntended(file, string.Format("this.on{0}CompletedDelegate = new global::System.Threading.SendOrPostCallback(this.On{0}Completed);", options.FixCase ? ToPascalCase(method.Name) : method.Name));
                        intend--;
                        file.WriteLine();

                        WriteLineIntended(file, string.Format("base.InvokeAsync(this.onBegin{0}Delegate,", options.FixCase ? ToPascalCase(method.Name) : method.Name));
                        intend++;
                        WriteLineIntended(file, "new object[] {request},");
                        WriteLineIntended(file, string.Format("this.onEnd{0}Delegate,", options.FixCase ? ToPascalCase(method.Name) : method.Name));
                        WriteLineIntended(file, string.Format("this.on{0}CompletedDelegate,", options.FixCase ? ToPascalCase(method.Name) : method.Name));
                        WriteLineIntended(file, "userState);");
                        intend--;

                        intend--;
                        WriteLineIntended(file, "}");
                        file.WriteLine();
                    }

                    intend--;
                    WriteLineIntended(file, "}");
                }
            }
        }

        /// <summary>
        /// Generate a class for each message.
        /// </summary>
        /// <param name="file">
        /// The file stream.
        /// </param>
        /// <param name="messages">
        /// The messages.
        /// </param>
        /// <param name="first">
        /// If not first write a blank line.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed. Suppression is OK here.")]
        private void WriteMessages(StreamWriter file, IEnumerable<MessageDescriptor> messages, bool first)
        {
            foreach (var item in messages)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    file.WriteLine();
                }

                var obsolete = false;
                if (options.IsObsolete)
                {
                    WriteLineIntended(file, string.Format("[global::System.Obsolete(\"Message {1} is obsolete.\", {0})]", options.TreatObsoleteAsError ? "true" : "false", item.Name));
                    obsolete = true;
                }
                else
                {
                    if (item.Options != null)
                    {
                        if (item.Options.Deprecated)
                        {
                            WriteLineIntended(file, string.Format("[global::System.Obsolete(\"Message {1} is obsolete.\", {0})]", options.TreatObsoleteAsError ? "true" : "false", item.Name));
                            obsolete = true;
                        }
                    }
                }

                if (!options.LightFramework)
                {
                    WriteLineIntended(file, "[global::System.Serializable]");
                }

                WriteLineIntended(file, string.Format("[global::ProtoBuf.ProtoContract(Name=@\"{0}\")]", item.Name));

                if (options.Xml)
                {
                    WriteLineIntended(file, string.Format("[global::System.Xml.Serialization.XmlType(TypeName=@\"{0}\")]", item.Name));
                }

                if (options.DataContract)
                {
                    WriteLineIntended(file, string.Format("[global::System.Runtime.Serialization.DataContract(Name=@\"{0}\")]", item.Name));
                }

                var line = string.Format("public partial class {0} : global::ProtoBuf.IExtensible", options.FixCase ? ToPascalCase(item.Name) : item.Name);

                if (options.Binary)
                {
                    line += ", global::System.Runtime.Serialization.ISerializable";
                }

                if (options.Observable)
                {
                    line += ", global::System.ComponentModel.INotifyPropertyChanged";
                }

                if (options.PreObservable)
                {
                    line += ", global::System.ComponentModel.INotifyPropertyChanging";
                }

                WriteLineIntended(file, line);
                WriteLineIntended(file, "{");
                intend++;

                if (!options.NoConstructor)
                {
                    WriteLineIntended(file, string.Format("public {0}() {{}}", options.FixCase ? ToPascalCase(item.Name) : item.Name));
                    file.WriteLine();
                }

                WriteFields(file, item.Fields, obsolete);

                if (item.NestedTypes.Any())
                {
                    file.WriteLine();
                    WriteMessages(file, item.NestedTypes, item.Fields.Any());
                }

                if (item.EnumTypes.Any())
                {
                    file.WriteLine();
                    WriteEnums(file, item.EnumTypes, item.Fields.Any() | item.NestedTypes.Any());
                }

                // write extension
                if (item.Fields.Any() | item.NestedTypes.Any() | item.EnumTypes.Any())
                {
                    file.WriteLine();
                }

                if (options.Binary)
                {
                    WriteLineIntended(
                        file,
                        string.Format(
                            "protected {0}(global::System.Runtime.Serialization.SerializationInfo info, global::System.Runtime.Serialization.StreamingContext context) : this()",
                            options.FixCase ? ToPascalCase(item.Name) : item.Name));
                    WriteLineIntended(file, "{");
                    intend++;
                    WriteLineIntended(file, "global::ProtoBuf.Serializer.Merge(info, this);");
                    intend--;
                    WriteLineIntended(file, "}");

                    file.WriteLine();

                    WriteLineIntended(
                        file,
                        "void global::System.Runtime.Serialization.ISerializable.GetObjectData(global::System.Runtime.Serialization.SerializationInfo info, global::System.Runtime.Serialization.StreamingContext context)");
                    WriteLineIntended(file, "{");
                    intend++;
                    WriteLineIntended(file, "global::ProtoBuf.Serializer.Serialize(info, this);");
                    intend--;
                    WriteLineIntended(file, "}");

                    file.WriteLine();
                }

                if (options.Observable)
                {
                    WriteLineIntended(file, "public event global::System.ComponentModel.PropertyChangedEventHandler PropertyChanged;");
                    WriteLineIntended(file, "protected virtual void OnPropertyChanged(string propertyName)");
                    WriteLineIntended(file, "{");
                    intend++;
                    WriteLineIntended(file, "if (PropertyChanged != null)");
                    intend++;
                    WriteLineIntended(file, "PropertyChanged(this, new global::System.ComponentModel.PropertyChangedEventArgs(propertyName));");
                    intend--;
                    intend--;
                    WriteLineIntended(file, "}");

                    file.WriteLine();
                }

                if (options.PreObservable)
                {
                    WriteLineIntended(file, "public event global::System.ComponentModel.PropertyChangingEventHandler PropertyChanging;");
                    WriteLineIntended(file, "protected virtual void OnPropertyChanging(string propertyName)");
                    WriteLineIntended(file, "{");
                    intend++;
                    WriteLineIntended(file, "if(PropertyChanging != null)");
                    intend++;
                    WriteLineIntended(file, "PropertyChanging(this, new global::System.ComponentModel.PropertyChangingEventArgs(propertyName));");
                    intend--;
                    intend--;
                    WriteLineIntended(file, "}");

                    file.WriteLine();
                }

                WriteLineIntended(file, "private global::ProtoBuf.IExtension extensionObject;");
                WriteLineIntended(file, "global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)");
                WriteLineIntended(file, "{");
                intend++;
                WriteLineIntended(file, "return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing);");
                intend--;
                WriteLineIntended(file, "}");
                intend--;
                WriteLineIntended(file, "}");
            }
        }

        /// <summary>
        /// Write the fields of a message.
        /// </summary>
        /// <param name="file">
        /// The file stream.
        /// </param>
        /// <param name="fields">
        /// The fields.
        /// </param>
        /// <param name="obsolete">
        /// True if this message is obsolete otherwise false.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed. Suppression is OK here.")]
        // ReSharper disable once FunctionComplexityOverflow
        private void WriteFields(StreamWriter file, IEnumerable<FieldDescriptor> fields, bool obsolete)
        {
            var first = true;

            foreach (var item in fields)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    file.WriteLine();
                }

                string valueType;
                var defaultValue = string.Empty;
                var protoValue = "Default";

                switch (item.Type)
                {
                    case FieldDescriptor.Types.TYPE_DOUBLE:
                        valueType = "double";
                        defaultValue = string.IsNullOrEmpty(item.DefaultValue) ? "default(double)" : "(double)" + item.DefaultValue;
                        protoValue = "TwosComplement";
                        break;
                    case FieldDescriptor.Types.TYPE_FLOAT:
                        valueType = "float";
                        defaultValue = string.IsNullOrEmpty(item.DefaultValue) ? "default(float)" : "(float)" + item.DefaultValue;
                        protoValue = "FixedSize";
                        break;
                    case FieldDescriptor.Types.TYPE_INT64:
                        valueType = "long";
                        defaultValue = string.IsNullOrEmpty(item.DefaultValue) ? "default(long)" : "(long)" + item.DefaultValue;
                        protoValue = "TwosComplement";
                        break;
                    case FieldDescriptor.Types.TYPE_SFIXED64:
                        valueType = "long";
                        defaultValue = string.IsNullOrEmpty(item.DefaultValue) ? "default(long)" : "(long)" + item.DefaultValue;
                        protoValue = "ZigZag";
                        break;
                    case FieldDescriptor.Types.TYPE_SINT64:
                        valueType = "long";
                        defaultValue = string.IsNullOrEmpty(item.DefaultValue) ? "default(long)" : "(long)" + item.DefaultValue;
                        protoValue = "ZigZag";
                        break;
                    case FieldDescriptor.Types.TYPE_FIXED64:
                        valueType = "ulong";
                        defaultValue = string.IsNullOrEmpty(item.DefaultValue) ? "default(ulong)" : "(ulong)" + item.DefaultValue;
                        protoValue = "FixedSize";
                        break;
                    case FieldDescriptor.Types.TYPE_UINT64:
                        valueType = "ulong";
                        defaultValue = string.IsNullOrEmpty(item.DefaultValue) ? "default(ulong)" : "(ulong)" + item.DefaultValue;
                        protoValue = "TwosComplement";
                        break;
                    case FieldDescriptor.Types.TYPE_INT32:
                        valueType = "int";
                        defaultValue = string.IsNullOrEmpty(item.DefaultValue) ? "default(int)" : "(int)" + item.DefaultValue;
                        protoValue = "TwosComplement";
                        break;
                    case FieldDescriptor.Types.TYPE_SFIXED32:
                        valueType = "int";
                        defaultValue = string.IsNullOrEmpty(item.DefaultValue) ? "default(int)" : "(int)" + item.DefaultValue;
                        protoValue = "FixedSize";
                        break;
                    case FieldDescriptor.Types.TYPE_SINT32:
                        valueType = "int";
                        defaultValue = string.IsNullOrEmpty(item.DefaultValue) ? "default(int)" : "(int)" + item.DefaultValue;
                        protoValue = "ZigZag";
                        break;
                    case FieldDescriptor.Types.TYPE_UINT32:
                        valueType = "int";
                        defaultValue = string.IsNullOrEmpty(item.DefaultValue) ? "default(int)" : "(int)" + item.DefaultValue;
                        protoValue = "TwosComplement";
                        break;
                    case FieldDescriptor.Types.TYPE_FIXED32:
                        valueType = "uint";
                        defaultValue = string.IsNullOrEmpty(item.DefaultValue) ? "default(uint)" : "(uint)" + item.DefaultValue;
                        protoValue = "FixedSize";
                        break;
                    case FieldDescriptor.Types.TYPE_BOOL:
                        valueType = "bool";
                        defaultValue = string.IsNullOrEmpty(item.DefaultValue) ? "default(bool)" : item.DefaultValue;
                        break;
                    case FieldDescriptor.Types.TYPE_STRING:
                        valueType = "string";
                        defaultValue = string.IsNullOrEmpty(item.DefaultValue) ? "\"\"" : "@\"" + item.DefaultValue + '\"';
                        break;
                    case FieldDescriptor.Types.TYPE_BYTES:
                        valueType = "byte[]";
                        defaultValue = string.IsNullOrEmpty(item.DefaultValue) ? "null" : string.Format("new byte[] {{ {0} }}", item.DefaultValue);
                        break;
                    case FieldDescriptor.Types.TYPE_GROUP:
                        valueType = item.TypeName;
                        valueType = options.FixCase ? ToPascalCase(valueType) : valueType;
                        break;
                    case FieldDescriptor.Types.TYPE_MESSAGE:
                        valueType = item.TypeName.StartsWith(".") ? item.TypeName.Substring(1) : item.TypeName;
                        valueType = options.FixCase ? ToPascalCase(valueType) : valueType;
                        defaultValue = "null";
                        break;
                    case FieldDescriptor.Types.TYPE_ENUM:
                        valueType = item.TypeName.StartsWith(".") ? item.TypeName.Substring(1) : item.TypeName;

                        if (string.IsNullOrEmpty(item.DefaultValue))
                        {
                            var e = currentProto.EnumTypes.FirstOrDefault(x => x.Name == valueType);

                            valueType = options.FixCase ? ToPascalCase(valueType) : valueType;

                            if (e != null)
                            {
                                defaultValue = valueType + '.' + (options.FixCase ? ToPascalCase(e.Values[0].Name) : e.Values[0].Name);
                            }
                            else
                            {
                                valueType = "0";
                            }
                        }
                        else
                        {
                            valueType = options.FixCase ? ToPascalCase(valueType) : valueType;
                            defaultValue = valueType + '.' + (options.FixCase ? ToPascalCase(item.DefaultValue) : item.DefaultValue);
                        }

                        protoValue = "TwosComplement";

                        break;
                    default:
                        valueType = "unknown field type";
                        break;
                }

                switch (item.Label)
                {
                    case FieldDescriptor.Labels.LABEL_OPTIONAL:
                    case FieldDescriptor.Labels.LABEL_REQUIRED:
                        {
                            // no default value if field is required
                            if (item.Label == FieldDescriptor.Labels.LABEL_REQUIRED)
                            {
                                defaultValue = null;
                            }

                            WriteLineIntended(
                                file,
                                string.IsNullOrEmpty(defaultValue)
                                    ? string.Format("private {0} _{1};", valueType, options.FixCase ? ToCamelCase(item.Name) : item.Name)
                                    : string.Format("private {0} _{1} = {2};", valueType, options.FixCase ? ToCamelCase(item.Name) : item.Name, defaultValue));

                            if (!obsolete)
                            {
                                if (item.Options != null)
                                {
                                    if (item.Options.Deprecated)
                                    {
                                        WriteLineIntended(file, string.Format("[global::System.Obsolete(\"Field {1} is obsolete.\", {0})]", options.TreatObsoleteAsError ? "true" : "false", item.Name));
                                    }
                                }
                            }

                            WriteLineIntended(
                                file,
                                string.Format(
                                    "[global::ProtoBuf.ProtoMember({0}, IsRequired = {1}, Name=@\"{2}\", DataFormat = global::ProtoBuf.DataFormat.{3})]",
                                    item.Number,
                                    item.Label == FieldDescriptor.Labels.LABEL_REQUIRED ? "true" : "false",
                                    item.Name,
                                    protoValue));

                            if (item.Label == FieldDescriptor.Labels.LABEL_OPTIONAL)
                            {
                                if (!string.IsNullOrEmpty(defaultValue))
                                {
                                    WriteLineIntended(file, string.Format("[global::System.ComponentModel.DefaultValue({0})]", defaultValue));
                                }
                            }

                            if (options.Xml)
                            {
                                WriteLineIntended(file, string.Format("[global::System.Xml.Serialization.XmlElement(@\"{0}\", Order = {1})]", item.Name, item.Number));
                            }

                            if (options.DataContract)
                            {
                                WriteLineIntended(file, string.Format("[global::System.Runtime.Serialization.DataMember(Name=@\"{0}\", Order = {1}, IsRequired = {2})]", item.Name, item.Number, item.Label == FieldDescriptor.Labels.LABEL_REQUIRED ? "true" : "false"));
                            }

                            var line = string.Format("public {0} {1}", valueType, options.FixCase ? ToPascalCase(item.Name) : item.Name);
                            WriteLineIntended(file, line);
                            WriteLineIntended(file, "{");
                            intend++;
                            line = string.Format("get {{ return _{0}; }}", options.FixCase ? ToCamelCase(item.Name) : item.Name);
                            WriteLineIntended(file, line);

                            // generate setter
                            line = "set {";

                            if (options.PartialMethods)
                            {
                                line += string.Format(" On{0}Changing(value);", options.FixCase ? ToPascalCase(item.Name) : item.Name);
                            }

                            if (options.PreObservable)
                            {
                                line += string.Format(" OnPropertyChanging(@\"{0}\");", options.FixCase ? ToPascalCase(item.Name) : item.Name);
                            }

                            line += string.Format(" _{0} = value;", options.FixCase ? ToCamelCase(item.Name) : item.Name);

                            if (options.Observable)
                            {
                                line += string.Format(" OnPropertyChanged(@\"{0}\");", options.FixCase ? ToPascalCase(item.Name) : item.Name);
                            }

                            if (options.PartialMethods)
                            {
                                line += string.Format(" On{0}Changed();", options.FixCase ? ToPascalCase(item.Name) : item.Name);
                            }

                            line += " }";

                            WriteLineIntended(file, line);
                            intend--;
                            WriteLineIntended(file, "}");

                            if (options.PartialMethods)
                            {
                                WriteLineIntended(file, string.Format("partial void On{0}Changing({1} value);", options.FixCase ? ToPascalCase(item.Name) : item.Name, valueType));
                                WriteLineIntended(file, string.Format("partial void On{0}Changed();", options.FixCase ? ToPascalCase(item.Name) : item.Name));
                            }

                            break;
                        }

                    case FieldDescriptor.Labels.LABEL_REPEATED:
                        {
                            WriteLineIntended(
                                file,
                                string.Format(
                                    "private {2}global::System.Collections.Generic.List<{0}> _{1} = new global::System.Collections.Generic.List<{0}>();",
                                    valueType,
                                    options.FixCase ? ToCamelCase(item.Name) : item.Name,
                                    options.Xml ? string.Empty : "readonly "));

                            if (!obsolete)
                            {
                                if (item.Options != null)
                                {
                                    if (item.Options.Deprecated)
                                    {
                                        WriteLineIntended(file, string.Format("[global::System.Obsolete(\"Field {1} is obsolete.\", {0})]", options.TreatObsoleteAsError ? "true" : "false", item.Name));
                                    }
                                }
                            }

                            WriteLineIntended(
                                file,
                                string.Format(
                                    "[global::ProtoBuf.ProtoMember({0}, Name=@\"{1}\", DataFormat = global::ProtoBuf.DataFormat.{2})]",
                                    item.Number,
                                    item.Name,
                                    protoValue));

                            if (options.Xml)
                            {
                                WriteLineIntended(file, string.Format("[global::System.Xml.Serialization.XmlElement(@\"{0}\", Order = {1})]", item.Name, item.Number));
                            }

                            if (options.DataContract)
                            {
                                WriteLineIntended(file, string.Format("[global::System.Runtime.Serialization.DataMember(Name=@\"{0}\", Order = {1}, IsRequired = false)]", item.Name, item.Number));
                            }

                            var line = string.Format("public global::System.Collections.Generic.List<{0}> {1}", valueType, options.FixCase ? ToPascalCase(item.Name) : item.Name);
                            WriteLineIntended(file, line);
                            WriteLineIntended(file, "{");
                            intend++;
                            line = string.Format("get {{ return _{0}; }}", options.FixCase ? ToCamelCase(item.Name) : item.Name);
                            WriteLineIntended(file, line);

                            if (options.Xml)
                            {
                                line = string.Format("set {{ _{0} = value; }}", options.FixCase ? ToCamelCase(item.Name) : item.Name);
                                WriteLineIntended(file, line);
                            }

                            intend--;
                            WriteLineIntended(file, "}");

                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Write the enums.
        /// </summary>
        /// <param name="file">
        /// The file stream.
        /// </param>
        /// <param name="enums">
        /// The enums.
        /// </param>
        /// <param name="first">
        /// The first.
        /// </param>
        private void WriteEnums(StreamWriter file, IEnumerable<EnumDescriptor> enums, bool first)
        {
            foreach (var item in enums)
            {
                if (!first)
                {
                    file.WriteLine();
                }

                var obsolete = false;
                if (options.IsObsolete)
                {
                    WriteLineIntended(file, string.Format("[global::System.Obsolete(\"Enum {1} is obsolete.\", {0})]", options.TreatObsoleteAsError ? "true" : "false", item.Name));
                    obsolete = true;
                }
                else
                {
                    if (item.Options != null)
                    {
                        if (item.Options.Deprecated)
                        {
                            WriteLineIntended(file, string.Format("[global::System.Obsolete(\"Enum {1} is obsolete.\", {0})]", options.TreatObsoleteAsError ? "true" : "false", item.Name));
                            obsolete = true;
                        }
                    }
                }

                WriteLineIntended(file, string.Format("[global::ProtoBuf.ProtoContract(Name=@\"{0}\")]", item.Name));

                if (options.Xml)
                {
                    WriteLineIntended(file, string.Format("[global::System.Xml.Serialization.XmlType(TypeName=@\"{0}\")]", item.Name));
                }

                if (options.DataContract)
                {
                    WriteLineIntended(file, string.Format("[global::System.Runtime.Serialization.DataContract(Name=@\"{0}\")]", item.Name));
                }

                WriteLineIntended(file, "public enum " + (options.FixCase ? ToPascalCase(item.Name) : item.Name));
                WriteLineIntended(file, "{");
                intend++;

                first = true;
                foreach (var e in item.Values)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        file.WriteLine();
                    }

                    if (!obsolete)
                    {
                        if (e.Options != null)
                        {
                            if (e.Options.Deprecated)
                            {
                                WriteLineIntended(file, string.Format("[global::System.Obsolete(\"Enum {1} is obsolete.\", {0})]", options.TreatObsoleteAsError ? "true" : "false", e.Name));
                            }
                        }
                    }

                    WriteLineIntended(file, string.Format("[global::ProtoBuf.ProtoEnum(Name=@\"{0}\", Value={1})]", e.Name, e.Number));

                    if (options.Xml)
                    {
                        WriteLineIntended(file, string.Format("[global::System.Xml.Serialization.XmlEnum(@\"{0}\")]", e.Name));
                    }

                    if (options.DataContract)
                    {
                        WriteLineIntended(file, string.Format("[global::System.Runtime.Serialization.EnumMember(Value=@\"{0}\")]", e.Name));
                    }

                    WriteLineIntended(file, string.Format("{0} = {1},", options.FixCase ? ToPascalCase(e.Name) : e.Name, e.Number));
                }

                intend--;
                WriteLineIntended(file, "}");
            }
        }

        /// <summary>
        /// Write the line intended.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        private void WriteLineIntended(StreamWriter file, string text)
        {
            file.Write(new string(' ', intend * 4));
            file.WriteLine(text);
        }

        /// <summary>
        /// Convert name to PascalCase.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string ToPascalCase(string name)
        {
            var parts = name.Split('.');
            if (parts.Any())
            {
                for (int ix = 0; ix < parts.Count(); ix++)
                {
                    var parts2 = parts[ix].Split('_');

                    if (parts2.Any())
                    {
                        parts[ix] = string.Empty;

                        for (int ix2 = 0; ix2 < parts2.Count(); ix2++)
                        {
                            parts[ix] += parts2[ix2].Substring(0, 1).ToUpper() + parts2[ix2].Substring(1);
                        }
                    }
                }
            }

            name = string.Join(".", parts);

            return name;
        }

        /// <summary>
        /// Convert name to camelCase.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string ToCamelCase(string name)
        {
            var parts = name.Split('_');

            if (parts.Any())
            {
                name = parts[0].Substring(0, 1).ToLower() + parts[0].Substring(1);

                for (int ix = 1; ix < parts.Count(); ix++)
                {
                    name += parts[ix].Substring(0, 1).ToUpper() + parts[ix].Substring(1);
                }
            }

            return name;
        }
    }
}