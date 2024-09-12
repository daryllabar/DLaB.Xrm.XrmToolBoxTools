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
    /// <summary>
    /// Defines the Pages to add to the wizard via InitializePages, and then maps the results to its own properties via the Create method.
    /// </summary>
    public class AddProjectToSolutionInfo : SolutionEditorInfo
    {
        private SolutionSettings SolutionSettings { get; set; }
        public ProjectFileParser SharedTestProject { get; private set; }

        // ReSharper disable UnusedMember.Local
        private static class Page
        {
            public static class SolutionName
            {
                public const int Index = 0;
                public static class Fields
                {
                    public const int SolutionPath = 0;
                    public const int HasCommonWorkflowProject = 1;
                    public const int CommonWorkflowProjectName = 2;
                    public const int CompanyName = 3;
                    public const int PacAuth = 4;
                }
            }
            public static class CreatePlugin
            {
                public const int Index = 1;
                public static readonly ProjectIndex Fields = new ProjectIndex();
            }
            public static class PluginAssembly
            {
                public const int Index = 2;
                public static class Fields {
                    public const int Company = 0;
                    public const int Description = 1;
                    public const int Solution = 2;
                    public const int PacAuth = 3;
                }
            }
            public static class CreatePluginTest
            {
                public const int Index = 3;
                public static readonly ProjectIndex Fields = new ProjectIndex();
            }
            public static class CreateWorkflow
            {
                public const int Index = 4;
                public static readonly ProjectIndex Fields = new ProjectIndex();
            }
            public static class CommonWorkflowName
            {
                public const int Index = 5;
                public static class Fields
                {
                    public const int Name = 0;
                }
            }
            public static class CreateWorkflowTest
            {
                public const int Index = 6;
                public static readonly ProjectIndex Fields = new ProjectIndex();
            }
        }

        private class ProjectIndex
        {
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public int Create { get; } = 0;
            public int Name { get; } = 1;
        }
        // ReSharper restore UnusedMember.Local

        public static List<IWizardPage> InitializePages(List<KeyValuePair<int, string>> solutionNames)
        {
            var pages = new List<IWizardPage>();
            for(var i = 0; i <= Page.CreateWorkflowTest.Index; i++)
            {
                switch (i)
                {
                    case Page.SolutionName.Index:
                        AddSolutionNameQuestion(pages); // 0
                        break;
                    case Page.CreatePlugin.Index:
                        AddCreateProjectQuestion(pages, "Plugin");
                        break;
                    case Page.PluginAssembly.Index:
                        AddPluginAssemblyInfoQuestions(pages, solutionNames);
                        break;
                    case Page.CreatePluginTest.Index:
                        AddCreateXrmUnitTestProjectQuestion(pages, Page.CreatePlugin.Index);
                        break;
                    case Page.CreateWorkflow.Index:
                        AddCreateProjectQuestion(pages, "Workflow");
                        break;
                    case Page.CommonWorkflowName.Index:
                        AddSharedCommonWorkflowNameQuestion(pages);
                        break;
                    case Page.CreateWorkflowTest.Index:
                        AddCreateXrmUnitTestProjectQuestion(pages, Page.CreateWorkflow.Index);
                        break;   
                }
            }

            return pages;
        }

        private static void AddSolutionNameQuestion(List<IWizardPage> pages)
        {
            var page = GenericPage.Create(new PathQuestionInfo("What Solution?")
            {
                Filter = "Solution Files (*.sln)|*.sln",
                Description = "This wizard will walk through the process of adding a plugin/workflow project to a solution that already has the DLaB/XrmUnitTest accelerators installed."
            });
            pages.Add(page);

            page.PostSave = list =>
            {
                var solutionPath = list[Page.SolutionName.Fields.SolutionPath];
                if (!File.Exists(solutionPath))
                {
                    Logger.AddDetail("Solution File does not exist!");
                    list.Add("N");
                    list.Add(string.Empty);
                    list.Add("Your Company Name");
                    list.Add("Dataverse Dev");
                    return;
                }
                
                // Determine if Solution already has shared workflow project
                var lines = File.ReadAllLines(solutionPath);
                var parser = new SolutionFileParser(lines);
                var sharedWorkflowProjectDirectory = parser.GlobalSharedProjects.Select(p => p.Split(new[] { ".projitems" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()?.Trim())
                    .Where(p => !string.IsNullOrWhiteSpace(p))
                    .Distinct()
                    .Select(sharedProject => Path.Combine(Path.GetDirectoryName(solutionPath) ?? string.Empty, Path.GetDirectoryName(sharedProject) ?? string.Empty))
                    .FirstOrDefault(sharedProjectDirectory => Directory.Exists(sharedProjectDirectory) && Directory.GetFiles(sharedProjectDirectory).Any(f => Path.GetFileName(f) == "CodeActivityBase.cs"));

                list.Add(sharedWorkflowProjectDirectory == null ? "N" : "Y");
                list.Add(sharedWorkflowProjectDirectory);

                // Add default values for Plugin Package
                var project = parser.Projects.Select(projectLine =>
                {
                    if (!projectLine.Contains(ProjectInfo.GetTypeId(ProjectInfo.ProjectType.CsProj)))
                    {
                        return string.Empty;
                    }
                    var projectPath = projectLine.Split('"').FirstOrDefault(p => p.EndsWith(".csproj"));
                    if (string.IsNullOrWhiteSpace(projectPath))
                    {
                        return string.Empty;
                    }
                    projectPath = Path.Combine(Path.GetDirectoryName(solutionPath) ?? string.Empty, projectPath);
                    return File.ReadAllText(projectPath);
                }).FirstOrDefault(p => p.Contains("<DeploymentPacAuthName>"));

                if (string.IsNullOrWhiteSpace(project))
                {
                    // No Plugin Packages Exists
                    var companyName = Path.GetFileNameWithoutExtension(solutionPath).Replace(".", " ");
                    list.Add(companyName);
                    list.Add(companyName + " Dev");
                    return;
                }
                list.Add(project.SubstringByString("<Company>", "</Company>"));
                list.Add(project.SubstringByString("<DeploymentPacAuthName>", "</DeploymentPacAuthName>"));
            };
        }

        private static void AddCreateProjectQuestion(List<IWizardPage> pages, string projectType)
        {
            pages.Add(GenericPage.Create(new ConditionalYesNoQuestionInfo($"Do you want to create a {projectType} project?")
            {
                Yes = new TextQuestionInfo($"What should the {projectType} project be called?")
                {
                    DefaultResponse = $"YourPrefix.Dataverse.Your{projectType}",
                    Description = $"The name and default namespace for the {projectType} project."
                },
                Description = $"This will add a new {projectType} project to the solution and wire up the appropriate references."
            }));
        }

        private static void AddPluginAssemblyInfoQuestions(List<IWizardPage> pages, List<KeyValuePair<int, string>> solutionNames)
        {
            var page = GenericPage.Create(new TextQuestionInfo("What is the name of the Company to use with the Plugin Assembly?")
            {
                DefaultResponse = GenericPage.GetSaveResultsFormat(Page.SolutionName.Index, Page.SolutionName.Fields.CompanyName),
                Description = "This information will be used in the plugin project file and will be used when generating the plugin assembly.",
            }, new TextQuestionInfo("Plugin Assembly Description?")
            {
                DefaultResponse = "Plugin with Dependent Assemblies",
            }, new ComboQuestionInfo("Solution to Deploy the Plugin to?")
            {
                Description = "The prefix of the solution publisher will be used to name the plugin package as well as upload a temporary plugin to for setting up a dev deployment build.",
                Options = solutionNames
            }, new TextQuestionInfo("Deployment PAC CLI Auth Name?")
            {
                DefaultResponse = GenericPage.GetSaveResultsFormat(Page.SolutionName.Index, Page.SolutionName.Fields.PacAuth),
                Description = "The PAC CLI Auth Name is the name of the used to PAC Auth context to use when deploying the plugin to dev.",
            });

            page.AddSavedValuedRequiredCondition(Page.CreatePlugin.Index, "Y");
            pages.Add(page);
        }

        private static void AddSharedCommonWorkflowNameQuestion(List<IWizardPage> pages)
        {
            var page = GenericPage.Create(new TextQuestionInfo("What do you want the name of the shared common workflow assembly to be?")
            {
                DefaultResponse = GenericPage.GetSaveResultsFormat(Page.CreateWorkflow.Index, Page.CreateWorkflow.Fields.Name) + "Core",
                Description = "This will be the name of a shared C# project that contains references to the workflow code.  It would only be required by assemblies containing a workflow."
            });
            pages.Add(page);

            // Has Shared Workflow Project
            page.AddSavedValuedRequiredCondition(Page.SolutionName.Index, Page.SolutionName.Fields.HasCommonWorkflowProject, "N");
            // Is Creating a Workflow
            page.AddSavedValuedRequiredCondition(Page.CreateWorkflow.Index, "Y");
        }

        private static void AddCreateXrmUnitTestProjectQuestion(List<IWizardPage> pages, int forPage)
        {
            var page = GenericPage.Create(new ConditionalYesNoQuestionInfo("Do you want to create a XrmUnitTest test project for the new assembly?")
            {
                Yes = new TextQuestionInfo("What do you want the name of the test project to be?")
                {
                    DefaultResponse = GenericPage.GetSaveResultsFormat(forPage, new ProjectIndex().Name) + ".Tests",
                    Description = "This will be the name of the Visual Studio Unit Test Project for the assembly."
                }
            });
            page.AddSavedValuedRequiredCondition(forPage, "Y");
            pages.Add(page);
        }

        private AddProjectToSolutionInfo(Queue<object> queue, Dictionary<int, Guid> solutionIdsByIndex)
        {
            for(var i=0; i <= Page.CreateWorkflowTest.Index; i++)
            {
                switch (i)
                {
                    case Page.SolutionName.Index:
                        InitializeSolution((List<string>)queue.Dequeue());
                        break;
                    case Page.CreatePlugin.Index:
                        InitializePluginProject(new YesNoResult(queue.Dequeue()));
                        break;
                    case Page.PluginAssembly.Index:
                        InitializePluginAssembly((List<string>)queue.Dequeue(), solutionIdsByIndex);
                        break;
                    case Page.CreatePluginTest.Index:
                        InitializePluginTest(new YesNoResult(queue.Dequeue()));
                        break;
                    case Page.CreateWorkflow.Index:
                        InitializeWorkflowProject(new YesNoResult(queue.Dequeue()));
                        break;
                    case Page.CommonWorkflowName.Index:
                        InitializeCommonWorkflowProject(queue);
                        break;
                    case Page.CreateWorkflowTest.Index:
                        InitializeWorkflowTest(new YesNoResult(queue.Dequeue()));
                        break;
                }
            }

            FindProjectsInSolution();
        }

        private void InitializeSolution(List<string> results)
        {
            SolutionPath = results[Page.SolutionName.Fields.SolutionPath];
            SolutionSettings = SolutionSettings.Load(SolutionPath);
            SharedCommonWorkflowProject = results[Page.SolutionName.Fields.CommonWorkflowProjectName];
            if (!string.IsNullOrEmpty(SharedCommonWorkflowProject))
            {
                SharedCommonWorkflowProject = Path.GetFileNameWithoutExtension(Directory.GetFiles(SharedCommonWorkflowProject, "*.shproj").FirstOrDefault() ?? string.Empty);
            }
        }

        public static AddProjectToSolutionInfo Create(object[] values, Dictionary<int, Guid> solutionIdsByIndex)
        {
            return new AddProjectToSolutionInfo(new Queue<object>(values), solutionIdsByIndex);
        }

        private void InitializePluginProject(YesNoResult result)
        {
            CreatePlugin = result.IsYes;
            PluginName = result[Page.CreatePlugin.Fields.Name];
        }

        private void InitializePluginTest(YesNoResult result)
        {
            CreatePluginTest = result.IsYes;
            PluginTestName = result[Page.CreatePluginTest.Fields.Name];
        }

        private void InitializeWorkflowProject(YesNoResult result)
        {
            CreateWorkflow = result.IsYes;
            WorkflowName = result[Page.CreateWorkflow.Fields.Name];
        }

        private void InitializeCommonWorkflowProject(Queue<object> queue)
        {
            if (string.IsNullOrWhiteSpace(SharedCommonWorkflowProject))
            {
                SharedCommonWorkflowProject = (string)queue.Dequeue();
                CreateCommonWorkflowProject = !string.IsNullOrEmpty(SharedCommonWorkflowProject);
            }
            else
            {
                CreateCommonWorkflowProject = false;
            }
        }

        private void InitializeWorkflowTest(YesNoResult result)
        {
            CreateWorkflowTest = result.IsYes;
            WorkflowTestName = result[Page.CreateWorkflowTest.Fields.Name];
        }

        private void FindProjectsInSolution()
        {
            var projects = ParseSolutionFileProjects();
            SharedTestProject = FindCommonProject(projects, "Shared Test Project", "UnitTestSettings.config");
        }

        private List<ProjectFileParser> ParseSolutionFileProjects()
        {
            var sharedProjects = new Dictionary<string, List<string>>();
            var projects = new List<ProjectFileParser>();
            var projectSolutionNames = new List<string>();
            var solution = new SolutionFileParser(File.ReadAllLines(SolutionPath));
            foreach (var projectLine in solution.Projects.Select(l => l.Replace(" ", string.Empty)).Where(l => l.StartsWith("Project(\"{")))
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
                    projectSolutionNames.Add(projectName);
                    projects.Add(new ProjectFileParser(path, File.ReadAllLines(path)));
                }
            }
            InstantiateSharedProjects(projects, sharedProjects, projectSolutionNames);
            return projects;
        }

        private void InstantiateSharedProjects(List<ProjectFileParser> projects, Dictionary<string, List<string>> sharedProjects, List<string> projectSolutionNames)
        {
            if (CreatePlugin || CreateWorkflow)
            {

                var project = FindCommonProject(projects, "Shared Common Base Project", SolutionSettings.BasePluginFileName, "Plugin");
                SharedCommonProject = projectSolutionNames[projects.IndexOf(project)];

                if (CreateWorkflow
                    && string.IsNullOrEmpty(SharedCommonWorkflowProject))
                {
                    SharedCommonWorkflowProject = FindSharedProjectWithFile(sharedProjects, "Shared Common Workflow Base Project", SolutionSettings.CodeActivityBaseFileName);
                }
            }

            if (CreateWorkflowTest || CreatePluginTest)
            {
                var project = FindCommonProject(projects, "Shared Test Project", SolutionSettings.TestMethodClassBaseFileName);
                TestBaseProject = projectSolutionNames[projects.IndexOf(project)];
            }
        }

        private ProjectFileParser FindCommonProject(List<ProjectFileParser> projects, string description, string fileName, string subDirectory = null)
        {
            Logger.AddDetail($"Searching For the {description}, which is a project containing the '{fileName}' file.");
            var project = projects.FirstOrDefault(p =>
                File.Exists(Path.Combine(Path.GetDirectoryName(p.Path) ?? Environment.GetLogicalDrives()[0], subDirectory ?? string.Empty, fileName)));
            return project ?? throw new Exception($"Unable to find the {description}!");
        }

        private static string FindSharedProjectWithFile(Dictionary<string, List<string>> sharedProjects, string actionMessage, string fileToFind)
        {
            Logger.AddDetail($"Searching For the {actionMessage}, which is a shared project containing the '{fileToFind}' file.");
            var shared = sharedProjects.FirstOrDefault(p => p.Value.Any(f => f == fileToFind || f.EndsWith("\\" + fileToFind)));
            if (shared.Key == null)
            {
                throw new Exception($"Unable to find the {actionMessage}!");
            }

            return shared.Key;
        }
    }
}
