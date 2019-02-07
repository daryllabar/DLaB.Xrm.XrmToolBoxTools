using System;
using System.Collections.Generic;
using System.IO;
using DLaB.VSSolutionAccelerator.Wizard;

namespace DLaB.VSSolutionAccelerator
{
    public class InitializeSolutionInfo
    {
        public string SolutionPath { get; set; }
        public string RootNamespace { get; set; }
        public NuGetPackage XrmPackage { get; set; }
        public bool ConfigureEarlyBound { get; set; }
        public string EarlyBoundSettingsPath { get; set; }
        public string SharedCommonProject { get; set; }
        public string SharedCommonWorkflowProject { get; set; }
        public bool ConfigureXrmUnitTest { get; set; }
        public string TestBaseProject { get; set; }
        public string SharedTestCoreProject { get; set; }
        public bool CreatePlugin { get; set; }
        public bool IncludeExamplePlugins { get; set; }
        public string PluginName { get; set; }
        public string PluginTestName { get; set; }
        public bool CreateWorkflow { get; set; }
        public bool IncludeExampleWorkflow { get; set; }
        public string WorkflowName { get; set; }
        public string WorkflowTestName { get; set; }

        private struct Page
        {
            public const int RootNamespace = 1;
            public const int UseXrmUnitTest = 6;
            public const int CreatePlugin = 7;
            public const int CreateWorkflow = 9;
        }

        public static List<IWizardPage> InitializePages(string settingsPath)
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var pages = new List<IWizardPage>();
            AddSolutionNameQuestion(pages); // 0
            AddRootNamespaceQuestion(pages);
            AddXrmNuGetVersionQuestion(pages);
            AddUseEarlyBoundQuestion(settingsPath, pages);
            AddSharedCommonNameQuestion(pages);
            AddSharedCommonWorkflowNameQuestion(pages); // 5
            AddUseXrmUnitTestQuestion(pages);
            AddCreatePluginProjectQuestion(pages);
            AddPluginTestProjectNameQuestion(pages);
            AddCreateWorkflowProjectQuestion(pages);
            AddWorkflowTestProjectNameQuestion(pages); // 10
            return pages;
        }

        private static void AddSolutionNameQuestion(List<IWizardPage> pages)
        {
            pages.Add(GenericPage.Create(new PathQuestionInfo("What Solution?")
            {
                Filter = "Solution Files (*.sln)|*.sln",
                Description = "This Wizard will walk through the process of adding isolation/plugin/workflow/testing projects based on the DLaB/XrmUnitTest framework, adding the projects to the solution defined here."
            }));
        }

        private static void AddRootNamespaceQuestion(List<IWizardPage> pages)
        {
            pages.Add(GenericPage.Create(new TextQuestionInfo("What is the root NameSpace?")
            {
                DefaultResponse = "YourCompanyNameOrAbbreviation.Xrm",
                Description = "This is the root namespace that will the Plugin and (if desired) Early Bound Entities will be appended to."
            }));
        }

        private static void AddXrmNuGetVersionQuestion(List<IWizardPage> pages)
        {
            pages.Add(NuGetVersionSelectorPage.Create(
                "What version of the SDK?",
                PackageLister.Ids.CoreXrmAssemblies,
                "This will determine the NuGet packages referenced and the version of the .Net Framework to use."));
        }

        private static void AddUseEarlyBoundQuestion(string settingsPath, List<IWizardPage> pages)
        {
            pages.Add(GenericPage.Create(new ConditionalYesNoQuestionInfo("Do you want to use the Early Bound Generator To Create Early Bound Entities?")
            {
                Yes = new PathQuestionInfo("What is the path to the Early Bound Generator Settings.xml file?")
                {
                    Filter = "EBG Setting File (*.xml)|*.xml",
                    DefaultResponse = Path.GetFullPath(Path.Combine(settingsPath, "DLaB.EarlyBoundGenerator.DefaultSettings.xml")),
                    Description = "The selected settings file will be moved to the folder of the solution, and configured to place the output of the files in the appropriate folders."
                                  + Environment.NewLine
                                  + "The Early Bound Generator will also be triggered upon completion to generated the Early Bound classes."
                },
                Description = "Configures the output paths of the Early Bound Generator to generate files in the appropriate shared project within the solution."
                              + Environment.NewLine
                              + "This requires the XrmToolBox Early Bound Generator to be installed."
            }));
        }

