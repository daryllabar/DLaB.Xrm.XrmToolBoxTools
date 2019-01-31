using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLaB.VSSolutionAccelerator.Wizard
{
    public class PathQuestionInfo : TextQuestionInfo
    {
        public string Filter { get; internal set; }
        public bool RequireFileExists { get; set; }

        public PathQuestionInfo(string question) : base(question)
        {
            RequireFileExists = true;
        }
    }
}
