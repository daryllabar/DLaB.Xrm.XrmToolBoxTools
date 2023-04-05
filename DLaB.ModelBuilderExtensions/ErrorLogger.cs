using System;
using System.IO;

namespace DLaB.ModelBuilderExtensions
{
    public class ErrorLogger
    {
        public static void Log(Exception ex)
        {
            try
            {
                var settings = ConfigHelper.Settings;
                if (string.IsNullOrWhiteSpace(settings.DLaBModelBuilder.XrmToolBoxPluginPath))
                {
                    // Only write a log file for the XrmToolBox since it doesn't have a good way to export
                    return;
                }
                var logFilePath = GetLogPath(settings.DLaBModelBuilder.XrmToolBoxPluginPath);
                if (!File.Exists(logFilePath))
                {
                    File.WriteAllText(logFilePath, ex.ToString());
                    return;
                }

                File.AppendAllText(logFilePath, Environment.NewLine + ex);

            }
            catch
            {
                // Eat it
            }
            finally
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static string GetLogPath(string xrmToolBoxPluginPath)
        {
            return Path.Combine(xrmToolBoxPluginPath, "DLaB.EarlyBoundGeneratorV2", "DLaB.ModelBuilderExtensions.Log.txt");
        }
    }
}
