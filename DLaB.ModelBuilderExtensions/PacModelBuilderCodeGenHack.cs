using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Source.DLaB.Xrm;

namespace DLaB.ModelBuilderExtensions
{
    internal class PacModelBuilderCodeGenHack
    {
        public ICodeGenerationService DefaultService { get; set; }
        public Dictionary<string, CodeNamespace> FilesWritten { get; set; } = new Dictionary<string, CodeNamespace>();
        public bool LegacyMode { get; set; }
        public DLaBModelBuilderSettings Settings { get; set; }
        public bool SplitFilesByObject { get; set; }

        public HashSet<string> ClassesToMakeStatic { get; set; }

        public PacModelBuilderCodeGenHack(DLaBModelBuilderSettings settings, ICodeGenerationService defaultService,  bool splitFilesByObject, bool legacyMode, string filePrefixText)
        {
            ClassesToMakeStatic = new HashSet<string>();
            DefaultService = defaultService;
            LegacyMode = legacyMode;
            Settings = settings;
            SplitFilesByObject = splitFilesByObject;

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
            ProcessModelInvoker.ModelBuilderLogger.TraceMethodStart("Entering {0}", nameof(Write));
            if (SplitFilesByObject)
            {
                Dictionary<string, CodeNamespace> dictionary = CodeGenerationService_BuildCodeDom2(organizationMetadata, outputNamespace, serviceProvider, language, LegacyMode);
                string str = string.IsNullOrWhiteSpace(Settings.ServiceContextName) ? "##@" : Settings.ServiceContextName;
                foreach (KeyValuePair<string, CodeNamespace> keyValuePair in dictionary)
                {
                    var filePath = GetFilePath(keyValuePair.Key, keyValuePair.Value);
                    CodeGenerationService_WriteFile(filePath, language, keyValuePair.Value, serviceProvider, keyValuePair.Key.Contains(str), true);
                    MakeConfiguredClassesStatic(filePath, keyValuePair.Value);
                    FilesWritten.Add(filePath, keyValuePair.Value);
                }

                if (str == "##@")
                {
                    ProcessModelInvoker.ModelBuilderLogger.TraceWarning("ProxyTypesAssemblyAttribute not written, Please add this to a file in your class");
                    Console.Out.WriteLine("ProxyTypesAssemblyAttribute not written, Please add [assembly: Microsoft.Xrm.Sdk.Client.ProxyTypesAssemblyAttribute()] to a file in your class");
                }
            }
            else
            {
                CodeNamespace codenamespace = CodeGenerationService_BuildCodeDom(organizationMetadata, outputNamespace, serviceProvider, LegacyMode);
                CodeGenerationService_WriteFile(outputFile, language, codenamespace, serviceProvider);
                MakeConfiguredClassesStatic(outputFile);
                FilesWritten.Add(outputFile, codenamespace);
            }
            ProcessModelInvoker.ModelBuilderLogger.TraceMethodStop("Exiting {0}", nameof(Write));
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
            var bpfInfo = Settings.DLaBModelBuilder.UseDisplayNameForBpfName
                ? BpfInfo.Parse(fileName)
                : new BpfInfo();

            if (bpfInfo.IsBpfName)
            {
                var bpfType = code.GetTypes().FirstOrDefault(t => t.GetEntityLogicalName() == fileName);
                if (bpfType != null)
                {
                    return GenerateFilePath(bpfType.Name);
                }
            }
            
            string actionFileName = null;
            foreach (var type in code.GetTypes())
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

        public void CodeGenerationService_WriteFile(
            string outputFile,
            string language,
            CodeNamespace codenamespace,
            IServiceProvider serviceProvider,
            bool writeProxyAttrib = true,
            bool isFileSplit = false)
        {
            EnsureFileIsAccessible(outputFile);
            WriteFile(
                outputFile,
                language,
                codenamespace,
                serviceProvider,
                writeProxyAttrib,
                isFileSplit
            );
        }

        public void WriteFile(
              string outputFile,
              string language,
              CodeNamespace codenamespace,
              IServiceProvider serviceProvider,
              bool writeProxyAttrib = true,
              bool isFileSplit = false)
        {
            ProcessModelInvoker.ModelBuilderLogger.TraceMethodStart("Entering {0}", (object)MethodBase.GetCurrentMethod().Name);
            FileInfo fileInfo = new FileInfo(outputFile);
            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();
            CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
            codeCompileUnit.Namespaces.Add(codenamespace);
            if (writeProxyAttrib)
                codeCompileUnit.AssemblyCustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ProxyTypesAssemblyAttribute))));
            serviceProvider.GetService<ICustomizeCodeDomService>().CustomizeCodeDom(codeCompileUnit, (IServiceProvider)serviceProvider);
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BlankLinesBetweenMembers = true;
            options.BracingStyle = "C";
            options.IndentString = "\t";
            options.VerbatimOrder = true;
            bool flag1 = language.Equals("CS", StringComparison.OrdinalIgnoreCase);
            bool flag2 = language.Equals("VB", StringComparison.OrdinalIgnoreCase);
            var commandLine = GetCommandLineToGenerate(codenamespace);
            using (var writer = Settings.DLaBModelBuilder.RemoveRuntimeVersionComment ? (TextWriter) new CustomTextWriter(new StreamWriter(outputFile), true): new StreamWriter(outputFile))
            {
                using (CodeDomProvider provider = CodeDomProvider.CreateProvider(language))
                {
                    if (flag1)
                    {
                        if (!string.IsNullOrWhiteSpace(Settings.DLaBModelBuilder.FilePrefixText))
                        {
                            provider.GenerateCodeFromCompileUnit(new CodeSnippetCompileUnit(string.Format(Settings.DLaBModelBuilder.FilePrefixText, Path.GetFileName(outputFile)) + Environment.NewLine), writer, options);
                        }
                        provider.GenerateCodeFromCompileUnit((CodeCompileUnit)new CodeSnippetCompileUnit("#pragma warning disable CS1591"), (TextWriter)writer, options);
                        if (Settings.SuppressGeneratedCodeAttribute)
                        {
                            provider.GenerateCodeFromCompileUnit((CodeCompileUnit)new CodeSnippetCompileUnit("// Code Generated by " + ApplicationName /*+ " (" + ApplicationVersion + ")"*/), (TextWriter)writer, options);
                        }
                        if(!string.IsNullOrWhiteSpace(commandLine))
                        {
                            provider.GenerateCodeFromCompileUnit(new CodeSnippetCompileUnit(commandLine), (TextWriter)writer, options);
                        }
                    }
                    if (flag2 && Settings.SuppressGeneratedCodeAttribute)
                        provider.GenerateCodeFromCompileUnit((CodeCompileUnit)new CodeSnippetCompileUnit("' Code Generated by " + ApplicationName + " (" + ApplicationVersion + ")"), (TextWriter)writer, options);
                    provider.GenerateCodeFromCompileUnit(codeCompileUnit, (TextWriter)writer, options);
                    if (flag1)
                        provider.GenerateCodeFromCompileUnit((CodeCompileUnit)new CodeSnippetCompileUnit("#pragma warning restore CS1591"), (TextWriter)writer, options);
                }
            }
            ProcessModelInvoker.ModelBuilderLogger.TraceInformation("Exit {0}: Code file written to {1}", (object)MethodBase.GetCurrentMethod().Name, (object)outputFile);
            Console.Out.WriteLine(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "\tCode written to {0}.", (object)Path.GetFullPath(outputFile)));
        }

        private string GetCommandLineToGenerate(CodeNamespace codenamespace)
        {
            var commandLine = string.Empty;
            if (Settings.DLaBModelBuilder.IncludeCommandLine
                && codenamespace.GetTypes().Any(t => t.IsContextType()))
            {
                commandLine = string.Join(" ",
                    ConfigHelper.Parameters.Where(kvp => kvp.Key == "outdirectory" || kvp.Key == "settingsTemplateFile")
                        .Select(kvp => "--" + kvp.Key + " " + kvp.Value));
                commandLine = $"// Created via this command line: PAC modelbuilder build {commandLine}".Replace(Environment.NewLine, Environment.NewLine + "//");
            }

            return commandLine;
        }

        protected void EnsureFileIsAccessible(string filePath)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                return;
            }
            // ReSharper restore AssignNullToNotNullAttribute

            if (!File.Exists(filePath) || !File.GetAttributes(filePath).HasFlag(FileAttributes.ReadOnly))
            {
                return;
            }
        
            try
            {
                new FileInfo(filePath) {IsReadOnly = false}.Refresh();
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to check out file " + filePath + Environment.NewLine + ex);
            }
        
            if (File.GetAttributes(filePath).HasFlag(FileAttributes.ReadOnly))
            {
                throw new Exception("File \"" + filePath + "\" is read only, please checkout the file before running");
            }
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

        public void WriteFileWithoutCustomizations(string outputFile, string language, CodeNamespace codenamespace, IServiceProvider serviceProvider, bool writeProxyAttrib = false)
        {
            serviceProvider.UpdateService<ICustomizeCodeDomService>(new CustomizeCodeDomServiceEmpty());
            EnsureFileIsAccessible(outputFile);
            CodeGenerationService_WriteFile(outputFile, language, codenamespace.OrderTypesByName(), serviceProvider, writeProxyAttrib);
        }

        public static void ClearMsUtilitiesCache()
        {
            var typeName = "Microsoft.PowerPlatform.Dataverse.ModelBuilderLib.Utility.Utilites,Microsoft.PowerPlatform.Dataverse.ModelBuilderLib";
            var fieldName = "_locatedKeys";
            var type = Type.GetType(typeName);
            if (type == null)
            {
                throw new Exception("Unable to lookup the type: " + typeName);
            }
            var field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
            if (field == null)
            {
                throw new Exception("Unable to lookup the field: " + typeName + " on type " + typeName);
            }
            var value = (Dictionary<string, object>)field.GetValue(null);
            if (value != null)
            {
                value.Clear();
            }
        }

        internal static string ApplicationName
        {
            get
            {
                try
                {
                    object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                    return customAttributes.Length != 0 ? ((AssemblyTitleAttribute)customAttributes[0]).Title : "Unknown Title";
                }
                catch
                {
                    return "Unknown Title";
                }
            }
        }
        internal static string ApplicationVersion
        {
            get
            {
                try
                {
                    object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true);
                    return customAttributes.Length != 0 ? ((AssemblyFileVersionAttribute)customAttributes[0]).Version : "Unknown Version";
                }
                catch
                {
                    return "Unknown Version";
                }
            }
        }
    }
}
