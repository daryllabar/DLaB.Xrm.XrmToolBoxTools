using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Source.DLaB.Common;

namespace DLaB.VSSolutionAccelerator
{
    /// <summary>
    /// This class can help you to store settings for your plugin
    /// </summary>
    /// <remarks>
    /// This class must be XML serializable
    /// </remarks>
    [Serializable]
    public class Settings
    {
        public const string TemplateFolder = "DLaB.VSSolutionAccelerator";

        /// <summary>
        /// Pipe delimited list of Nuget Sources.  Defaults to  https://api.nuget.org/v3/index.json
        /// </summary>
        public string NugetSources { get; set; }

        [XmlIgnore]
        public List<string> NugetSourcesList { get; set; }

        public void Initialize()
        {
            NugetSourcesList = NugetSources.GetList<string>();
            if (NugetSourcesList.Count != 0)
            {
                return;
            }

            NugetSources = @"https://api.nuget.org/v3/index.json";
            NugetSourcesList.Add(NugetSources);
        }
    }
}