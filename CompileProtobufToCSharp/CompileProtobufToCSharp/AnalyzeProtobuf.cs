#region Copyright © 2015 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnalyzeProtobuf.cs" company="Michael Reukauff">
//   Copyright © 2015 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace CompileProtobufToCSharp
{
    using System;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// The analyze protobuf.
    /// </summary>
    internal class AnalyzeProtobuf
    {
        /// <summary>
        /// The ix.
        /// </summary>
        private int ix;

        /// <summary>
        /// The intend.
        /// </summary>
        private int intend;

        /// <summary>
        /// The array.
        /// </summary>
        private byte[] array;

        /// <summary>
        /// The run.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <returns>
        /// The <see cref="FileDescriptorSet"/>.
        /// </returns>
        internal FileDescriptorSet Run(string filename)
        {
            array = File.ReadAllBytes(filename);

            Write("Message langth = " + array.Length);

            var fds = new FileDescriptorSet();

            var fd = new FileDescriptor();
            fds.Files.Add(fd);

            FileDescriptorSet(fd);

            return fds;
        }

        /// <summary>
        /// The file descriptor set.
        /// </summary>
        /// <param name="fd">
        /// The file descriptor.
        /// </param>
        private void FileDescriptorSet(FileDescriptor fd)
        {
            if (array[ix] == 0x0a)
            {
                ix++;
                uint length;
                var ixl = GetVarint(out length);
                Write("FileDescriptorProto Length = " + length);
                ix += ixl;

                FileDescriptorProto(length, fd);
            }
        }

        /// <summary>
        /// The file descriptor proto.
        /// </summary>
        /// <param name="totalLength">
        /// The total length.
        /// </param>
        /// <param name="fd">
        /// The file descriptor.
        /// </param>
        // ReSharper disable once FunctionComplexityOverflow
        private void FileDescriptorProto(uint totalLength, FileDescriptor fd)
        {
            intend++;

            while (totalLength != 0)
            {
                int type, no;
                var l = GetTypeAndFieldNo(out type, out no);
                ix += l;
                totalLength -= (uint)l;

                switch (no)
                {
                    case 1: // string name
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            fd.Name = GetString(length);
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, Name = {3}", type, no, length, fd.Name));
                            ix += (int)length;
                            totalLength -= length;
                            break;
                        }

                    case 2: // string package
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            fd.Package = GetString(length);
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, Package = {3}", type, no, length, fd.Package));
                            ix += (int)length;
                            totalLength -= length;
                            break;
                        }

                    case 3: // string[] dependecy
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            var value = GetString(length);
                            fd.Dependencies.Add(value);
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, Dependency = {3}", type, no, length, value));
                            ix += (int)length;
                            totalLength -= length;
                            break;
                        }

                    case 4: // DescriptorProto[] message type
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2} - DescriptorProto", type, no, length));
                            var md = new MessageDescriptor();
                            fd.MessageTypes.Add(md);
                            DescriptorProto(length, md);
                            totalLength -= length;
                            break;
                        }

                    case 5: // EnumDescriptorProto[] enum type
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2} - EnumDescriptorProto", type, no, length));
                            var ed = new EnumDescriptor();
                            fd.EnumTypes.Add(ed);
                            EnumDescriptorProto(length, ed);
                            totalLength -= length;
                            break;
                        }

                    case 6: // ServiceDescriptorProto[] service
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2} - ServiceDescriptorProto", type, no, length));
                            var sd = new ServiceDescriptor();
                            fd.Services.Add(sd);
                            ServiceDescriptorProto(length, sd);
                            totalLength -= length;
                            break;
                        }

                    case 7: // FieldDescriptorProto[] extension
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2} - FieldDescriptorProto", type, no, length));
                            var fd2 = new FieldDescriptor();
                            fd.Extensions.Add(fd2);
                            FieldDescriptorProto(length, fd2);
                            totalLength -= length;
                            break;
                        }

                    case 8: // FileOptions options
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2} - FileOptions", type, no, length));
                            if (fd.Options == null)
                            {
                                fd.Options = new FileOptions();
                            }

                            FileOptions(length, fd.Options);
                            totalLength -= length;
                            break;
                        }

                    case 9: // SourceCodeInfo source_code_info
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2} - SourceCodeInfo", type, no, length));
                            fd.SourceCodeInfo = new SourceCodeInfo();
                            ////SourceCodeInfo(length, fd.SourceCodeInfo);
                            ix += (int)length;
                            totalLength -= length;
                            break;
                        }

                    case 10: // int32[] public_dependency
                        {
                            uint value;
                            var ixl = GetVarint(out value);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            fd.PublicDependencies.Add((int)value);
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, public_dependency = {3}", type, no, ixl, value));
                            break;
                        }

                    case 11: // int32[] weak_dependency
                        {
                            uint value;
                            var ixl = GetVarint(out value);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            fd.WeakDependencies.Add((int)value);
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, weak_dependency = {3}", type, no, ixl, value));
                            break;
                        }
                }
            }

            intend--;
        }

        /// <summary>
        /// The service descriptor proto.
        /// </summary>
        /// <param name="totalLength">
        /// The total length.
        /// </param>
        /// <param name="sd">
        /// The sd.
        /// </param>
        private void ServiceDescriptorProto(uint totalLength, ServiceDescriptor sd)
        {
            intend++;

            while (totalLength != 0)
            {
                int type, no;
                var l = GetTypeAndFieldNo(out type, out no);
                ix += l;
                totalLength -= (uint)l;

                switch (no)
                {
                    case 1: // string name
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            sd.Name = GetString(length);
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, name = {3}", type, no, length, sd.Name));
                            ix += (int)length;
                            totalLength -= length;
                            break;
                        }

                    case 2: // MethodDescriptorProto options
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2} - MethodDescriptorProto", type, no, length));
                            var md = new MethodDescriptor();
                            sd.Methods.Add(md);
                            MethodDescriptorProto(length, md);
                            totalLength -= length;
                            break;
                        }

                    case 3: // ServiceOptions options
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2} - ServiceOptions", type, no, length));
                            if (sd.Options == null)
                            {
                                sd.Options = new ServiceOptions();
                            }

                            ServiceOptions(length, sd.Options);
                            totalLength -= length;
                            break;
                        }
                }
            }

            intend--;
        }

        /// <summary>
        /// The method descriptor proto.
        /// </summary>
        /// <param name="totalLength">
        /// The total length.
        /// </param>
        /// <param name="md">The method descriptor.</param>
        private void MethodDescriptorProto(uint totalLength, MethodDescriptor md)
        {
            intend++;

            while (totalLength != 0)
            {
                int type, no;
                var l = GetTypeAndFieldNo(out type, out no);
                ix += l;
                totalLength -= (uint)l;

                switch (no)
                {
                    case 1: // string name
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            md.Name = GetString(length);
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, name = {3}", type, no, length, md.Name));
                            ix += (int)length;
                            totalLength -= length;
                            break;
                        }

                    case 2: // string input_type
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            md.InputType = GetString(length);
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, input_type = {3}", type, no, length, md.InputType));
                            ix += (int)length;
                            totalLength -= length;
                            break;
                        }

                    case 3: // string output_type
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            md.OutputType = GetString(length);
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, output_type = {3}", type, no, length, md.OutputType));
                            ix += (int)length;
                            totalLength -= length;
                            break;
                        }

                    case 4: // MethodOptions options
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2} - MethodOptions", type, no, length));
                            if (md.Options == null)
                            {
                                md.Options = new MethodOptions();
                            }

                            MethodOptions(length, md.Options);
                            totalLength -= length;
                            break;
                        }
                }
            }

            intend--;
        }

        /// <summary>
        /// The method options.
        /// </summary>
        /// <param name="totalLength">
        /// The total length.
        /// </param>
        /// <param name="options">The options.</param>
        private void MethodOptions(uint totalLength, MethodOptions options)
        {
            intend++;

            while (totalLength != 0)
            {
                int type, no;
                var l = GetTypeAndFieldNo(out type, out no);
                ix += l;
                totalLength -= (uint)l;

                switch (no)
                {
                    case 33: // bool deprecated
                        {
                            uint value;
                            var ixl = GetVarint(out value);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            options.Deprecated = value != 0;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, deprecated = {3}", type, no, ixl, value));
                            break;
                        }
                }
            }

            intend--;
        }

        /// <summary>
        /// The service options.
        /// </summary>
        /// <param name="totalLength">
        /// The total length.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        private void ServiceOptions(uint totalLength, ServiceOptions options)
        {
            intend++;

            while (totalLength != 0)
            {
                int type, no;
                var l = GetTypeAndFieldNo(out type, out no);
                ix += l;
                totalLength -= (uint)l;

                switch (no)
                {
                    case 33: // bool deprecated
                        {
                            uint value;
                            var ixl = GetVarint(out value);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            options.Deprecated = value != 0;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, deprecated = {3}", type, no, ixl, value));
                            break;
                        }
                }
            }

            intend--;
        }

        /// <summary>
        /// The file options.
        /// </summary>
        /// <param name="totalLength">
        /// The total length.
        /// </param>
        /// <param name="options">File options.</param>
        // ReSharper disable once FunctionComplexityOverflow
        private void FileOptions(uint totalLength, FileOptions options)
        {
            intend++;

            while (totalLength != 0)
            {
                int type, no;
                var l = GetTypeAndFieldNo(out type, out no);
                ix += l;
                totalLength -= (uint)l;

                switch (no)
                {
                    case 1: // string java_package
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            options.JavaPackage = GetString(length);
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, java_package = {3}", type, no, length, options.JavaPackage));
                            ix += (int)length;
                            totalLength -= length;
                            break;
                        }

                    case 8: // string java_outer_classname
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            options.JavaOuterClassname = GetString(length);
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, java_outer_classname = {3}", type, no, length, options.JavaOuterClassname));
                            ix += (int)length;
                            totalLength -= length;
                            break;
                        }

                    case 9: // OptimizeMode optimize_for
                        {
                            uint value;
                            var ixl = GetVarint(out value);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            options.OptimizeFor = (FileOptions.OptimizeMode)value;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, optimize_for = {3}", type, no, ixl, value));
                            break;
                        }

                    case 10: // bool java_multiple_files
                        {
                            uint value;
                            var ixl = GetVarint(out value);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            options.JavaMultipleFiles = value != 0;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, java_multiple_files = {3}", type, no, ixl, value));
                            break;
                        }

                    case 11: // string go_package
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            options.GoPackage = GetString(length);
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, go_package = {3}", type, no, length, options.GoPackage));
                            ix += (int)length;
                            totalLength -= length;
                            break;
                        }

                    case 16: // bool cc_generic_services
                        {
                            uint value;
                            var ixl = GetVarint(out value);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            options.CcGenericServices = value != 0;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, cc_generic_services = {3}", type, no, ixl, value));
                            break;
                        }

                    case 17: // bool java_generic_services
                        {
                            uint value;
                            var ixl = GetVarint(out value);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            options.JavaGenericServices = value != 0;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, java_generic_services = {3}", type, no, ixl, value));
                            break;
                        }

                    case 18: // bool py_generic_services
                        {
                            uint value;
                            var ixl = GetVarint(out value);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            options.PyGenericServices = value != 0;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, py_generic_services = {3}", type, no, ixl, value));
                            break;
                        }

                    case 20: // bool java_generate_equals_and_hash
                        {
                            uint value;
                            var ixl = GetVarint(out value);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            options.JavaGenerateEqualsAndHash = value != 0;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, java_generate_equals_and_hash = {3}", type, no, ixl, value));
                            break;
                        }

                    case 23: // bool deprecated
                        {
                            uint value;
                            var ixl = GetVarint(out value);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            options.Deprecated = value != 0;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, deprecated = {3}", type, no, ixl, value));
                            break;
                        }

                    case 27: // bool java_string_check_utf8
                        {
                            uint value;
                            var ixl = GetVarint(out value);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            options.JavaStringCheckUtf8 = value != 0;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, java_string_check_utf8 = {3}", type, no, ixl, value));
                            break;
                        }
                }
            }

            intend--;
        }

        /// <summary>
        /// The message descriptor proto.
        /// </summary>
        /// <param name="totalLength">
        /// The total length.
        /// </param>
        /// <param name="md">
        /// The message descriptor.
        /// </param>
        private void DescriptorProto(uint totalLength, MessageDescriptor md)
        {
            intend++;

            while (totalLength != 0)
            {
                int type, no;
                var l = GetTypeAndFieldNo(out type, out no);
                ix += l;
                totalLength -= (uint)l;

                switch (no)
                {
                    case 1: // string name
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            md.Name = GetString(length);
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, Name = {3}", type, no, length, md.Name));
                            ix += (int)length;
                            totalLength -= length;
                            break;
                        }

                    case 2: // FieldDescriptorProto[] field
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2} - FieldDescriptorProto field", type, no, length));
                            var fd = new FieldDescriptor();
                            md.Fields.Add(fd);
                            FieldDescriptorProto(length, fd);
                            totalLength -= length;
                            break;
                        }

                    case 3: // DescriptorProto[] nested_type
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2} - DescriptorProto", type, no, length));
                            var msg = new MessageDescriptor();
                            md.NestedTypes.Add(msg);
                            DescriptorProto(length, msg);
                            totalLength -= length;
                            break;
                        }

                    case 4: // EnumDescriptorProto[] enum_type
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2} - EnumDescriptorProto", type, no, length));
                            var ed = new EnumDescriptor();
                            md.EnumTypes.Add(ed);
                            EnumDescriptorProto(length, ed);
                            totalLength -= length;
                            break;
                        }

                    case 5: // ExtensionRange[] extension_range
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2} - ExtensionRange", type, no, length));
                            var er = new MessageDescriptor.ExtensionRange();
                            md.ExtensionRanges.Add(er);
                            ExtensionRange(length, er);
                            totalLength -= length;
                            break;
                        }

                    case 6: // FieldDescriptorProto[] extension
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2} - FieldDescriptorProto extension", type, no, length));
                            var fd = new FieldDescriptor();
                            md.Fields.Add(fd);
                            FieldDescriptorProto(length, fd);
                            totalLength -= length;
                            break;
                        }

                    case 7: // MessageOptions options
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2} - MessageOptions", type, no, length));
                            if (md.Options == null)
                            {
                                md.Options = new MessageOptions();
                            }

                            MessageOptions(length, md.Options);
                            totalLength -= length;
                            break;
                        }

                    case 8: // OneofDescriptorProto[] oneof_decl
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2} - OneofDescriptorProto", type, no, length));
                            var od = new OneofDescriptor();
                            md.OneofDecl.Add(od);
                            OneofDescriptorProto(length, od);
                            totalLength -= length;
                            break;
                        }
                }
            }

            intend--;
        }

        /// <summary>
        /// The extension range of the message.
        /// </summary>
        /// <param name="totalLength">
        /// The total length.
        /// </param>
        /// <param name="er">
        /// The extension range.
        /// </param>
        private void ExtensionRange(uint totalLength, MessageDescriptor.ExtensionRange er)
        {
            intend++;

            while (totalLength != 0)
            {
                int type, no;
                var l = GetTypeAndFieldNo(out type, out no);
                ix += l;
                totalLength -= (uint)l;

                switch (no)
                {
                    case 1: // int32 start
                        {
                            uint value;
                            var ixl = GetVarint(out value);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            er.Start = (int)value;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, start = {3}", type, no, ixl, value));
                            break;
                        }

                    case 2: // int32 end
                        {
                            uint value;
                            var ixl = GetVarint(out value);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            er.End = (int)value;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, end = {3}", type, no, ixl, value));
                            break;
                        }
                }
            }

            intend--;
        }

        /// <summary>
        /// The field descriptor proto.
        /// </summary>
        /// <param name="totalLength">
        /// The total length.
        /// </param>
        /// <param name="fd">
        /// The field descriptor.
        /// </param>
        // ReSharper disable once FunctionComplexityOverflow
        private void FieldDescriptorProto(uint totalLength, FieldDescriptor fd)
        {
            intend++;

            while (totalLength != 0)
            {
                int type, no;
                var l = GetTypeAndFieldNo(out type, out no);
                ix += l;
                totalLength -= (uint)l;

                switch (no)
                {
                    case 1: // string name
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            fd.Name = GetString(length);
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, Name = {3}", type, no, length, fd.Name));
                            ix += (int)length;
                            totalLength -= length;
                            break;
                        }

                    case 2: // string extendee
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            fd.Extendee = GetString(length);
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, extendee = {3}", type, no, length, fd.Extendee));
                            ix += (int)length;
                            totalLength -= length;
                            break;
                        }

                    case 3: // int32 number
                        {
                            uint value;
                            var ixl = GetVarint(out value);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            fd.Number = (int)value;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, number = {3}", type, no, ixl, value));
                            break;
                        }

                    case 4: // Label label
                        {
                            uint value;
                            var ixl = GetVarint(out value);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            fd.Label = (FieldDescriptor.Labels)value;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, label = {3}", type, no, ixl, value));
                            break;
                        }

                    case 5: // Type type
                        {
                            uint value;
                            var ixl = GetVarint(out value);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            fd.Type = (FieldDescriptor.Types)value;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, type = {3}", type, no, ixl, value));
                            break;
                        }

                    case 6: // string type_name
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            fd.TypeName = GetString(length);
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, type_name = {3}", type, no, length, fd.TypeName));
                            ix += (int)length;
                            totalLength -= length;
                            break;
                        }

                    case 7: // string default_value
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            fd.DefaultValue = GetString(length);
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, default_value = {3}", type, no, length, fd.DefaultValue));
                            ix += (int)length;
                            totalLength -= length;
                            break;
                        }

                    case 8: // FieldOptions options
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, FieldOptions", type, no, length));
                            if (fd.Options == null)
                            {
                                fd.Options = new FieldOptions();
                            }

                            FieldOptions(length, fd.Options);
                            totalLength -= length;
                            break;
                        }

                    case 9: // int32 oneof_index
                        {
                            uint value;
                            var ixl = GetVarint(out value);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            fd.OneofIndex = (int)value;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, oneof_index = {3}", type, no, ixl, value));
                            break;
                        }
                }
            }

            intend--;
        }

        /// <summary>
        /// The field options.
        /// </summary>
        /// <param name="totalLength">
        /// The total length.
        /// </param>
        /// <param name="options">The options.</param>
        private void FieldOptions(uint totalLength, FieldOptions options)
        {
            intend++;

            while (totalLength != 0)
            {
                int type, no;
                var l = GetTypeAndFieldNo(out type, out no);
                ix += l;
                totalLength -= (uint)l;

                switch (no)
                {
                    case 1: // CType ctype
                        {
                            uint value;
                            var ixl = GetVarint(out value);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            options.Ctype = (FieldOptions.CType)value;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, ctype = {3}", type, no, ixl, value));
                            break;
                        }

                    case 2: // bool packed
                        {
                            uint value;
                            var ixl = GetVarint(out value);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            options.Packed = value != 0;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, packed = {3}", type, no, ixl, value));
                            break;
                        }

                    case 3: // bool deprecated
                        {
                            uint value;
                            var ixl = GetVarint(out value);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            options.Deprecated = value != 0;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, Deprecated = {3}", type, no, ixl, value));
                            break;
                        }

                    case 5: // bool lazy
                        {
                            uint value;
                            var ixl = GetVarint(out value);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            options.Lazy = value != 0;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, lazy = {3}", type, no, ixl, value));
                            break;
                        }

                    case 9: // string experimental_map_key
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            options.ExperimentalMapKey = GetString(length);
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, experimental_map_key = {3}", type, no, length, options.ExperimentalMapKey));
                            ix += (int)length;
                            totalLength -= length;
                            break;
                        }

                    case 10: // bool weak
                        {
                            uint value;
                            var ixl = GetVarint(out value);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            options.Weak = value != 0;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, weak = {3}", type, no, ixl, value));
                            break;
                        }
                }
            }

            intend--;
        }

        /// <summary>
        /// The message options.
        /// </summary>
        /// <param name="totalLength">
        /// The total length.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        private void MessageOptions(uint totalLength, MessageOptions options)
        {
            intend++;

            while (totalLength != 0)
            {
                int type, no;
                var l = GetTypeAndFieldNo(out type, out no);
                ix += l;
                totalLength -= (uint)l;

                switch (no)
                {
                    case 1: // bool message_set_wire_format
                        {
                            uint value;
                            var ixl = GetVarint(out value);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            options.MessageSetWireFormat = value != 0;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, message_set_wire_format = {3}", type, no, ixl, value));
                            break;
                        }

                    case 2: // bool no_standard_descriptor_accessor
                        {
                            uint value;
                            var ixl = GetVarint(out value);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            options.NoStandardDescriptorAccessor = value != 0;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, no_standard_descriptor_accessor = {3}", type, no, ixl, value));
                            break;
                        }

                    case 3: // bool deprecated
                        {
                            uint value;
                            var ixl = GetVarint(out value);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            options.Deprecated = value != 0;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, Deprecated = {3}", type, no, ixl, value));
                            break;
                        }
                }
            }

            intend--;
        }

        /// <summary>
        /// The oneof descriptor proto.
        /// </summary>
        /// <param name="totalLength">
        /// The total length.
        /// </param>
        /// <param name="od">
        /// The oneof descriptor.
        /// </param>
        private void OneofDescriptorProto(uint totalLength, OneofDescriptor od)
        {
            intend++;

            while (totalLength != 0)
            {
                int type, no;
                var l = GetTypeAndFieldNo(out type, out no);
                ix += l;
                totalLength -= (uint)l;

                switch (no)
                {
                    case 1: // string name
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            od.Name = GetString(length);
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, Name = {3}", type, no, length, od.Name));
                            ix += (int)length;
                            totalLength -= length;
                            break;
                        }
                }
            }

            intend--;
        }

        /// <summary>
        /// The enum descriptor proto.
        /// </summary>
        /// <param name="totalLength">
        /// The total length.
        /// </param>
        /// <param name="ed">
        /// The enum descriptor.
        /// </param>
        private void EnumDescriptorProto(uint totalLength, EnumDescriptor ed)
        {
            intend++;

            while (totalLength != 0)
            {
                int type, no;
                var l = GetTypeAndFieldNo(out type, out no);
                ix += l;
                totalLength -= (uint)l;

                switch (no)
                {
                    case 1: // string name
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            ed.Name = GetString(length);
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, Name = {3}", type, no, length, ed.Name));
                            ix += (int)length;
                            totalLength -= length;
                            break;
                        }

                    case 2: // EnumValueDescriptorProto value
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2} - EnumValueDescriptorProto", type, no, length));
                            var evd = new EnumValueDescriptor();
                            ed.Values.Add(evd);
                            EnumValueDescriptorProto(length, evd);
                            totalLength -= length;
                            break;
                        }

                    case 3: // EnumOptions options
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2} - EnumOptions", type, no, length));
                            if (ed.Options == null)
                            {
                                ed.Options = new EnumOptions();
                            }

                            EnumOptions(length, ed.Options);
                            totalLength -= length;
                            break;
                        }
                }
            }

            intend--;
        }

        /// <summary>
        /// The enum options.
        /// </summary>
        /// <param name="totalLength">
        /// The total length.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        private void EnumOptions(uint totalLength, EnumOptions options)
        {
            intend++;

            while (totalLength != 0)
            {
                int type, no;
                var l = GetTypeAndFieldNo(out type, out no);
                ix += l;
                totalLength -= (uint)l;

                switch (no)
                {
                    case 2: // bool allow_alias
                        {
                            uint value;
                            var ixl = GetVarint(out value);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            options.AllowAlias = value != 0;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, allow_alias = {3}", type, no, ixl, value));
                            break;
                        }

                    case 3: // bool deprecated
                        {
                            uint value;
                            var ixl = GetVarint(out value);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            options.Deprecated = value != 0;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, Deprecated = {3}", type, no, ixl, value));
                            break;
                        }
                }
            }

            intend--;
        }

        /// <summary>
        /// The enum value descriptor proto.
        /// </summary>
        /// <param name="totalLength">
        /// The total length.
        /// </param>
        /// <param name="ed">
        /// The ed.
        /// </param>
        private void EnumValueDescriptorProto(uint totalLength, EnumValueDescriptor ed)
        {
            intend++;

            while (totalLength != 0)
            {
                int type, no;
                var l = GetTypeAndFieldNo(out type, out no);
                ix += l;
                totalLength -= (uint)l;

                switch (no)
                {
                    case 1: // string name
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            ed.Name = GetString(length);
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, Name = {3}", type, no, length, ed.Name));
                            ix += (int)length;
                            totalLength -= length;
                            break;
                        }

                    case 2: // string number
                        {
                            uint value;
                            var ixl = GetVarint(out value);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            ed.Number = (int)value;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, Number = {3}", type, no, ixl, value));
                            break;
                        }

                    case 3: // EnumValueOptions options
                        {
                            uint length;
                            var ixl = GetVarint(out length);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2} - EnumValueOptions", type, no, length));
                            if (ed.Options == null)
                            {
                                ed.Options = new EnumValueOptions();
                            }

                            EnumValueOptions(length, ed.Options);
                            totalLength -= length;
                            break;
                        }
                }
            }

            intend--;
        }

        /// <summary>
        /// The enum value options.
        /// </summary>
        /// <param name="totalLength">
        /// The total length.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        private void EnumValueOptions(uint totalLength, EnumValueOptions options)
        {
            intend++;

            while (totalLength != 0)
            {
                int type, no;
                var l = GetTypeAndFieldNo(out type, out no);
                ix += l;
                totalLength -= (uint)l;

                switch (no)
                {
                    case 1: // bool deprecated
                        {
                            uint value;
                            var ixl = GetVarint(out value);
                            ix += ixl;
                            totalLength -= (uint)ixl;
                            options.Deprecated = value != 0;
                            Write(string.Format("Type = {0}, F#= {1}, Length = {2}, Deprecated = {3}", type, no, ixl, value));
                            break;
                        }
                }
            }

            intend--;
        }

        /// <summary>
        /// The get string.
        /// </summary>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetString(uint length)
        {
            var result = System.Text.Encoding.UTF8.GetString(array.Skip(ix).Take((int)length).ToArray());
            return result;
        }

        /// <summary>
        /// The get varint.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        /// <exception cref="OverflowException">
        /// Vakue too big.
        /// </exception>
        private int GetVarint(out uint value)
        {
            int b = array[ix];
            value = (uint)b;
            if ((value & 0x80) == 0)
            {
                return 1;
            }

            value &= 0x7F;

            b = array[ix + 1];
            value |= ((uint)b & 0x7F) << 7;
            if ((b & 0x80) == 0)
            {
                return 2;
            }

            b = array[ix + 2];
            value |= ((uint)b & 0x7F) << 14;
            if ((b & 0x80) == 0)
            {
                return 3;
            }

            b = array[ix + 3];
            value |= ((uint)b & 0x7F) << 21;
            if ((b & 0x80) == 0)
            {
                return 4;
            }

            b = array[ix + 4];
            value |= (uint)b << 28; // can only use 4 bits from this chunk
            if ((b & 0xF0) == 0)
            {
                return 5;
            }

            throw new OverflowException();
        }

        /// <summary>
        /// The get type and field no.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="fieldNo">
        /// The field no.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int GetTypeAndFieldNo(out int type, out int fieldNo)
        {
            uint length;
            var ixl = GetVarint(out length);

            type = (int)(length & 0x07);
            fieldNo = (int)(length >> 3);
            return ixl;
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        private void Write(string text)
        {
            Console.Write(new string(' ', intend * 2));
            Console.WriteLine(text);
        }
    }
}
