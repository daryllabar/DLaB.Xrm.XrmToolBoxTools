using System.Collections.Generic;

namespace DLaB.VSSolutionAccelerator.Wizard
{
    public class ComboQuestionInfo: QuestionInfo
    {
        public List<KeyValuePair<int, string>> Options { get; set; }

        public ComboQuestionInfo(string question) : base(question) { }

        public int? DefaultResponse { get; set; }
        public int? DefaultSaveResultIndex { get; set; }
    }
}
