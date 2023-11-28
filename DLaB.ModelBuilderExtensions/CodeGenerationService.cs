using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using DLaB.Common;
using DLaB.Common.VersionControl;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Path = System.IO.Path;

namespace DLaB.ModelBuilderExtensions
{
    public class CodeGenerationService : TypedServiceBase<ICodeGenerationService>, ICodeGenerationService
    {
        public bool UseTfsToCheckoutFiles { get; } = false; //ConfigHelper.GetAppSettingOrDefault("UseTfsToCheckoutFiles", false);

        public bool AddNewFilesToProject { get => DLaBSettings.AddNewFilesToProject; set => DLaBSettings.AddNewFilesToProject = value; }
        public bool CleanupCrmSvcUtilLocalOptionSets { get => DLaBSettings.CleanupCrmSvcUtilLocalOptionSets && !DeleteFilesFromOutputFolders; set => DLaBSettings.CleanupCrmSvcUtilLocalOptionSets = value; }
        public bool CreateOneFilePerMessage { get => DLaBSettings.CreateOneFilePerMessage; set => DLaBSettings.CreateOneFilePerMessage = value; }
        public bool CreateOneFilePerEntity { get => DLaBSettings.CreateOneFilePerEntity; set => DLaBSettings.CreateOneFilePerEntity = value; }
        public bool CreateOneFilePerOptionSet { get => DLaBSettings.CreateOneFilePerOptionSet; set => DLaBSettings.CreateOneFilePerOptionSet = value; }
        public bool DeleteFilesFromOutputFolders { get => DLaBSettings.DeleteFilesFromOutputFolders; set => DLaBSettings.DeleteFilesFromOutputFolders = value; }
        public string EntitiesFileName { get => DLaBSettings.EntitiesFileName; set => DLaBSettings.EntitiesFileName = value; }
        public string EntityTypesFolder { get => Settings.EntityTypesFolder; set => Settings.EntityTypesFolder = value; }
        public string FilePrefixText { get => DLaBSettings.FilePrefixText; set => DLaBSettings.FilePrefixText = value; }
        public string ServiceContextName { get => Settings.ServiceContextName; set => Settings.ServiceContextName = value; }
        public bool GenerateActions { get => Settings.GenerateActions; set => Settings.GenerateActions = value; }
        public bool GroupMessageRequestWithResponse { get => DLaBSettings.GroupMessageRequestWithResponse; set => DLaBSettings.GroupMessageRequestWithResponse = value; }
        public bool GroupLocalOptionSetsByEntity { get => DLaBSettings.GroupLocalOptionSetsByEntity && CreateOneFilePerOptionSet; set => DLaBSettings.GroupLocalOptionSetsByEntity = value; }
        public bool LoggingEnabled { get => DLaBSettings.LoggingEnabled; set => DLaBSettings.LoggingEnabled = value; }
        public string MessagesFileName { get => DLaBSettings.MessagesFileName; set => DLaBSettings.MessagesFileName = value; }
        public string MessageTypesFolder { get => Settings.MessagesTypesFolder; set => Settings.MessagesTypesFolder = value; }
        public string OptionSetsFileName { get => DLaBSettings.OptionSetsFileName; set => DLaBSettings.OptionSetsFileName = value; }
        public string OptionSetTypesFolder { get => Settings.OptionSetsTypesFolder; set => Settings.OptionSetsTypesFolder = value; }
        public string OutDirectory { get => Settings.OutDirectory; set => Settings.OutDirectory = value; }
        public string ProjectNameForEarlyBoundFiles { get => DLaBSettings.ProjectNameForEarlyBoundFiles; set => DLaBSettings.ProjectNameForEarlyBoundFiles = value; }

        public bool RemoveRuntimeVersionComment { get => DLaBSettings.RemoveRuntimeVersionComment; set => DLaBSettings.RemoveRuntimeVersionComment = value; }

        private VsTfsSourceControlProvider Tfs { get; set; }

        public CodeGenerationService(ICodeGenerationService defaultService, IDictionary<string, string> parameters) : base(defaultService, parameters)
        {
        }

        public CodeGenerationService(ICodeGenerationService defaultService, DLaBModelBuilderSettings settings = null) : base(defaultService, settings)
        {
        }

        #region ICodeGenerationService Members

        public CodeGenerationType GetTypeForAttribute(Microsoft.Xrm.Sdk.Metadata.EntityMetadata entityMetadata, Microsoft.Xrm.Sdk.Metadata.AttributeMetadata attributeMetadata, IServiceProvider services)
        {
            SetServiceCache(services);
            return DefaultService.GetTypeForAttribute(entityMetadata, attributeMetadata, services);
        }

        public CodeGenerationType GetTypeForEntity(Microsoft.Xrm.Sdk.Metadata.EntityMetadata entityMetadata, IServiceProvider services)
        {
            SetServiceCache(services);
            return DefaultService.GetTypeForEntity(entityMetadata, services);
        }

        public CodeGenerationType GetTypeForMessagePair(SdkMessagePair messagePair, IServiceProvider services)
        {
            SetServiceCache(services);
            return DefaultService.GetTypeForMessagePair(messagePair, services);
        }

