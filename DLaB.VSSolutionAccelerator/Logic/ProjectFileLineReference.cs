namespace DLaB.VSSolutionAccelerator.Logic
{
    public class ProjectFileLineReference
    {
        public FileGroup Group { get; set; }
        public int Index { get; set; }
        public string Line { get; set; }

        public ProjectFileLineReference() { }

        public ProjectFileLineReference(FileGroup group)
        {
            Group = group;
            Index = group.Length - 1;
            Line = group.Last;
        }
    }
}
