using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DLaB.Xrm;
using DLaB.Xrm.Entities;
using DLaB.XrmToolboxCommon;
using Microsoft.Xrm.Sdk.Metadata;
using XrmToolBox.Extensibility;

namespace DLaB.AttributeManager
{
    partial class AttributeManagerPlugin
    {
        private List<Publisher> Publishers { get; set; }
        private List<OptionMetadata> LocalOptions { get; set; }
        private OptionMetadata TrueOption { get; set; }
        private OptionMetadata FalseOption { get; set; }
        private int OptionSetPrefix { get; set; }

        private void ShowLocalOptionSet()
        {
            if (Publishers.Count == 0)
            {
                ExecuteMethod(LoadPublishers);
            }
            else
            {
                PnlLocalOptionSet.Visible = true;
            }
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


        private void PicAttLocalOptionAdd_Click(object sender, EventArgs e)
        {

        }

        private void PicAttLocalOptionDelete_Click(object sender, EventArgs e)
        {

        }

        private void PicAttLocalOptionMoveUp_Click(object sender, EventArgs e)
        {

        }

        private void PicAttLocalOptionMoveDown_Click(object sender, EventArgs e)
        {

        }

        private void PicAttLocalOptionSortAsc_Click(object sender, EventArgs e)
        {

        }

        private void PicAttLocalOptionSortDesc_Click(object sender, EventArgs e)
        {

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
