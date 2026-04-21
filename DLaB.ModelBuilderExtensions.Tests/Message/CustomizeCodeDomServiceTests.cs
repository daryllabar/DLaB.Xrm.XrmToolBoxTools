using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System.CodeDom;
using System.Linq;
using MemberAttributes = System.CodeDom.MemberAttributes;

namespace DLaB.ModelBuilderExtensions.Tests.Message
{
    [TestClass]
    public class CustomizeCodeDomServiceTests
    {
        private static CodeCompileUnit BuildCodeUnit(CodeTypeDeclaration type)
        {
            var code = new CodeCompileUnit();
            var ns = new CodeNamespace("TestNamespace");
            code.Namespaces.Add(ns);
            ns.Types.Add(type);
            return code;
        }

        private static CodeTypeDeclaration CreateResponseClass(string className = "TestResponse")
        {
            var type = new CodeTypeDeclaration(className) { IsClass = true };
            type.BaseTypes.Add(new CodeTypeReference(typeof(OrganizationResponse)));
            return type;
        }

        private static CodeTypeDeclaration CreateRequestClass(string className = "TestRequest")
        {
            var type = new CodeTypeDeclaration(className) { IsClass = true };
            type.BaseTypes.Add(new CodeTypeReference(typeof(OrganizationRequest)));
            return type;
        }

        /// <summary>
        /// Creates a response output property whose getter uses this.Results (as PAC ModelBuilder correctly generates)
        /// but whose setter incorrectly references this.Parameters (the bug).
        /// </summary>
        private static CodeMemberProperty CreateOutputPropertyWithParametersSetter(string propertyName, string fieldKey)
        {
            var prop = new CodeMemberProperty
            {
                Name = propertyName,
                Type = new CodeTypeReference(typeof(string)),
                Attributes = MemberAttributes.Public
            };

            // Correct GET: if (this.Results.Contains("fieldKey")) return (string)this.Results["fieldKey"]; ...
            prop.GetStatements.Add(new CodeConditionStatement(
                new CodeMethodInvokeExpression(
                    new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "Results"),
                    "Contains",
                    new CodePrimitiveExpression(fieldKey)),
                new CodeStatement[]
                {
                    new CodeMethodReturnStatement(
                        new CodeCastExpression(typeof(string),
                            new CodeArrayIndexerExpression(
                                new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "Results"),
                                new CodePrimitiveExpression(fieldKey))))
                },
                new CodeStatement[]
                {
                    new CodeMethodReturnStatement(new CodePrimitiveExpression(null))
                }));

