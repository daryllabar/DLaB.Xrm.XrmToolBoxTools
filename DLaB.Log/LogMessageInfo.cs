using System;
using System.Collections.Generic;
using System.Text;

namespace DLaB.Log
{
    public class LogMessageInfo
    {
        public string ModalMessage { get; set; }
        public string Detail { get; set; }
        public int PercentProgress { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }

        public LogMessageInfo(string modalMessage, string detail, int percentProgress = 0, int? width = null, int? height = null)
        {
            ModalMessage = modalMessage;
            Detail = detail;
            PercentProgress = percentProgress;
            Width = width;
            Height = height;
        }
    }

    public class LogMessageInfoFormat : LogMessageInfo
    {
        public LogMessageInfoFormat(string messageFormat, params object[] args) : base(null, string.Format(messageFormat, args)) { }
    }
}
