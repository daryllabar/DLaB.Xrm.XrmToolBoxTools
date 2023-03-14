namespace DLaB.ModelBuilderExtensions
{
    public class FileToWrite
    {
        public string Directory => System.IO.Path.GetDirectoryName(Path);
        public string Path { get; }
        public string Contents { get; }
        public bool IsMainFile { get; }
        public bool HasChanged { get; set; }
        public string TempFile { get; set; }

        public FileToWrite(string path, string contents, bool isMainFile = false)
        {
            Path = path;
            Contents = contents;
            IsMainFile = isMainFile;
            HasChanged = true;
        }
    }
}
