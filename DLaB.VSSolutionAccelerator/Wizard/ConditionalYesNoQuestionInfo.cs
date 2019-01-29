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
        public string YesQuestion { get; set; }
        public string NoQuestion { get; set; }
        public int YesRow { get; set; }
        public int NoRow { get; set; }
        public string QuestionDescription { get; set; }
        public string YesQuestionDescription { get; set; }
        public string NoQuestionDescription { get; set; }

        public ConditionalYesNoQuestionInfo(string question)
        {
            Question = question;
        }
    }
}
