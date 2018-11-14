using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Crm.Services.Utility;
using Source.DLaB.Common;
using Source.DLaB.Common.VersionControl;

namespace DLaB.CrmSvcUtilExtensions
{
    public abstract class BaseCustomCodeGenerationService : ICodeGenerationService
    {
        public static bool UseTfsToCheckoutFiles { get; } = ConfigHelper.GetAppSettingOrDefault("UseTfsToCheckoutFiles", false);
        public static bool AddNewFilesToProject => ConfigHelper.GetAppSettingOrDefault("AddNewFilesToProject", false);
        public static bool RemoveRuntimeVersionComment => ConfigHelper.GetAppSettingOrDefault("RemoveRuntimeVersionComment", true);
        private static bool LoggingEnabled => ConfigHelper.GetAppSettingOrDefault("LoggingEnabled", false);
        public static IOrganizationMetadata Metadata { get; set; }
        public static IServiceProvider ServiceProvider { get; set; }

        protected virtual string CommandLineText => ConfigHelper.GetAppSettingOrDefault("EntityCommandLineText", string.Empty);
        protected virtual CodeUnit SplitByCodeUnit => CodeUnit.Class;
        protected abstract bool CreateOneFilePerCodeUnit { get; }

        private VsTfsSourceControlProvider Tfs { get; set; }
        protected ICodeGenerationService DefaultService { get; }

        protected BaseCustomCodeGenerationService(ICodeGenerationService service)
        {
            DefaultService = service;
        }

        #region ICodeGenerationService Members

        public CodeGenerationType GetTypeForAttribute(Microsoft.Xrm.Sdk.Metadata.EntityMetadata entityMetadata, Microsoft.Xrm.Sdk.Metadata.AttributeMetadata attributeMetadata, IServiceProvider services)
        {
            return GetTypeForAttributeInternal(entityMetadata, attributeMetadata, services);
        }

        public CodeGenerationType GetTypeForEntity(Microsoft.Xrm.Sdk.Metadata.EntityMetadata entityMetadata, IServiceProvider services)
        {
            return GetTypeForEntityInternal(entityMetadata, services);
        }

        public CodeGenerationType GetTypeForMessagePair(SdkMessagePair messagePair, IServiceProvider services)
        {
            return GetTypeForMessagePairInternal(messagePair, services);
        }

        public CodeGenerationType GetTypeForOption(Microsoft.Xrm.Sdk.Metadata.OptionSetMetadataBase optionSetMetadata, Microsoft.Xrm.Sdk.Metadata.OptionMetadata optionMetadata, IServiceProvider services)
        {
            return GetTypeForOptionInternal(optionSetMetadata, optionMetadata, services);
        }

        public CodeGenerationType GetTypeForOptionSet(Microsoft.Xrm.Sdk.Metadata.EntityMetadata entityMetadata, Microsoft.Xrm.Sdk.Metadata.OptionSetMetadataBase optionSetMetadata, IServiceProvider services)
        {
            return GetTypeForOptionSetInternal(entityMetadata, optionSetMetadata, services);
        }

        public CodeGenerationType GetTypeForRequestField(SdkMessageRequest request, SdkMessageRequestField requestField, IServiceProvider services)
        {
            return GetTypeForRequestFieldInternal(request, requestField, services);
        }

        public CodeGenerationType GetTypeForResponseField(SdkMessageResponse response, SdkMessageResponseField responseField, IServiceProvider services)
        {
            return GetTypeForResponseFieldInternal(response, responseField, services);
        }

