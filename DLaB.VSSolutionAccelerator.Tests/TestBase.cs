using System.IO;
using System.Reflection;

namespace DLaB.VSSolutionAccelerator.Tests
{
    public static class TestBase
    {
        public static void ClearDirectory(string directory)
        {
            var di = new DirectoryInfo(directory);
            foreach (var file in di.EnumerateFiles())
            {
                file.Delete();
            }
            foreach (var dir in di.EnumerateDirectories())
            {
                try
                {
                    dir.Delete(true);
                }
                catch
                {
                    System.Threading.Thread.Sleep(2000);
                    dir.Delete(true);
                }
            }
        }
        public static string GetTemplatePath()
        {
            var pluginsPath = GetPluginsPath();
            var templatePath = Path.GetFullPath(Path.Combine(pluginsPath, Settings.TemplateFolder));
            return templatePath;
        }

        public static string GetPluginsPath()
        {
            var output = Path.GetFileName(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            var pluginsPath = Path.Combine(Assembly.GetExecutingAssembly().Location, $@"..\..\..\..\DLaB.VSSolutionAccelerator\bin\{output}\Plugins");
            return pluginsPath;
        }
    }
}
