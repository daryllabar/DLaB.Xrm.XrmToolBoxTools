using DLaB.VSSolutionAccelerator.Wizard;
using System;
using System.Collections.Generic;
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

        public static InitializeSolutionInfo InitializeSolutionInfo(string solutionPath, AddAllWizardResults results = null, Dictionary<int, Guid> solutions = null)
        {
            var values = (results ?? new AddAllWizardResults
            {
                P0AddToExistingSolution = true, P0SolutionPath = solutionPath,
                P1Namespace = "Abc.Xrm",
                P2EarlyBound = true,
                P3SharedCommonAssemblyName = "Abc.Xrm",
                P4UseXrmUnitTest = true, P4TestSettingsProjectName = "Abc.Xrm.Test",
                P5CreatePluginProject = true, P5PluginProjectName = "Abc.Xrm.Plugin", P5IncludeExamples = true,
                P6CompanyName = "Acme", P6PluginDescription = "Test Description For Plugin", P6PluginSolutionIndex = 0, P6PacAuthName = "Abc Dev",
                P7PluginTestProjectName = "Abc.Xrm.Plugin.Tests",
                P8CreateWorkflowProject = true, P8WorkflowProjectName = "Abc.Xrm.Workflow", P8IncludeExamples = true,
                P9SharedWorkflowProjectName = "Abc.Xrm.Workflow",
                P10WorkflowTestProjectName = "Abc.Xrm.Workflow.Tests",
                P11InstallCodeSnippets = true, P11IncludeCodeGen = true

            }).GetResults();
            solutions = solutions ?? new Dictionary<int, Guid> { { 0, Guid.Empty } };
            return VSSolutionAccelerator.InitializeSolutionInfo.Create(values, solutions);
        }
    }
}