            // Wrong SET: this.Parameters["fieldKey"] = value  (bug — should be this.Results)
            prop.SetStatements.Add(new CodeAssignStatement(
                new CodeIndexerExpression(
                    new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "Parameters"),
                    new CodePrimitiveExpression(fieldKey)),
                new CodePropertySetValueReferenceExpression()));

            return prop;
        }

        private static CodeMemberProperty CreateOutputPropertyWithNoSetter(string propertyName, string fieldKey)
        {
            var prop = new CodeMemberProperty
            {
                Name = propertyName,
                Type = new CodeTypeReference(typeof(string)),
                Attributes = MemberAttributes.Public
            };

            prop.GetStatements.Add(new CodeConditionStatement(
                new CodeMethodInvokeExpression(
                    new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "Results"),
                    "Contains",
                    new CodePrimitiveExpression(fieldKey)),
                new CodeStatement[]
                {
                    new CodeMethodReturnStatement(
                        new CodeCastExpression(typeof(string),
                            new CodeArrayIndexerExpression(
                                new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "Results"),
                                new CodePrimitiveExpression(fieldKey))))
                },
                new CodeStatement[]
                {
                    new CodeMethodReturnStatement(new CodePrimitiveExpression(null))
                }));

            return prop;
        }

        private static bool SetterUsesResults(CodeMemberProperty prop)
        {
            if (prop.SetStatements.Count != 1) return false;
            if (prop.SetStatements[0] is not CodeAssignStatement assign) return false;

            string target = null;
            if (assign.Left is CodeArrayIndexerExpression arrayIndexer)
            {
                target = (arrayIndexer.TargetObject as CodeFieldReferenceExpression)?.FieldName
                      ?? (arrayIndexer.TargetObject as CodePropertyReferenceExpression)?.PropertyName;
            }
            else if (assign.Left is CodeIndexerExpression indexer)
            {
                target = (indexer.TargetObject as CodeFieldReferenceExpression)?.FieldName
                      ?? (indexer.TargetObject as CodePropertyReferenceExpression)?.PropertyName;
            }

            return target == "Results";
        }

        [TestMethod]
        public void ProcessMessage_ResponseProperty_WithParametersSetter_ShouldBeReplacedWithResultsSetter()
        {
            var responseType = CreateResponseClass();
            responseType.Members.Add(CreateOutputPropertyWithParametersSetter("OutputParam", "Base64PNGImage"));
            var code = BuildCodeUnit(responseType);

            var sut = new CustomizeCodeDomService(null, new DLaBModelBuilderSettings { MakeResponseMessagesEditable = true });
            sut.CustomizeCodeDom(code, null);

            var prop = responseType.Members.OfType<CodeMemberProperty>().First();
            Assert.IsTrue(SetterUsesResults(prop), "SET accessor should reference this.Results, not this.Parameters.");
            Assert.AreEqual(1, prop.SetStatements.Count, "Should have exactly one set statement.");
        }

        [TestMethod]
        public void ProcessMessage_ResponseProperty_WithoutSetter_ShouldAddResultsSetter()
        {
            var responseType = CreateResponseClass();
            responseType.Members.Add(CreateOutputPropertyWithNoSetter("OutputParam", "Base64PNGImage"));
            var code = BuildCodeUnit(responseType);

            var sut = new CustomizeCodeDomService(null, new DLaBModelBuilderSettings { MakeResponseMessagesEditable = true });
            sut.CustomizeCodeDom(code, null);

            var prop = responseType.Members.OfType<CodeMemberProperty>().First();
            Assert.IsTrue(SetterUsesResults(prop), "SET accessor should reference this.Results.");
            Assert.AreEqual(1, prop.SetStatements.Count, "Should have exactly one set statement.");
        }

        [TestMethod]
        public void ProcessMessage_RequestProperty_WithParametersSetter_ShouldNotBeModified()
        {
            var requestType = CreateRequestClass();
            var inputProp = new CodeMemberProperty
            {
                Name = "InputParam",
                Type = new CodeTypeReference(typeof(string)),
                Attributes = MemberAttributes.Public
            };
            // Request property correctly uses Parameters
            inputProp.SetStatements.Add(new CodeAssignStatement(
                new CodeIndexerExpression(
                    new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "Parameters"),
                    new CodePrimitiveExpression("InputParam")),
                new CodePropertySetValueReferenceExpression()));
            requestType.Members.Add(inputProp);
            var code = BuildCodeUnit(requestType);

            var sut = new CustomizeCodeDomService(null, new DLaBModelBuilderSettings { MakeResponseMessagesEditable = true });
            sut.CustomizeCodeDom(code, null);

            var prop = requestType.Members.OfType<CodeMemberProperty>().First();
            Assert.AreEqual(1, prop.SetStatements.Count, "Request property setter should remain unchanged.");
            var assign = (CodeAssignStatement)prop.SetStatements[0];
            var target = ((CodeIndexerExpression)assign.Left).TargetObject as CodePropertyReferenceExpression;
            Assert.AreEqual("Parameters", target?.PropertyName, "Request property should still use Parameters collection.");
        }

        [TestMethod]
        public void ProcessMessage_MakeResponseMessagesEditableFalse_ShouldNotModifyProperties()
        {
            var responseType = CreateResponseClass();
            responseType.Members.Add(CreateOutputPropertyWithParametersSetter("OutputParam", "Base64PNGImage"));
            var code = BuildCodeUnit(responseType);

            var sut = new CustomizeCodeDomService(null, new DLaBModelBuilderSettings { MakeResponseMessagesEditable = false });
            sut.CustomizeCodeDom(code, null);

            var prop = responseType.Members.OfType<CodeMemberProperty>().First();
            // When MakeResponseMessagesEditable is false, setter should remain as-is (using Parameters)
            Assert.AreEqual(1, prop.SetStatements.Count);
            var assign = (CodeAssignStatement)prop.SetStatements[0];
            var target = ((CodeIndexerExpression)assign.Left).TargetObject as CodePropertyReferenceExpression;
            Assert.AreEqual("Parameters", target?.PropertyName, "When disabled, setter should not be modified.");
        }
    }
}
