using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using DLaB.Common;
using DLaB.Xrm;
using DLaB.XrmToolboxCommon;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace DLaB.AttributeManager
{
    public partial class AttributeManager : PluginControlBase
    {
        private bool AttributesNeedLoaded { get; set; }
        private Object[] RenameSteps { get; set; }
        private Object[] DefaultSteps { get; set; }
        private Dictionary<string, Logic.Steps> StepMapper { get; set; }
        public Config Settings { get; set; }
        private EntityMetadata Metadata { get; set; }

        public AttributeManager()
        {
            InitializeComponent();

            StepMapper = new Dictionary<string, Logic.Steps>
            {
                {"Create Temporary Attribute", Logic.Steps.CreateTemp},
                {"Migrate to Temporary Attribute", Logic.Steps.MigrateToTemp},
                {"Remove Existing Attribute", Logic.Steps.RemoveExistingAttribute},
                {"Create New Attribute", Logic.Steps.CreateNewAttribute},
                {"Migrate to New Attribute", Logic.Steps.MigrateToNewAttribute},
                {"Remove Temporary Attribute", Logic.Steps.RemoveTemp}
            };

            DefaultSteps = StepMapper.Keys.Cast<Object>().ToArray();

            RenameSteps = new Object[]
            {
                StepMapper.First(p => p.Value == Logic.Steps.CreateNewAttribute).Key,
                StepMapper.First(p => p.Value == Logic.Steps.MigrateToNewAttribute).Key,
                StepMapper.First(p => p.Value == Logic.Steps.RemoveExistingAttribute).Key
            };

            lblNewAttributeType.Visible = false;
            cmbNewAttributeType.Visible = false;
            tabAttributeType.Visible = false;
        }

        public override void ClosingPlugin(PluginCloseInfo info)
        {
            base.ClosingPlugin(info);
            if (info.Cancel) return;

            Settings.Save();
        }

        public void LoadEntities()
        {
            Enabled = false;
            WorkAsync("Retrieving Entities...", e =>
            {
                e.Result = Service.Execute(new RetrieveAllEntitiesRequest { EntityFilters = EntityFilters.Entity, RetrieveAsIfPublished = true });
            }, e =>
            {
                cmbEntities.BeginUpdate();
                cmbEntities.Items.Clear();

                var result = ((RetrieveAllEntitiesResponse)e.Result).EntityMetadata.
                    Select(m => new ObjectCollectionItem<EntityMetadata>(m.DisplayName.GetLocalOrDefaultText("N/A") + " (" + m.LogicalName + ")",m)).
                    OrderBy(r => r.DisplayName);

                cmbEntities.Items.AddRange(result.Cast<Object>().ToArray());
                cmbEntities.EndUpdate();
                Enabled = true;
            });
        }

        public void LoadAttributes()
        {
            if (!AttributesNeedLoaded)
            {
                return;
            }

            AttributesNeedLoaded = false;

            var entity = cmbEntities.SelectedItem as ObjectCollectionItem<EntityMetadata>;
            if(entity == null){
                return;
            }

            Enabled = false;

            WorkAsync("Retrieving Attributes...", e =>
            {
                e.Result = Service.Execute(new RetrieveEntityRequest
                {
                    LogicalName = entity.Value.LogicalName,
                    EntityFilters = EntityFilters.Attributes | EntityFilters.Relationships,
                    RetrieveAsIfPublished = true
                });
            }, e =>
            {
                Metadata = ((RetrieveEntityResponse)e.Result).EntityMetadata;
                var attributes = Metadata.Attributes.Where(a => a.IsManaged == false && a.AttributeOf == null).
                    Select(a => new ObjectCollectionItem<AttributeMetadata>((a.DisplayName.GetLocalOrDefaultText("N/A")) + " (" + a.LogicalName + ")", a)).
                    OrderBy(r => r.DisplayName).
                    Cast<Object>().
                    ToArray();

                cmbAttributes.LoadItems(attributes);
                cmbNewAttribute.LoadItems(attributes);
                Enabled = true;
            });
        }

        #region Steps

        private void CheckAllSteps()
        {
            clbSteps.ItemCheck -= clbSteps_ItemCheck;
            for (int i = 0; i < clbSteps.Items.Count; i++)
            {
                clbSteps.SetItemChecked(i, true);
            }
            clbSteps.ItemCheck += clbSteps_ItemCheck;
        }

        public void ExecuteSteps()
        {
            Enabled = false;
            var attribute = cmbAttributes.SelectedItem as ObjectCollectionItem<AttributeMetadata>;
            if(attribute == null)
            {
                MessageBox.Show("No Attribute Selected!", "Unable To Execute", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Enabled = true;
                return;
            }

            var newName = txtNewAttributeName.Text;
            if (String.IsNullOrWhiteSpace(newName) || !newName.Contains('_'))
            {
                MessageBox.Show("Invalid new Schema Name!  Schema name must contain an '_'.", "Unable To Execute", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Enabled = true;
                return;
            }
            
            Logic.Steps steps = clbSteps.CheckedItems.Cast<string>().Aggregate<string, Logic.Steps>(0, (current, item) => current | StepMapper[item]);

            if(steps == 0){
                MessageBox.Show("No Steps Selected!", "Unable To Execute", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Enabled = true;
                return;
            }

            Logic.LogHandler onLog;
            var action = GetCurrentAction();
            var newType = GetNewAttributeType();

            // Update Display Name
            var langCode = attribute.Value.DisplayName.UserLocalizedLabel.LanguageCode;
            attribute.Value.DisplayName.LocalizedLabels.First(l => l.LanguageCode == langCode).Label = txtDisplayName.Text;

            WorkAsync("Performing Steps...", (w,e) => 
            {
                var migrator = new Logic(Service, ConnectionDetail, Metadata, Settings.TempSchemaPostfix, chkMigrate.Checked);
                onLog = m => w.ReportProgress(0, m);
                migrator.OnLog += onLog;
                try
                {
                    migrator.Run(attribute.Value, txtNewAttributeName.Text, steps, action, newType);
                    w.ReportProgress(99, "Steps Completed!");
                }
                catch (InvalidOperationException ex)
                {
                    w.ReportProgress(int.MinValue, ex);
                }
                catch (Exception ex)
                {
                    w.ReportProgress(int.MinValue, ex);
                }
                finally
                {
                    migrator.OnLog -= onLog;
                }

            }, e =>
            {
                Enabled = true;
            }, e => {             
                var text = e.UserState.ToString();
                        
                if (e.ProgressPercentage != int.MinValue)
                {
                    var state = e.UserState as Exception;
                    SetWorkingMessage(state != null ? state.Message : text);
                }
                txtLog.AppendText(text + Environment.NewLine);
            });
        }

        private AttributeMetadata GetNewAttributeType()
        {
            AttributeMetadata att;
            switch (cmbNewAttributeType.SelectedItem as string)
            {
                case "Single Line of Text":
                    att = NewTypeAttributeCreationLogic.CreateText();
                    break;
                case "Option Set":
                    att = NewTypeAttributeCreationLogic.CreateOptionSet(null);
                    break;
                case "Two Options":
                    att = NewTypeAttributeCreationLogic.CreateTwoOptions(null);
                    break;
                case "Image":
                    att = NewTypeAttributeCreationLogic.CreateImage(null);
                    break;
                case "Whole Number":
                    att = NewTypeAttributeCreationLogic.CreateWholeNumber();
                    break;
                case "Floating Point Number":
                    att = NewTypeAttributeCreationLogic.CreateFloatingPoint();
                    break;
                case "Decimal Number":
                    att = NewTypeAttributeCreationLogic.CreateDecimal();
                    break;
                case "Currency":
                    att = NewTypeAttributeCreationLogic.CreateCurrency();
                    break;
                case "Multiple Lines of Text":
                    att = NewTypeAttributeCreationLogic.CreateText(2000, StringFormat.TextArea);
                    break;
                case "Date and Time":
                    att = NewTypeAttributeCreationLogic.CreateDateTime();
                    break;
                case "Lookup":
                    att = NewTypeAttributeCreationLogic.CreateLookup(null);
                    break;
                default:
                    throw new Exception("Unepxected Type: " + cmbNewAttributeType.SelectedItem);
            
            }

            return att;
        }

        #endregion Steps

        private void HideNewAttribute(bool isVisible)
        {
            lblNewAttribute.Visible = isVisible;
            cmbNewAttribute.Visible = isVisible;
            lblSchemaName.Visible = !isVisible;
            txtNewAttributeName.Visible = !isVisible;
        }

        private Logic.Action GetCurrentAction()
        {
            var item = cmbAttributes.SelectedItem as ObjectCollectionItem<AttributeMetadata>;
            Logic.Action action;
            //if (item != null && item.Value.SchemaName.EndsWith(Settings.TempSchemaPostfix))
            //{
            //    action = Logic.Action.RemoveTemp;
            //}
            //else 
            if (item != null && String.Equals(txtNewAttributeName.Text, item.Value.SchemaName, StringComparison.OrdinalIgnoreCase))
            {
                action = Logic.Action.ChangeCase;
            }
            else
            {
                action = Logic.Action.Rename;
            }

            if (chkConvertAttributeType.Checked)
            {
                action |= Logic.Action.ChangeType;
            }

            return action;
        }

        #region Event Handlers

        private void btnLoadEntities_Click(object sender, EventArgs e)
        {
            ExecuteMethod(LoadEntities);
        }

        private void tsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

        #region cmbEntities

        private void cmbEntities_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            AttributesNeedLoaded = true;
            cmbAttributes.Items.Clear();
            cmbAttributes.SelectedItem = null;
            cmbAttributes.ResetText();
            cmbNewAttribute.Items.Clear();
            cmbNewAttribute.SelectedItem = null;
            cmbNewAttribute.ResetText();
        }

        private void cmbEntities_Leave(object sender, EventArgs e)
        {
            ExecuteMethod(LoadAttributes);
        }

        #endregion // cmbEntities

        private void AttributeManager_Load(object sender, EventArgs e)
        {
            HideNewAttribute(false);
            CheckAllSteps();
            AttributesNeedLoaded = false;
            Settings = Config.Load(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        }

        #region cmbAttributes

        private void cmbAttributes_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = cmbAttributes.SelectedItem as ObjectCollectionItem<AttributeMetadata>;
            if (item == null)
            {
                HideNewAttribute(true);
            }
            else
            {
                HideNewAttribute(GetCurrentAction().HasFlag(Logic.Action.RemoveTemp));
                txtNewAttributeName.Text = item.Value.SchemaName;
                txtDisplayName.Text = item.Value.DisplayName.UserLocalizedLabel.Label;
            }

            btnExecuteSteps.Enabled = cmbAttributes.SelectedText == String.Empty;
        }

        private void cmbAttributes_MouseEnter(object sender, EventArgs e)
        {
            ExecuteMethod(LoadAttributes);
        }

        #endregion // cmbAttributes

        private void chkConvertAttributeType_CheckedChanged(object sender, EventArgs e)
        {
            var visible = chkConvertAttributeType.Checked;
            lblNewAttributeType.Visible = visible;
            cmbNewAttributeType.Visible = visible;
            tabAttributeType.Visible = false;
            UpdateDisplayedSteps();
        }

        private void cmbNewAttribute_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = cmbNewAttribute.SelectedItem as ObjectCollectionItem<AttributeMetadata>;

            //   Hide New Attribute Name Text Field
            //   Populate the New Attribute Name Text Field
            if (item == null || ! GetCurrentAction().HasFlag(Logic.Action.RemoveTemp))
            {
                return;
            }

            txtNewAttributeName.Text = item.Value.SchemaName;
        }

        private void clbSteps_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            clbSteps.ItemCheck -= clbSteps_ItemCheck;
            if (e.NewValue == CheckState.Unchecked)
            {
                if (e.Index != 0 && clbSteps.GetItemChecked(e.Index - 1))
                {
                    for (int i = e.Index + 1; i < clbSteps.Items.Count; i++)
                    {
                        clbSteps.SetItemChecked(i, false);
                    }
                }
            }
            else if (e.NewValue == CheckState.Checked)
            {
                bool hasPostUnchecked = false;
                for (int i = e.Index + 1; i < clbSteps.Items.Count; i++)
                {
                    if (!clbSteps.GetItemChecked(i))
                    {
                        hasPostUnchecked = true;
                    }
                    else if (hasPostUnchecked)
                    {
                        // Check previous
                        for (int p = e.Index + 1; p < i; p++)
                        {
                            clbSteps.SetItemChecked(p, true);
                        }
                        break;
                    }
                }

                bool hasPreUnchecked = false;
                for (int i = e.Index - 1; i >= 0; i--)
                {
                    if (!clbSteps.GetItemChecked(i))
                    {
                        hasPreUnchecked = true;
                    }
                    else if (hasPreUnchecked)
                    {
                        // Check previous
                        for (int p = i; p < e.Index; p++)
                        {
                            clbSteps.SetItemChecked(p, true);
                        }
                        break;
                    }
                }
            }
            clbSteps.ItemCheck += clbSteps_ItemCheck;
        }

        private void btnExecuteSteps_Click(object sender, EventArgs e)
        {
            ExecuteMethod(ExecuteSteps);
        }

        private void txtNewAttributeName_TextChanged(object sender, EventArgs e)
        {
            UpdateDisplayedSteps();
        }

        private void UpdateDisplayedSteps()
        {
          var item = cmbAttributes.SelectedItem as ObjectCollectionItem<AttributeMetadata>;
            if (item == null)
            {
                return;
            }

            var action = GetCurrentAction();
            if (txtNewAttributeName.Text == item.Value.SchemaName && !action.HasFlag(Logic.Action.ChangeType))
            {
                // Exactly Equal
                if (action.HasFlag(Logic.Action.RemoveTemp))
                {
                    clbSteps.LoadItems(RenameSteps);
                } 
                else
                {
                    clbSteps.Items.Clear();
                }
            }
            else if (action.HasFlag(Logic.Action.ChangeCase))
            {
                // Change Case
                clbSteps.LoadItems(DefaultSteps);
            }
            else if (action.HasFlag(Logic.Action.RemoveTemp))
            {
                // Partial Completion.  Need to allow for Migrate to Temp and Remove
                var steps = StepMapper.Where(p => p.Value == Logic.Steps.MigrateToNewAttribute || p.Value == Logic.Steps.RemoveTemp).Select(v => v.Key);
                clbSteps.LoadItems(steps.ToObjectArray());
            }
            else if (clbSteps.Items.Count != RenameSteps.Length)
            {
                // Rename
                clbSteps.LoadItems(RenameSteps);
            }

            CheckAllSteps();
        }

        #endregion // Event Handlers
    }

    [Export(typeof(IXrmToolBoxPlugin)),
     ExportMetadata("Name", "Attribute Manager"),
     ExportMetadata("Description", "Renames an attribute for an Entity, optionally preserving Data as well."),
     ExportMetadata("SmallImageBase64", SmallImage32X32), // null for "no logo" image or base64 image content 
     ExportMetadata("BigImageBase64", LargeImage120X120), // null for "no logo" image or base64 image content 
     ExportMetadata("BackgroundColor", "White"), // Use a HTML color name
     ExportMetadata("PrimaryFontColor", "#000000"), // Or an hexadecimal code
     ExportMetadata("SecondaryFontColor", "DarkGray")]
    public class EarlyBoundGenerator : PluginFactory
    {
        public override IXrmToolBoxPluginControl GetControl()
        {
            return new AttributeManager();
        }
    }
}
