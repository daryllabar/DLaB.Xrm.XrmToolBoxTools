using System.Collections.Generic;

namespace DLaB.VSSolutionAccelerator.Wizard
{
    public class AddPluginWorkflowWizardResults
    { 
        public string P0SolutionPath { get; set; }
        public bool P1CreatePluginProject { get; set; }
        public string P1PluginProjectName { get; set; }
        public bool P2CreatePluginXrmUnitTest { get; set; }
        public string P2PluginTestProjectName { get; set; }
        public bool P3CreateWorkflowProject { get; set; }
        public string P3WorkflowProjectName { get; set; }
        public bool P4CreateWorkflowXrmUnitTest { get; set; }
        public string P4WorkflowTestProjectName { get; set; }

        public object[] GetResults()
        {
            return new object[]
            {
                P0SolutionPath,
                new List<string>{ToYn(P1CreatePluginProject), P1PluginProjectName },
                new List<string>{ToYn(P2CreatePluginXrmUnitTest), P2PluginTestProjectName },
                new List<string>{ToYn(P3CreateWorkflowProject), P3WorkflowProjectName},
                new List<string>{ToYn(P4CreateWorkflowXrmUnitTest), P4WorkflowTestProjectName },
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
