using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLaB.VSSolutionAccelerator
{
    public static class Logic
    {
        public static void Execute(InitializeSolutionInfo info)
        {
            // Create Shared Project with the SharedCommonProject name/path
            // - Replace namespace
            // - Add project to ToBeAddedToSolution
            // Create Shared Workflow Project with the SharedCommonWorkflowProject name/path
            // - Replace namespace
            // - Add project to ToBeAddedToSolution
            // Add the Code Generation Files and projects to the solution, and VS install folder
            // If ConfigureXrmUnitTest
            // - Create SharedTestProject and SharedTestCoreProject
            // - Replace namespace
            // - Add project to ToBeAddedToSolution
            // If CreatePlugin
            // - Create Project
            // - Replace Namespace
            // - Add references to shared Common Project
            // - Add project to ToBeAddedToSolution
            // - If ConfigureXrmUnitTest
            // - - Create Test Project
            // - - Replace namespace
            // - - Add project to ToBeAddedToSolution
            // - - Update Reference to Test plugin
            // If CreateWorkflow
            // - Create Project
            // - Replace Namespace
            // - Add references to shared Common Project and Workflow Project
            // - Add project to ToBeAddedToSolution
            // - If ConfigureXrmUnitTest
            // - - Create Test Project
            // - - Replace namespace
            // - - Add project to ToBeAddedToSolution
            // - - Update Reference to Test plugin
            // Update the solution with the ToBeAddedToSolution Projects
            // If EarlyBound
            // - Move the Settings File to the Code Generation Folder add to clipboard, and Update Paths and Open EBG
            // 
        }
    }
}
