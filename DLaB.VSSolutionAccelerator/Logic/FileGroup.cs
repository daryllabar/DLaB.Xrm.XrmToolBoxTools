using System.Collections.Generic;

namespace DLaB.VSSolutionAccelerator.Logic
{
    public class FileGroup
    {
        private readonly List<string> _lines = new List<string>();

        public GroupType GroupType { get; }
        public IEnumerable<string> Lines => _lines;
        public int Length => _lines.Count;
        public string Last => _lines.Count == 0 ? null : _lines[Length - 1];

        public FileGroup(GroupType groupType, string line)
        {
            GroupType = groupType;
            _lines.Add(line);
        }

        public FileGroup(GroupType groupType) {
            GroupType = groupType;    
        }

        public virtual void AddLine(string line)
        {
            _lines.Add(line);
        }
    }

    public enum GroupType
    {
        Generic,
        ItemGroup,
        PropertyGroup,
        Root,
        Target,
    }
}
