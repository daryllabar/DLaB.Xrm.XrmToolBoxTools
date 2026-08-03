using DLaB.ModelBuilderExtensions.Entity;
using FakeItEasy;
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

namespace DLaB.ModelBuilderExtensions.Tests
{
    [TestClass]
    public class AttributeConstGeneratorTests
    {
        private static CodeCompileUnit BuildEntityCodeUnit(string className, string entityLogicalName, params (string DisplayName, string LogicalName)[] namePairs)
        {
            var code = new CodeCompileUnit();
            var ns = new CodeNamespace("TestNamespace");
            code.Namespaces.Add(ns);
            var entity = new CodeTypeDeclaration(className) { IsClass = true };
            entity.CustomAttributes.Add(new CodeAttributeDeclaration("Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute", new CodeAttributeArgument(new CodePrimitiveExpression(entityLogicalName))));

            var lines = new List<string>
            {
                "\t\t/// <summary>",
                $"\t\t/// Available fields, at the time of codegen, for the {entityLogicalName} entity",
                "\t\t/// </summary>",
                "\t\tpublic partial class Fields",
                "\t\t{"
            };
            lines.AddRange(namePairs.Select(namePair => $"\t\t\tpublic const string {namePair.DisplayName} = \"{namePair.LogicalName}\";"));
            lines.Add("\t\t}");

            entity.Members.Add(new CodeSnippetTypeMember(string.Join(Environment.NewLine, lines)));
            ns.Types.Add(entity);
            return code;
        }

        [TestMethod]
        public void CustomizeCodeDom_ShouldReplaceFieldsCommentTypo_InConstClassSnippet()
        {
            var sut = new AttributeConstGenerator(A.Fake<ICustomizeCodeDomService>(), new DLaBModelBuilderSettings
            {
                EmitFieldsClasses = true
            });


            var code = BuildEntityCodeUnit("Account", "account",
                ("Name", "name"));
            sut.CustomizeCodeDom(code, null!);
            var entity = code.Namespaces[0].Types[0];

            var fieldsClass = entity.Members.OfType<CodeSnippetTypeMember>().First(m => m.Text.Contains("public partial class Fields"));
            Assert.IsTrue(fieldsClass.Text.Contains("Available fields, at the time of codegen, for the account entity"));
            Assert.IsFalse(fieldsClass.Text.Contains("Available fields, a the time of codegen, for the account entity"));
        }

    }
}