        public void Write(IOrganizationMetadata organizationMetadata, string language, string outputFile, string outputNamespace, IServiceProvider services)
        {
            try
            {
                WriteInternal(organizationMetadata, language, Path.GetFullPath(outputFile), outputNamespace, services);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        #endregion // ICodeGenerationService Members

        #region Internal Implementations

        protected virtual CodeGenerationType GetTypeForAttributeInternal(Microsoft.Xrm.Sdk.Metadata.EntityMetadata entityMetadata, Microsoft.Xrm.Sdk.Metadata.AttributeMetadata attributeMetadata, IServiceProvider services)
        {
            return DefaultService.GetTypeForAttribute(entityMetadata, attributeMetadata, services);
        }

        protected virtual CodeGenerationType GetTypeForEntityInternal(Microsoft.Xrm.Sdk.Metadata.EntityMetadata entityMetadata, IServiceProvider services)
        {
            return DefaultService.GetTypeForEntity(entityMetadata, services);
        }

        protected virtual CodeGenerationType GetTypeForMessagePairInternal(SdkMessagePair messagePair, IServiceProvider services)
        {
            return DefaultService.GetTypeForMessagePair(messagePair, services);
        }

        protected virtual CodeGenerationType GetTypeForOptionInternal(Microsoft.Xrm.Sdk.Metadata.OptionSetMetadataBase optionSetMetadata, Microsoft.Xrm.Sdk.Metadata.OptionMetadata optionMetadata, IServiceProvider services)
        {
            return DefaultService.GetTypeForOption(optionSetMetadata, optionMetadata, services);
        }

        protected virtual CodeGenerationType GetTypeForOptionSetInternal(Microsoft.Xrm.Sdk.Metadata.EntityMetadata entityMetadata, Microsoft.Xrm.Sdk.Metadata.OptionSetMetadataBase optionSetMetadata, IServiceProvider services)
        {
            return DefaultService.GetTypeForOptionSet(entityMetadata, optionSetMetadata, services);
        }

        protected virtual CodeGenerationType GetTypeForRequestFieldInternal(SdkMessageRequest request, SdkMessageRequestField requestField, IServiceProvider services)
        {
            return DefaultService.GetTypeForRequestField(request, requestField, services);
        }

        protected virtual CodeGenerationType GetTypeForResponseFieldInternal(SdkMessageResponse response, SdkMessageResponseField responseField, IServiceProvider services)
        {
            return DefaultService.GetTypeForResponseField(response, responseField, services);
        }

        protected virtual void WriteInternal(IOrganizationMetadata organizationMetadata, string language, string outputFile, string targetNamespace, IServiceProvider services)
        {
            Metadata = organizationMetadata;
            ServiceProvider = services;
            if (UseTfsToCheckoutFiles)
            {
                Tfs = new VsTfsSourceControlProvider(null, new ProcessExecutorInfo
                {
                    OnOutputReceived = Log,
                    OnErrorReceived = DisplayMessage
                });
            }
            DisplayMessage("Ensuring Context File is Accessible");
            EnsureFileIsAccessible(outputFile);

            Log("Creating Temp file");
            var tempFile = Path.GetTempFileName();
            Log("File " + tempFile + " Created");
            
            // Write the file out as normal
            DisplayMessage($"Writing file {Path.GetFileName(outputFile)} to {tempFile}");
            DefaultService.Write(organizationMetadata, language, tempFile, targetNamespace, services);
            Log("Completed writing file {0} to {1}", Path.GetFileName(outputFile), tempFile);

            // Check if the Header needs to be updated and or the file needs to be split
            if (!string.IsNullOrWhiteSpace(CommandLineText) || RemoveRuntimeVersionComment)
            {
                var lines = GetFileTextWithUpdatedClassComment(tempFile, CommandLineText, RemoveRuntimeVersionComment);
                if (CreateOneFilePerCodeUnit)
                {
                    DisplayMessage($"Splitting File {outputFile} By Code Unit");
                    SplitFileByCodeUnit(SplitByCodeUnit, outputFile, lines);
                }
                else
                {
                    DisplayMessage($"Updating File {outputFile}");
                    File.WriteAllLines(outputFile, lines);
                    if (UndoCheckoutIfUnchanged(outputFile))
                    {
                        Console.WriteLine(outputFile + " was unchanged.");
                    }
                }
            }
            else if (CreateOneFilePerCodeUnit)
            {
                DisplayMessage($"Splitting File {outputFile} By Code Unit");
                SplitFileByCodeUnit(SplitByCodeUnit, outputFile, File.ReadAllLines(tempFile));
            }
            else
            {
                DisplayMessage($"Copying File {outputFile}");
                File.Copy(tempFile, outputFile, true);
                if (UndoCheckoutIfUnchanged(outputFile))
                {
                    Console.WriteLine(outputFile + " was unchanged.");
                }
            }

            DisplayMessage($"Cleaning up Temporary File {tempFile}");
            File.Delete(tempFile);
            Log("Completed cleaning up Temporary File");
            DisplayMessage(tempFile + " Moved To: " + outputFile);
        }

        #endregion // Internal Implementations

        #region Insert Command Line Into Header

        protected IEnumerable<string> GetFileTextWithUpdatedClassComment(string filePath, string commandLineText, bool removeRuntimeVersionComment)
        {
            var skipLine = removeRuntimeVersionComment ? 3 : -1;
            commandLineText = string.IsNullOrWhiteSpace(commandLineText)
                ? ""
                : "// Created via this command line: " + commandLineText;
            return GetNewLines(File.ReadAllLines(filePath), 8, commandLineText, skipLine);
        }

        private IEnumerable<string> GetNewLines(string[] lines, int insertAtLine, string text, int skipLine)
        {
            for (var i = 0; i < insertAtLine; i++)
            {
                if (skipLine == i) { continue; }
                yield return lines[i];
            }
            yield return text;
            for (var i = insertAtLine; i < lines.Length; i++)
            {
                if (skipLine == i) { continue; }
                yield return lines[i];
            }
        }

        #endregion // Insert Command Line Into Header

        #region Checkout File / Remove Readonly Flag

        protected void EnsureFileIsAccessible(string filePath)
        {
            Log("Checking accessibility for file: " + filePath);
            // ReSharper disable AssignNullToNotNullAttribute
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                return;
            }
            // ReSharper restore AssignNullToNotNullAttribute

            if(!File.Exists(filePath) || !File.GetAttributes(filePath).HasFlag(FileAttributes.ReadOnly)) { return; }

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

        private void SplitFileByCodeUnit(CodeUnit codeUnit, string filePath, IEnumerable<string> lines)
        {
            var directory = Path.GetDirectoryName(filePath) ?? string.Empty;
            var currentStage = SplitStage.Header;
            var header = new List<string>();
            var code = new List<string>();
            var name = string.Empty;
            var proxyTypesAssemblyAttributeLine = string.Empty;
            var skipNext = false;
            var commandLine = string.Empty;
            var codeUnitStartsWith = codeUnit == CodeUnit.Class ? "public partial class" : "public enum";
            var files = new List<FileToWrite>(); // Delay this to the end to multithread the creation.  100's of small files takes a long time if checking with TFS sequentially

            foreach (var line in lines)
            {
                if (skipNext) { skipNext = false; continue; }

                switch (currentStage)
                {
                    case SplitStage.Header:
                        if (string.IsNullOrEmpty(line))
                        {
                            currentStage = SplitStage.Namespace;
                            header.Add(line);
                        }
                        else if (line.StartsWith(@"// Created via this command line:"))
                        {
                            // Since we are splitting the files, we don't want to put the command line in each file.  It would show every file as having been updated each time you change the path or user
                            commandLine = line;
                        }
                        else
                        {
                            header.Add(line);
                        }
                        break;

                    case SplitStage.Namespace:
                        if (header.Last().StartsWith("namespace "))
                        {
                            currentStage = SplitStage.CodeUnitHeader;
                            skipNext = true;
                        }
                        if (line.Contains("ProxyTypesAssemblyAttribute"))
                        {
                            proxyTypesAssemblyAttributeLine = line;
                            skipNext = true;
                            continue;
                        }
                        header.Add(line);
                        break;

                    case SplitStage.CodeUnitHeader:
                        if (line.TrimStart().StartsWith(codeUnitStartsWith))
                        {
                            name = GetName(codeUnit, line);
                            if (line.Contains(": Microsoft.Xrm.Sdk.Client.OrganizationServiceContext") || // Standard
                                line.Contains(": Microsoft.Xrm.Client.CrmOrganizationServiceContext")) // Xrm Client
                            {
                                // Put Created Via Command Line Back in
                                header.Insert(header.IndexOf(@"// </auto-generated>")+1, commandLine);
                                commandLine = string.Empty;
                                // Put Proxy Types Assembly Attribute Line back in
                                var i = header.IndexOf(string.Empty, 0) + 1;
                                header.Insert(i++, proxyTypesAssemblyAttributeLine);
                                header.Insert(i, string.Empty);
                                currentStage = SplitStage.ServiceContext;
                            }
                            else
                            {
                                currentStage = SplitStage.CodeUnit;
                            }
                        }
                        code.Add(line);
                        break;

                    case SplitStage.CodeUnit:
                        code.Add(line);
                        if (line == "\t}")
                        {
                            code.Add("}");
                            var fileName = Path.Combine(directory, name + ".cs");
                            files.Add(new FileToWrite(fileName, string.Join(Environment.NewLine, header.Concat(code))));
                            code.Clear();
                            currentStage = SplitStage.CodeUnitHeader;
                        }
                        break;

                    case SplitStage.ServiceContext:
                        code.Add(line);
                        break;

                    default:
                        throw new Exception("No Enum Defined");
                }
            }

            files.Add(new FileToWrite(filePath, string.IsNullOrWhiteSpace(commandLine) ? string.Join(Environment.NewLine, header.Concat(code)) : commandLine, true));

            WriteFilesAsync(files);
        }

        private void WriteFilesAsync(List<FileToWrite> files)
        {
            var project = new ProjectFile(files, AddNewFilesToProject, Tfs);

            if (UseTfsToCheckoutFiles)
            {
                DisplayMessage("Creating Required Directories");
                foreach (var dir in files.Select(f => f.Directory).Distinct())
                {
                    if (dir != null && !Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                }
                DisplayMessage("Getting Latest from TFS");
                // Get Latest Version of all files
                foreach (var batch in files.Select(f => f.Path).Batch(50))
                {
                    Tfs.Get(true, batch.ToArray());
                }

                if (Debugger.IsAttached)
                {
                    DisplayMessage("Creating Temporary Files");
                    foreach (var file in files)
                    {
                        WriteFilesForTfs(project, file);
                    }
                    CheckoutChangedFiles(files);
                    DisplayMessage("Updating Changed Files");
                    foreach (var file in files)
                    {
                        CopyChangedFiles(file);
                    }
                }
                else
                {
                    DisplayMessage("Creating Temporary Files");
                    Parallel.ForEach(files, f => WriteFilesForTfs(project, f));
                    CheckoutChangedFiles(files);
                    DisplayMessage("Updating Changed Files");
                    Parallel.ForEach(files, CopyChangedFiles);
                }
            }
            else
            {
                if (Debugger.IsAttached)
                {
                    foreach (var file in files)
                    {
                        WriteFileIfDifferent(project, file);
                    }
                }
                else
                {
                    Parallel.ForEach(files, f => WriteFileIfDifferent(project, f));
                }
            }

            if (AddNewFilesToProject && !project.ProjectFound)
            {
                Log("Unable to find a Project file to add newly created files to.  Either output the files into a directory that has a single project in it's path, or uncheck the \"Add New Files to Project\" Setting");
            }

            if (project.ProjectUpdated)
            {
                WriteFileIfDifferent(new ProjectFile(null, false, null), new FileToWrite(project.ProjectPath, project.GetContents()));
            }
        }
        private void CheckoutChangedFiles(List<FileToWrite> files)
        {
            DisplayMessage("Checking out Changed Files From TFS");
            Tfs.Checkout(files.Where(f => f.HasChanged && f.TempFile != null).Select(f => f.Path).ToArray());
        }

        private void WriteFilesForTfs(ProjectFile project, FileToWrite file)
        {
            Log("Processing file: " + file.Path);
            if (File.Exists(file.Path))
            {
                var tfsFile = File.ReadAllText(file.Path);
                if (tfsFile.Equals(file.Contents))
                {
                    Log(tfsFile);
                    Log(file.Contents);
                    Log(file.Path + " was unchanged.");
                    return;
                }
                file.TempFile = Path.Combine(file.Directory, "zzzTempEarlyBoundGenerator." + Path.GetFileName(file.Path) + ".tmp");
                Log("Creating Temp File " + file.TempFile);
                File.WriteAllText(file.TempFile, file.Contents);
                file.HasChanged = true;
            }
            else
            {
                File.WriteAllText(file.Path, file.Contents);
                Tfs.Add(file.Path);
                Console.WriteLine(file.Path + " created.");
                project.AddFileIfMissing(file.Path);
            }
        }

        private void CopyChangedFiles(FileToWrite file)
        {
            if (file.TempFile == null)
            {
                return;                
            }
            File.Copy(file.TempFile, file.Path, true);
            File.Delete(file.TempFile);
        }

        private void WriteFileIfDifferent(ProjectFile project, FileToWrite file)
        {
            Log("Processing file: " + file.Path);
            if (UseTfsToCheckoutFiles)
            {
                if (File.Exists(file.Path))
                {
                    Trace.TraceInformation(Path.GetFileName(file.Path) + " Checking out and updating if different.");
                    var tempFile = Path.Combine(file.Directory, "zzzTempEarlyBoundGenerator." + Path.GetFileName(file.Path) + ".tmp");
                    try
                    {
                        Log("Creating Temp File " + tempFile);
                        File.WriteAllText(tempFile, file.Contents);
                        var hasChanged = Tfs.AreDifferent(file.Path, tempFile);
                        if (hasChanged)
                        {
                            Console.WriteLine($"{file.Path} was changed.  Checking Out from TFS.");
                            Tfs.Checkout(file.Path);
                            Log("Updating File locally");
                            File.Copy(tempFile, file.Path, true);

                        }
                        var message = file.Path + $" was {(hasChanged ? "" : "un")}changed.";
                        if (file.IsMainFile)
                        {
                            Console.WriteLine(message);
                        }
                        else
                        {
                            Log(message);
                        }
                    }
                    finally
                    {
                        File.Delete(tempFile);
                    }
                }
                else
                {
                    File.WriteAllText(file.Path, file.Contents);
                    Tfs.Add(file.Path);
                    Console.WriteLine(file.Path + " created.");
                    project.AddFileIfMissing(file.Path);
                }
                return;
            }

            EnsureFileIsAccessible(file.Path);
            project.AddFileIfMissing(file.Path);

            Trace.TraceInformation(Path.GetFileName(file.Path) + " created / updated.");
            Log("Writing file: " + file.Path);
            File.WriteAllText(file.Path, file.Contents);
            Log("Completed file: " + file);
        }

        private string GetName(CodeUnit codeUnit, string line)
        {
            int start;
            int end;
            if (codeUnit == CodeUnit.Class)
            {
                start = 22; // "\tpublic partial class ".Length
                end = line.IndexOf(':', start)-1;
            }
            else // if (codeUnit == CodeUnit.Enum)
            {
                start = 13; // "\tpublic enum ".Length
                end = line.Length;
            }

            return line.Substring(start, end - start);
        }

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

        enum SplitStage
        {
            Header,
            CodeUnit,
            CodeUnitHeader,
            Namespace,
            ServiceContext,
        }

        protected enum CodeUnit
        {
            Class,
            Enum
        }

        private class FileToWrite
        {
            public string Directory => System.IO.Path.GetDirectoryName(Path);
            public string Path { get; private set; }
            public string Contents { get; private set; }
            public bool IsMainFile { get; private set; }
            public bool HasChanged { get; set; }
            public string TempFile { get; set; }

            public FileToWrite(string path, string contents, bool isMainFile = false)
            {
                Path = path;
                Contents = contents;
                IsMainFile = isMainFile;
                HasChanged = true;
            }
        }

        private class ProjectFile
        {
            private List<string> Lines { get; set; }
            public bool ProjectFound { get; private set; }
            public string ProjectPath { get; private set; }
            private string ProjectDir { get; set; }
            private int ProjectFileIndexStart { get; set; }
            private int ProjectFileIndexEnd { get; set; }
            private bool UpdateProjectFile { get; set; }
            private SortedDictionary<string, string> ProjectFiles { get; set; }
            internal bool ProjectUpdated { get; private set; }
            private string LineFormat { get; set; }
            private VsTfsSourceControlProvider Tfs { get; set; }

            // ReSharper disable once SuggestBaseTypeForParameter
            public ProjectFile(List<FileToWrite> files, bool updateProjectFile, VsTfsSourceControlProvider tfs)
            {
                if (updateProjectFile && files.Count > 0)
                {
                    ProjectFiles = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    // ReSharper disable once AssignNullToNotNullAttribute
                    var file = GetProjectPath(new DirectoryInfo(files[0].Directory));
                    ProjectFound = file != null;
                    if (file == null)
                    {
                        return;
                    }
                    ProjectPath = file.FullName;
                    Lines = File.ReadAllLines(ProjectPath).ToList();
                    ProjectDir = Path.GetDirectoryName(ProjectPath);
                    if (!Lines.Any(l => l.Contains("<Compile Include=")))
                    {
                        ProjectFileIndexStart = Lines.FindLastIndex(l => l.Contains("</PropertyGroup>"))+1;
                        Lines.Insert(ProjectFileIndexStart, "</ItemGroup>");
                        Lines.Insert(ProjectFileIndexStart, "<ItemGroup>");
                        ProjectFileIndexStart++;
                    }
                    else
                    {
                        ProjectFileIndexStart = Lines.FindIndex(l => l.Contains("<Compile Include="));
                    }
                    foreach (var line in Lines.Skip(ProjectFileIndexStart).TakeWhile(l => l.Contains("<Compile Include=")))
                    {
                        ProjectFiles.Add(line, line);
                    }
                    ProjectFileIndexEnd = ProjectFileIndexStart + ProjectFiles.Count;

                    // Determine Line Format, defaulting if none currently exist
                    var first = ProjectFiles.Keys.FirstOrDefault() ?? (ProjectPath.EndsWith("projitems") 
                                    ? "    <Compile Include=\"$(MSBuildThisFileDirectory)\" />"
                                    : "    <Compile Include=\"\" />");

                    var startEndIndex = first.Contains("$(") 
                        ? first.IndexOf(")", first.IndexOf("$(", StringComparison.Ordinal), StringComparison.Ordinal) + 1 // Path contains Ms Build Variable
                        : first.IndexOf("\"", StringComparison.Ordinal) + 1;
                    LineFormat = first.Substring(0, startEndIndex) + "{0}" + first.Substring(first.LastIndexOf("\"", StringComparison.Ordinal), first.Length - first.LastIndexOf("\"", StringComparison.Ordinal));
                }

                UpdateProjectFile = updateProjectFile;
                ProjectUpdated = false;
                Tfs = tfs;
            }

            private FileInfo GetProjectPath(DirectoryInfo directory)
            {
                while (true)
                {
                    if (directory == null)
                    {
                        return null;
                    }

                    var firstOrDefault = directory.GetFiles("*.csproj").FirstOrDefault() ?? directory.GetFiles("*.projitems").FirstOrDefault();
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

                if (!UpdateProjectFile || !ProjectFound)
                {
                    return;
                }

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
                if (UseTfsToCheckoutFiles)
                {
                    Tfs.Add(path);
                }
                
                Console.WriteLine(path + " added to project.");
                ProjectUpdated = true;
            }

            internal string GetContents()
            {
                // Return lines before, plus ordered compile files, plus lines after
                return string.Join(Environment.NewLine, Lines.Take(ProjectFileIndexStart).Concat(ProjectFiles.Keys).Concat(Lines.Skip(ProjectFileIndexEnd)));
            }

            // ReSharper disable once UnusedMember.Local
            private void Log(string log)
            {
                if (LoggingEnabled)
                {
                    Console.WriteLine(DateTime.Now.ToString("hh:MM:ss:fff") + ": " + log);
                }
            }

            private void Log(string logFormat, params object[] args)
            {
                if (LoggingEnabled)
                {
                    Console.WriteLine(DateTime.Now.ToString("hh:MM:ss:fff") + ": " + logFormat, args);
                }
            }
        }
        #endregion // Split File Into Multiple Files By Class
    }
}
