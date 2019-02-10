using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace DLaB.Log
{
    public class Logger
    {
        public static Logger Instance = new Logger();

        public delegate void LogHandler(LogMessageInfo info);

        public event LogHandler OnLog;

        private readonly Dictionary<BackgroundWorker, LogHandler> _handlersByWorkerProcess = new Dictionary<BackgroundWorker, LogHandler>();

        public static void AddDetail(string message)
        {
            Show(new LogMessageInfo(null, message));
        }

        public static void Show(string summary)
        {
            Show(new LogMessageInfo(summary, summary));
        }

        public static void Show(string summary, string detail)
        {
            Show(new LogMessageInfo(summary, detail));
        }

        public static void Show(LogMessageInfo info)
        {
            Instance.OnLog?.Invoke(info);
        }

        public static void WireUpToReportProgress(BackgroundWorker w)
        {
            Instance.WireUpInstanceToReportProgress(w);
        }

        private readonly object _lock = new object();

        public void WireUpInstanceToReportProgress(BackgroundWorker w)
        {
            void LocalOnLog(LogMessageInfo m) => w.ReportProgress(m.PercentProgress, m);
            lock (_lock)
            {
                if (_handlersByWorkerProcess.ContainsKey(w))
                {
                    throw new Exception("Background worker has already been wired up for Reporting Progress!  Did you remember to call UnwireFromReportProgress?");
                }

                _handlersByWorkerProcess.Add(w, LocalOnLog);
            }

            OnLog += LocalOnLog;
        }

        public static void UnwireFromReportProgress(BackgroundWorker w)
        {
            Instance.UnwireInstanceFromReportProgress(w);
        }

        public void UnwireInstanceFromReportProgress(BackgroundWorker w)
        {
            lock (_lock)
            {
                var callback = _handlersByWorkerProcess[w];
                _handlersByWorkerProcess.Remove(w);
                OnLog -= callback;
            }
        }

        public static void DisplayLog(ProgressChangedEventArgs args, Action<string, int, int> setWorkingMessage, TextBox detailTextBox)
        {
            Instance.DisplayInstanceLog(args, setWorkingMessage, detailTextBox);
        }

        public void DisplayInstanceLog(ProgressChangedEventArgs args, Action<string, int, int> setWorkingMessage, TextBox detailTextBox)
        {
            try
            {
                string summary;
                var width = 340;
                var height = 150;
                if (args.UserState is LogMessageInfo result)
                {
                    if (result.Detail != null)
                    {
                        detailTextBox.AppendText(result.Detail + Environment.NewLine);
                    }

                    if (result.Height.HasValue)
                    {
                        height = result.Height.Value;
                    }

                    if (result.Width.HasValue)
                    {
                        width = result.Width.Value;
                    }

                    summary = result.ModalMessage;
                }
                else
                {
                    summary = args.UserState.ToString();
                    detailTextBox.AppendText(summary + Environment.NewLine);
                }

                // Status Update
                if (args.ProgressPercentage == int.MinValue)
                {
                    detailTextBox.AppendText(args.UserState + Environment.NewLine);
                }
                else if (summary != null)
                {
                    setWorkingMessage(summary, width, height);
                }
            }
            catch
            {
                // Eat exceptions when logging
            }
        }

        public static void DisplayLog(RunWorkerCompletedEventArgs args, TextBox detailTextBox)
        {
            Instance.DisplayInstanceLog(args, detailTextBox);
        }

        public void DisplayInstanceLog(RunWorkerCompletedEventArgs args, TextBox detailTextBox)
        {
            if (args.Result is LogMessageInfo result)
            {
                detailTextBox.AppendText(result.Detail + Environment.NewLine);
            }
        }
    }
}
