using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLaB.VSSolutionAccelerator.Wizard
{
    public abstract class QuestionInfo
    {
        protected QuestionInfo(string question)
        {
            Question = question;
        }
        public string Description { get; set; }
        public string Question { get; set; }
    }
}
