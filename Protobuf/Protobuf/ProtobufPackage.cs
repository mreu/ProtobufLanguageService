// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtobufPackage.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MichaelReukauff.Protobuf
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    using EnvDTE;

    using MichaelReukauff.Protobuf.Options;

    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;

    using Constants = EnvDTE.Constants;

    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell.
    /// </para>
    /// </summary>
    //// This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]

    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]

    // guid of this package
    [Guid(GuidList.guidProtobufPkgString)]

    // load this package even if no solution or project is loaded
    [ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string)]

    [ProvideOptionPage(typeof(OptionPage1), "Protobuf", "General", 0, 0, true)]

    // ReSharper disable once InconsistentNaming
    public sealed class ProtobufPackage : Package
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProtobufPackage"/> class.
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require
        /// any Visual Studio service because at this point the package object is created but
        /// not sited yet inside Visual Studio environment. The place to do all the other
        /// initialization is the Initialize method.
        /// </summary>
        public ProtobufPackage()
        {
            var line = string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", ToString());
            Debug.WriteLine(line);
        }

        /////////////////////////////////////////////////////////////////////////////
        // Overridden Package Implementation
        /////////////////////////////////////////////////////////////////////////////

        #region Package Members
        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", ToString()));
            base.Initialize();

            SetupEvents();
        }
        #endregion Package Members

        #region Private Methods
        /// <summary>
        /// Set up the events we wnt to handle.
        /// </summary>
        private void SetupEvents()
        {
            var dte = (DTE)GetService(typeof(SDTE));
            var dteEvents = dte.Events;
            var documentEvents = dteEvents.DocumentEvents;
            documentEvents.DocumentSaved += OnDocumentSaved;
        }

        /// <summary>
        /// Event handler when a decument was saved.
        /// </summary>
        /// <param name="document">The document.</param>
        private void OnDocumentSaved(Document document)
        {
            var page = (OptionPage1)GetDialogPage(typeof(OptionPage1));
            if (!page.AutoConvert)
            {
                return;
            }

            var path = document.FullName;
            var ext = Path.GetExtension(path);
            if (ext == null || ext.ToLowerInvariant() != ".proto")
            {
                return;
            }

            SaveProtoWithoutBom(path, document);

            ////var file = path + ".cs";

            //// remove all dependent files
            ////while (document.ProjectItem.ProjectItems.Count > 0)
            ////{
            ////    document.ProjectItem.ProjectItems.Item(1).Delete();
            ////}

            ////using (var str = new StreamWriter(file))
            ////{
            ////    str.WriteLine("//-----------------------------------------------------------------------------");
            ////    str.WriteLine("// <auto-generated>");
            ////    str.WriteLine("//     This code was generated by a tool.");
            ////    str.WriteLine("//");
            ////    str.WriteLine("//     Changes to this file may cause incorrect behavior and will be lost if");
            ////    str.WriteLine("//     the code is regenerated.");
            ////    str.WriteLine("// </auto-generated>");
            ////    str.WriteLine("//------------------------------------------------------------------------------");
            ////}

            ////document.ProjectItem.ProjectItems.AddFromFile(file);
        }

        /// <summary>
        /// Save the file without a bom.
        /// </summary>
        /// <param name="file">The filename.</param>
        /// <param name="doc">The document.</param>
        private void SaveProtoWithoutBom(string file, Document doc)
        {
            byte[] bytes;

            // read the first three byte to see if a byte order mark (BOM) is set
            using (var reader = new BinaryReader(File.Open(file, FileMode.Open)))
            {
                ////bytes = reader.ReadBytes((int)reader.BaseStream.Length);
                bytes = reader.ReadBytes(3);
            }

            if (bytes[0] != 0xef || bytes[1] != 0xbb || bytes[2] != 0xbf)
            {
                return;
            }

            foreach (Project project in doc.DTE.Solution.Projects)
            {
                foreach (ProjectItem item in project.ProjectItems)
                {
                    Debug.WriteLine(item.Name);
                    var projfile = (string)item.Properties.Item("FullPath").Value;

                    if (string.Equals(projfile, file, StringComparison.CurrentCultureIgnoreCase))
                    {
                        doc.Close(vsSaveChanges.vsSaveChangesYes);

                        item.Remove();

                        ////using (var writer = new BinaryWriter(File.Open(file, FileMode.Truncate)))
                        ////{
                        ////    writer.Write(bytes.Skip(3).ToArray());
                        ////}

                        var text = File.ReadAllText(file);

                        File.Delete(file);

                        using (var stream = new StreamWriter(file, false, new UTF8Encoding(false)))
                        {
                            stream.Write(text);
                            stream.Flush();
                        }

                        project.ProjectItems.AddFromFile(file);
                        var win = doc.DTE.OpenFile(Constants.vsViewKindCode, file);
                        win.Activate();

                        break;
                    }

                    ////foreach (Property property in item.Properties)
                    ////{
                    ////    Debug.WriteLine("  " + property.Name + " - " + property.Value);
                    ////}
                }
            }
        }
        #endregion Private Methods
    }
}