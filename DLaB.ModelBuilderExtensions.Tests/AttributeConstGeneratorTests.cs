using DLaB.ModelBuilderExtensions.Entity;
using FakeItEasy;
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var sut = new AttributeConstGenerator(A.Fake<ICustomizeCodeDomService>(), new DLaBModelBuilderSettings
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
            entity.CustomAttributes.Add(new CodeAttributeDeclaration("Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute", new CodeAttributeArgument(new CodePrimitiveExpression("account"))));
            entity.Members.Add(new CodeSnippetTypeMember(
                """
                		/// <summary>
                		/// Available fields, a the time of codegen, for the account entity
                		/// </summary>
                		public partial class Fields
                		{
                			public const string Name = "name";
                		}
                """
            ));
            ns.Types.Add(entity);

            sut.CustomizeCodeDom(code, null!);

            var fieldsClass = entity.Members.OfType<CodeSnippetTypeMember>().First(m => m.Text.Contains("public partial class Fields"));
            Assert.IsTrue(fieldsClass.Text.Contains("Available fields, at the time of codegen, for the account entity"));
            Assert.IsFalse(fieldsClass.Text.Contains("Available fields, a the time of codegen, for the account entity"));
        }

        [TestMethod]
        public void CustomizeCodeDom_ShouldAddObsoleteAttributeToFieldConst_WhenPropertyIsObsolete()
        {
            var sut = new AttributeConstGenerator(A.Fake<ICustomizeCodeDomService>(), new DLaBModelBuilderSettings
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
            entity.CustomAttributes.Add(new CodeAttributeDeclaration("Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute", new CodeAttributeArgument(new CodePrimitiveExpression("account"))));
            entity.Members.Add(new CodeMemberProperty
            {
                Name = "DeprecatedField",
                Type = new CodeTypeReference(typeof(string)),
                Attributes = System.CodeDom.MemberAttributes.Public | System.CodeDom.MemberAttributes.Final,
                HasGet = true,
                HasSet = true
            });
            entity.GetMembers<CodeMemberProperty>().Single().CustomAttributes.Add(
                new CodeAttributeDeclaration("System.Obsolete", new CodeAttributeArgument(new CodePrimitiveExpression("This attribute is deprecated."))));
            entity.Members.Add(new CodeSnippetTypeMember(
                """
                		public partial class Fields
                		{
                			public const string DeprecatedField = "deprecatedfield";
                			public const string ActiveField = "activefield";
                		}
                """
            ));
            ns.Types.Add(entity);

            sut.CustomizeCodeDom(code, null!);

            var fieldsClass = entity.Members.OfType<CodeSnippetTypeMember>().First(m => m.Text.Contains("public partial class Fields"));
            Assert.IsTrue(fieldsClass.Text.Contains("[System.Obsolete(\"This attribute is deprecated.\")]" + System.Environment.NewLine + "\t\t\tpublic const string DeprecatedField = \"deprecatedfield\";"));
            Assert.IsFalse(fieldsClass.Text.Contains("[System.Obsolete(\"This attribute is deprecated.\")]" + System.Environment.NewLine + "\t\t\tpublic const string ActiveField = \"activefield\";"));
        }
    }
}