﻿using Source.DLaB.Common;
using System;
using System.Collections.Generic;
using System.IO;
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
            public const int Combo = 5;
            public const int Question2 = 6;
            public const int Text2 = 7;
            public const int Path2Text = 8;
            public const int Combo2 = 9;
            public const int Question3 = 10;
            public const int Text3 = 11;
            public const int Path3Text = 12;
            public const int Combo3 = 13;
            public const int Question4 = 14;
            public const int Text4 = 15;
            public const int Path4Text = 16;
            public const int Combo4 = 17;
            public const int Description = 18;
            public static readonly int[] All = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 };
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
        private PathQuestionInfo Path3Info { get; set; }
        private PathQuestionInfo Path4Info { get; set; }
        private List<Row> Heights { get; }
        private bool CheckFileExists { get; set; }
        private bool CheckFile2Exists { get; set; }
        private bool CheckFile3Exists { get; set; }
        private bool CheckFile4Exists { get; set; }
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
            SavedValueRequiredValue = new List<KeyValuePair<Tuple<int,int>, string>>();
            DefaultYesNoText = new string[4];
        }

        #region Values 1

        private int SetValues(ComboQuestionInfo info)
        {
            SetValues((QuestionInfo)info);
            Combo.DataSource = info.Options;
            Combo.ValueMember = "Key";
            Combo.DisplayMember = "Value";
            SetDefaultOnLoad(Combo, info.DefaultResponse, info.DefaultSaveResultIndex);
            return Rows.Combo;
        }

        private int SetValues(TextQuestionInfo info)
        {
            SetDefaultOnLoad(ResponseText, info.DefaultResponse, info.EditDefaultResponse);
            return SetValues((QuestionInfo)info);
        }

        private int SetValues(PathQuestionInfo info)
        {
            SetValues((QuestionInfo)info);
            PathInfo = info;
            SetDefaultOnLoad(Path, info.DefaultResponse, info.EditDefaultResponse);
            CheckFileExists = info.RequireFileExists;
            return Rows.PathText;
        }

        private int SetValues(QuestionInfo info)
        {
            QuestionLabel.Text = info.Question;
            DescriptionText.Text = info.Description;
            return Rows.Text;
        }

        #endregion Values 1

        #region Values 2

        private int SetValues2(ComboQuestionInfo info)
        {
            SetValues2((QuestionInfo)info);
            Combo2.DataSource = info.Options;
            Combo2.ValueMember = "Key";
            Combo2.DisplayMember = "Value";
            SetDefaultOnLoad(Combo2, info.DefaultResponse, info.DefaultSaveResultIndex);
            return Rows.Combo2;
        }

        private int SetValues2(PathQuestionInfo info)
        {
            SetValues2((QuestionInfo)info);
            Path2Info = info;
            SetDefaultOnLoad(Path2, info.DefaultResponse, info.EditDefaultResponse);
            CheckFile2Exists = info.RequireFileExists;
            return Rows.Path2Text;
        }

        private int SetValues2(TextQuestionInfo info)
        {
            SetDefaultOnLoad(Response2Text, info.DefaultResponse, info.EditDefaultResponse);
            return SetValues2((QuestionInfo)info);
        }

        private int SetValues2(QuestionInfo info)
        {
            var separateText = string.IsNullOrWhiteSpace(DescriptionText.Text) || string.IsNullOrWhiteSpace(info.Description)
                ? string.Empty
                : Environment.NewLine + Environment.NewLine;
            Question2Label.Text = info.Question;
            DescriptionText.Text += separateText + info.Description;
            return Rows.Text2;
        }

        #endregion Values 2

        #region Values 3

        private int SetValues3(ComboQuestionInfo info)
        {
            SetValues3((QuestionInfo)info);
            Combo3.DataSource = info.Options;
            Combo3.ValueMember = "Key";
            Combo3.DisplayMember = "Value";
            SetDefaultOnLoad(Combo3, info.DefaultResponse, info.DefaultSaveResultIndex);
            return Rows.Combo3;
        }

        private int SetValues3(PathQuestionInfo info)
        {
            SetValues3((QuestionInfo)info);
            Path3Info = info;
            SetDefaultOnLoad(Path3, info.DefaultResponse, info.EditDefaultResponse);
            CheckFile3Exists = info.RequireFileExists;
            return Rows.Path3Text;
        }

        private int SetValues3(TextQuestionInfo info)
        {
            SetDefaultOnLoad(Response3Text, info.DefaultResponse, info.EditDefaultResponse);
            return SetValues3((QuestionInfo)info);
        }

        private int SetValues3(QuestionInfo info)
        {
            var separateText = string.IsNullOrWhiteSpace(DescriptionText.Text) || string.IsNullOrWhiteSpace(info.Description)
                ? string.Empty
                : Environment.NewLine + Environment.NewLine;
            Question3Label.Text = info.Question;
            DescriptionText.Text += separateText + info.Description;
            return Rows.Text3;
        }

        #endregion Values 3

        #region Values 4

        private int SetValues4(ComboQuestionInfo info)
        {
            SetValues4((QuestionInfo)info);
            Combo4.DataSource = info.Options;
            Combo4.ValueMember = "Key";
            Combo4.DisplayMember = "Value";
            SetDefaultOnLoad(Combo4, info.DefaultResponse, info.DefaultSaveResultIndex);
            return Rows.Combo4;
        }

        private int SetValues4(PathQuestionInfo info)
        {
            SetValues4((QuestionInfo)info);
            Path4Info = info;
            SetDefaultOnLoad(Path4, info.DefaultResponse, info.EditDefaultResponse);
            CheckFile4Exists = info.RequireFileExists;
            return Rows.Path4Text;
        }

        private int SetValues4(TextQuestionInfo info)
        {
            SetDefaultOnLoad(Response4Text, info.DefaultResponse, info.EditDefaultResponse);
            return SetValues4((QuestionInfo)info);
        }

        private int SetValues4(QuestionInfo info)
        {
            var separateText = string.IsNullOrWhiteSpace(DescriptionText.Text) || string.IsNullOrWhiteSpace(info.Description)
                ? string.Empty
                : Environment.NewLine + Environment.NewLine;
            Question4Label.Text = info.Question;
            DescriptionText.Text += separateText + info.Description;
            return Rows.Text4;
        }

        #endregion Values 4

        private void SetDefaultOnLoad(TextBox box, string defaultText, Func<string, string> editDefaultResponse)
        {
            if (defaultText == null 
                || !defaultText.Contains(SaveResultsPrefix))
            {
                box.Text = defaultText + string.Empty;
                return;
            }

            void Action(GenericPage page, object[] saveResults)
            {
                var defaultValue = GetDefaultValue(defaultText, saveResults);
                if (editDefaultResponse != null)
                {
                    defaultValue = editDefaultResponse(defaultValue);
                }
                box.Text = defaultValue;
            }

            AddOnLoadAction(Action);
        }

        private void SetDefaultOnLoad(ComboBox box, int? defaultValue, int? defaultResultIndex)
        {
            if (defaultValue == null
                && defaultResultIndex == null)
            {
                return;
            }

            void Action(GenericPage page, object[] saveResults)
            {
                var defaultText = defaultValue == null
                    ? GetSaveResultsFormat(defaultResultIndex.GetValueOrDefault())
                    : defaultValue.ToString();
                var value = GetDefaultValue(defaultText, saveResults);
                box.SelectedValue = int.Parse(value);
            }

            AddOnLoadAction(Action);
        }

        private void AddOnLoadAction(Action<GenericPage, object[]> action)
        {
            if (OnLoadAction == null)
            {
                OnLoadAction = action;
            }
            else
            {
                var oldAction = OnLoadAction;
                OnLoadAction = delegate (GenericPage page, object[] saveResults)
                {
                    oldAction(page, saveResults);
                    action(page, saveResults);
                };
            }
        }

        public static string GetSaveResultsFormat(int saveResultIndex)
        {
            return SaveResultsPrefix + saveResultIndex + "]";
        }

        /// <summary>
        /// Used to lookup the indexed value of the result index
        /// </summary>
        /// <returns></returns>
        public static string GetSaveResultsFormat(int saveResultIndex, int enumerableIndex)
        {
            return $"{SaveResultsPrefix}{saveResultIndex}|{enumerableIndex}]";
        }

        private static string GetDefaultValue(string defaultText, object[] saveResults)
        {
            var format = defaultText;
            var previousLength = format.Length + 1;
            while (format.Contains(SaveResultsPrefix) && previousLength > format.Length)
            {
                previousLength = format.Length;
                var index = format.SubstringByString(SaveResultsPrefix, "]");
                if (index.Contains('|'))
                {
                    var parts = index.Split('|');
                    var value = saveResults[int.Parse(parts[0])];
                    var listIndex = int.Parse(parts[1]);
                    if (value is List<string> list
                        && list.Count > listIndex)
                    {
                        format = format.Replace($"{SaveResultsPrefix}{index}]", list[listIndex]);
                    }
                    else
                    {
                        format = string.Empty; 
                    }
                }
                else
                {
                    format = format.Replace($"{SaveResultsPrefix}{index}]", saveResults[int.Parse(index)].ToString());
                }
            }

            return format;
        }

        public static GenericPage Create(QuestionInfo info, QuestionInfo info2 = null, QuestionInfo info3 = null, QuestionInfo info4 = null)
        {
            var page = new GenericPage();
            var rows = new List<int>
            {
                Rows.Question, Rows.Description,
                page.SetValues((dynamic)info)
            };
            if (info2 != null)
            {
                rows.Add(Rows.Question2);
                rows.Add(page.SetValues2((dynamic) info2));
            }
            if (info3 != null)
            {
                rows.Add(Rows.Question3);
                rows.Add(page.SetValues3((dynamic)info3));
            }
            if (info4 != null)
            {
                rows.Add(Rows.Question4);
                rows.Add(page.SetValues4((dynamic)info4));
            }

            page.HideAllExceptRows(rows.ToArray());
            return page;
        }

        public static GenericPage Create(QuestionInfo info, Action<GenericPage, object[]> onLoadAction)
        {
            return Create(info, null, onLoadAction);
        }

        public static GenericPage Create(QuestionInfo info, QuestionInfo info2, Action<GenericPage, object[]> onLoadAction)
        {
            var page = Create(info, info2);
            page.OnLoadAction = onLoadAction;
            return page;
        }

        public static GenericPage Create(ConditionalYesNoQuestionInfo info)
        {
            var page = new GenericPage
            {
                YesNoInfo = info,
                YesNoGroup = { Text = info.Question }
            };

            if (info.Yes != null)
            {
                page.SetYesNoDefaultOnLoad((dynamic)info.Yes, 0);
                if (info.Yes2 != null)
                {
                    page.SetYesNoDefaultOnLoad((dynamic)info.Yes2, 1);
                }
            }


            if (info.No != null)
            {
                page.SetYesNoDefaultOnLoad((dynamic)info.No, 2);
                if (info.No2 != null)
                {
                    page.SetYesNoDefaultOnLoad((dynamic)info.No2, 3);
                }
            }

            page.HideAllExceptRows(Rows.YesNoQuestion, Rows.Description);

            return page;
        }

        public static GenericPage Create(ConditionalYesNoQuestionInfo info, Action<GenericPage, object[]> onLoadAction)
        {
            var page = Create(info);
            page.OnLoadAction = onLoadAction;
            return page;
        }

        private void SetYesNoDefaultOnLoad(TextQuestionInfo info, int index)
        {
            var defaultText = info?.DefaultResponse;
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
            AddOnLoadAction(Action);
        }

        private void SetYesNoDefaultOnLoad(ComboQuestionInfo info, int index)
        {
            var defaultResponse = info?.DefaultResponse;
            var defaultResultIndex = info?.DefaultSaveResultIndex;

            if (defaultResponse == null
                && defaultResultIndex == null)
            {
                DefaultYesNoText[index] = string.Empty;
                return;
            }

            void Action(GenericPage page, object[] saveResults)
            {
                var defaultText = defaultResponse == null
                    ? GetSaveResultsFormat(defaultResultIndex.GetValueOrDefault())
                    : defaultResponse.ToString();
                DefaultYesNoText[index] = GetDefaultValue(defaultText, saveResults);
            }
            AddOnLoadAction(Action);
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

        private void DisplayRowsForYesNoValue(QuestionInfo info, QuestionInfo info2, int defaultTextIndex)
        {
            var rows = new List<int> { Rows.YesNoQuestion, Rows.Description };
            if (info == null)
            {
                DescriptionText.Text = string.Empty;
            }
            else
            {
                var text = DefaultYesNoText[defaultTextIndex];
                rows.Add(Rows.Question);
                rows.Add(SetValues((dynamic)info));
                ResponseText.Text = text;
                Path.Text = text;
                if (int.TryParse(text, out var value))
                {
                    Combo.SelectedValue = value;
                }

            }

            if (info2 != null)
            {
                var text = DefaultYesNoText[defaultTextIndex+1];
                rows.Add(Rows.Question2);
                rows.Add(SetValues2((dynamic)info2));
                Response2Text.Text = text;
                Path2.Text = text;
                if (int.TryParse(text, out var value))
                {
                    Combo2.SelectedValue = value;
                }
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

            if (filter == "Folder")
            {
                var fileName = System.IO.Path.GetFileName(path.Text);
                var folder = Directory.Exists(path.Text)
                    ? path.Text
                    : File.Exists(path.Text)
                        ? System.IO.Path.GetDirectoryName(path.Text)
                        : path.Text;

                while (!string.IsNullOrWhiteSpace(folder))
                {
                    if (Directory.Exists(folder))
                    {
                        break;
                    }
                    folder = System.IO.Path.GetDirectoryName(folder);
                }

                if (string.IsNullOrWhiteSpace(folder))
                {
                    folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                }
                var dlg = new FolderPicker
                {
                    InputPath = folder
                };

                if (dlg.ShowDialog() == true)
                {
                    path.Text = System.IO.Path.Combine(dlg.ResultPath, fileName.EndsWith(".sln")
                        ? fileName
                        : "YourCompanyAbbreviation.Dataverse.sln");
                }
            }
            else
            {
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
}
