
namespace DLaB.VSSolutionAccelerator.Wizard
{
    public class TextQuestionInfo
    {
        public TextQuestionInfo(string question)
        {
            Question = question;
        }
        public string DefaultResponse { get; set; }
        public string Description { get; set; }
        public string Question { get; set; }
    }
}
