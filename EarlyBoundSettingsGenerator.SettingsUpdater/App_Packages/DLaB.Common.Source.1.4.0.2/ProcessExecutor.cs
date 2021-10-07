using System;
using System.Diagnostics;
using System.Text;

#if DLAB_UNROOT_COMMON_NAMESPACE
namespace DLaB.Common
#else
namespace Source.DLaB.Common
#endif
{
    /// <summary>
    /// Executes Command Line calls, redirecting output to Console
    /// </summary>
#if DLAB_PUBLIC
    public class ProcessExecutor
#else
    internal class ProcessExecutor
#endif
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns></returns>
        public static string ExecuteCmd(string fileName, string arguments)
        {
            return ExecuteCmd(new ProcessExecutorInfo(fileName, arguments));
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns></returns>
        public static string ExecuteCmd(ProcessExecutorInfo info)
        {
            var color = Console.ForegroundColor;

            if (!info.RedirectStandardOutput.HasValue)
            {
                info.RedirectStandardOutput = true;
            }

            if (!info.RedirectStandardError.HasValue)
            {
                info.RedirectStandardError = true;
            }

            var cmdProcess = new Process
            {
                StartInfo = info.GetStartInfo()
            };

            var commandOutput = new StringBuilder();
            var commandOutputLock = new object();
            cmdProcess.ErrorDataReceived += (sender, e) => HandleErrorReceived(e, info, commandOutput, commandOutputLock);
            cmdProcess.OutputDataReceived += (sender, e) => HandleDataReceived(e, info, commandOutput, commandOutputLock);
            cmdProcess.EnableRaisingEvents = true;
            cmdProcess.Start();
            cmdProcess.BeginOutputReadLine();
            cmdProcess.BeginErrorReadLine();

            cmdProcess.StandardInput.WriteLine("exit");  //Execute exit.

            cmdProcess.WaitForExit();
            Console.ForegroundColor = color;
            return commandOutput.ToString();
        }

        private static void HandleDataReceived(DataReceivedEventArgs e, ProcessExecutorInfo info, StringBuilder sb, object sbLock)
        {
            lock (sbLock)
            {
                sb.AppendLine(e?.Data);
                if (info.OnOutputReceived == null)
                {
                    var color = Console.ForegroundColor;

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(e?.Data);
                    Console.ForegroundColor = color;
                }
                else
                {
                    info.OnOutputReceived(e?.Data);
                }
            }
        }

        private static void HandleErrorReceived(DataReceivedEventArgs e, ProcessExecutorInfo info, StringBuilder sb, object sbLock)
        {
            lock (sbLock)
            {
                sb.AppendLine(e?.Data);
                if (info.OnErrorReceived == null)
                {
                    var color = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e?.Data);
                    Console.ForegroundColor = color;
                }
                else
                {
                    info.OnErrorReceived(e?.Data);
                }
            }
        }
    }
}
