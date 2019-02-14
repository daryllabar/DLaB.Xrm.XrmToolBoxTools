
using System;

namespace DLaB.VSSolutionAccelerator.Wizard
{
    public class TextQuestionInfo : QuestionInfo
    {
        public TextQuestionInfo(string question) : base(question) { }
        public string DefaultResponse { get; set; }
        public Func<string, string> EditDefaultResponse { get; set; }
    }
}
