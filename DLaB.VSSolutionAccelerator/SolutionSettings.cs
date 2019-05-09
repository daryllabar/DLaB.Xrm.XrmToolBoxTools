using System;
using System.IO;
using System.Xml.Serialization;

namespace DLaB.VSSolutionAccelerator
{
    [Serializable]
    [XmlType("VsSolutionAccelerator")]
    [XmlRoot("VsSolutionAccelerator")]
    public class SolutionSettings
    {
        /// <summary>
        /// The name of the Base Plugin File
        /// </summary>
        public string BasePluginFileName { get; set; }
        public string CodeActivityBaseFileName { get; set; }
        public string TestMethodClassBaseFileName { get; set; }

        public static SolutionSettings Load(string solutionPath)
        {
            var filePath = "UNKNOWN";
            try
            {
                filePath = GetPathFromSolution(solutionPath);
                if (!File.Exists(filePath))
                {
                    return GetDefault();
                }

                var serializer = new XmlSerializer(typeof(SolutionSettings));
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var settings = (SolutionSettings)serializer.Deserialize(fs);
                    fs.Close();
                    return settings;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured attempting to load Xml configuration: " + filePath, ex);
            }
        }

        /// <summary>
        /// Settings should be loaded from "{SolutionName}.vssacc"
        /// </summary>
        /// <param name="solutionPath"></param>
        /// <returns></returns>
        public static string GetPathFromSolution(string solutionPath)
        {
             return Path.Combine(Path.GetDirectoryName(solutionPath) ?? string.Empty,
                                        Path.GetFileNameWithoutExtension(solutionPath) + ".vssacc");
        }

        private static SolutionSettings GetDefault()
        {
            return new SolutionSettings
            {
                //BasePluginClassName = "PluginBase",
                BasePluginFileName = "PluginBase.cs",
                CodeActivityBaseFileName = "CodeActivityBase.cs",
                TestMethodClassBaseFileName = "TestMethodClassBase.cs"

            };
        }
    }
}
