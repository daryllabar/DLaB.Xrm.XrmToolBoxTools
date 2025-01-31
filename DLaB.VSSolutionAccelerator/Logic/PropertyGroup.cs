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

    public class PropertyGroup : FileGroup
    {
        public string OpenTag { get; set; }
        public PropertyGroupType Type { get; set; }

        public struct ConfigTags
        {
            public const string ProjectGuid = "<ProjectGuid>";
            public const string RootNamespace = "<RootNamespace>";
            public const string AssemblyName = "<AssemblyName>";
            public const string TargetFrameworkVersion = "<TargetFrameworkVersion>";
        }

        public PropertyGroup(string openTag) : base(GroupType.PropertyGroup, openTag)
        {
            OpenTag = openTag;
            Type = openTag.Contains("PropertyGroup Condition=")
                ? PropertyGroupType.CompileConfiguration
                : PropertyGroupType.Unknown;
        }

        public override void AddLine(string line)
        {
            base.AddLine(line);
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
