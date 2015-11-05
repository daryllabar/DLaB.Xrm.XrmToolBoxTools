using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Crm.Services.Utility;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace DLaB.CrmSvcUtilExtensions
{
    public abstract class BaseCustomCodeGenerationService : ICodeGenerationService
    {
        public static bool UseTfsToCheckoutFiles => ConfigHelper.GetAppSettingOrDefault("UseTfsToCheckoutFiles", false);
        public static bool AddNewFilesToProject => ConfigHelper.GetAppSettingOrDefault("AddNewFilesToProject", false);
        public static bool RemoveRuntimeVersionComment => ConfigHelper.GetAppSettingOrDefault("RemoveRuntimeVersionComment", true);
        private static bool LoggingEnabled => ConfigHelper.GetAppSettingOrDefault("LoggingEnabled", false);

        protected virtual string CommandLineText => ConfigHelper.GetAppSettingOrDefault("EntityCommandLineText", string.Empty);
        protected virtual CodeUnit SplitByCodeUnit => CodeUnit.Class;
        protected abstract bool CreateOneFilePerCodeUnit { get; }
        protected Workspace TfsWorkspace { get; set; }

        private readonly ICodeGenerationService _defaultService;

        protected BaseCustomCodeGenerationService(ICodeGenerationService service)
        {
            _defaultService = service;
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

        public void Write(IOrganizationMetadata organizationMetadata, string language, string outputFile, string targetNamespace, IServiceProvider services)
        {
            try
            {
                WriteInternal(organizationMetadata, language, outputFile, targetNamespace, services);
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
            return _defaultService.GetTypeForAttribute(entityMetadata, attributeMetadata, services);
        }

        protected virtual CodeGenerationType GetTypeForEntityInternal(Microsoft.Xrm.Sdk.Metadata.EntityMetadata entityMetadata, IServiceProvider services)
        {
            return _defaultService.GetTypeForEntity(entityMetadata, services);
        }

        protected virtual CodeGenerationType GetTypeForMessagePairInternal(SdkMessagePair messagePair, IServiceProvider services)
        {
            return _defaultService.GetTypeForMessagePair(messagePair, services);
        }

        protected virtual CodeGenerationType GetTypeForOptionInternal(Microsoft.Xrm.Sdk.Metadata.OptionSetMetadataBase optionSetMetadata, Microsoft.Xrm.Sdk.Metadata.OptionMetadata optionMetadata, IServiceProvider services)
        {
            return _defaultService.GetTypeForOption(optionSetMetadata, optionMetadata, services);
        }

        protected virtual CodeGenerationType GetTypeForOptionSetInternal(Microsoft.Xrm.Sdk.Metadata.EntityMetadata entityMetadata, Microsoft.Xrm.Sdk.Metadata.OptionSetMetadataBase optionSetMetadata, IServiceProvider services)
        {
            return _defaultService.GetTypeForOptionSet(entityMetadata, optionSetMetadata, services);
        }

        protected virtual CodeGenerationType GetTypeForRequestFieldInternal(SdkMessageRequest request, SdkMessageRequestField requestField, IServiceProvider services)
        {
            return _defaultService.GetTypeForRequestField(request, requestField, services);
        }

        protected virtual CodeGenerationType GetTypeForResponseFieldInternal(SdkMessageResponse response, SdkMessageResponseField responseField, IServiceProvider services)
        {
            return _defaultService.GetTypeForResponseField(response, responseField, services);
        }

        protected virtual void WriteInternal(IOrganizationMetadata organizationMetadata, string language, string outputFile, string targetNamespace, IServiceProvider services)
        {
            if (UseTfsToCheckoutFiles)
            {
                var workspaceInfo = Workstation.Current.GetLocalWorkspaceInfo(outputFile);
                var server = new TfsTeamProjectCollection(workspaceInfo.ServerUri);
                TfsWorkspace = workspaceInfo.GetWorkspace(server);
            }

            EnsureFileIsAccessible(outputFile);

            Log("Creating Temp file");
            var tempFile = Path.GetTempFileName();
            Log("File " + tempFile + " Created");
            
            // Write the file out as normal
            Log("Writing file {0} to {1}", Path.GetFileName(outputFile), tempFile);
            _defaultService.Write(organizationMetadata, language, tempFile, targetNamespace, services);
            Log("Completed writing file {0} to {1}", Path.GetFileName(outputFile), tempFile);

            // Check if the Header needs to be updated and or the file needs to be split
            if (!string.IsNullOrWhiteSpace(CommandLineText) || RemoveRuntimeVersionComment)
            {
                var lines = GetFileTextWithUpdatedClassComment(tempFile, CommandLineText, RemoveRuntimeVersionComment);
                if (CreateOneFilePerCodeUnit)
                {
                    SplitFileByCodeUnit(SplitByCodeUnit, outputFile, lines);
                }
                else
                {
                    Log("Updating file " + outputFile);
                    File.WriteAllLines(outputFile, lines);
                    if (UndoCheckoutIfUnchanged(outputFile))
                    {
                        Console.WriteLine(outputFile + " was unchanged.");
                    }
                }
            }
            else if (CreateOneFilePerCodeUnit)
            {
                SplitFileByCodeUnit(SplitByCodeUnit, outputFile, File.ReadAllLines(tempFile));
            }
            else
            {
                File.Copy(tempFile, outputFile);
                if (UndoCheckoutIfUnchanged(outputFile))
                {
                    Console.WriteLine(outputFile + " was unchanged.");
                }
            }

            Log("Cleaning up Temporary File " + tempFile);
            File.Delete(tempFile);
            Log("Completed cleaning up Temporary File");

            Console.WriteLine(tempFile + " Moved To: " + outputFile);
        }

        #endregion // Internal Implementations

        #region Insert Command Line Into Header

        protected IEnumerable<string> GetFileTextWithUpdatedClassComment(string filePath, string commandLineText, bool removeRuntimeVersionComment)
        {
            var skipLine = removeRuntimeVersionComment ? 3 : -1;
            return GetNewLines(File.ReadAllLines(filePath), 8, "// Created via this command line: " + commandLineText, skipLine);
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

        private void CheckoutFile(string fileName)
        {
            TfsWorkspace.PendEdit(fileName);
        }

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

            if (!File.Exists(filePath) || 
                !File.GetAttributes(filePath).HasFlag(FileAttributes.ReadOnly)) { return; }

            try
            {
                if (UseTfsToCheckoutFiles)
                {
                    Log("Checking out from TFS file: " + filePath);
                    CheckoutFile(filePath);
                    Log("Check out complete for file: " + filePath);
                }
                else
                {
                    new FileInfo(filePath) {IsReadOnly = false}.Refresh();
                }
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

            var items = TfsWorkspace.VersionControlServer.GetItems(fileName, new WorkspaceVersionSpec(TfsWorkspace), RecursionType.None).Items;
            if (!items.Any())
            {
                // Most likely the item doesn't exist in TFS and so no need to undo checkout if unchanged
                return false;
            }
            var item = items[0];
            bool unchanged;

            using (var fs = File.OpenRead(fileName))
            {
                unchanged = item.HashValue.SequenceEqual(MD5.Create().ComputeHash(fs));
            }

            if (unchanged)
            {
                TfsWorkspace.Undo(fileName);
            }

            return unchanged;
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
            var files = new List<FileToCreate>(); // Delay this to the end to multithread the creation.  100's of small files takes a long time if checking with TFS sequentially

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
                            files.Add(new FileToCreate(fileName, string.Join(Environment.NewLine, header.Concat(code))));
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

            files.Add(new FileToCreate(filePath, string.IsNullOrWhiteSpace(commandLine) ? string.Join(Environment.NewLine, header.Concat(code)) : commandLine, true));

            WriteFilesAsync(files);
        }

        private void WriteFilesAsync(List<FileToCreate> files)
        {
            var project = new ProjectFile(files, AddNewFilesToProject, UseTfsToCheckoutFiles ? TfsWorkspace: null);

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

            if (AddNewFilesToProject && !project.ProjectFound)
            {
                Log("Unable to find a Project file to add newly created files to.  Either output the files into a directory that has a single project in it's path, or uncheck the \"Add New Files to Project\" Setting");
            }

            if (project.ProjectUpdated)
            {
                WriteFileIfDifferent(new ProjectFile(null, false, null), new FileToCreate(project.ProjectPath, project.GetContents()));
            }
        }

        private void SetSourceControlInfo (FileToCreate file)
        {
            if (!UseTfsToCheckoutFiles)
            {
                return;
            }

            var items = TfsWorkspace.VersionControlServer.GetItems(file.Path, new WorkspaceVersionSpec(TfsWorkspace), RecursionType.None).Items;
            if (items.Any())
            {
                file.TfsItem = items[0];
            }
            // else, Most likely the item doesn't exist in TFS and so nothing to add
        }

        private void WriteFileIfDifferent(ProjectFile project, FileToCreate file)
        {
            SetSourceControlInfo(file);
            if (!file.HasChanged)
            {
                if (file.IsMainFile)
                {
                    if (UseTfsToCheckoutFiles)
                    {
                        TfsWorkspace.Undo(file.Path);
                    }
                    Console.WriteLine(file.Path + " was unchanged.");
                }
                return;
            }

            EnsureFileIsAccessible(file.Path);
            project.AddFileIfMissing(file.Path);

            Trace.TraceInformation(Path.GetFileName(file.Path) + " created / updated..");
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

        private class FileToCreate
        {
            public string Path { get; private set; }
            public string Contents { get; private set; }
            public Item TfsItem { get; set; }
            public bool IsMainFile { get; private set; }

            public Boolean HasChanged
            {
                get
                {
                    return TfsItem == null || !TfsItem.HashValue.SequenceEqual(MD5.Create().ComputeHash(System.Text.Encoding.Default.GetBytes(Contents)));
                }
            }

            public FileToCreate(string path, string contents, bool isMainFile = false)
            {
                Path = path;
                Contents = contents;
                IsMainFile = isMainFile;
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
            private Workspace TfsWorkspace { get; set; }

            // ReSharper disable once SuggestBaseTypeForParameter
            public ProjectFile(List<FileToCreate> files, bool updateProjectFile, Workspace tfsWorkspace)
            {
                if (updateProjectFile && files.Count > 0)
                {
                    ProjectFiles = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    // ReSharper disable once AssignNullToNotNullAttribute
                    var file = GetProjectPath(new DirectoryInfo(Path.GetDirectoryName(files[0].Path)));
                    ProjectFound = file != null;
                    if (file == null)
                    {
                        return;
                    }
                    ProjectPath = file.FullName;
                    Lines = File.ReadAllLines(ProjectPath).ToList();
                    ProjectDir = Path.GetDirectoryName(ProjectPath);
                    ProjectFileIndexStart = Lines.FindIndex(l => l.Contains("<Compile Include="));
                    foreach (var line in Lines.Skip(ProjectFileIndexStart).TakeWhile(l => l.Contains("<Compile Include=")))
                    {
                        ProjectFiles.Add(line, line);
                    }
                    ProjectFileIndexEnd = ProjectFileIndexStart + ProjectFiles.Count;

                    // Determine Line Format, defaulting if none currently exist
                    var first = ProjectFiles.Keys.FirstOrDefault() ?? "    <Compile Include=\"\" />";
                    LineFormat = first.Substring(0, first.IndexOf("\"", StringComparison.Ordinal) + 1) + "{0}" + first.Substring(first.LastIndexOf("\"", StringComparison.Ordinal), first.Length - first.LastIndexOf("\"", StringComparison.Ordinal));
                }

                UpdateProjectFile = updateProjectFile;
                ProjectUpdated = false;
                TfsWorkspace = tfsWorkspace;
            }

            private FileInfo GetProjectPath(DirectoryInfo directory)
            {
                while (true)
                {
                    if (directory == null)
                    {
                        return null;
                    }

                    var firstOrDefault = directory.GetFiles("*.csproj").FirstOrDefault();
                    if (firstOrDefault != null) return firstOrDefault;
                    directory = directory.Parent;
                }
            }

            private readonly object _dictionaryLock = new object();
            internal void AddFileIfMissing(string path)
            {
                path = Path.GetFullPath(path);
                if (!UpdateProjectFile || File.Exists(path) || !ProjectFound)
                {
                    return;
                }

                var line = string.Format(LineFormat, path.Substring(ProjectDir.Length + 1, path.Length - ProjectDir.Length - 1));
                lock (_dictionaryLock)
                {
                    if (ProjectFiles.ContainsKey(line))
                    {
                        return;
                    }

                    ProjectFiles.Add(line, line);
                }
                if (TfsWorkspace != null)
                {
                    // Create File so it can be added:
                    using (File.Create(path))
                    {
                        TfsWorkspace.PendAdd(path, false);
                    }
                }

                ProjectUpdated = true;
            }

            internal string GetContents()
            {
                // Return lines before, plus ordered compile files, plus lines after
                return string.Join(Environment.NewLine, Lines.Take(ProjectFileIndexStart).Concat(ProjectFiles.Keys).Concat(Lines.Skip(ProjectFileIndexEnd)));
            }
        }
        #endregion // Split File Into Multiple Files By Class
    }
}
