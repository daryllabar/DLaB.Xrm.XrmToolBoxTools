using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Web.XmlTransform;
using Source.DLaB.Common;

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
            public const int Question2 = 5;
            public const int Text2 = 6;
            public const int Path2Text = 7;
            public const int Description = 8;
            public static readonly int[] All = {1, 2, 3, 4, 5, 6, 7, 8};
        }

        public const string SaveResultsPrefix = "SaveResults[";
        public class Row
        {
            public Control Control { get; set; }
            public SizeType SizeType { get; set; }
            public float Height { get; set; }
            public RowStyle Parent { get; set; }
        }
        /// <summary>
        /// The Page itself, and the SaveResults array
        /// </summary>
        public Action<GenericPage, object[]> OnLoadAction{ get; set; }
        private ConditionalYesNoQuestionInfo YesNoInfo { get; set; }
        private PathQuestionInfo PathInfo { get; set; }
        private PathQuestionInfo Path2Info { get; set; }
        private List<Row> Heights { get; }
        private bool CheckFileExists { get; set; }
        private bool CheckFile2Exists { get; set; }
        private string[] DefaultYesNoText { get; set; }

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
            DefaultYesNoText = new string[4];
        }

        private int SetValues(TextQuestionInfo info)
        {
            QuestionLabel.Text = info.Question;
            DescriptionText.Text = info.Description;
            SetDefaultOnLoad(ResponseText, info.DefaultResponse);
            return Rows.Text;
        }

        private int SetValues2(TextQuestionInfo info)
        {
            var separateText = string.IsNullOrWhiteSpace(DescriptionText.Text)
                ? string.Empty
                : Environment.NewLine + Environment.NewLine;
            Question2Label.Text = info.Question;
            SetDefaultOnLoad(Response2Text, info.DefaultResponse);
            DescriptionText.Text += separateText + info.Description;
            return Rows.Text2;
        }

        private void SetDefaultOnLoad(TextBox box, string defaultText)
        {
            if (defaultText == null 
                || !defaultText.Contains(SaveResultsPrefix))
            {
                box.Text = defaultText + string.Empty;
                return;
            }

            void Action(GenericPage page, object[] saveResults)
            {
                box.Text = GetDefaultValue(defaultText, saveResults);
            }

            if (OnLoadAction == null)
            {
                OnLoadAction = Action;
            }
            else
            {
                var oldAction = OnLoadAction;
                OnLoadAction = delegate(GenericPage page, object[] saveResults)
                {
                    oldAction(page, saveResults);
                    Action(page, saveResults);
                };
            }
        }

        private static string GetDefaultValue(string defaultText, object[] saveResults)
        {
            var format = defaultText;
            var previousLength = format.Length + 1;
            while (format.Contains(SaveResultsPrefix) && previousLength > format.Length)
            {
                previousLength = format.Length;
                var index = format.SubstringByString(SaveResultsPrefix, "]");
                format = format.Replace($"{SaveResultsPrefix}{index}]", saveResults[int.Parse(index)].ToString());
            }

            return format;
        }

        public static GenericPage Create(TextQuestionInfo info, TextQuestionInfo info2 = null)
        {
            var page = new GenericPage();
            var rows = new List<int> {Rows.Question, Rows.Description};
            rows.Add(page.SetValues((dynamic) info));
            if (info2 != null)
            {
                rows.Add(Rows.Question2);
                rows.Add(page.SetValues2((dynamic) info2));
            }

            page.HideAllExceptRows(rows.ToArray());
            return page;
        }

        public static GenericPage Create(TextQuestionInfo info, Action<GenericPage, object[]> onLoadAction)
        {
            return Create(info, null, onLoadAction);
        }

        public static GenericPage Create(TextQuestionInfo info, TextQuestionInfo info2, Action<GenericPage, object[]> onLoadAction)
        {
            var page = Create(info, info2);
            page.OnLoadAction = onLoadAction;
            return page;
        }

        private int SetValues(PathQuestionInfo info)
        {
            SetValues((TextQuestionInfo)info);
            PathInfo = info;
            SetDefaultOnLoad(Path, info.DefaultResponse);
            CheckFileExists = info.RequireFileExists;
            return Rows.PathText;
        }

        private int SetValues2(PathQuestionInfo info)
        {
            SetValues2((TextQuestionInfo)info);
            Path2Info = info;
            SetDefaultOnLoad(Path2, info.DefaultResponse);
            CheckFile2Exists = info.RequireFileExists;
            return Rows.Path2Text;
        }

        public static GenericPage Create(ConditionalYesNoQuestionInfo info)
        {
            var page = new GenericPage
            {
                YesNoInfo = info,
                YesNoGroup = { Text = info.Question }
            };

            page.SetYesNoDefaultOnLoad(info.Yes?.DefaultResponse, 0);
            page.SetYesNoDefaultOnLoad(info.Yes2?.DefaultResponse, 1);
            page.SetYesNoDefaultOnLoad(info.No?.DefaultResponse, 2);
            page.SetYesNoDefaultOnLoad(info.No2?.DefaultResponse, 3);

            page.HideAllExceptRows(Rows.YesNoQuestion, Rows.Description);

            return page;
        }

        public static GenericPage Create(ConditionalYesNoQuestionInfo info, Action<GenericPage, object[]> onLoadAction)
        {
            var page = Create(info);
            page.OnLoadAction = onLoadAction;
            return page;
        }

        private void SetYesNoDefaultOnLoad(string defaultText, int index)
        {
            if (defaultText == null
                || !defaultText.Contains(SaveResultsPrefix))
            {
                DefaultYesNoText[index] = defaultText + string.Empty;
                return;
            }

            void Action(GenericPage page, object[] saveResults)
            {
                DefaultYesNoText[index] = GetDefaultValue(defaultText, saveResults);
            }

            if (OnLoadAction == null)
            {
                OnLoadAction = Action;
            }
            else
            {
                var oldAction = OnLoadAction;
                OnLoadAction = delegate (GenericPage page, object[] saveResults)
                {
                    oldAction(page, saveResults);
                    Action(page, saveResults);
                };
            }
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
                DisplayRowsForYesNoValue(YesNoInfo.Yes, YesNoInfo.Yes2, 0);
            }
            else if (sender == NoRadio && NoRadio.Checked)
            {
                DisplayRowsForYesNoValue(YesNoInfo.No, YesNoInfo.No2, 2);
            }
        }

        private void DisplayRowsForYesNoValue(TextQuestionInfo info, TextQuestionInfo info2, int defaultTextIndex)
        {
            var rows = new List<int> { Rows.YesNoQuestion, Rows.Description };
            if (info == null)
            {
                DescriptionText.Text = string.Empty;
            }
            else
            {
                rows.Add(Rows.Question);
                rows.Add(SetValues((dynamic)info));
                ResponseText.Text = DefaultYesNoText[defaultTextIndex];
                Path.Text = DefaultYesNoText[defaultTextIndex];

            }

            if (info2 != null)
            {
                rows.Add(Rows.Question2);
                rows.Add(SetValues2((dynamic)info2));
                Response2Text.Text = DefaultYesNoText[defaultTextIndex+1];
                Path2.Text = DefaultYesNoText[defaultTextIndex];
            }

            HideAllExceptRows(rows.ToArray());
        }

        private void OpenFileBtn_Click(object sender, EventArgs e)
        {
            string filter;
            TextBox path;
            if (sender == OpenFileBtn)
            {
                filter = PathInfo.Filter;
                path = Path;
            }
            else
            {
                filter = Path2Info.Filter;
                path = Path2;
            }

            OpenFileDialog.Multiselect = false;
            OpenFileDialog.Filter = filter;
            if (!string.IsNullOrWhiteSpace(path.Text))
            {
                OpenFileDialog.FileName = path.Text;
            }

            if (OpenFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                path.Text = OpenFileDialog.FileName;
            }
        }
    }
}