        public CodeGenerationType GetTypeForOption(Microsoft.Xrm.Sdk.Metadata.OptionSetMetadataBase optionSetMetadata, Microsoft.Xrm.Sdk.Metadata.OptionMetadata optionMetadata, IServiceProvider services)
        {
            SetServiceCache(services);
            return DefaultService.GetTypeForOption(optionSetMetadata, optionMetadata, services);
        }

        public CodeGenerationType GetTypeForOptionSet(Microsoft.Xrm.Sdk.Metadata.EntityMetadata entityMetadata, Microsoft.Xrm.Sdk.Metadata.OptionSetMetadataBase optionSetMetadata, IServiceProvider services)
        {
            SetServiceCache(services);
            return DefaultService.GetTypeForOptionSet(entityMetadata, optionSetMetadata, services);
        }

        public CodeGenerationType GetTypeForRequestField(SdkMessageRequest request, SdkMessageRequestField requestField, IServiceProvider services)
        {
            SetServiceCache(services);
            return DefaultService.GetTypeForRequestField(request, requestField, services);
        }

        public CodeGenerationType GetTypeForResponseField(SdkMessageResponse response, SdkMessageResponseField responseField, IServiceProvider services)
        {
            SetServiceCache(services);
            return DefaultService.GetTypeForResponseField(response, responseField, services);
        }

        public void Write(IOrganizationMetadata organizationMetadata, string language, string outputFile, string outputNamespace, IServiceProvider services)
        {
            try
            {
                SetServiceCache(services);
                WriteInternal(organizationMetadata, language, outputFile, outputNamespace, services);
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                throw;
            }
            finally
            {
                CamelCaser.ClearCache();
                ConfigHelper.ClearCache();
                ServiceCache.ClearCache();
                PacModelBuilderCodeGenHack.ClearMsUtilitiesCache();
            }
        }

        #endregion // ICodeGenerationService Members

        protected virtual void WriteInternal(IOrganizationMetadata organizationMetadata, string language, string outputFile, string targetNamespace, IServiceProvider services)
        {
            if (OutDirectory == null)
            {
                throw new ArgumentNullException(nameof(OutDirectory));
            }

            if (UseTfsToCheckoutFiles)
            {
                Tfs = new VsTfsSourceControlProvider(null, new ProcessExecutorInfo
                {
                    OnOutputReceived = Log,
                    OnErrorReceived = DisplayMessage
                });
            }
            
            DeleteExistingFiles();

            var timePriorToFileGeneration = DateTime.Now;
            // Write the files out as normal
            var hack = new PacModelBuilderCodeGenHack(Settings, DefaultService, true, false);
            hack.Write(organizationMetadata, language, outputFile, targetNamespace, services);

            CleanupLocalOptionSets(hack, timePriorToFileGeneration);
            ConditionallyCombineFiles(language, hack);
            ConditionallySplitMessageFiles(language, hack);
            UpdateProjectFile(hack.FilesWritten.Keys);


            // TODO: Move removal of Runtime Version to Code Gen
            // Handle 
            // Check if the Header needs to be updated and or the file needs to be split
            //if (RemoveRuntimeVersionComment)
            //{
            //    var lines = GetFileTextWithUpdatedClassComment(fileContents, CommandLineText, RemoveRuntimeVersionComment);
            //    if (CreateOneFilePerCodeUnit)
            //    {
            //        DisplayMessage($"Splitting File {outputFile} By Code Unit");
            //        SplitFileByCodeUnit(SplitByCodeUnit, outputFile, lines);
            //    }
            //    else
            //    {
            //        DisplayMessage($"Updating File {outputFile}");
            //        File.WriteAllLines(outputFile, new []{ GetFormattedPrefixText(outputFile) }.Concat(lines));
            //        if (UseTfsToCheckoutFiles && UndoCheckoutIfUnchanged(outputFile))
            //        {
            //            Console.WriteLine(outputFile + " was unchanged.");
            //        }
            //    }
            //}
            //else if (CreateOneFilePerCodeUnit)
            //{
            //    //TODO handle combining files
            //    DisplayMessage($"Splitting File {outputFile} By Code Unit");
            //    SplitFileByCodeUnit(SplitByCodeUnit, outputFile, File.ReadAllLines(tempFile));
            //}
            //else
            //{
            //    DisplayMessage($"Copying File {outputFile}");
            //    fileContents[0] = GetFormattedPrefixText(outputFile) + fileContents[0];
            //    File.WriteAllLines(outputFile, fileContents);
            //    if (UseTfsToCheckoutFiles && UndoCheckoutIfUnchanged(outputFile))
            //    {
            //        Console.WriteLine(outputFile + " was unchanged.");
            //    }
            //}
        }

        private void ConditionallySplitMessageFiles(string language, PacModelBuilderCodeGenHack hack)
        {
            if (!GenerateActions
                || !CreateOneFilePerMessage
                || GroupMessageRequestWithResponse)
            {
                return;
            }

            DisplayMessage("Splitting Message Request/Response classes into separate files.");
            foreach (var file in hack.FilesWritten.Where(kvp => kvp.Value.GetTypes().Any(t => t.IsMessageType())).ToList())
            {
                var code = file.Value;
                foreach (var type in file.Value.GetTypes().ToList())
                {
                    code.Types.Clear();
                    code.Types.Add(type);
                    hack.WriteFileWithoutCustomizations(Path.Combine(OutDirectory, MessageTypesFolder, type.Name + ".cs"), language, code, ServiceProvider);
                }

                File.Delete(file.Key);
                hack.FilesWritten.Remove(file.Key);
            }
        }
        #region Combine Files

