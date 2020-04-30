using System;
using System.Collections.Generic;
using System.IO;

namespace DLaB.VSSolutionAccelerator.Logic
{
    [Serializable]
    public class NuGetSettings
    {
        public string ExePath { get; set; }
        public List<string> Sources { get; set; }
        public string ContentInstallerPath { get; set; }

        public NuGetSettings(string templateDirectory): this()
        {
            ExePath = Path.Combine(templateDirectory, "bin\\nuget.exe");
            ContentInstallerPath = Path.Combine(templateDirectory, "bin\\nugetContentInstaller.exe");
        }

        public NuGetSettings()
        {
            Sources = new List<string>
            {
                @"https://api.nuget.org/v3/index.json"
            };
        }
    }
}
