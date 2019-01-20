using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Source.DLaB.Xrm;
using DLaB.Xrm.Entities;
using DLaB.XrmToolBoxCommon;
using XrmLabel = Microsoft.Xrm.Sdk.Label;
using Microsoft.Xrm.Sdk.Metadata;
using XrmToolBox.Extensibility;

namespace DLaB.AttributeManager
{
    partial class AttributeManagerPlugin
    {
        private List<Publisher> Publishers { get; set; }
        private List<OptionMetadata> LocalOptions { get; set; }
        private bool DefaultTwoOptionSetValue => ((ObjectCollectionItem<OptionMetadata>)optAttDefaultValueCmb.SelectedItem).Value.Value == 1;
        private OptionMetadata TrueOption { get; set; }
        private OptionMetadata FalseOption { get; set; }
        private ObjectCollectionItem<OptionMetadata> SelectedOption => CmbAttLocalOptionSet.SelectedItem as ObjectCollectionItem<OptionMetadata>;
        private int OptionSetPrefix { get; set; }

        private void ShowLocalOptionSet(bool isBooleanOptionSet)
        {
            var isMultipleOptionSet = !isBooleanOptionSet;
            if (isMultipleOptionSet && Publishers.Count == 0)
            {
                ExecuteMethod(LoadPublishers);
            }
            else
            {
                PnlLocalOptionSet.Visible = true;
            }

            CmbAttLocalOptionSet.Items.Clear();
            optAttDefaultValueCmb.Items.Clear();
            LocalOptions = new List<OptionMetadata>();
            TrueOption = null;
            FalseOption = null;
            CmbPublisher.Visible = isMultipleOptionSet;
            PicAttLocalOptionAdd.Visible = isMultipleOptionSet;
            PicAttLocalOptionDelete.Visible = isMultipleOptionSet;
            PicAttLocalOptionSortAsc.Visible = isMultipleOptionSet;
            PicAttLocalOptionSortDesc.Visible = isMultipleOptionSet;
            LblOptionSetDescription.Visible = isMultipleOptionSet;
            TxtOptionSetValue.Enabled = isMultipleOptionSet;
            TxtOptionSetDescription.Visible = isMultipleOptionSet;

            if (isBooleanOptionSet)
            {
                var item = CreateOptionMetadataItem("No", 0);
                CmbAttLocalOptionSet.Items.Add(item);
                optAttDefaultValueCmb.Items.Add(item);
                optAttDefaultValueCmb.SelectedIndex = 0;
                FalseOption = item.Value;
                item = CreateOptionMetadataItem("Yes", 1);
                CmbAttLocalOptionSet.Items.Add(item);
                TrueOption = item.Value;
                CmbAttLocalOptionSet.SelectedIndex = 0;
            }
            else
            {
                optAttDefaultValueCmb.Items.Add(CreateOptionMetadataItem("Unassigned Value", null));
            }
        }

        private static ObjectCollectionItem<OptionMetadata> CreateOptionMetadataItem(string label, int? value, string color = "#0000ff", string description = null)
        {
            var option = new OptionMetadata
            {
                Value = value.GetValueOrDefault(),
                Label = new XrmLabel(label, 1033),
                Color = color,
                Description = description == null ? null : new Microsoft.Xrm.Sdk.Label(description, 1033)
                
            };
            return new ObjectCollectionItem<OptionMetadata>(label, value.HasValue ? option : null);
        }

        /// <summary>
        /// Loads the publishers.
        /// </summary>
        public void LoadPublishers()
        {
            Enabled = false;

            WorkAsync(new WorkAsyncInfo("Loading Publishers...", (w, e) =>
            {
                try
                {
                    e.Result = Service.GetEntities<Publisher>();
                }
                catch (InvalidOperationException ex)
                {
                    w.ReportProgress(int.MinValue, ex);
                    e.Result = false;
                }
                catch (Exception ex)
                {
                    w.ReportProgress(int.MinValue, ex);
                    e.Result = false;
                }

            })
            {
                PostWorkCallBack = e =>
                {
                    Publishers = (List<Publisher>) e.Result;
                    
                    CmbPublisher.Items.Clear();
                    CmbPublisher.LoadItems(Publishers.Select(p => (object)new ObjectCollectionItem<Publisher>($"{p.FriendlyName} ({p.UniqueName})", p)).ToArray());
                    CmbPublisher.Text = string.Empty;
                    PnlLocalOptionSet.Visible = true;
                    Enabled = true;
                }
            });
        }

