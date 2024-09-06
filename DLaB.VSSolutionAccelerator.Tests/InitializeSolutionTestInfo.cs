using System;

namespace DLaB.VSSolutionAccelerator.Tests
{
    public class InitializeSolutionTestInfo : IDisposable
    {
        public Logic.SolutionInitializer SolutionInitializer { get; set; }
        public InitializeSolutionInfo Info { get; set; }
        public ITempDir TempDir { get; set; }
        public string TemplatePath { get; internal set; }
        public string SolutionDirectory { get; set; }

        public void Dispose()
        {
            TempDir?.Dispose();
        }
    }
}
