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
        public QuestionInfo Yes { get; set; }
        public QuestionInfo Yes2 { get; set; }
        public QuestionInfo No { get; set; }
        public QuestionInfo No2 { get; set; }
        public string Description { get; set; }

        public ConditionalYesNoQuestionInfo(string question)
        {
            Question = question;
        }
    }
}
