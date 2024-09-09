using System.Collections.Generic;

namespace DLaB.VSSolutionAccelerator.Wizard
{
    public class AddAllWizardResults
    {
        public bool P0AddToExistingSolution { get; set; }
        public string P0SolutionPath { get; set; }
        public string P1Namespace { get; set; }
        public bool P2EarlyBound { get; set; }
        public string P3SharedCommonAssemblyName { get; set; }
        public string P4SharedWorkflowProjectName { get; set; }
        public bool P5UseXrmUnitTest { get; set; }
        public string P5TestSettingsProjectName { get; set; }
        public bool P6CreatePluginProject { get; set; }
        public string P6PluginProjectName { get; set; }
        public bool P6IncludeExamples { get; set; }
        public string P7CompanyName { get; set; }
        public string P7PluginDescription { get; set; }
        public string P7PacAuthName { get; set; }
        public string P8PluginTestProjectName { get; set; }
        public bool P9CreateWorkflowProject { get; set; }
        public string P9WorkflowProjectName { get; set; }
        public bool P9IncludeExamples { get; set; }
        public string P10WorkflowTestProjectName { get; set; }
        public bool P11InstallCodeSnippets { get; set; }
        public bool P11IncludeCodeGen { get; set; }

        public object[] GetResults()
        {
            return new object[]
            {
                    new List<string>{ToYn(P0AddToExistingSolution), P0SolutionPath },
                    P1Namespace,
                    //new NuGetPackage
                    //{
                    //    Id = "Microsoft.CrmSdk.CoreAssemblies",
                    //    LicenseUrl = "http://download.microsoft.com/download/E/1/8/E18C0FAD-FEC8-44CD-9A16-98EDC4DAC7A2/LicenseTerms.docx",
                    //    Name = "Microsoft Dynamics 365 SDK core assemblies",
                    //    Version = new Version("9.2.0.56"),
                    //    VersionText = "9.2.0.56",
                    //    XrmToolingClient = false
                    //},
                    ToYn(P2EarlyBound),
                    P3SharedCommonAssemblyName,
                    P4SharedWorkflowProjectName,
                    new List<string>{ToYn(P5UseXrmUnitTest), P5TestSettingsProjectName },
                    new List<string>{ToYn(P6CreatePluginProject), P6PluginProjectName, To01(P6IncludeExamples) },
                    new List<string>{P7CompanyName, P7PluginDescription, P7PacAuthName },
                    P8PluginTestProjectName,
                    new List<string>{ToYn(P9CreateWorkflowProject), P9WorkflowProjectName, To01(P9IncludeExamples) },
                    P10WorkflowTestProjectName,
                    new List<string>{To01(P11InstallCodeSnippets), To01(P11IncludeCodeGen) },
            };

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
