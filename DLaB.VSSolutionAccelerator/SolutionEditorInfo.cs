﻿using System;
using System.Collections.Generic;

namespace DLaB.VSSolutionAccelerator
{
    public abstract class SolutionEditorInfo
    {
        public string SolutionPath { get; set; }
        public bool CreatePlugin { get; set; }
        public bool CreatePluginTest { get; set; }
        public bool CreateWorkflowTest { get; set; }
        public bool IncludeCodeGenerationFiles { get; set; }
        public bool InstallSnippets { get; set; }

        public bool IncludeExamplePlugins { get; set; }
        public bool CreateCommonWorkflowProject { get; set; }
        public bool CreateWorkflow { get; set; }
        public bool IncludeExampleWorkflow { get; set; }
        public string PluginName { get; set; }
        public string WorkflowName { get; set; }
        public PluginPackageInfo PluginPackage { get; set; } = new PluginPackageInfo();
        public string PluginTestName { get; set; }
        public string WorkflowTestName { get; set; }
        public string SharedCommonProject { get; set; }
        public string SharedCommonWorkflowProject { get; set; }
        public string TestBaseProject { get; set; }


        protected void InitializePluginAssembly(List<string> result, Dictionary<int, Guid> solutionIdsByIndex)
        {
            PluginPackage.Company = result[0];
            PluginPackage.Description = result[1];
            PluginPackage.SolutionId = solutionIdsByIndex[int.Parse(result[2])];
            PluginPackage.PacAuthName = result[3];
        }

        protected class YesNoResult
        {
            public bool IsYes => this[0] == "Y";
            private List<string> List { get; }
            public YesNoResult(object value)
            {
                if (value is List<string> list)
                {
                    List = list;
                }
                else
                {
                    List = new List<string> {value.ToString()};
                }
            }

            public string this[int index] => List.Count > index ? List[index] : string.Empty;
        }

        public class PluginPackageInfo
        {
            public string Company { get; set; }
            public string Description { get; set; }
            public string PacAuthName { get; set; }
            public Guid SolutionId { get; set; }
            public string PackageId { get; set; }
        }
    }
}