        private void ConditionallyCombineFiles(string language, PacModelBuilderCodeGenHack hack)
        {
            if (!CreateOneFilePerEntity || !CreateOneFilePerOptionSet)
            {
                var optionSetsInEntities = ExtractOptionSetsAndConditionallyCombineEntities(language, hack);
                CombineOrWriteEntityOptionSets(language, hack, optionSetsInEntities);
            }

            if (GenerateActions && !CreateOneFilePerMessage)
            {
                CombineMessages(language, hack);
            }

            DeleteEmptyFolder(EntityTypesFolder);
            DeleteEmptyFolder(MessageTypesFolder);
            DeleteEmptyFolder(OptionSetTypesFolder);

            void DeleteEmptyFolder(string relativeFolder)
            {
                if (relativeFolder == null)
                {
                    return;
                }
                var entityFolder = Path.Combine(OutDirectory, relativeFolder);
                if (Directory.Exists(entityFolder) && !Directory.EnumerateFiles(entityFolder).Any())
                {
                    Directory.Delete(entityFolder);
                }
            }
        }

        private List<CodeTypeDeclaration> ExtractOptionSetsAndConditionallyCombineEntities(string language, PacModelBuilderCodeGenHack hack)
        {
            DisplayMessage(!CreateOneFilePerEntity ? "Combining entities into single file." : "Extracting OptionSets from entities.");
            var optionSetsInEntities = new List<CodeTypeDeclaration>();
            CodeNamespace code = null;
            foreach (var file in hack.FilesWritten.Where(kvp => kvp.Value.GetTypes().Any(t => t.IsEntityType())).ToList())
            {
                // Remove Option Sets from Entity
                var enums = file.Value.GetTypes().Where(t => t.IsEnum).OrderBy(t => t.Name).ToArray();
                foreach (var value in enums)
                {
                    file.Value.Types.Remove(value);
                }

                if (GroupLocalOptionSetsByEntity)
                {
                    var temp = file.Value;
                    var existing = temp.GetTypes();
                    temp.Types.Clear();
                    temp.Types.AddRange(enums);
                    hack.WriteFileWithoutCustomizations(Path.Combine(OutDirectory, OptionSetTypesFolder, Path.GetFileNameWithoutExtension(file.Key) + "Sets.cs"), language, temp, ServiceProvider);
                    temp.Types.Clear();
                    temp.Types.AddRange(existing.ToArray());
                }
                else
                {
                    optionSetsInEntities.AddRange(enums);
                }

                // ReSharper disable once AssignNullToNotNullAttribute
                File.Delete(file.Key);
                if (CreateOneFilePerEntity)
                {
                    // Recreate entity file without Option Sets
                    hack.WriteFileWithoutCustomizations(file.Key, language, file.Value, ServiceProvider);
                }
                else
                {
                    // Add entity types to single type to be written later
                    if (code == null)
                    {
                        code = file.Value;
                    }
                    else
                    {
                        code.Types.AddRange(file.Value.Types);
                    }

                    hack.FilesWritten.Remove(file.Key);
                }
            }

            if (code != null)
            {
                // The service Context will have the proxy types attribute.  If it's not being generated, it should be added to the entity file.
                var isServiceContextGenerated = !string.IsNullOrWhiteSpace(ServiceContextName);
                hack.WriteFileWithoutCustomizations(Path.Combine(OutDirectory, EntitiesFileName), language, code, ServiceProvider, !isServiceContextGenerated);
            }

            return optionSetsInEntities;
        }

        private void CombineOrWriteEntityOptionSets(string language, PacModelBuilderCodeGenHack hack, List<CodeTypeDeclaration> optionSetsInEntities)
        {
            var optionSetsFolder = Path.Combine(OutDirectory, OptionSetTypesFolder);
            if (CreateOneFilePerOptionSet)
            {
                DisplayMessage("Writing OptionSets from Entities.");
                var code = hack.FilesWritten.FirstOrDefault(IsOptionSetFile).Value
                           ?? hack.FilesWritten.First().Value.CloneShallow(); // Generate Global Option Sets is false, so grab a file to use as the template
                foreach (var optionSet in optionSetsInEntities)
                {
                    code.Types.Clear();
                    code.Types.Add(optionSet);
                    hack.WriteFileWithoutCustomizations(Path.Combine(optionSetsFolder, optionSet.Name + ".cs"), language, code, ServiceProvider);
                }
            }
            else
            {
                DisplayMessage("Combining OptionSets into single file.");
                CodeNamespace code = null;
                foreach (var file in hack.FilesWritten.Where(IsOptionSetFile).ToList())
                {
                    File.Delete(file.Key);
                    if (code == null)
                    {
                        code = file.Value;
                    }
                    else
                    {
                        code.Types.AddRange(file.Value.Types);
                    }
                }

                if (code != null)
                {
                    optionSetsInEntities.AddRange(code.GetTypes());
                    code.Types.Clear();
                    code.Types.AddRange(optionSetsInEntities.OrderBy(o => o.Name).ToArray());
                    hack.WriteFileWithoutCustomizations(Path.Combine(OutDirectory, OptionSetsFileName), language, code, ServiceProvider);
                }
            }

            bool IsOptionSetFile(KeyValuePair<string, CodeNamespace> kvp)
            {
                return string.Equals(Path.GetDirectoryName(kvp.Key), optionSetsFolder, StringComparison.CurrentCultureIgnoreCase) && kvp.Value.GetTypes().All(t => t.IsEnum);
            }
        }

