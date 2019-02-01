using System;
using DLaB.VSSolutionAccelerator.Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DLaB.VSSolutionAccelerator.Tests
{
    /// <summary>
    /// Summary description for ProjectInfoTest
    /// </summary>
    [TestClass]
    public class ProjectInfoTest
    {
        [TestMethod]
        public void SolutionHeader_ForCsProj_Should_CreateCorrectGuid()
        {
            /*
                 Should Generate the following output
                Project("{D954291E-2A0B-460D-934E-DC6B0785DB48}") = "Xyz.Xrm", "Xyz.Xrm\Xyz.Xrm.shproj", "{B22B3BC6-0AC6-4CDD-A118-16E318818AD7}"
                EndProject
             */
            var sut = new ProjectInfo()
            {
                Type = ProjectInfo.ProjectType.CsProj,
                Name = "CsProjTest",
                Id = Guid.NewGuid()
            };
            Assert.AreEqual("Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"CsProjTest\", \"CsProjTest\\CsProjTest.csproj\", \"{" + sut.Id.ToString().ToUpper() + "}\"" + Environment.NewLine + "EndProject", sut.SolutionProjectHeader);
        }

        [TestMethod]
        public void SolutionHeader_ForShProj_Should_CreateCorrectGuid()
        {
            /*
                 Should Generate the following output
                Project("{D954291E-2A0B-460D-934E-DC6B0785DB48}") = "Xyz.Xrm", "Xyz.Xrm\Xyz.Xrm.shproj", "{B22B3BC6-0AC6-4CDD-A118-16E318818AD7}"
                EndProject
             */
            var sut = new ProjectInfo()
            {
                Type = ProjectInfo.ProjectType.SharedProj,
                Name = "ShProjTest",
                Id = Guid.NewGuid()
            };
            Assert.AreEqual("Project(\"{D954291E-2A0B-460D-934E-DC6B0785DB48}\") = \"ShProjTest\", \"ShProjTest\\ShProjTest.shproj\", \"{" + sut.Id.ToString().ToUpper() + "}\"" + Environment.NewLine + "EndProject", sut.SolutionProjectHeader);
        }
    }
}
