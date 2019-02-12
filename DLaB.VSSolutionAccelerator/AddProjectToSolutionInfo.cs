using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DLaB.Log;
using DLaB.VSSolutionAccelerator.Logic;
using DLaB.VSSolutionAccelerator.Wizard;
using Source.DLaB.Common;

namespace DLaB.VSSolutionAccelerator
{
    public class AddProjectToSolutionInfo : SolutionEditorInfo
    {
        public bool CreatePluginTest { get; set; }
        public bool CreateWorkflowTest { get; set; }
        public ProjectFileParser SharedTestProject { get; private set; }

        private struct Page
        {
            public const int CreatePlugin = 1;
            public const int CreateWorkflow = 3;
        }

        public static List<IWizardPage> InitializePages()
        {
            var pages = new List<IWizardPage>();
            AddSolutionNameQuestion(pages); // 0
            AddCreateProjectQuestion(pages, "Plugin");
            AddCreateXrmUnitTestProjectQuestion(pages, Page.CreatePlugin);
            AddCreateProjectQuestion(pages, "Workflow");
            AddCreateXrmUnitTestProjectQuestion(pages, Page.CreateWorkflow);

            return pages;
        }

        private static void AddSolutionNameQuestion(List<IWizardPage> pages)
        {
            pages.Add(GenericPage.Create(new PathQuestionInfo("What Solution?")
            {
                Filter = "Solution Files (*.sln)|*.sln",
                Description = "This wizard will walk through the process of adding a plugin/workflow project to a solution that already has the DLaB/XrmUnitTest accelerators installed."
            }));
        }

        private static void AddCreateProjectQuestion(List<IWizardPage> pages, string projectType)
        {
            pages.Add(GenericPage.Create(new ConditionalYesNoQuestionInfo($"Do you want to create a {projectType} project?")
            {
                Yes = new TextQuestionInfo($"What should the {projectType} project be called?")
                {
                    DefaultResponse = $"Something.{projectType}",
                    Description = $"The name and default namespace for the {projectType} project."
                },
                Description = $"This will add a new {projectType} project to the solution and wire up the appropriate references."
            }));
        }

        private static void AddCreateXrmUnitTestProjectQuestion(List<IWizardPage> pages, int forPage)
        {
            var page = GenericPage.Create(new ConditionalYesNoQuestionInfo("Do you want to create a XrmUnitTest test project for the new assembly?"));
            page.AddSavedValuedRequiredCondition(forPage, "Y");
            pages.Add(page);
        }

        private AddProjectToSolutionInfo(Queue<object> queue)
        {
            SolutionPath = (string)queue.Dequeue(); // 0
            InitializePluginProject(queue.Dequeue());
            CreatePluginTest = (string)queue.Dequeue() == "Y";
            InitializeWorkflowProject(queue.Dequeue());
            CreateWorkflowTest = (string)queue.Dequeue() == "Y";
            FindProjectsInSolution();
        }

        public static AddProjectToSolutionInfo Create(object[] values)
        {
            var info = new AddProjectToSolutionInfo(new Queue<object>(values));
            info.XrmVersion = info.GetXrmVersion();
            return info;
        }

        private void InitializePluginProject(object yesNoList)
        {
            var list = (List<string>)yesNoList;
            CreatePlugin = list[0] == "Y";
            PluginName = CreatePlugin ? list[1] : string.Empty;
        }

        private void InitializeWorkflowProject(object yesNoList)
        {
            var list = (List<string>)yesNoList;
            CreateWorkflow = list[0] == "Y";
            WorkflowName = CreatePlugin ? list[1] : string.Empty;
        }

        private void FindProjectsInSolution()
        {
            var sharedProjects = new Dictionary<string, List<string>>();
            var projects = new List<ProjectFileParser>();
            ParseSolutionFileProjects(sharedProjects, projects);
            FindSharedProjects(sharedProjects);
            FindCommonTestProject(projects);
        }