        private void CombineMessages(string language, PacModelBuilderCodeGenHack hack)
        {
            DisplayMessage("Combining Messages into single file.");
            CodeNamespace code = null;
            foreach (var file in hack.FilesWritten.Where(kvp => kvp.Value.GetTypes().Any(t => t.IsMessageType())).ToList())
            {
                File.Delete(file.Key);
                if (code == null)
                {
                    code = file.Value;
                }
                else
                {
                    code.Types.AddRange(file.Value.Types);
                }
            }

            if (code != null)
            {
                hack.WriteFileWithoutCustomizations(Path.Combine(OutDirectory, MessagesFileName), language, code, ServiceProvider);
            }
        }

        #endregion Combine Files

        #region Cleanup Files

        private void CleanupLocalOptionSets(PacModelBuilderCodeGenHack hack, DateTime timePriorToFileGeneration)
        {
            if (!CleanupCrmSvcUtilLocalOptionSets)
            {
                return;
            }
            var optionSetFolder = Path.Combine(OutDirectory, OptionSetTypesFolder);
            var types = hack.FilesWritten.Values.SelectMany(v => v.GetTypes()).ToList();
            foreach (var enumType in types.Where(t => t.IsEnum).Select(t => t.Name))
            {
                var file = Path.Combine(optionSetFolder, enumType + ".cs");
                if (File.Exists(file) && File.GetLastWriteTime(file) < timePriorToFileGeneration)
                {
                    File.Delete(file);
                }
            }

            foreach (var entity in types.Where(t => t.IsEntityType()))
            {
                var file = Path.Combine(optionSetFolder, entity.Name + "Sets.cs");
                if (File.Exists(file) && File.GetLastWriteTime(file) < timePriorToFileGeneration)
                {
                    File.Delete(file);
                }
            }
        }

        private void UpdateProjectFile(IEnumerable<string> files)
        {
            if (!AddNewFilesToProject)
            {
                return;
            }

            var project = new ProjectFile(OutDirectory, this);
            if (!project.ProjectFound)
            {
                Log($"Project file {project.ProjectPath} not found in directory {OutDirectory}!");
                return;
            }

            foreach (var file in files)
            {
                if (File.Exists(file))
                {
                    project.AddFileIfMissing(file);
                }
            }

            project.RemoveMissingFiles();

            if (project.ProjectUpdated)
            {
                File.WriteAllText(project.ProjectPath, project.GetContents());
            }
        }

        //private string GetFormattedPrefixText(string outputFile)
        //{
        //    if (string.IsNullOrWhiteSpace(FilePrefixText))
        //    {
        //        return string.Empty;
        //    }
        //
        //    return string.Format(FilePrefixText, Path.GetFileName(outputFile)) + Environment.NewLine;
        //}

        private void DeleteExistingFiles()
        {
            if (!DeleteFilesFromOutputFolders)
            {
                return;
            }

            DeleteCSharpFilesInDirectory(Path.Combine(OutDirectory, EntityTypesFolder));
            if (GenerateActions)
            {
                DeleteCSharpFilesInDirectory(Path.Combine(OutDirectory, MessageTypesFolder));
            }
            DeleteCSharpFilesInDirectory(Path.Combine(OutDirectory, OptionSetTypesFolder));
        }

        private void DeleteCSharpFilesInDirectory(string directory, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            if (!Directory.Exists(directory))
            {
                return;
            }

            DisplayMessage($"Deleting *.cs Files From {directory} By Code Unit");
            foreach (var file in Directory.EnumerateFiles(directory, "*.cs", searchOption))
            {
                Log("Deleting file: " + file);
                File.Delete(file);
            }

            DisplayMessage($"Finished Deleting *.cs Files From {directory} By Code Unit");
        }

        #endregion Cleanup Files

        #region Insert Command Line Into Header

        //protected IEnumerable<string> GetFileTextWithUpdatedClassComment(string[] fileContents, string commandLineText, bool removeRuntimeVersionComment)
        //{
        //    var skipLine = removeRuntimeVersionComment ? 3 : -1;
        //    commandLineText = string.IsNullOrWhiteSpace(commandLineText)
        //        ? ""
        //        : "// Created via this command line: " + commandLineText.Replace(Environment.NewLine, Environment.NewLine + "//");
        //    return GetNewLines(fileContents, 8, commandLineText, skipLine);
        //}
        //
        //private IEnumerable<string> GetNewLines(string[] lines, int insertAtLine, string text, int skipLine)
        //{
        //    for (var i = 0; i < insertAtLine; i++)
        //    {
        //        if (skipLine == i) { continue; }
        //        yield return lines[i];
        //    }
        //    yield return text;
        //    for (var i = insertAtLine; i < lines.Length; i++)
        //    {
        //        if (skipLine == i) { continue; }
        //        yield return lines[i];
        //    }
        //}

        #endregion // Insert Command Line Into Header

        #region Checkout File / Remove Readonly Flag

