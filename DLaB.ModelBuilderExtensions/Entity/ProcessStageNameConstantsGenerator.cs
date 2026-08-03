using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DLaB.ModelBuilderExtensions.Entity
{
    /// <summary>
    /// Generates a <c>StageNames</c> constants class inside the <c>ProcessStage</c> entity,
    /// containing a <c>public const string</c> for every distinct stage name retrieved from Dataverse.
    /// </summary>
    public class ProcessStageNameConstantsGenerator : TypedServiceSettings<ICustomizeCodeDomService>, ICustomizeCodeDomService
    {
        private const string ProcessStageLogicalName = "processstage";

        /// <summary>
        /// The name of the generated inner class.
        /// </summary>
        public const string StageNamesClassName = "StageNames";

        public bool GenerateProcessStageNames
        {
            get => DLaBSettings.GenerateProcessStageNames;
            set => DLaBSettings.GenerateProcessStageNames = value;
        }

        public ProcessStageNameConstantsGenerator(ICustomizeCodeDomService defaultService, IDictionary<string, string> parameters)
            : base(defaultService, parameters)
        {
        }

        public ProcessStageNameConstantsGenerator(ICustomizeCodeDomService defaultService, DLaBModelBuilderSettings settings = null)
            : base(defaultService, settings ?? new DLaBModelBuilderSettings())
        {
        }

        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            if (!GenerateProcessStageNames)
            {
                return;
            }

            var stageNames = ProcessStageNameCache.StageNames;
            if (stageNames == null || stageNames.Count == 0)
            {
                return;
            }

            foreach (var type in codeUnit.GetEntityTypes())
            {
                if (type.GetEntityLogicalName() != ProcessStageLogicalName)
                {
                    continue;
                }

                var stageNamesClass = BuildStageNamesClass(stageNames);
                type.Members.Add(GenerateTypeWithoutEmptyLines(stageNamesClass));
            }
        }

        private static CodeTypeDeclaration BuildStageNamesClass(List<string> stageNames)
        {
            var stageNamesClass = new CodeTypeDeclaration
            {
                Name = StageNamesClassName,
                IsClass = true,
                TypeAttributes = TypeAttributes.Public
            };

            var sanitized = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var name in stageNames)
            {
                var constName = SanitizeConstantName(name);
                if (string.IsNullOrEmpty(constName) || !sanitized.Add(constName))
                {
                    continue;
                }

                stageNamesClass.Members.Add(new CodeMemberField
                {
                    // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
                    Attributes = System.CodeDom.MemberAttributes.Public | System.CodeDom.MemberAttributes.Const,
                    Name = constName,
                    Type = new CodeTypeReference(typeof(string)),
                    InitExpression = new CodePrimitiveExpression(name)
                });
            }

            return stageNamesClass;
        }

        /// <summary>
        /// Converts a stage name string into a valid C# identifier by replacing non-alphanumeric
        /// characters with underscores and ensuring it does not start with a digit.
        /// </summary>
        internal static string SanitizeConstantName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return string.Empty;
            }

            var chars = name.ToCharArray();
            for (var i = 0; i < chars.Length; i++)
            {
                if (!char.IsLetterOrDigit(chars[i]) && chars[i] != '_')
                {
                    chars[i] = '_';
                }
            }

            var result = new string(chars).Trim('_');
            if (result.Length == 0)
            {
                return string.Empty;
            }

            if (char.IsDigit(result[0]))
            {
                result = "_" + result;
            }

            return result;
        }

        /// <summary>
        /// Generates the class as a snippet without blank lines between members.
        /// </summary>
        private static CodeSnippetTypeMember GenerateTypeWithoutEmptyLines(CodeTypeDeclaration type)
        {
            var provider = CodeDomProvider.CreateProvider("CSharp");
            using (var sourceWriter = new StringWriter())
            using (var tabbedWriter = new IndentedTextWriter(sourceWriter, "\t"))
            {
                tabbedWriter.Indent = 2;
                provider.GenerateCodeFromType(type, tabbedWriter, new CodeGeneratorOptions
                {
                    BracingStyle = "C",
                    IndentString = "\t",
                    BlankLinesBetweenMembers = false
                });
                var stringSource = sourceWriter.ToString().Replace("public class", "public static class");
                var lastNewLine = stringSource.LastIndexOf(Environment.NewLine, StringComparison.Ordinal);
                if (lastNewLine >= 0)
                {
                    stringSource = stringSource.Remove(lastNewLine);
                }
                return new CodeSnippetTypeMember("\t\t" + stringSource);
            }
        }
    }
}
