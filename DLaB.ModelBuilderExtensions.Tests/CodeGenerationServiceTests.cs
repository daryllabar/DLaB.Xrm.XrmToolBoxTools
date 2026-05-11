using DLaB.ModelBuilderExtensions.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.CodeDom;
using System.Linq;

namespace DLaB.ModelBuilderExtensions.Tests
{
    [TestClass]
    public class AttributeConstGeneratorTests
    {
        [TestMethod]
        public void CustomizeCodeDom_ShouldReplaceFieldsCommentTypo_InConstClassSnippet()
        {
            var sut = new AttributeConstGenerator(null!, new DLaBModelBuilderSettings
            {
                EmitFieldsClasses = true
            });

            var code = new CodeCompileUnit();
            var ns = new CodeNamespace("TestNamespace");
            code.Namespaces.Add(ns);
            var entity = new CodeTypeDeclaration("Account")
            {
                IsClass = true
            };
            entity.CustomAttributes.Add(new CodeAttributeDeclaration(
                "Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute",
                new CodeAttributeArgument(new CodePrimitiveExpression("account"))));
            entity.Members.Add(new CodeSnippetTypeMember(string.Join(Environment.NewLine, new[]
            {
                "\t\t/// <summary>",
                "\t\t/// Available fields, a the time of codegen, for the account entity",
                "\t\t/// </summary>",
                "\t\tpublic partial class Fields",
                "\t\t{",
                "\t\t\tpublic const string Name = \"name\";",
                "\t\t}"
            })));
            ns.Types.Add(entity);

            sut.CustomizeCodeDom(code, null!);

            var fieldsClass = entity.Members.OfType<CodeSnippetTypeMember>().First(m => m.Text.Contains("public partial class Fields"));
            Assert.IsTrue(fieldsClass.Text.Contains("Available fields, at the time of codegen, for the account entity"));
            Assert.IsFalse(fieldsClass.Text.Contains("Available fields, a the time of codegen, for the account entity"));
        }
    }
}