        /// <summary>
        /// Returns true if the file was unchanged and an undo operation was performed
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        protected bool UndoCheckoutIfUnchanged(string fileName)
        {
            if (!UseTfsToCheckoutFiles)
            {
                return false;
            }

            Log("Checking accessibility for file " + fileName);

            return Tfs.UndoCheckoutIfUnchanged(fileName);
        }

        #endregion // Checkout File / Remove Readonly Flag

        #region Split File Into Multiple Files By Class

        //private void SplitFileByCodeUnit(CodeUnit codeUnit, string filePath, IEnumerable<string> lines)
        //{
        //    var directory = Path.GetDirectoryName(filePath) ?? string.Empty;
        //    var currentStage = SplitStage.Header;
        //    var header = new List<string>();
        //    var code = new List<string>();
        //    var name = string.Empty;
        //    var proxyTypesAssemblyAttributeLine = string.Empty;
        //    var generatedCodeAttributeLine = string.Empty;
        //    var skipNext = false;
        //    var commandLine = string.Empty;
        //    var codeUnitStartsWith = codeUnit == CodeUnit.Class ? "public partial class" : "public enum";
        //    var files = new List<FileToWrite>(); // Delay this to the end to multi-thread the creation.  100's of small files takes a long time if checking with TFS sequentially
        //
        //    foreach (var line in lines)
        //    {
        //        if (skipNext) { skipNext = false; continue; }
        //
        //        switch (currentStage)
        //        {
        //            case SplitStage.Header:
        //                if (string.IsNullOrEmpty(line))
        //                {
        //                    currentStage = SplitStage.Namespace;
        //                    header.Add(line);
        //                }
        //                else if (line.StartsWith(@"// Created via this command line:"))
        //                {
        //                    // Since we are splitting the files, we don't want to put the command line in each file.  It would show every file as having been updated each time you change the path or user
        //                    commandLine = line;
        //                }
        //                else
        //                {
        //                    header.Add(line);
        //                }
        //                break;
        //
        //            case SplitStage.Namespace:
        //                if (header.Last().StartsWith("namespace "))
        //                {
        //                    currentStage = SplitStage.CodeUnitHeader;
        //                    skipNext = true;
        //                }
        //                if (line.Contains("ProxyTypesAssemblyAttribute"))
        //                {
        //                    proxyTypesAssemblyAttributeLine = line;
        //                    continue;
        //                }
        //
        //                if (line.Contains("GeneratedCodeAttribute"))
        //                {
        //                    generatedCodeAttributeLine = line;
        //                    continue;
        //                }
        //                header.Add(line);
        //                break;
        //
        //            case SplitStage.CodeUnitHeader:
        //                if (line.TrimStart().StartsWith(codeUnitStartsWith))
        //                {
        //                    name = GetName(codeUnit, line);
        //                    if (line.Contains(": Microsoft.Xrm.Sdk.Client.OrganizationServiceContext") // Standard
        //                    {
        //                        // Put Created Via Command Line Back in
        //                        header.Insert(header.IndexOf(@"// </auto-generated>")+1, commandLine);
        //                        commandLine = string.Empty;
        //                        // Put Assembly Assembly Attribute Lines back in
        //                        var i = header.IndexOf(string.Empty, 0);
        //                        header.Insert(++i, proxyTypesAssemblyAttributeLine);
        //                        header.Insert(++i, generatedCodeAttributeLine);
        //                        currentStage = SplitStage.ServiceContext;
        //                    }
        //                    else
        //                    {
        //                        currentStage = SplitStage.CodeUnit;
        //                    }
        //                }
        //                code.Add(line);
        //                break;
        //
        //            case SplitStage.CodeUnit:
        //                code.Add(line);
        //                if (line == "\t}")
        //                {
        //                    code.Add("}");
        //                    var fileName = Path.Combine(directory, name + ".cs");
        //                    files.Add(new FileToWrite(fileName, string.Join(Environment.NewLine, header.Concat(code))));
        //                    code.Clear();
        //                    currentStage = SplitStage.CodeUnitHeader;
        //                }
        //                break;
        //
        //            case SplitStage.ServiceContext:
        //                code.Add(line);
        //                break;
        //
        //            default:
        //                throw new Exception("No Enum Defined");
        //        }
        //    }
        //
        //    var finalFileText = GetFormattedPrefixText(filePath) + (
        //        string.IsNullOrWhiteSpace(commandLine)
        //        ? string.Join(Environment.NewLine, header.Concat(code))
        //        : commandLine);
        //
        //    files.Add(new FileToWrite(filePath, finalFileText, true));
        //
        //    UpdateFilesToWrite(files);
        //
        //    WriteFilesAsync(files);
        //}