        private void ParseSolutionFileProjects(Dictionary<string, List<string>> sharedProjects, List<ProjectFileParser> projects)
        {
            var solution = new SolutionFileParser(File.ReadAllLines(SolutionPath));
            foreach (var projectLine in solution.Projects.Select(l => l.Replace(" ", string.Empty)))
            {
                var projectName = projectLine.SubstringByString("=\"", "\",\"");
                var projectRelativePath = projectLine.SubstringByString(",\"", "\",\"");
                var path = Path.Combine(Path.GetDirectoryName(SolutionPath) ?? "", projectRelativePath);
                if (projectLine.Contains(ProjectInfo.GetTypeId(ProjectInfo.ProjectType.SharedProj)))
                {
                    var files = File.ReadAllLines(path.Substring(0, path.Length - "shproj".Length) + "projitems")
                        .Where(l => l.Contains("<Compile Include=\"$(MSBuildThisFileDirectory)"))
                        .Select(l => l.SubstringByString("<Compile Include=\"$(MSBuildThisFileDirectory)", "\""))
                        .ToList();
                    sharedProjects.Add(projectName, files);
                }
                else if (projectLine.Contains(ProjectInfo.GetTypeId(ProjectInfo.ProjectType.CsProj)))
                {
                    projects.Add(new ProjectFileParser(File.ReadAllLines(path)));
                }
            }
        }

        private void FindSharedProjects(Dictionary<string, List<string>> sharedProjects)
        {
            if (CreatePlugin || CreateWorkflow)
            {
                SharedCommonProject = FindSharedProjectWithFile(sharedProjects, "Shared Common Base Project", "PluginBase.cs");

                if (CreateWorkflow)
                {
                    SharedCommonWorkflowProject = FindSharedProjectWithFile(sharedProjects, "Shared Common Workflow Base Project", "CodeActivityBase.cs");
                }
            }

            if (CreateWorkflowTest || CreatePluginTest)
            {
                SharedTestCoreProject = FindSharedProjectWithFile(sharedProjects, "Shared Test Core Project", "TestMethodClassBase.cs");
            }
        }

        private void FindCommonTestProject(List<ProjectFileParser> projects)
        {
            Logger.AddDetail("Searching For the Shared Test Project, which is a project containing the 'UnitTestSettings.config' file.");
            var project = projects.FirstOrDefault(p =>
                p.ItemGroups.TryGetValue(ProjectFileParser.ItemGroupTypes.None, out var noneItemGroup)
                && noneItemGroup.Any(l => l.Contains("<None Include=\"UnitTestSettings.config\">")));
            SharedTestProject = project ?? throw new Exception("Unable to find the Shared Test Project!");
        }

        private static string FindSharedProjectWithFile(Dictionary<string, List<string>> sharedProjects, string actionMessage, string fileToFind)
        {
            Logger.AddDetail($"Searching For the {actionMessage}, which is a shared project containing the '{fileToFind}' file.");
            var shared = sharedProjects.FirstOrDefault(p => p.Value.Any(f => f.EndsWith("\\" + fileToFind)));
            if (shared.Key == null)
            {
                throw new Exception($"Unable to find the {actionMessage}!");
            }

            return shared.Key;
        }

        private Version GetXrmVersion()
        {
            var packagesPath = Path.Combine(Path.GetDirectoryName(SolutionPath) ?? "", "packages");
            var directories = Directory.GetDirectories(packagesPath, "");
            var xrmDirectory = directories.OrderByDescending(d => d).FirstOrDefault();
            if (xrmDirectory == null)
            {
                throw new Exception("Unable to find the Microsoft.CrmSdk.CoreAssemblies Package and therefore to determine the Xrm Version to reference.");
            }

            var version = xrmDirectory.SubstringByString("Microsoft.CrmSdk.CoreAssemblies.");
            var versionParts = version.Split('.');
            if (versionParts.Length < 4)
            {
                throw new Exception($"Expected Microsoft.CrmSdk.CoreAssemblies folder in {packagesPath} to contain the Xrm Sdk Version number but found {version}.");
            }
            return new Version(version);
        }
    }
}