        private static void AddSharedCommonNameQuestion(List<IWizardPage> pages)
        {
            pages.Add(GenericPage.Create(new TextQuestionInfo("What do you want the name of the shared common assembly to be?")
            {
                DefaultResponse = GenericPage.GetSaveResultsFormat(Page.RootNamespace),
                Description = "This will be the name of a shared C# project that will be used to store common code including plugin logic and early bound entities if applicable"
            }));
        }

        private static void AddSharedCommonWorkflowNameQuestion(List<IWizardPage> pages)
        {
            pages.Add(GenericPage.Create(new TextQuestionInfo("What do you want the name of the shared common workflow assembly to be?")
            {
                DefaultResponse = GenericPage.GetSaveResultsFormat(Page.RootNamespace) + ".WorkflowCore",
                Description = "This will be the name of a shared C# project that contains references to the workflow code.  It would only be required by assemblies containing a workflow."
            }));
        }

        private static void AddUseXrmUnitTestQuestion(List<IWizardPage> pages)
        {
            pages.Add(GenericPage.Create(new ConditionalYesNoQuestionInfo("Do you want to use XrmUnitTest for unit testing?")
            {
                Yes = new TextQuestionInfo("What do you want the Test Settings project to be called?")
                {
                    DefaultResponse = GenericPage.GetSaveResultsFormat(Page.RootNamespace) + ".Test",
                    Description = "The Test Settings project will contain the single test settings config file and assumption xml files."
                },
                Yes2 = new TextQuestionInfo("What do you want the shared core test project to be called?")
                {
                    DefaultResponse = GenericPage.GetSaveResultsFormat(Page.RootNamespace) + ".TestCore",
                    Description = "The shared Test Project will contain all other shared test code (Assumption Definitions, Builders, Test Base Class, etc)"
                },
                Description = "This will add the appropriate NuGet References and create the appropriate isolation projects."
            }));
        }

        private static void AddCreatePluginProjectQuestion(List<IWizardPage> pages)
        {
            pages.Add(GenericPage.Create(new ConditionalYesNoQuestionInfo("Do you want to create a Plugin Project?")
            {
                Yes = new TextQuestionInfo("What should the plugin project be called?")
                {
                    DefaultResponse = GenericPage.GetSaveResultsFormat(Page.RootNamespace) + ".Plugin",
                    Description = "The name and default namespace for the plugin project."
                },
                Yes2 = new ComboQuestionInfo("Include example plugin classes?")
                {
                    DefaultResponse = 0,
                    Description = "If example plugin classes are included, it may contain compiler errors if the Early Bound Entities used in the files are not generated.",
                    Options = new List<KeyValuePair<int, string>>
                    {
                        new KeyValuePair<int, string>(0, "Yes"),
                        new KeyValuePair<int, string>(1, "No"),
                    }
                },
                Description = "This will add a new plugin project to the solution and wire up the appropriate references."
            }));
        }

        private static void AddPluginTestProjectNameQuestion(List<IWizardPage> pages)
        {
            var page = GenericPage.Create(new TextQuestionInfo("What do you want the name of the plugin test project to be?")
            {
                DefaultResponse = GenericPage.GetSaveResultsFormat(Page.CreatePlugin, 1) + ".Tests",
                Description = "This will be the name of the Visual Studio Unit Test Project for the plugin assembly."
            });
            page.AddSavedValuedRequiredCondition(Page.CreatePlugin, "Y");
            page.AddSavedValuedRequiredCondition(Page.UseXrmUnitTest, "Y");
            pages.Add(page);
        }