        //protected virtual void UpdateFilesToWrite(List<FileToWrite> files)
        //{
        //    //if (!GroupLocalOptionSetsByEntity)
        //    //{
        //    //    return;
        //    //}
        //
        //    var metadata = ServiceCache.MetadataForLocalEnumsByName;
        //    var groupFilesByEntity = new Dictionary<string, FileToWrite>();
        //    foreach (var file in files.ToArray())
        //    {
        //        var optionSetName = Path.GetFileNameWithoutExtension(file.Path) ?? string.Empty;
        //        if (!metadata.TryGetValue(optionSetName, out var localOptionSet))
        //        {
        //            continue;
        //        }
        //
        //        files.Remove(file);
        //        if (groupFilesByEntity.TryGetValue(localOptionSet.Item1, out var entityFile))
        //        {
        //            var closingNamespaceIndex = entityFile.Contents.LastIndexOf("}", StringComparison.InvariantCulture);
        //            var startingNamespaceIndex = file.Contents.IndexOf("{", StringComparison.InvariantCulture);
        //            var content = entityFile.Contents.Substring(0, closingNamespaceIndex) + file.Contents.Substring(startingNamespaceIndex + 1);
        //            groupFilesByEntity[localOptionSet.Item1] = new FileToWrite(entityFile.Path, content);
        //        }
        //        else
        //        {
        //            //groundFilesByEntity[localOptionSet.Item1] = new FileToWrite(Path.Combine(file.Directory, string.Format(LocalOptionSetEntityFileNameFormat, localOptionSet.Item1)), file.Contents);
        //        }
        //    }
        //
        //    files.AddRange(groupFilesByEntity.Values);
        //}
        //
        //private void WriteFilesAsync(List<FileToWrite> files)
        //{
        //    var project = new ProjectFile(files, this);
        //
        //    if (UseTfsToCheckoutFiles)
        //    {
        //        DisplayMessage("Creating Required Directories");
        //        foreach (var dir in files.Select(f => f.Directory).Distinct())
        //        {
        //            if (dir != null && !Directory.Exists(dir))
        //            {
        //                Directory.CreateDirectory(dir);
        //            }
        //        }
        //        DisplayMessage("Getting Latest from TFS");
        //        // Get Latest Version of all files
        //        foreach (var batch in files.Select(f => f.Path).Batch(50))
        //        {
        //            Tfs.Get(true, batch.ToArray());
        //        }
        //
        //        if (Debugger.IsAttached)
        //        {
        //            DisplayMessage("Creating Temporary Files");
        //            foreach (var file in files)
        //            {
        //                WriteFilesForTfs(project, file);
        //            }
        //            CheckoutChangedFiles(files);
        //            DisplayMessage("Updating Changed Files");
        //            foreach (var file in files)
        //            {
        //                CopyChangedFiles(file);
        //            }
        //        }
        //        else
        //        {
        //            DisplayMessage("Creating Temporary Files");
        //            Parallel.ForEach(files, f => WriteFilesForTfs(project, f));
        //            CheckoutChangedFiles(files);
        //            DisplayMessage("Updating Changed Files");
        //            Parallel.ForEach(files, CopyChangedFiles);
        //        }
        //    }
        //    else
        //    {
        //        if (Debugger.IsAttached)
        //        {
        //            foreach (var file in files)
        //            {
        //                WriteFileIfDifferent(project, file);
        //            }
        //        }
        //        else
        //        {
        //            Parallel.ForEach(files, f => WriteFileIfDifferent(project, f));
        //        }
        //    }
        //
        //    if (AddNewFilesToProject && !project.ProjectFound)
        //    {
        //        Log("Unable to find a Project file to add newly created files to.  Either output the files into a directory that has a single project in it's path, or uncheck the \"Add New Files to Project\" Setting");
        //    }
        //
        //    if (project.ProjectUpdated)
        //    {
        //        WriteFileIfDifferent(new ProjectFile(null, this), new FileToWrite(project.ProjectPath, project.GetContents()));
        //    }
        //}

