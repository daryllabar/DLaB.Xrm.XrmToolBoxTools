using System;
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
                    foreach(var i in new[] {200, 2000, 10000 })
                    {
                        System.Threading.Thread.Sleep(i);
                        if (dir.Exists)
                        {
                            try
                            {
                                System.Threading.Thread.Sleep(2000);
                                if (dir.Exists)
                                {
                                    dir.Delete(true);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(@"Unable to delete directory {0} due to error: {1}.  Potentially trying again, post thead sleep.", dir.FullName, ex);
                            }
                        }
                    }
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
