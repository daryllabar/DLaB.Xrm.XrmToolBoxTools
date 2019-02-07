using System;
using System.Collections.Generic;
using DLaB.VSSolutionAccelerator.Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DLaB.VSSolutionAccelerator.Tests
{
    [TestClass]
    public class SolutionFileEditorTests
    {
        private static List<ProjectInfo> GetProjectsWithSharedReference()
        {
            var projects = new List<ProjectInfo>
            {
                new ProjectInfo
                {
                    Id = new Guid("21e587df-00a7-4015-8992-6af82c55c970"),
                    Name = "P1",
                    Type = ProjectInfo.ProjectType.SharedProj
                },
                new ProjectInfo
                {
                    Id = new Guid("7956eba3-330f-4ccb-aaaa-221dfc833b46"),
                    Name = "P2",
                    Type = ProjectInfo.ProjectType.CsProj
                },
                new ProjectInfo
                {
                    Id = new Guid("80f91288-cc6c-49d7-a4d0-7e06fedf3888"),
                    Name = "P3",
                    Type = ProjectInfo.ProjectType.CsProj
                },
            };
            projects[2].SharedProjectsReferences.Add(projects[0]);
            return projects;
        }

        [TestMethod]
        public void AddMissingProjects_ForEmptySolution_Should_AddMissingSections()
        {
            var result = SolutionFileEditor.AddMissingProjects(null, GetProjectsWithSharedReference());
            var expected = @"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio 15
VisualStudioVersion = 15.0.28307.329
MinimumVisualStudioVersion = 10.0.40219.1
Project(""{D954291E-2A0B-460D-934E-DC6B0785DB48}"") = ""P1"", ""P1\P1.shproj"", ""{21E587DF-00A7-4015-8992-6AF82C55C970}""
EndProject
Project(""{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"") = ""P2"", ""P2\P2.csproj"", ""{7956EBA3-330F-4CCB-AAAA-221DFC833B46}""
EndProject
Project(""{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"") = ""P3"", ""P3\P3.csproj"", ""{80F91288-CC6C-49D7-A4D0-7E06FEDF3888}""
EndProject
Global
	GlobalSection(SharedMSBuildProjectFiles) = preSolution
		P1\P1.projitems*{21e587df-00a7-4015-8992-6af82c55c970}*SharedItemsImports = 13
		P1\P1.projitems*{80f91288-cc6c-49d7-a4d0-7e06fedf3888}*SharedItemsImports = 4
	EndGlobalSection
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{7956eba3-330f-4ccb-aaaa-221dfc833b46}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{7956eba3-330f-4ccb-aaaa-221dfc833b46}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{7956eba3-330f-4ccb-aaaa-221dfc833b46}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{7956eba3-330f-4ccb-aaaa-221dfc833b46}.Release|Any CPU.Build.0 = Release|Any CPU
		{80f91288-cc6c-49d7-a4d0-7e06fedf3888}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{80f91288-cc6c-49d7-a4d0-7e06fedf3888}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{80f91288-cc6c-49d7-a4d0-7e06fedf3888}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{80f91288-cc6c-49d7-a4d0-7e06fedf3888}.Release|Any CPU.Build.0 = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
	GlobalSection(ExtensibilityGlobals) = postSolution
		SolutionGuid = {A603CAAE-8991-405D-906A-4D5ABE1E9314}
	EndGlobalSection
EndGlobal";
            Assert.That.LinesAreEqual(expected, result);
        }   
    }
}
