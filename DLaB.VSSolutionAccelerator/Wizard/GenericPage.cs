using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DLaB.VSSolutionAccelerator.Wizard
{
    public partial class GenericPage : UserControl
    {
        public static class Rows
        {
            public const int YesNoQuestion = 1;
            public const int Question = 2;
            public const int Text = 3;
            public const int PathText = 4;
            public const int Description = 5;
            public static readonly int[] All = {1, 2, 3, 4, 5};
        }

        public class Row
        {
            public Control Control { get; set; }
            public SizeType SizeType { get; set; }
            public float Height { get; set; }
            public RowStyle Parent { get; set; }
        }
        public Action<GenericPage> OnLoadAction{ get; set; }

        private ConditionalYesNoQuestionInfo YesNoInfo { get; set; }
        private PathQuestionInfo PathInfo { get; set; }
        private List<Row> Heights { get; }

        public GenericPage()
        {
            InitializeComponent();
            Heights = new List<Row>();
            Heights.AddRange(Table.RowStyles.Cast<RowStyle>().Select((r, i) => new Row
            {
                Control = Table.GetControlFromPosition(1, i),
                Height = r.Height,
                SizeType = r.SizeType,
                Parent = r
            }));
        }

        private void SetValues(TextQuestionInfo info)
        {
            QuestionLabel.Text = info.Question;
            ResponseText.Text = info.DefaultResponse;
            DescriptionText.Text = info.Description;
        }

        public static GenericPage CreateTextQuestion(TextQuestionInfo info)
        {
            var page = new GenericPage();
            page.SetValues(info);

            page.HideAllExceptRows(Rows.Question, Rows.Text, Rows.Description);
            return page;
        }


        private void SetValues(PathQuestionInfo info)
        {
            SetValues((TextQuestionInfo)info);
            PathInfo = info;
            Path.Text = info.DefaultResponse;
        }

        public static GenericPage CreatePathQuestion(PathQuestionInfo info)
        {
            var page = new GenericPage();
            page.SetValues(info);
            page.HideAllExceptRows(Rows.Question, Rows.PathText, Rows.Description);
            return page;
        }

        public static GenericPage CreateConditionalYesNoQuestion(ConditionalYesNoQuestionInfo info)
        {
            var page = new GenericPage
            {
                YesNoInfo = info,
                YesNoGroup = { Text = info.Question }
            };

            page.HideAllExceptRows(Rows.YesNoQuestion, Rows.Description);

            return page;
        }

        private void HideAllExceptRows(params int[] rows)
        {
            var rowHash = new HashSet<int>(rows);
            Table.SuspendLayout();
            foreach (var row in Heights.Where(r => r.Control != null))
            {
                var position = Table.GetCellPosition(row.Control);
                if (rowHash.Contains(position.Row))
                {
                    row.Parent.Height = row.Height;
                    row.Parent.SizeType = row.SizeType;
                    row.Control.Visible = true;
                }
                else
                {
                    row.Parent.Height = 0f;
                    row.Parent.SizeType = SizeType.Absolute;
                    row.Control.Visible = false;
                }
            }

            Table.ResumeLayout();
        }

        private void YesNoRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (sender == YesRadio && YesRadio.Checked)
            {
                DisplayRowsForYesNoValue(YesNoInfo.Yes);
            }
            else if (sender == NoRadio && NoRadio.Checked)
            {
                DisplayRowsForYesNoValue(YesNoInfo.No);
            }
        }

        private void DisplayRowsForYesNoValue(TextQuestionInfo info)
        {
            var rows = new List<int> { Rows.YesNoQuestion, Rows.Description };
            switch (info)
            {
                case null:
                    DescriptionText.Text = string.Empty;
                    break;
                case PathQuestionInfo pathInfo:
                    SetValues(pathInfo);
                    rows.Add(Rows.Question);
                    rows.Add(Rows.PathText);
                    break;
                default:
                    SetValues(info);
                    rows.Add(Rows.Question);
                    rows.Add(Rows.Text);
                    break;
            }

            HideAllExceptRows(rows.ToArray());
        }

        private void OpenFileBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog.CheckFileExists = true;
            OpenFileDialog.Multiselect = false;
            OpenFileDialog.Filter = PathInfo.Filter;
            if (!string.IsNullOrWhiteSpace(Path.Text))
            {
                OpenFileDialog.FileName = Path.Text;
            }

            if (OpenFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                Path.Text = OpenFileDialog.FileName;
            }
        }
    }
}