        //private void CheckoutChangedFiles(List<FileToWrite> files)
        //{
        //    DisplayMessage("Checking out Changed Files From TFS");
        //    Tfs.Checkout(files.Where(f => f.HasChanged && f.TempFile != null).Select(f => f.Path).ToArray());
        //}
        //
        //private void WriteFilesForTfs(ProjectFile project, FileToWrite file)
        //{
        //    Log("Processing file: " + file.Path);
        //    if (File.Exists(file.Path))
        //    {
        //        var tfsFile = File.ReadAllText(file.Path);
        //        if (tfsFile.Equals(file.Contents))
        //        {
        //            Log(tfsFile);
        //            Log(file.Contents);
        //            Log(file.Path + " was unchanged.");
        //            return;
        //        }
        //        file.TempFile = Path.Combine(file.Directory, "zzzTempEarlyBoundGenerator." + Path.GetFileName(file.Path) + ".tmp");
        //        Log("Creating Temp File " + file.TempFile);
        //        File.WriteAllText(file.TempFile, file.Contents);
        //        file.HasChanged = true;
        //    }
        //    else
        //    {
        //        File.WriteAllText(file.Path, file.Contents);
        //        Tfs.Add(file.Path);
        //        Console.WriteLine(file.Path + " created.");
        //        project.AddFileIfMissing(file.Path);
        //    }
        //}
        //
        //private void CopyChangedFiles(FileToWrite file)
        //{
        //    if (file.TempFile == null)
        //    {
        //        return;                
        //    }
        //    File.Copy(file.TempFile, file.Path, true);
        //    File.Delete(file.TempFile);
        //}
        //
        //private void WriteFileIfDifferent(ProjectFile project, FileToWrite file)
        //{
        //    Log("Processing file: " + file.Path);
        //    if (UseTfsToCheckoutFiles)
        //    {
        //        if (File.Exists(file.Path))
        //        {
        //            Trace.TraceInformation(Path.GetFileName(file.Path) + " Checking out and updating if different.");
        //            var tempFile = Path.Combine(file.Directory, "zzzTempEarlyBoundGenerator." + Path.GetFileName(file.Path) + ".tmp");
        //            try
        //            {
        //                Log("Creating Temp File " + tempFile);
        //                File.WriteAllText(tempFile, file.Contents);
        //                var hasChanged = Tfs.AreDifferent(file.Path, tempFile);
        //                if (hasChanged)
        //                {
        //                    Console.WriteLine($"{file.Path} was changed.  Checking Out from TFS.");
        //                    Tfs.Checkout(file.Path);
        //                    Log("Updating File locally");
        //                    File.Copy(tempFile, file.Path ?? "Unknown", true);
        //
        //                }
        //                var message = file.Path + $" was {(hasChanged ? "" : "un")}changed.";
        //                if (file.IsMainFile)
        //                {
        //                    Console.WriteLine(message);
        //                }
        //                else
        //                {
        //                    Log(message);
        //                }
        //            }
        //            finally
        //            {
        //                File.Delete(tempFile);
        //            }
        //        }
        //        else
        //        {
        //            File.WriteAllText(file.Path, file.Contents);
        //            Tfs.Add(file.Path);
        //            Console.WriteLine(file.Path + " created.");
        //            project.AddFileIfMissing(file.Path);
        //        }
        //        return;
        //    }
        //
        //    //EnsureFileIsAccessible(file.Path);
        //    project.AddFileIfMissing(file.Path);
        //
        //    Trace.TraceInformation(Path.GetFileName(file.Path) + " created / updated.");
        //    Log("Writing file: " + file.Path);
        //    File.WriteAllText(file.Path ?? "Unknown", file.Contents);
        //    Log("Completed file: " + file);
        //}
        //
        //private string GetName(CodeUnit codeUnit, string line)
        //{
        //    int start;
        //    int end;
        //    if (codeUnit == CodeUnit.Class)
        //    {
        //        start = 22; // "\tpublic partial class ".Length
        //        end = line.IndexOf(':', start)-1;
        //    }
        //    else // if (codeUnit == CodeUnit.Enum)
        //    {
        //        start = 13; // "\tpublic enum ".Length
        //        end = line.Length;
        //    }
        //
        //    return line.Substring(start, end - start);
        //}

        /// <summary>
        /// Displays the message with a header always
        /// </summary>
        /// <param name="message">The message.</param>
        private void DisplayMessage(string message)
        {
            Console.WriteLine($"[**** {message} ****]");
        }

        protected void Log(string log)
        {
            if (LoggingEnabled)
            {
                Console.WriteLine(DateTime.Now.ToString("hh:MM:ss:fff") + ": " + log);
            }
        }

        protected void Log(string logFormat, params object[] args)
        {
            if (LoggingEnabled)
            {
                Console.WriteLine(DateTime.Now.ToString("hh:MM:ss:fff") + ": " + logFormat, args);
            }
        }

        //enum SplitStage
        //{
        //    Header,
        //    CodeUnit,
        //    CodeUnitHeader,
        //    Namespace,
        //    ServiceContext,
        //}
        //
        //protected enum CodeUnit
        //{
        //    Class,
        //    Enum
        //}

        private class ProjectFile
        {
            private List<string> Lines { get; }
            public bool ProjectFound { get; }
            public string ProjectPath { get; }
            private string ProjectDir { get; }
            private int ProjectFileIndexStart { get; }
            private int ProjectFileIndexEnd { get; }
            private SortedDictionary<string, string> ProjectFiles { get; }
            internal bool ProjectUpdated { get; private set; }
            private string LineFormat { get; }
            private CodeGenerationService CodeGenService { get; }
            private Dictionary<string, string> InitialFiles { get; }

            // ReSharper disable once SuggestBaseTypeForParameter
            public ProjectFile(string directory, CodeGenerationService codeGenerationService)
            {
                CodeGenService = codeGenerationService;
                InitialFiles = new Dictionary<string, string>();
                ProjectFiles = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                // ReSharper disable once AssignNullToNotNullAttribute
                var file = GetProjectPath(new DirectoryInfo(directory));
                ProjectFound = file != null;
                if (file == null)
                {
                    return;
                }

                ProjectPath = file.FullName;
                Lines = File.ReadAllLines(ProjectPath).ToList();
                CombineMultiLinedCompileStatements();

                ProjectDir = Path.GetDirectoryName(ProjectPath) ?? "NULL";
                if (!Lines.Any(l => l.Contains("<Compile Include=")))
                {
                    ProjectFileIndexStart = Lines.FindLastIndex(l => l.Contains("</PropertyGroup>")) + 1;
                    Lines.Insert(ProjectFileIndexStart, "</ItemGroup>");
                    Lines.Insert(ProjectFileIndexStart, "<ItemGroup>");
                    ProjectFileIndexStart++;
                }
                else
                {
                    ProjectFileIndexStart = Lines.FindIndex(l => l.Contains("<Compile Include="));
                }

                foreach (var line in Lines.Skip(ProjectFileIndexStart).TakeWhile(l => l.Contains("<Compile Include=") && !ProjectFiles.ContainsKey(l)))
                {
                    ProjectFiles.Add(line, line);
                    var start = line.IndexOf(')') > 0
                        ? ")"
                        : "\"";
                    InitialFiles.Add(Path.Combine(ProjectDir, line.SubstringByString(start, "\"")), line);
                }

                ProjectFileIndexEnd = ProjectFileIndexStart + ProjectFiles.Count;

                // Determine Line Format, defaulting if none currently exist
                // Shared project format: <Compile Include="$(MSBuildThisFileDirectory)Entities\Entities\Contact.cs" />
                // Standard project format: <Compile Include="Entities\Entities\Contact.cs" />
                var first = ProjectFiles.Keys.FirstOrDefault() ?? (ProjectPath.EndsWith("projitems")
                    ? "    <Compile Include=\"$(MSBuildThisFileDirectory)\" />"
                    : "    <Compile Include=\"\" />");

                var startEndIndex = first.Contains("$(")
                    ? first.IndexOf(")", first.IndexOf("$(", StringComparison.Ordinal), StringComparison.Ordinal) + 1 // Path contains Ms Build Variable
                    : first.IndexOf("\"", StringComparison.Ordinal) + 1;
                LineFormat = first.Substring(0, startEndIndex) + "{0}" + first.Substring(first.LastIndexOf("\"", StringComparison.Ordinal),
                    first.Length - first.LastIndexOf("\"", StringComparison.Ordinal));

                ProjectUpdated = false;
            }

