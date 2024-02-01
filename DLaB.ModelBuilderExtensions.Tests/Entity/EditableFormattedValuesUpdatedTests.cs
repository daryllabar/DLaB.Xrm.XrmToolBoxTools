using DLaB.ModelBuilderExtensions.Entity;
using Microsoft.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using MemberAttributes = System.CodeDom.MemberAttributes;

namespace DLaB.ModelBuilderExtensions.Tests.Entity
{
    [TestClass]
    public class EditableFormattedValuesUpdaterTests
    {
        private CodeTypeDeclaration _class;
        private CodeCompileUnit _code;
        private EditableFormattedValuesUpdater _sut;

        [TestInitialize]
        public void Initialize()
        {
            // Create a CodeCompileUnit to hold the generated code
            _code = new CodeCompileUnit();

            // Create a namespace
            var myNamespace = new CodeNamespace("MyNamespace");
            _code.Namespaces.Add(myNamespace);

            // Create a class
            _class = new CodeTypeDeclaration("MyClass");
            _class.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(EntityLogicalNameAttribute))));
            myNamespace.Types.Add(_class);

            _sut = new EditableFormattedValuesUpdater();
        }

        [TestMethod]
        public void FormattedValuesProperty_Should_BeFixed()
        {
            // Add the property to the class
            _class.Members.Add(GetCreatedByNameFormattedValuesProperty());

            _sut.CustomizeCodeDom(_code, null);
            var code = GenerateCode(_code).Split(new [] { Environment.NewLine }, StringSplitOptions.None);
            var expected = TestProject.GetResourceText("Resources.EditableFormattedValuesGeneration.txt").Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            for (var i = 0; i < code.Length; i++)
            {
                var line = code[i];
                Assert.AreEqual(expected[i].Trim(), line.Trim(), $"Line {i + 1} does not match the expected!");
            }
        }

        private static string GenerateCode(CodeCompileUnit code)
        {
            // Generate the C# code and store it in an in-memory string
            var codeProvider = new CSharpCodeProvider();
            using (var writer = new StringWriter()) // Use StringWriter to capture the output
            {
                codeProvider.GenerateCodeFromCompileUnit(code, writer, new CodeGeneratorOptions());
                return writer.ToString();
            }
        }

        private static CodeMemberProperty GetCreatedByNameFormattedValuesProperty()
        {
            // Create the CreatedByName property
            var property = new CodeMemberProperty
            {
                Name = "CreatedByName",
                Type = new CodeTypeReference(typeof(string)),
                Attributes = MemberAttributes.Public
            };

            // Create the getter for CreatedByName
            var ifContainsKey = new CodeConditionStatement(
                new CodeMethodInvokeExpression(
                    new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "FormattedValues"),
                    "ContainsKey",
                    new CodePrimitiveExpression("createdby")),
                new CodeStatement[]
                {
                    new CodeMethodReturnStatement(
                        new CodeIndexerExpression(
                            new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "FormattedValues"),
                            new CodePrimitiveExpression("createdby")))
                },
                new CodeStatement[]
                {
                    new CodeMethodReturnStatement(new CodeDefaultValueExpression(new CodeTypeReference(typeof(string))))
                }
            );

            property.GetStatements.Add(ifContainsKey);

            // Create the setter for CreatedByName
            var setAttributeValue = new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                "SetAttributeValue",
                new CodePrimitiveExpression("createdbyname"), // Attribute name
                new CodePropertySetValueReferenceExpression()); // Value

            property.SetStatements.Add(setAttributeValue);
            return property;
        }
    }
}
