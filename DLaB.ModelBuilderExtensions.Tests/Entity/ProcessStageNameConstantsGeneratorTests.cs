using DLaB.ModelBuilderExtensions.Entity;
using FakeItEasy;
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

namespace DLaB.ModelBuilderExtensions.Tests.Entity
{
    [TestClass]
    public class ProcessStageNameConstantsGeneratorTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            ProcessStageNameCache.Clear();
        }

        [TestMethod]
        public void CustomizeCodeDom_WhenGenerateProcessStageNamesFalse_ShouldNotAddStageNamesClass()
        {
            ProcessStageNameCache.StageNames = new List<string> { "Qualify", "Develop" };
            var sut = new ProcessStageNameConstantsGenerator(A.Fake<ICustomizeCodeDomService>(), new DLaBModelBuilderSettings
            {
                DLaBModelBuilder = new DLaBModelBuilder { GenerateProcessStageNames = false }
            });

            var code = BuildProcessStageCodeUnit();
            sut.CustomizeCodeDom(code, null!);

            var entity = code.Namespaces[0].Types[0];
            Assert.IsFalse(entity.Members.OfType<CodeSnippetTypeMember>().Any(m => m.Text.Contains(ProcessStageNameConstantsGenerator.StageNamesClassName)));
        }

        [TestMethod]
        public void CustomizeCodeDom_WhenCacheEmpty_ShouldNotAddStageNamesClass()
        {
            ProcessStageNameCache.StageNames = null;
            var sut = new ProcessStageNameConstantsGenerator(A.Fake<ICustomizeCodeDomService>(), new DLaBModelBuilderSettings
            {
                DLaBModelBuilder = new DLaBModelBuilder { GenerateProcessStageNames = true }
            });

            var code = BuildProcessStageCodeUnit();
            sut.CustomizeCodeDom(code, null!);

            var entity = code.Namespaces[0].Types[0];
            Assert.IsFalse(entity.Members.OfType<CodeSnippetTypeMember>().Any(m => m.Text.Contains(ProcessStageNameConstantsGenerator.StageNamesClassName)));
        }

        [TestMethod]
        public void CustomizeCodeDom_WhenNonProcessStageEntity_ShouldNotAddStageNamesClass()
        {
            ProcessStageNameCache.StageNames = new List<string> { "Qualify", "Develop" };
            var sut = new ProcessStageNameConstantsGenerator(A.Fake<ICustomizeCodeDomService>(), new DLaBModelBuilderSettings
            {
                DLaBModelBuilder = new DLaBModelBuilder { GenerateProcessStageNames = true }
            });

            var code = BuildEntityCodeUnit("Account", "account");
            sut.CustomizeCodeDom(code, null!);

            var entity = code.Namespaces[0].Types[0];
            Assert.IsFalse(entity.Members.OfType<CodeSnippetTypeMember>().Any(m => m.Text.Contains(ProcessStageNameConstantsGenerator.StageNamesClassName)));
        }

        [TestMethod]
        public void CustomizeCodeDom_WhenProcessStageEntityAndNamesLoaded_ShouldAddStageNamesClass()
        {
            ProcessStageNameCache.StageNames = new List<string> { "Qualify", "Develop", "Propose" };
            var sut = new ProcessStageNameConstantsGenerator(A.Fake<ICustomizeCodeDomService>(), new DLaBModelBuilderSettings
            {
                DLaBModelBuilder = new DLaBModelBuilder { GenerateProcessStageNames = true }
            });

            var code = BuildProcessStageCodeUnit();
            sut.CustomizeCodeDom(code, null!);

            var entity = code.Namespaces[0].Types[0];
            var stageNamesSnippet = entity.Members.OfType<CodeSnippetTypeMember>().FirstOrDefault(m => m.Text.Contains(ProcessStageNameConstantsGenerator.StageNamesClassName));
            Assert.IsNotNull(stageNamesSnippet, "StageNames class should be added to ProcessStage entity");

            Assert.IsTrue(stageNamesSnippet.Text.Contains("Qualify"), "StageNames class should contain Qualify constant");
            Assert.IsTrue(stageNamesSnippet.Text.Contains("Develop"), "StageNames class should contain Develop constant");
            Assert.IsTrue(stageNamesSnippet.Text.Contains("Propose"), "StageNames class should contain Propose constant");
        }

        [TestMethod]
        public void CustomizeCodeDom_ShouldGenerateValidConstantNamesForSpecialCharacters()
        {
            ProcessStageNameCache.StageNames = new List<string> { "Stage 1", "Stage-Two", "Stage_Three", "123Stage" };
            var sut = new ProcessStageNameConstantsGenerator(A.Fake<ICustomizeCodeDomService>(), new DLaBModelBuilderSettings
            {
                DLaBModelBuilder = new DLaBModelBuilder { GenerateProcessStageNames = true }
            });

            var code = BuildProcessStageCodeUnit();
            sut.CustomizeCodeDom(code, null!);

            var entity = code.Namespaces[0].Types[0];
            var stageNamesSnippet = entity.Members.OfType<CodeSnippetTypeMember>().FirstOrDefault(m => m.Text.Contains(ProcessStageNameConstantsGenerator.StageNamesClassName));
            Assert.IsNotNull(stageNamesSnippet);

            Assert.IsTrue(stageNamesSnippet.Text.Contains("Stage_1"), "Space should be replaced with underscore");
            Assert.IsTrue(stageNamesSnippet.Text.Contains("Stage_Two"), "Hyphen should be replaced with underscore");
            Assert.IsTrue(stageNamesSnippet.Text.Contains("Stage_Three"), "Underscore should remain");
            Assert.IsTrue(stageNamesSnippet.Text.Contains("_123Stage"), "Leading digit should be prefixed with underscore");
        }

        [TestMethod]
        public void SanitizeConstantName_ShouldHandleEdgeCases()
        {
            Assert.AreEqual(string.Empty, ProcessStageNameConstantsGenerator.SanitizeConstantName(null));
            Assert.AreEqual(string.Empty, ProcessStageNameConstantsGenerator.SanitizeConstantName(""));
            Assert.AreEqual(string.Empty, ProcessStageNameConstantsGenerator.SanitizeConstantName("   "));
            Assert.AreEqual(string.Empty, ProcessStageNameConstantsGenerator.SanitizeConstantName("---"));
            Assert.AreEqual("ValidName", ProcessStageNameConstantsGenerator.SanitizeConstantName("ValidName"));
            Assert.AreEqual("_1Start", ProcessStageNameConstantsGenerator.SanitizeConstantName("1Start"));
            Assert.AreEqual("Hello_World", ProcessStageNameConstantsGenerator.SanitizeConstantName("Hello World"));
        }

        private static CodeCompileUnit BuildProcessStageCodeUnit()
        {
            return BuildEntityCodeUnit("ProcessStage", "processstage");
        }

        private static CodeCompileUnit BuildEntityCodeUnit(string className, string entityLogicalName)
        {
            var code = new CodeCompileUnit();
            var ns = new CodeNamespace("TestNamespace");
            code.Namespaces.Add(ns);
            var entity = new CodeTypeDeclaration(className) { IsClass = true };
            entity.CustomAttributes.Add(new CodeAttributeDeclaration(
                "Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute",
                new CodeAttributeArgument(new CodePrimitiveExpression(entityLogicalName))));
            ns.Types.Add(entity);
            return code;
        }
    }
}
