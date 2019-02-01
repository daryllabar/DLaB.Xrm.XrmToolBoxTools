using System;
using System.Collections.Generic;
using System.Linq;
using DLaB.VSSolutionAccelerator.Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DLaB.VSSolutionAccelerator.Tests
{
    [TestClass]
    public class SolutionFileEditorTests
    {
        [TestMethod]
        public void ForEmptySolution_Should_AddProjectHeaders()
        {
            var projects = new List<ProjectInfo>
            {
                new ProjectInfo
                {
                    Id = Guid.NewGuid(),
                    Name = "P1"
                }
            };
            var editor = new SolutionFileEditor(projects);
            editor.StateProcessor[SolutionFileEditor.States.Start]("MinimumVisualStudioVersion = 10.0.40219.1");
            Assert.AreEqual(SolutionFileEditor.States.Project, editor.State);
            Assert.AreEqual(1, editor.Result.Count);
            editor.StateProcessor[editor.State]("Global");
            Assert.AreEqual(3, editor.Result.Count);
            Assert.IsTrue(editor.Result[1].StartsWith("Project"));
            Assert.AreEqual("Global", editor.Result.Last());
        }

        [TestMethod]
        public void ForNonEmptySolution_Should_AddProjectHeadersInOrder()
        {
            var projects = new List<ProjectInfo>
            {
                new ProjectInfo
                {
                    Id = Guid.NewGuid(),
                    Name = "C"
                },
                new ProjectInfo
                {
                    Id = Guid.NewGuid(),
                    Type = ProjectInfo.ProjectType.SharedProj,
                    Name = "A"
                }
            };
            var editor = new SolutionFileEditor(projects);
            editor.StateProcessor[SolutionFileEditor.States.Project]("Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"B\", \"B\\B.csproj\", \"{3E728B41-02E5-42FD-B8D3-CF2C664E2226}\"");
            editor.StateProcessor[SolutionFileEditor.States.Project]("EndProject");
            editor.StateProcessor[SolutionFileEditor.States.Project]("Global"); 
            Assert.AreEqual(
$@"Project(""{{D954291E-2A0B-460D-934E-DC6B0785DB48}}"") = ""A"", ""A\A.shproj"", ""{{{projects[1].Id.ToString().ToUpper()}}}""
EndProject
Project(""{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}"") = ""B"", ""B\B.csproj"", ""{{3E728B41-02E5-42FD-B8D3-CF2C664E2226}}""
EndProject
Project(""{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}"") = ""C"", ""C\C.csproj"", ""{{{projects[0].Id.ToString().ToUpper()}}}""
EndProject
Global", string.Join(Environment.NewLine, editor.Result));
            Assert.AreEqual(SolutionFileEditor.States.Global, editor.State);
        }

        [TestMethod]
        public void ForEmptySolution_Should_AddProjectFilesToGlobal()
        {
            var projects = GetProjectsWithSharedReference();
            var editor = new SolutionFileEditor(projects);
            editor.StateProcessor[SolutionFileEditor.States.Global]("\tGlobalSection(SolutionProperties) = preSolution");
            Assert.AreEqual(
                $@"	GlobalSection(SharedMSBuildProjectFiles) = preSolution
		P1\P1.projitems*{{21e587df-00a7-4015-8992-6af82c55c970}}*SharedItemsImports = 13
		P1\P1.projitems*{{80f91288-cc6c-49d7-a4d0-7e06fedf3888}}*SharedItemsImports = 4
	EndGlobalSection
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
		{{7956eba3-330f-4ccb-aaaa-221dfc833b46}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{{7956eba3-330f-4ccb-aaaa-221dfc833b46}}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{{7956eba3-330f-4ccb-aaaa-221dfc833b46}}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{{7956eba3-330f-4ccb-aaaa-221dfc833b46}}.Release|Any CPU.Build.0 = Release|Any CPU
		{{80f91288-cc6c-49d7-a4d0-7e06fedf3888}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{{80f91288-cc6c-49d7-a4d0-7e06fedf3888}}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{{80f91288-cc6c-49d7-a4d0-7e06fedf3888}}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{{80f91288-cc6c-49d7-a4d0-7e06fedf3888}}.Release|Any CPU.Build.0 = Release|Any CPU
	GlobalSection(SolutionProperties) = preSolution", string.Join(Environment.NewLine, editor.Result));
            Assert.AreEqual(SolutionFileEditor.States.Final, editor.State);
        }

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
            var solution = @"

Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio 15
VisualStudioVersion = 15.0.28307.329
MinimumVisualStudioVersion = 10.0.40219.1
Global
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
	GlobalSection(ExtensibilityGlobals) = postSolution
		SolutionGuid = {A603CAAE-8991-405D-906A-4D5ABE1E9314}
	EndGlobalSection
EndGlobal
";
            var result = SolutionFileEditor.AddMissingProjects(solution.Split(new[] {Environment.NewLine}, StringSplitOptions.None), GetProjectsWithSharedReference());
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
            AssertLinesAreEqual(expected, result);
        }

        private void AssertLinesAreEqual(string expected, IEnumerable<string> actual)
        {
            var splitExpected = expected.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            var splitActual = string.Join(Environment.NewLine, actual).Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < splitExpected.Length; i++)
            {
                Assert.IsTrue(i<splitActual.Length, "Actual is missing lines!");
                Assert.AreEqual(splitExpected[i], splitActual[i]);
            }
        }
    }
}
