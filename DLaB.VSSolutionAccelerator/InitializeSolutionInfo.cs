using System.Collections.Generic;

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
        public string PluginName { get; set; }
        public bool CreateWorkflow { get; set; }
        public string WorkflowName { get; set; }

        public static InitializeSolutionInfo InitializeSolution(object[] values)
        {
            var queue = new Queue<object>(values);
            // ReSharper disable once UseObjectOrCollectionInitializer
            var info = new InitializeSolutionInfo();
            info.SolutionPath = (string)queue.Dequeue();
            info.RootNamespace = (string)queue.Dequeue();
            info.XrmPackage = (NuGetPackage)queue.Dequeue();
            info.InitializeEarlyBound(queue.Dequeue());
            info.SharedCommonProject = (string)queue.Dequeue();
            info.SharedCommonWorkflowProject = (string)queue.Dequeue();
            info.InitializeXrmUnitTest(queue.Dequeue());
            info.InitializePlugin(queue.Dequeue());
            info.InitializeWorkflow(queue.Dequeue());
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
                SharedTestCoreProject = list[1];
            }
        }

        private void InitializePlugin(object yesNoList)
        {
            var list = (List<string>)yesNoList;
            CreatePlugin = list[0] == "Y";
            if (CreatePlugin)
            {
                PluginName = list[1];
            }
        }

        private void InitializeWorkflow(object yesNoList)
        {
            var list = (List<string>)yesNoList;
            CreateWorkflow = list[0] == "Y";
            if (CreateWorkflow)
            {
                WorkflowName = list[1];
            }
        }
    }
}
