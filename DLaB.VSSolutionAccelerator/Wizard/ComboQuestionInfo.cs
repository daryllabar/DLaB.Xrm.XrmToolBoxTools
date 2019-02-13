using System.Collections.Generic;

namespace DLaB.VSSolutionAccelerator.Wizard
{
    public class ComboQuestionInfo: QuestionInfo
    {
        public List<KeyValuePair<int, string>> Options { get; set; }

        public ComboQuestionInfo(string question) : base(question)
        {
            Options = new List<KeyValuePair<int, string>>
            {
                new KeyValuePair<int, string>(0, "Yes"),
                new KeyValuePair<int, string>(1, "No"),
            };
        }

        public int? DefaultResponse { get; set; }
        public int? DefaultSaveResultIndex { get; set; }
    }
}
