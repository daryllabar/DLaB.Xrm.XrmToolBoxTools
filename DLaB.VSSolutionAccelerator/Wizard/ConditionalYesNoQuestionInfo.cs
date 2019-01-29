using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLaB.VSSolutionAccelerator.Wizard
{
    public class ConditionalYesNoQuestionInfo
    {
        public string Question { get; set; }
        public TextQuestionInfo Yes { get; set; }
        public TextQuestionInfo No { get; set; }
        public string Description { get; set; }

        public ConditionalYesNoQuestionInfo(string question)
        {
            Question = question;
        }
    }
}