            /// <summary>
            /// Moves all Compile sections that are multi-lined onto a single line
            /// </summary>
            private void CombineMultiLinedCompileStatements()
            {
                for (var i = 0; i < Lines.Count; i++)
                {
                    var line = Lines[i];
                    var trimmed = line.Trim();
                    if (trimmed.StartsWith("<Compile Include =")
                        && !trimmed.EndsWith("/>"))
                    {
                        var index = i;
                        i++;
                        while (i < Lines.Count
                               && !Lines[i].Contains("</Compile>"))
                        {
                            trimmed += Environment.NewLine + Lines[i];
                            Lines.RemoveAt(i);
                        }

                        if (i < Lines.Count)
                        {
                            trimmed += Environment.NewLine + Lines[i];
                            Lines.RemoveAt(i);
                        }

                        Lines[index] = trimmed;
                    }
                }
            }

            private FileInfo GetProjectPath(DirectoryInfo directory)
            {
                bool IsProjectFile(FileInfo fi)
                {
                    return string.IsNullOrWhiteSpace(CodeGenService.ProjectNameForEarlyBoundFiles) 
                           || fi.FullName.EndsWith(CodeGenService.ProjectNameForEarlyBoundFiles);
                }

                while (true)
                {
                    if (directory == null)
                    {
                        return null;
                    }

                    var firstOrDefault = directory.GetFiles("*.csproj").FirstOrDefault(IsProjectFile) 
                                         ?? directory.GetFiles("*.projitems").FirstOrDefault(IsProjectFile);
                    if (firstOrDefault != null)
                    {
                        return firstOrDefault;
                    }
                    directory = directory.Parent;
                }
            }

            private readonly object _dictionaryLock = new object();
            internal void AddFileIfMissing(string path)
            {
                path = Path.GetFullPath(path);

                Log("Checking if project \"{0}\" contains file \"{1}\"", ProjectPath, path);
                var line = string.Format(LineFormat, path.Substring(ProjectDir.Length + 1, path.Length - ProjectDir.Length - 1));
                lock (_dictionaryLock)
                {
                    if (ProjectFiles.ContainsKey(line))
                    {
                        Log("Project \"{0}\" contains file \"{1}\"", ProjectPath, path);
                        return;
                    }

                    Log("Project \"{0}\" does not contains file \"{1}\"", ProjectPath, path);
                    ProjectFiles.Add(line, line);
                }
                if (CodeGenService.UseTfsToCheckoutFiles)
                {
                    CodeGenService.Tfs.Add(path);
                }
                
                Console.WriteLine(path + " added to project.");
                ProjectUpdated = true;
            }

            internal void RemoveMissingFiles()
            {
                lock (_dictionaryLock)
                {
                    foreach (var missingFile in InitialFiles.Where(f => !File.Exists(f.Key)))
                    {
                        Log("Removing file \"{0}\" from Project \"{1}\"", missingFile.Key, ProjectPath);
                        ProjectFiles.Remove(missingFile.Value);
                        ProjectUpdated = true;
                    }
                }
            }

            internal string GetContents()
            {
                // Return lines before, plus ordered compile files, plus lines after
                return string.Join(Environment.NewLine, Lines.Take(ProjectFileIndexStart).Concat(ProjectFiles.Keys).Concat(Lines.Skip(ProjectFileIndexEnd)));
            }

            // ReSharper disable once UnusedMember.Local
#pragma warning disable IDE0051 // Remove unused private members
            private void Log(string log)
#pragma warning restore IDE0051 // Remove unused private members
            {
                if (CodeGenService.LoggingEnabled)
                {
                    Console.WriteLine(DateTime.Now.ToString("hh:MM:ss:fff") + ": " + log);
                }
            }

            private void Log(string logFormat, params object[] args)
            {
                if (CodeGenService.LoggingEnabled)
                {
                    Console.WriteLine(DateTime.Now.ToString("hh:MM:ss:fff") + ": " + logFormat, args);
                }
            }
        }
        #endregion // Split File Into Multiple Files By Class
    }
}