        private static void AddCreateWorkflowProjectQuestion(List<IWizardPage> pages)
        {
            pages.Add(GenericPage.Create(new ConditionalYesNoQuestionInfo("Do you want to create a Workflow Project?")
            {
                Yes = new TextQuestionInfo("What should the workflow project be called??")
                {
                    DefaultResponse = GenericPage.GetSaveResultsFormat(Page.RootNamespace) + ".Workflow",
                    Description = "The name and default namespace for the workflow project."
                },
                Yes2 = new ComboQuestionInfo("Include example custom workflow activity?")
                {
                    DefaultResponse = 0,
                    Options = new List<KeyValuePair<int, string>>
                    {
                        new KeyValuePair<int, string>(0, "Yes"),
                        new KeyValuePair<int, string>(1, "No"),
                    }
                },
                Description = "This will add a new workflow project to the solution and wire up the appropriate references."
            }));
        }

        private static void AddWorkflowTestProjectNameQuestion(List<IWizardPage> pages)
        {
            var page = GenericPage.Create(new TextQuestionInfo("What do you want the name of the workflow test project to be?")
            {
                DefaultResponse = GenericPage.GetSaveResultsFormat(Page.CreateWorkflow, 1) + ".Tests",
                Description = "This will be the name of the Visual Studio Unit Test Project for the workflow assembly."
            });
            page.AddSavedValuedRequiredCondition(Page.CreateWorkflow, "Y");
            page.AddSavedValuedRequiredCondition(Page.UseXrmUnitTest, "Y");
            pages.Add(page);
        }

        public static InitializeSolutionInfo InitializeSolution(object[] values)
        {
            var queue = new Queue<object>(values);
            // ReSharper disable once UseObjectOrCollectionInitializer
            var info = new InitializeSolutionInfo();
            info.SolutionPath = (string)queue.Dequeue(); // 0
            info.RootNamespace = (string)queue.Dequeue();
            info.XrmPackage = (NuGetPackage)queue.Dequeue();
            info.InitializeEarlyBound(queue.Dequeue());
            info.SharedCommonProject = (string)queue.Dequeue();
            info.SharedCommonWorkflowProject = (string)queue.Dequeue(); // 5
            info.InitializeXrmUnitTest(queue.Dequeue());
            info.InitializePlugin(queue.Dequeue());
            info.PluginTestName = (string)queue.Dequeue();
            info.InitializeWorkflow(queue.Dequeue());
            info.WorkflowTestName = (string)queue.Dequeue(); // 10
            return info;
        }

        private void InitializeEarlyBound(object yesNoList)
        {
            var list = (List<string>) yesNoList;
            ConfigureEarlyBound = list[0] == "Y";
            if (ConfigureEarlyBound)
            {
                EarlyBoundSettingsPath = list[1];
            }
        }

        private void InitializeXrmUnitTest(object yesNoList)
        {
            var list = (List<string>)yesNoList;
            ConfigureXrmUnitTest = list[0] == "Y";
            if (ConfigureXrmUnitTest)
            {
                TestBaseProject = list[1];
                SharedTestCoreProject = list[2];
            }
        }

        private void InitializePlugin(object yesNoList)
        {
            var list = (List<string>)yesNoList;
            CreatePlugin = list[0] == "Y";
            if (CreatePlugin)
            {
                PluginName = list[1];
                IncludeExamplePlugins = list[2] == "0";
            }
        }

        private void InitializeWorkflow(object yesNoList)
        {
            var list = (List<string>)yesNoList;
            CreateWorkflow = list[0] == "Y";
            if (CreateWorkflow)
            {
                WorkflowName = list[1];
                IncludeExampleWorkflow = list[2] == "0";
            }
        }
    }
}
