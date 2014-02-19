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
using Microsoft.VisualStudio.XmlEditor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace GProssliner.NuGetTemplatesPlus_VSPackage
{
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
    public sealed class NuGetTemplatesPlus_VSPackagePackage : Package
    {
        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public NuGetTemplatesPlus_VSPackagePackage()
        {
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

        void _windowEvents_WindowActivated(Window gotFocus, Window lostFocus) {
            if (gotFocus.ProjectItem == null)
                return;


            var projectItem = new EnvDTEProjectItemInfo(gotFocus.ProjectItem);
            var schemas = projectItem.GetXmlSchemaInfos().ToList();

            if (schemas.Count() == 0)
                return;

            var textManager = (IVsTextManager)this.GetService(typeof(SVsTextManager));

            var textView = default(IVsTextView);
            textManager.GetActiveView(1, null, out textView);

            if (textView == null)
                return;

            var textLines = default(IVsTextLines);
            textView.GetBuffer(out textLines);
            if (textLines == null)
                return;

            var userData = textLines as IVsUserData;
            if (userData == null)
                return;

            XmlSchemaContext schemaContext;
            object obj2;
            Guid gUID = typeof(XmlSchemaContext).GUID;
            userData.GetData(ref gUID, out obj2);


            if (obj2 is XmlSchemaContext) {
                schemaContext = (XmlSchemaContext)obj2;
            } else
                return;


            var project = new EnvDTEProjectInfo(ActiveProject);

            foreach (var schemaInfo in schemas) {
                var existingRef = schemaContext.Included.Where(s => s.TargetNamespace == schemaInfo.TargetNamespace).SingleOrDefault();
                if (existingRef != null)
                    schemaContext.Included.Remove(existingRef);

                schemaContext.Included.Add(new XmlSchemaReference(schemaInfo.TargetNamespace, new Uri(schemaInfo.XsdFilePath)));
            }

            schemaContext.OnChanged();

        }

        Project ActiveProject {
            get {
                return (DTE.ActiveSolutionProjects as Array).GetValue(0) as Project;
            }
        }


        List<string> _filesToDelete;

        void _addNewProjectItemEvents_AfterExecute(string Guid, int ID, object CustomIn, object CustomOut) {
            foreach (var fileToDelete in _filesToDelete) {
                File.Delete(fileToDelete);
            }
        }

        void _addNewProjectItemEvents_BeforeExecute(string Guid, int ID, object CustomIn, object CustomOut, ref bool CancelDefault) {

            _filesToDelete = new List<string>();

            var project = new EnvDTEProjectInfo(ActiveProject);
            foreach (var templatePath in project.GetItemTemplates("Visual C#")) {
                var profilePath = Path.Combine(userItemTemplatesLocation, "Visual C#", Path.GetFileName(templatePath));
                File.Copy(templatePath, profilePath);
            }

        }



        #endregion

    }
}
