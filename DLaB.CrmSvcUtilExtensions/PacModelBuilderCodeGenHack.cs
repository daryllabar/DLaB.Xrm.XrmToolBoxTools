using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DLaB.ModelBuilderExtensions
{
    internal class PacModelBuilderCodeGenHack
    {
        public ICodeGenerationService DefaultService { get; set; }
        public List<string> FilesWritten { get; set; } = new List<string>();
        public bool LegacyMode { get; set; }
        public DLaBModelBuilderSettings Settings { get; set; }
        public bool SplitFilesByObject { get; set; }

        public HashSet<string> ClassesToMakeStatic { get; set; }

        public PacModelBuilderCodeGenHack(DLaBModelBuilderSettings settings, ICodeGenerationService defaultService,  bool splitFilesByObject, bool legacyMode)
        {
            Settings = settings;
            SplitFilesByObject = splitFilesByObject;
            LegacyMode = legacyMode;
            DefaultService = defaultService;
            ClassesToMakeStatic = new HashSet<string>();
            if (settings.DLaBModelBuilder.GenerateOptionSetMetadataAttribute)
            {
                ClassesToMakeStatic.Add("OptionSetExtension");
            }
        }

        internal void Write(
            IOrganizationMetadata organizationMetadata,
            string language,
            string outputFile,
            string outputNamespace,
            IServiceProvider serviceProvider)
        {
            FilesWritten.Clear();
            ProcessModelInvoker_ModelBuilderLogger_TraceMethodStart("Entering {0}", nameof(Write));
            if (SplitFilesByObject)
            {
                Dictionary<string, CodeNamespace> dictionary = CodeGenerationService_BuildCodeDom2(organizationMetadata, outputNamespace, serviceProvider, language, LegacyMode);
                string str = string.IsNullOrWhiteSpace(Settings.ServiceContextName) ? "##@" : Settings.ServiceContextName;
                foreach (KeyValuePair<string, CodeNamespace> keyValuePair in dictionary)
                {
                    var filePath = GetFilePath(keyValuePair.Key, keyValuePair.Value);
                    CodeGenerationService_WriteFile(filePath, language, keyValuePair.Value, serviceProvider, keyValuePair.Key.Contains(str), true);
                    MakeConfiguredClassesStatic(filePath, keyValuePair.Value);
                    FilesWritten.Add(filePath);
                }

                if (string.IsNullOrEmpty(str))
                {
                    ProcessModelInvoker_ModelBuilderLogger_TraceWarning("ProxyTypesAssemblyAttribute not written, Please add this to a file in your class");
                    Console.Out.WriteLine("ProxyTypesAssemblyAttribute not written, Please add [assembly: Microsoft.Xrm.Sdk.Client.ProxyTypesAssemblyAttribute()] to a file in your class");
                }
            }
            else
            {
                CodeNamespace codenamespace = CodeGenerationService_BuildCodeDom(organizationMetadata, outputNamespace, serviceProvider, LegacyMode);
                CodeGenerationService_WriteFile(outputFile, language, codenamespace, serviceProvider);
                MakeConfiguredClassesStatic(outputFile);
                FilesWritten.Add(outputFile);
            }
            ProcessModelInvoker_ModelBuilderLogger_TraceMethodStop("Exiting {0}", nameof(Write));
        }

        public void MakeConfiguredClassesStatic(string filePath, CodeNamespace code)
        {
            if (ClassesToMakeStatic.Count == 0
                || !code.Types.OfType<CodeTypeDeclaration>().Any(t => ClassesToMakeStatic.Contains(t.Name)))
            {
                return;
            }

            MakeConfiguredClassesStatic(filePath);
        }

        public void MakeConfiguredClassesStatic(string filePath)
        {
            var fileContents = File.ReadAllLines(filePath);

            var pattern = @"\b(?:" + string.Join("|", ClassesToMakeStatic.Select(v => $"class {v}")) + @")\b";
            for (var i = 0; i < fileContents.Length; i++)
            {
                fileContents[i] = Regex.Replace(fileContents[i], pattern, t => t.Value.Replace("class ", "static class "));
            }

            File.WriteAllLines(filePath, fileContents);
        }

        public string GetFilePath(string filePath, CodeNamespace code)
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            string actionFileName = null;
            foreach (var type in code.Types.OfType<CodeTypeDeclaration>())
            {
                if (string.Equals(fileName, type.Name, StringComparison.CurrentCultureIgnoreCase))
                {
                    return GenerateFilePath(type.Name);
                }

                if (string.Equals(fileName + "Request", type.Name, StringComparison.CurrentCultureIgnoreCase))
                {
                    actionFileName = GenerateFilePath(type.Name.Substring(0, type.Name.Length - "Request".Length));
                }
            }

            return actionFileName ?? filePath;

            string GenerateFilePath(string newName)
            {
                return Path.Combine(Path.GetDirectoryName(filePath) ?? string.Empty, newName + Path.GetExtension(filePath));
            }
        }

        private Dictionary<string, CodeNamespace> CodeGenerationService_BuildCodeDom2(
            IOrganizationMetadata organizationMetadata,
            string outputNamespace,
            IServiceProvider serviceProvider,
            string language,
            bool useLegacyMode)
        {
            return (Dictionary<string, CodeNamespace>)InvokeMicrosoft_PowerPlatform_Dataverse_ModelBuilderLib_NonPublicStaticMethod("BuildCodeDom2", new object[]
            {
                organizationMetadata,
                outputNamespace,
                serviceProvider,
                language,
                useLegacyMode
            });
        }

        private CodeNamespace CodeGenerationService_BuildCodeDom(
            IOrganizationMetadata organizationMetadata,
            string outputNamespace,
            IServiceProvider serviceProvider,
            bool useLegacyMode)
        {
            return (CodeNamespace) InvokeMicrosoft_PowerPlatform_Dataverse_ModelBuilderLib_NonPublicStaticMethod("BuildCodeDom", new object[]
            {
                organizationMetadata,
                outputNamespace,
                serviceProvider,
                useLegacyMode
            });
        }

        private void CodeGenerationService_WriteFile(
            string outputFile,
            string language,
            CodeNamespace codenamespace,
            IServiceProvider serviceProvider,
            bool writeProxyAttrib = true,
            bool isFileSplit = false)
        {
            InvokeMicrosoft_PowerPlatform_Dataverse_ModelBuilderLib_NonPublicStaticMethod("WriteFile", new object[]
            {
                outputFile,
                language,
                codenamespace,
                serviceProvider,
                writeProxyAttrib,
                isFileSplit
            });
        }

        private void ProcessModelInvoker_ModelBuilderLogger_TraceMethodStart(string message, params object[] messageData)
        {
            InvokePublicInstanceMethodOnNonPublicStaticField<ProcessModelInvoker>("ModelBuilderLogger", "TraceMethodStart",
                new object[] { message, messageData });
        }

        private void ProcessModelInvoker_ModelBuilderLogger_TraceMethodStop(string message, params object[] messageData)
        {
            InvokePublicInstanceMethodOnNonPublicStaticField<ProcessModelInvoker>("ModelBuilderLogger", "TraceMethodStop",
                new object[] { message, messageData });
        }

        private void ProcessModelInvoker_ModelBuilderLogger_TraceWarning(string message, params object[] messageData)
        {
            InvokePublicInstanceMethodOnNonPublicStaticField<ProcessModelInvoker>("ModelBuilderLogger", "TraceWarning",
                new object[] { message, messageData });
        }

        private object InvokeMicrosoft_PowerPlatform_Dataverse_ModelBuilderLib_NonPublicStaticMethod(string methodName, object[] parameters)
        {
            var method = DefaultService.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
            if (method == null)
            {
                throw new NotSupportedException($"Unable to lookup non-public static method {methodName} of type {DefaultService.GetType().FullName}!");
            }

            return method.Invoke(null, parameters);
        }

        private object InvokePublicInstanceMethodOnNonPublicStaticField<TStaticFieldType>(string fieldName, string methodName, object[] parameters)
        {
            // System.NotSupportedException: Unable to lookup non-public instance method TraceMethodStart of type Microsoft.PowerPlatform.Dataverse.ModelBuilderLib.TraceLogger!
            var type = typeof(TStaticFieldType);
            var field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
            if (field == null)
            {
                throw new NotSupportedException($"Unable to lookup static field {fieldName} of type {type.FullName}!");
            }

            var instance = field.GetValue(null);
            var method = instance.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
            if (method == null)
            {
                throw new NotSupportedException($"Unable to lookup non-public instance method {methodName} of type {instance.GetType().FullName}!");
            }

            return method.Invoke(instance, parameters);
        }
    }
}
