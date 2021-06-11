using System;
using System.IO;
using System.Linq;
using System.Text;

#if DLAB_UNROOT_COMMON_NAMESPACE
namespace DLaB.Common.VersionControl
#else
namespace Source.DLaB.Common.VersionControl
#endif
{
    /// <summary>
    /// Class to handle Checkingout from TFS
    /// </summary>
#if DLAB_PUBLIC
    public class VsTfsSourceControlProvider : ISourceControlProvider
#else
    internal class VsTfsSourceControlProvider : ISourceControlProvider
#endif
    {
        private const int MaxCommandLength = 30000; // Total max Length for a command is 32768 I believe.  Just limit it to 30000 to allow for the length of other values.
        private string TfPath { get; }
        private ProcessExecutorInfo DefaultProcessExectorInfo { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VsTfsSourceControlProvider" /> class.
        /// </summary>
        /// <param name="tfPath">The tf path.</param>
        /// <param name="info">The default Process Executor Info information.</param>
        public VsTfsSourceControlProvider(string tfPath = null, ProcessExecutorInfo info = null)
        {
            TfPath = tfPath ?? Config.GetAppSettingOrDefault("DLaB.Common.VersionControl.TfsPath", GetDefaultTfPath);
            if (!File.Exists(TfPath))
            {
                throw new Exception($"No TF.exe was found at '{TfPath}'.  Please create/update the app setting for 'DLaB.Common.VersionControl.TfsPath' to a valid path");
            }
            DefaultProcessExectorInfo = info;
        }

        private string GetDefaultTfPath()
        {
            var programFiles = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            if (programFiles == null)
            {
                throw new Exception($"Error in TfsSourceContorlProvider.GetDefaultTfPath: Unable to get Environment Variable ProgramFiles(x86).");
            }
            var path = Path.Combine(programFiles, @"Microsoft Visual Studio 14.0\Common7\IDE\TF.exe");
            if (File.Exists(path))
            {
                return path;
            }
            else
            {
                // VS 2017 changed the location to be under the format of "Microsoft Visual Studio\(Year?Version?)\Edition"
                // attempt to future proof by checking all version and all editions
                path = Path.Combine(programFiles, @"Microsoft Visual Studio");
                if (!Directory.Exists(path))
                {
                    return path;
                }
                foreach(var version in Directory.GetDirectories(path))
                {
                    foreach(var edition in Directory.GetDirectories(version))
                    {
                        var tmp = Path.Combine(edition, @"Common7\IDE\CommonExtensions\Microsoft\TeamFoundation\Team Explorer\TF.exe");
                        if (File.Exists(tmp))
                        {
                            return tmp;
                        }
                    }
                }
                return path;
            }
        }

        private string WrapPathInQuotes(string filePath)
        {
            if (filePath == null)
            {
                return null;
            }
            filePath = filePath.Trim();
            if (filePath.EndsWith("\"") && filePath.StartsWith("\""))
            {
                return filePath;
            }

            return $"\"{filePath}\"";
        }

        /// <summary>
        /// Adds the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <exception cref="System.Exception">Unable to Add the file {filePath}</exception>
        public void Add(string filePath)
        {
            try
            {
                var info = CreateProcessExecutorInfo("add", filePath);
                ProcessExecutor.ExecuteCmd(info);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to Add the file " + filePath + Environment.NewLine + ex);
            }
        }

        /// <summary>
        /// Determines whether the specified files are different.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="diffPath">The difference path.</param>
        /// <returns></returns>
        public bool AreDifferent(string sourcePath, string diffPath)
        {
            try
            {
                var info = CreateProcessExecutorInfo("Diff", sourcePath, WrapPathInQuotes(diffPath) + " /format:Brief");
                var output = ProcessExecutor.ExecuteCmd(info);

                return output.Contains("files differ");
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to determine if \"{sourcePath}\" is different than \"{diffPath}" + sourcePath + Environment.NewLine + ex);
            }
        }

        private ProcessExecutorInfo CreateProcessExecutorInfo(string action, string filePath, string postArguments = null, string workingDirectory = null)
        {
            workingDirectory = workingDirectory ?? Directory.GetParent(filePath).FullName;
            var info = DefaultProcessExectorInfo ?? new ProcessExecutorInfo();
            info.FileName = $"\"{TfPath}\"";
            info.Arguments = $"{action} {WrapPathInQuotes(filePath)} {postArguments}";
            info.WorkingDirectory = workingDirectory;
            return info;
        }

        /// <summary>
        /// Checks the file(s) out.
        /// </summary>
        /// <param name="fileNames">The file names.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Unable to check out file
        /// or
        /// File is read only, please checkout the file before running</exception>
        public string Checkout(params string[] fileNames)
        {
            if (fileNames == null || fileNames.Length == 0)
            {
                return "No Files Given";
            }

            var fileNamesToCheckoutBatches = fileNames.Where(f => File.GetAttributes(f).HasFlag(FileAttributes.ReadOnly))
                                                      .BatchLessThanMaxLength(MaxCommandLength, 
                                                                              "Filename \"{0}\" is longer than the max length {1}.", 
                                                                              3 // 1 for each quote, and one more for the space between.
                                                                              );

            var output = new StringBuilder();
            foreach (var batch in fileNamesToCheckoutBatches)
            {
                try
                {
                    var files = string.Join(" ", batch.Select(WrapPathInQuotes));
                    var info = CreateProcessExecutorInfo("checkout", null, files, Directory.GetParent(fileNames.First()).FullName);
                    output.AppendLine(ProcessExecutor.ExecuteCmd(info));
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to check out files " + string.Join(", ", batch) + Environment.NewLine + ex);
                }
            }

            foreach (var file in fileNamesToCheckoutBatches.SelectMany(v => v))
            {
                if (File.GetAttributes(file).HasFlag(FileAttributes.ReadOnly))
                {
                    throw new Exception("File \"" + file + "\" is read only even though it should have been checked out, please checkout the file before running.  Output: " + output);
                }
            }
            return output.ToString();
        }

        /// <summary>
        /// Returns true if the file was unchanged and so it was checked out
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="contents">The contents.</param>
        /// <returns></returns>
        public bool CheckoutAndUpdateIfDifferent(string filePath, string contents)
        {
            try
            {
                Checkout(filePath);
                File.WriteAllText(filePath, contents);

                return UndoCheckoutIfUnchanged(filePath);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to Checkout and Update if Different for file " + filePath + Environment.NewLine + ex);
            }
        }

        /// <summary>
        /// Gets the specified files from the server, potentially overwriting it, even if it's checked out.
        /// </summary>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
        /// <param name="fileNames">The file names.</param>
        /// <exception cref="System.Exception">Unable to determin if file is different than contents for the file  + sourcePath + Environment.NewLine + ex</exception>
        public string Get(bool overwrite, params string[] fileNames)
        {
            if (fileNames == null || fileNames.Length == 0)
            {
                return "No Files Given";
            }

            var fileNameBatches = fileNames.BatchLessThanMaxLength(MaxCommandLength,
                                                      "Filename \"{0}\" is longer than the max length {1}.",
                                                      3 // 1 for each quote, and one more for the space between.
                                                      );

            var output = new StringBuilder();
            foreach (var batch in fileNameBatches)
            {
                try
                {
                    var files = string.Join(" ", batch.Select(WrapPathInQuotes));
                    var info = CreateProcessExecutorInfo("get", null, files + (overwrite ? " /overwrite": ""), Directory.GetParent(fileNames.First()).FullName);
                    output.AppendLine(ProcessExecutor.ExecuteCmd(info));
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to get files " + string.Join(", ", batch) + Environment.NewLine + ex);
                }
            }
            return output.ToString();
        }

        /// <summary>
        /// Returns true if the file was unchanged and an undo operation was performed
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool UndoCheckoutIfUnchanged(string filePath)
        {
            try
            {
                var info = CreateProcessExecutorInfo("Diff", filePath, "/format:Brief");
                var output = ProcessExecutor.ExecuteCmd(info);

                if (output.Contains("files differ"))
                {
                    return false;
                }

                Undo(filePath);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to Undo Checkout If Unchanged for file " + filePath + Environment.NewLine + ex);
            }
        }

        /// <summary>
        /// Un-does the specified file path.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <exception cref="System.Exception">Unable to Undo Checkout If Unchanged for file  + filePath + Environment.NewLine + ex</exception>
        public void Undo(string filePath)
        {
            try
            {
                var info = CreateProcessExecutorInfo("undo", filePath, "/noprompt");

                ProcessExecutor.ExecuteCmd(info);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to Undo Checkout for file " + filePath + Environment.NewLine + ex);
            }
        }
    }
}