using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using System.IO;
using System.Collections.Generic;
using EnvDTE;
using System.Linq;
using Microsoft.VisualStudio.TextManager.Interop;
using NuGetTemplatesPlus.Engine;
using NuGetTemplatesPlus.Library.Interface;

namespace GProssliner.NuGetTemplatesPlus_VSPackage {
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [Guid(GuidList.guidNuGetTemplatesPlus_VSPackagePkgString)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string)]
    public sealed class NuGetTemplatesPlus_VSPackagePackage : Package {
        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public NuGetTemplatesPlus_VSPackagePackage() {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }



        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation
        #region Package Members


        CommandEvents _addNewProjectItemEvents;
        WindowEvents _windowEvents;

        EnvDTE.DTE DTE;
        string userItemTemplatesLocation;

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize() {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));

            DTE = this.GetService(typeof(Microsoft.VisualStudio.Shell.Interop.SDTE)) as DTE;

            using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(DTE.RegistryRoot)) {
                userItemTemplatesLocation = key.GetValue("UserItemTemplatesLocation").ToString();
            }


            _addNewProjectItemEvents = DTE.Events.CommandEvents["{5EFC7975-14BC-11CF-9B2B-00AA00573819}", 220];
            _addNewProjectItemEvents.BeforeExecute += _addNewProjectItemEvents_BeforeExecute;
            _addNewProjectItemEvents.AfterExecute += _addNewProjectItemEvents_AfterExecute;

            _windowEvents = DTE.Events.WindowEvents;
            _windowEvents.WindowActivated += _windowEvents_WindowActivated;

            base.Initialize();

        }

        IVsTextView GetIVsTextViewFromWindow(Window window) {

            var filePath = window.ProjectItem.FileNames[0];

            var windowFrame = default(IVsWindowFrame);
            var unusedUiHierarchy = default(IVsUIHierarchy);
            var unusedItemID = default(uint);

            if (VsShellUtilities.IsDocumentOpen(this, filePath, Guid.Empty, out unusedUiHierarchy, out unusedItemID, out windowFrame)) {
                return VsShellUtilities.GetTextView(windowFrame);
            }

            return null;
        }

        object GetXmlSchemaContextFromTextView(IVsTextView textView) {

            var textLines = default(IVsTextLines);
            textView.GetBuffer(out textLines);
            if (textLines == null)
                return null;

            var userData = textLines as IVsUserData;
            if (userData == null)
                return null;


            var xmlSchemaContext = default(object);
            var xmlSchemaContextGuid = new Guid("191c32ae-4c86-4a77-8acf-c0317afb9172");
            userData.GetData(ref xmlSchemaContextGuid, out xmlSchemaContext);

            return xmlSchemaContext;

        }

        class XmlSchemaSourceFile : ISourceFile {

            #region ISourceFile Members

            public string FilePath {
                get;
                set;
            }

            public string FileContent {
                get { return File.ReadAllText(this.FilePath); }
            }

            #endregion
        }

        void _windowEvents_WindowActivated(Window gotFocus, Window lostFocus) {

            if (gotFocus.ProjectItem == null || gotFocus.ProjectItem.Name == null || gotFocus.ProjectItem.Name.Length == 0)
                return;

            object schemaContext = GetXmlSchemaContextFromTextView(GetIVsTextViewFromWindow(gotFocus));
            if (schemaContext == null)
                return;



            var projectItem = new EnvDTEProjectItemInfo(gotFocus.ProjectItem);
            var schemas = projectItem.GetXmlSchemaInfos(new XmlSchemaSourceFile { FilePath = projectItem.FullName }).ToList();

            if (schemas.Count() == 0)
                return;


            var helper = new SchemaContextHelper(schemaContext);
            helper.ApplySchemaInfos(schemas);

        }

        class SchemaContextHelper {

            dynamic SchemaContext;

            public SchemaContextHelper(dynamic schemaContext) {
                this.SchemaContext = schemaContext;
            }

            public void ApplySchemaInfos(IEnumerable<XmlSchemaInfo> schemaInfos) {

                var dirty = false;

                foreach (var schemaInfo in schemaInfos) {
                    var existingReference = GetExistingSchemaReference(schemaInfo.TargetNamespace);
                    if (existingReference != null) {
                        if (existingReference.Location != schemaInfo.Location) {
                            this.SchemaContext.Included.Remove(existingReference);
                            AddXmlSchemaReference(schemaInfo.TargetNamespace, schemaInfo.Location);
                            dirty = true;
                        }
                    } else {
                        AddXmlSchemaReference(schemaInfo.TargetNamespace, schemaInfo.Location);
                        dirty = true;
                    }
                }

                if (dirty)
                    this.SchemaContext.OnChanged();
            }

            private void AddXmlSchemaReference(string targetNamespace, Uri location) {
                var xmlSchemaReferenceType = ((object)this.SchemaContext).GetType().Assembly.GetType("Microsoft.VisualStudio.XmlEditor.XmlSchemaReference");
                var ctor = xmlSchemaReferenceType.GetConstructor(new Type[] { typeof(string), typeof(Uri) });
                var xmlSchemaReference = ctor.Invoke(new object[] { targetNamespace, location });

                // during limitation of calling interface-methods from dynamic we need to do some reflection-stuff here
                var iCollectionType = typeof(ICollection<>).MakeGenericType(xmlSchemaReferenceType);
                var addMethod = iCollectionType.GetMethod("Add");
                addMethod.Invoke(this.SchemaContext.Included, new object[] { xmlSchemaReference });
            }

            dynamic GetExistingSchemaReference(string targetNamespace) {
                foreach (var reference in this.SchemaContext.Included) {
                    if (reference.TargetNamespace == targetNamespace) return reference;
                }

                return null;
            }
        }


        Project ActiveProject {
            get {
                return (DTE.ActiveSolutionProjects as Array).GetValue(0) as Project;
            }
        }

        void DeleteTempoaryTemplates() {
            foreach (var filePath in Directory.GetFiles(userItemTemplatesLocation, "*_NuGetTemplatesPlus_temp.zip", SearchOption.AllDirectories)) {
                File.Delete(filePath);
            }
        }


        void _addNewProjectItemEvents_AfterExecute(string Guid, int ID, object CustomIn, object CustomOut) {
            DeleteTempoaryTemplates();
        }

        void _addNewProjectItemEvents_BeforeExecute(string Guid, int ID, object CustomIn, object CustomOut, ref bool CancelDefault) {

            DeleteTempoaryTemplates();

            var project = new EnvDTEProjectInfo(ActiveProject);
            foreach (var templatePath in project.GetItemTemplates("Visual C#")) {
                var profilePath = Path.Combine(userItemTemplatesLocation, "Visual C#", Path.GetFileNameWithoutExtension(templatePath) + "_NuGetTemplatesPlus_temp.zip");
                File.Copy(templatePath, profilePath);
            }

        }



        #endregion

    }
}