        private string GetText(object sender)
        {
            string text = null;
            if (SelectedOption?.Value != null)
            {
                text = ((TextBox)sender).Text;
            }
            return text;
        }

        private void PicAttLocalOptionAdd_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"Not Supported!");
        }

        private void PicAttLocalOptionDelete_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"Not Supported!");
        }

        private void PicAttLocalOptionMoveUp_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"Not Supported!");
        }

        private void PicAttLocalOptionMoveDown_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"Not Supported!");
        }

        private void PicAttLocalOptionSortAsc_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"Not Supported!");
        }

        private void PicAttLocalOptionSortDesc_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"Not Supported!");
        }

        private void CmbPublisher_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selected = CmbPublisher.SelectedItem as ObjectCollectionItem<Publisher>;
            if (selected?.Value == null)
            {
                return;
            }

            OptionSetPrefix = selected.Value.CustomizationOptionValuePrefix.GetValueOrDefault(10000);
            var nextValue = OptionSetPrefix*10000 + 1;

            while (LocalOptions.Any(o => o.Value.GetValueOrDefault() == nextValue))
            {
                nextValue++;
            }

            TxtOptionSetValue.Text = nextValue.ToString();
        }

        private void CmbAttLocalOptionSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selected = CmbAttLocalOptionSet.SelectedItem as ObjectCollectionItem<OptionMetadata>;
            if (selected?.Value == null)
            {
                return;
            }

            var option = selected.Value;
            TxtOptionSetLabel.Text = option.Label.GetLocalOrDefaultText();
            TxtOptionSetColor.Text = option.Color;
            if (option.Description != null)
            {
                TxtOptionSetDescription.Text = option.Description?.GetLocalOrDefaultText();
            }
            TxtOptionSetValue.Text = option.Value?.ToString();
        }

        private void TxtOptionSetLabel_TextChanged(object sender, EventArgs e)
        {
            var text = GetText(sender);
            if (text != null)
            {
                SelectedOption.DisplayName = text;
                SelectedOption.Value.Label = new XrmLabel(text, 1033);
            }
        }


        private void TxtOptionSetValue_TextChanged(object sender, EventArgs e)
        {
            var text = GetText(sender);
            if (text != null)
            {
                SelectedOption.Value.Value = int.Parse(text);
            }
        }

        private void TxtOptionSetColor_TextChanged(object sender, EventArgs e)
        {
            var text = GetText(sender);
            if (text != null)
            {
                SelectedOption.Value.Color = text;
            }
        }

        private void TxtOptionSetDescription_TextChanged(object sender, EventArgs e)
        {
            var text = GetText(sender);
            if (text != null)
            {
                SelectedOption.Value.Description = new XrmLabel(text, 1033);
            }
        }

        private void ImageButton_MouseEnter(object sender, EventArgs e)
        {
            SetBackgroundColor(sender, Color.LightBlue, Color.RoyalBlue);
        }

        private void SetBackgroundColor(object sender, Color color, Color colorIfMouseDown)
        {
            var image = sender as PictureBox;
            if (image == null)
            {
                return;
            }
            image.BackColor = System.Windows.Input.Mouse.LeftButton == System.Windows.Input.MouseButtonState.Pressed ? colorIfMouseDown : color;
        }

        private void ImageButton_MouseLeave(object sender, EventArgs e)
        {
            SetBackgroundColor(sender, Color.Transparent, Color.LightBlue);
        }

        private void ImageButton_MouseUp(object sender, MouseEventArgs e)
        {
            SetBackgroundColor(sender, Color.Transparent, Color.Transparent);
        }

        private void ImageButton_MouseDown(object sender, MouseEventArgs e)
        {
            SetBackgroundColor(sender, Color.RoyalBlue, Color.RoyalBlue);
        }
    }
}
