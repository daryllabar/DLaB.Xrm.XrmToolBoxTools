using System.Collections.Generic;
using DLaB.VSSolutionAccelerator.Logic;

namespace DLaB.VSSolutionAccelerator.Wizard
{
    public class AddAllWizardResults
    {
        public bool P0AddToExistingSolution { get; set; }
        public string P0SolutionPath { get; set; }
        public string P1Namespace { get; set; }
        public bool P2EarlyBound { get; set; }
        public string P3SharedCommonAssemblyName { get; set; }
        public string P9SharedWorkflowProjectName { get; set; }
        public bool P4UseXrmUnitTest { get; set; }
        public string P4TestSettingsProjectName { get; set; }
        public bool P5CreatePluginProject { get; set; }
        public string P5PluginProjectName { get; set; }
        public bool P5IncludeExamples { get; set; }
        public string P6CompanyName { get; set; }
        public string P6PluginDescription { get; set; }
        public int P6PluginSolutionIndex { get; set; }
        public string P6PacAuthName { get; set; }
        public string P7PluginTestProjectName { get; set; }
        public bool P8CreateWorkflowProject { get; set; }
        public string P8WorkflowProjectName { get; set; }
        public bool P8IncludeExamples { get; set; }
        public string P10WorkflowTestProjectName { get; set; }
        public bool P11InstallCodeSnippets { get; set; }
        public bool P11IncludeCodeGen { get; set; }

        public object[] GetResults()
        {
            var results = new List<object>();
            for (var i = 0; i <= InitializeSolutionInfo.Page.CodeSnippets; i++)
            {
                switch (i)
                {
                    case InitializeSolutionInfo.Page.SolutionPath:
                        results.Add(new List<string> { ToYn(P0AddToExistingSolution), P0SolutionPath });
                        break;
                    case InitializeSolutionInfo.Page.RootNamespace:
                        results.Add(P1Namespace);
                        break;
                    case InitializeSolutionInfo.Page.EarlyBound:
                        results.Add(ToYn(P2EarlyBound));
                        break;
                    case InitializeSolutionInfo.Page.CommonName:
                        results.Add(P3SharedCommonAssemblyName);
                        break;
                    case InitializeSolutionInfo.Page.UseXrmUnitTest:
                        results.Add(new List<string>{ToYn(P4UseXrmUnitTest), P4TestSettingsProjectName });
                        break;
                    case InitializeSolutionInfo.Page.CreatePlugin:
                        results.Add(new List<string>{ToYn(P5CreatePluginProject), P5PluginProjectName, To01(P5IncludeExamples) });
                        break;
                    case InitializeSolutionInfo.Page.PluginAssembly:
                        results.Add(new List<string>{P6CompanyName, P6PluginDescription, P6PluginSolutionIndex.ToString(), P6PacAuthName });
                        break;
                    case InitializeSolutionInfo.Page.PluginTest:
                        results.Add(P7PluginTestProjectName);
                        break;
                    case InitializeSolutionInfo.Page.CreateWorkflow:
                        results.Add(new List<string>{ToYn(P8CreateWorkflowProject), P8WorkflowProjectName, To01(P8IncludeExamples) });
                        break;
                    case InitializeSolutionInfo.Page.CommonWorkflowName:
                        results.Add(P9SharedWorkflowProjectName);
                        break;
                    case InitializeSolutionInfo.Page.WorkflowTest:
                        results.Add(P10WorkflowTestProjectName);
                        break;
                    case InitializeSolutionInfo.Page.CodeSnippets:
                        results.Add(new List<string>{To01(P11InstallCodeSnippets), To01(P11IncludeCodeGen) });
                        break;
                }
            }

            return results.ToArray();
            string ToYn(bool value)
            {
                return value ? "Y" : "N";
            }

            string To01(bool value)
            {
                return value ? "0" : "1";
            }
        }
    }
}
