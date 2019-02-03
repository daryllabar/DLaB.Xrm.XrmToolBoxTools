using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLaB.VSSolutionAccelerator.Logic
{
    public enum PropertyGroupType
    {
        ProjectProperties,
        CompileConfiguration,
        SignAssembly,
        KeyFile,
        Unknown
    }

    public class PropertyGroup
    {
        public string OpenTag { get; set; }
        public PropertyGroupType Type { get; set; }
        public List<string> Lines { get; set; }

        public PropertyGroup(string openTag)
        {
            OpenTag = openTag;
            Type = openTag.Contains("PropertyGroup Condition=")
                ? PropertyGroupType.CompileConfiguration
                : PropertyGroupType.Unknown;
            Lines = new List<string> {openTag};
        }

        public void AddLine(string line)
        {
            Lines.Add(line);
            if (Type != PropertyGroupType.Unknown)
            {
                return;
            }

            if (line.Contains("<ProjectGuid>"))
            {
                Type = PropertyGroupType.ProjectProperties;
            }else if (line.Contains("<SignAssembly>"))
            {
                Type = PropertyGroupType.SignAssembly;
            }
            else if (line.Contains("AssemblyOriginatorKeyFile"))
            {
                Type = PropertyGroupType.KeyFile;
            }
        }
    }
}
