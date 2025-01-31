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
    public class VsSolutionAccelerator : PluginFactory
    {
        public override IXrmToolBoxPluginControl GetControl()
        {
            return new VsSolutionAcceleratorPlugin();
        }
    }
}