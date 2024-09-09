using DLaB.XrmToolBoxCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using XrmToolBox.Extensibility.Interfaces;

namespace DLaB.VSSolutionAccelerator
{
    [Export(typeof(IXrmToolBoxPlugin)),
         ExportMetadata("Name", "Visual Studio Solution Accelerator"),
         ExportMetadata("Description", "Adds recommended isolation/accelerator projects for use with the DLaB.Xrm and XrmUnitTest framework to your Visual Studio solution."),
         ExportMetadata("SmallImageBase64", SmallImage32X32), // null for "no logo" image or base64 image content 
         ExportMetadata("BigImageBase64", LargeImage120X120), // null for "no logo" image or base64 image content 
         ExportMetadata("BackgroundColor", "White"), // Use a HTML color name
         ExportMetadata("PrimaryFontColor", "#000000"), // Or an hexadecimal code
         ExportMetadata("SecondaryFontColor", "DarkGray")]
    public class VsSolutionAccelerator : PluginFactory, INoConnectionRequired
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public VsSolutionAccelerator()
        {
            // If you have external assemblies that you need to load, uncomment the following to
            // hook into the event that will fire when an Assembly fails to resolve
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolveEventHandler;
        }

        public override IXrmToolBoxPluginControl GetControl()
        {
            return new VsSolutionAcceleratorPlugin();
        }

        private Assembly AssemblyResolveEventHandler(object sender, ResolveEventArgs args)
        {
            Assembly loadAssembly = null;
            Assembly currAssembly = Assembly.GetExecutingAssembly();

            // base name of the assembly that failed to resolve
            var argName = args.Name.Substring(0, args.Name.IndexOf(","));

            // check to see if the failing assembly is one that we reference.
            List<AssemblyName> refAssemblies = currAssembly.GetReferencedAssemblies().ToList();
            var refAssembly = refAssemblies.Where(a => a.Name == argName).FirstOrDefault();

            // if the current unresolved assembly is referenced by our plugin, attempt to load
            if (refAssembly != null)
            {
                // load from the path to this plugin assembly, not host executable
                string dir = Path.GetDirectoryName(currAssembly.Location).ToLower();
                string folder = Path.GetFileNameWithoutExtension(currAssembly.Location);
                dir = Path.Combine(dir, folder);

                var assmbPath = Path.Combine(dir, $"{argName}.dll");

                if (File.Exists(assmbPath))
                {
                    loadAssembly = Assembly.LoadFrom(assmbPath);
                }
                else
                {
                    throw new FileNotFoundException($"Unable to locate dependency: {assmbPath}");
                }
            }

            return loadAssembly;
        }
    }
}