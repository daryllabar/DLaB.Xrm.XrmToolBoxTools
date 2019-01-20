using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Source.DLaB.Common;
using Source.DLaB.Common.Exceptions;
using Source.DLaB.Xrm;
using DLaB.Xrm.Entities;
using DLaB.XrmToolBoxCommon;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace DLaB.AttributeManager
{
    public partial class AttributeManagerPlugin : DLaBPluginControlBase
    {
        private bool AttributesNeedLoaded { get; set; }
        private object[] DeleteSteps { get; }

        private object[] RenameSteps
        {
            get
            {
                return chkMigrate.Checked
                    ? new object[]  {
                            StepMapper.First(p => p.Value == Logic.Steps.CreateNewAttribute).Key,
                            StepMapper.First(p => p.Value == Logic.Steps.MigrateDataToNewAttribute).Key,
                            StepMapper.First(p => p.Value == Logic.Steps.MigrateToNewAttribute).Key,
                            StepMapper.First(p => p.Value == Logic.Steps.RemoveExistingAttribute).Key
                    }
                    : new object[] {
                        StepMapper.First(p => p.Value == Logic.Steps.CreateNewAttribute).Key,
                        StepMapper.First(p => p.Value == Logic.Steps.MigrateToNewAttribute).Key,
                        StepMapper.First(p => p.Value == Logic.Steps.RemoveExistingAttribute).Key
                    };
            }
        }

        private Dictionary<string, Logic.Steps> StepMapper { get; }
        public Config Settings { get; set; }
        private EntityMetadata Metadata { get; set; }
        private IEnumerable<OptionSetMetadataBase> OptionSetsMetadata { get; set; }

        public AttributeManagerPlugin()
        {
            InitializeComponent();
            Publishers = new List<Publisher>();
            LocalOptions = new List<OptionMetadata>();

            StepMapper = new Dictionary<string, Logic.Steps>
            {
                {"Create Temporary Attribute", Logic.Steps.CreateTemp},
                {"Migrate Data to Temporary Attribute", Logic.Steps.MigrateDataToTemp},
                {"Migrate to Temporary Attribute", Logic.Steps.MigrateToTemp},
                {"Remove Existing Attribute", Logic.Steps.RemoveExistingAttribute},
                {"Create New Attribute", Logic.Steps.CreateNewAttribute},
                {"Migrate Data to New Attribute", Logic.Steps.MigrateDataToNewAttribute},
                {"Migrate to New Attribute", Logic.Steps.MigrateToNewAttribute},
                {"Remove Temporary Attribute", Logic.Steps.RemoveTemp}
            };

            DeleteSteps = new object[]
            {
                StepMapper.First(p => p.Value == Logic.Steps.RemoveExistingAttribute).Key
            };

            cmbNewAttributeType.Visible = false;
            SetTabVisible(tabStringAttribute, false);
            SetTabVisible(tabNumberAttribute, false);
            SetTabVisible(tabOptionSetAttribute, false);
            SetTabVisible(tabDelete, false);
            SetMappingFileVisible();
        }

        #region XrmToolBox Menu Interfaces

        #region IPayPalPlugin

        public override string DonationDescription => "Support Development for the Attribute Manager!";

        #endregion IPayPalPlugin

        #region IHelpPlugin

        public override string HelpUrl => "https://github.com/daryllabar/DLaB.Xrm.XrmToolBoxTools/wiki/Attribute-Manager";

        #endregion IHelpPlugin

        #endregion XrmToolBox Menu Interfaces

        public override void ClosingPlugin(PluginCloseInfo info)
        {
            base.ClosingPlugin(info);
            if (info.Cancel) return;

            Settings.Save();
        }

        public void LoadEntities()
        {
            Enabled = false;
            WorkAsync(new WorkAsyncInfo("Retrieving Entities...",
                e =>
                {
                    e.Result = Service.Execute(new RetrieveAllEntitiesRequest() {EntityFilters = EntityFilters.Entity, RetrieveAsIfPublished = true});
                })
            {
                PostWorkCallBack = e =>
                {
                    cmbEntities.BeginUpdate();
                    cmbEntities.Items.Clear();
                    OptionSetsMetadata = null; // Clear to be loaded again

                    var result = ((RetrieveAllEntitiesResponse) e.Result).EntityMetadata.
                        Select(m => new ObjectCollectionItem<EntityMetadata>(m.DisplayName.GetLocalOrDefaultText("N/A") + " (" + m.LogicalName + ")", m)).
                        OrderBy(r => r.DisplayName);

                    cmbEntities.Items.AddRange(result.Cast<object>().ToArray());
                    cmbEntities.EndUpdate();
                    cmbEntities.Enabled = true;
                    cmbAttributes.Enabled = true;
                    Enabled = true;
                }
            });
        }

        public void LoadAttributes()
        {
            if (!AttributesNeedLoaded)
            {
                return;
            }

            AttributesNeedLoaded = false;

            if (!(cmbEntities.SelectedItem is ObjectCollectionItem<EntityMetadata> entity))
            {
                return;
            }

            Enabled = false;

            WorkAsync(new WorkAsyncInfo("Retrieving Attributes...", e =>
            {
                e.Result = Service.Execute(new RetrieveEntityRequest
                {
                    LogicalName = entity.Value.LogicalName,
                    EntityFilters = EntityFilters.Attributes | EntityFilters.Relationships,
                    RetrieveAsIfPublished = true
                });
            })
            {
                PostWorkCallBack = e =>
                {
                    Metadata = ((RetrieveEntityResponse) e.Result).EntityMetadata;
                    var attributes = Metadata.Attributes.Where(a => 
                            !a.IsManaged.GetValueOrDefault() && 
                            a.AttributeOf == null && 
                            a.IsCustomizable.Value &&
                            !a.IsPrimaryId.GetValueOrDefault() &&
                            !IsBaseCurrency(a)).
                        Select(a => new ObjectCollectionItem<AttributeMetadata>((a.DisplayName.GetLocalOrDefaultText("N/A")) + " (" + a.LogicalName + ")", a)).
                        OrderBy(r => r.DisplayName).
                        Cast<object>().
                        ToArray();

                    cmbAttributes.LoadItems(attributes);
                    cmbAttributes.Text = string.Empty;
                    cmbNewAttribute.LoadItems(attributes);
                    cmbNewAttribute.Text = string.Empty;
                    txtOldSchema.Text = string.Empty;
                    txtOldDisplay.Text = string.Empty;
                    txtOldAttributType.Text = string.Empty;
                    txtNewAttributeName.Text = string.Empty;
                    txtDisplayName.Text = string.Empty;
                    chkDelete.Checked = false;
                    chkMigrate.Checked = true;
                    chkConvertAttributeType.Checked = false;
                    chkIgnoreUpdateErrors.Checked = false;
                    cmbNewAttributeType.SelectedIndex = -1;
                    Enabled = true;
                }
            });
        }

        private bool IsBaseCurrency(AttributeMetadata attribute)
        {
            return attribute.AttributeType == AttributeTypeCode.Money && ((MoneyAttributeMetadata) attribute).IsBaseCurrency.GetValueOrDefault(false);
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
            if (!(cmbAttributes.SelectedItem is ObjectCollectionItem<AttributeMetadata> attribute))
            {
                MessageBox.Show(@"No Attribute Selected!", @"Unable To Execute", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Enabled = true;
                return;
            }

            var newName = txtNewAttributeName.Text;
            if (string.IsNullOrWhiteSpace(newName) || !newName.Contains('_'))
            {
                MessageBox.Show(@"Invalid new Schema Name!  Schema name must contain an '_'.", @"Unable To Execute", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Enabled = true;
                return;
            }

            var steps = clbSteps.CheckedItems.Cast<string>().Aggregate<string, Logic.Steps>(0, (current, item) => current | StepMapper[item]);

            if (steps == 0)
            {
                MessageBox.Show(@"No Steps Selected!", @"Unable To Execute", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Enabled = true;
                return;
            }

            if (clbSteps.Items.Count == 6)
            {
                // Create of Temporary is required.  Add it to the steps
                steps |= Logic.Steps.MigrationToTempRequired;
            }

            if ((steps.HasFlag(Logic.Steps.CreateNewAttribute)
                    || steps.HasFlag(Logic.Steps.CreateTemp)) 
                && cmbNewAttributeType.Text == @"Global Option Set"
                && optAttGlobalOptionSetCmb.SelectedItem == null)
            {
                MessageBox.Show(@"You must select an existing Global Option Set Type in the Option Set Settings Tab!", @"Unable To Execute", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Enabled = true;
                return;
            }

            // Update Display Name
            var langCode = attribute.Value.DisplayName.UserLocalizedLabel.LanguageCode;
            attribute.Value.DisplayName.LocalizedLabels.First(l => l.LanguageCode == langCode).Label = txtDisplayName.Text;
            tabControl.SelectedTab = tabLog;

            WorkAsync(new WorkAsyncInfo("Performing Steps...", (w, e) =>
            {
                var info = (ExecuteStepsInfo) e.Argument;
                Logic.LogHandler onLog = m => w.ReportProgress(0, m);
                info.Migrator.OnLog += onLog;
                var result = new ExecuteStepsResult
                {
                    Steps = info.Steps,
                    Successful = false
                };
                e.Result = result;
                try
                {
                    info.Migrator.Run(info);
                    w.ReportProgress(99, "Steps Completed!");
                    result.Successful = true;
                }
                catch (Exception ex)
                {
                    w.ReportProgress(int.MinValue, ex);
                    result.Successful = false;
                }
                finally
                {
                    info.Migrator.OnLog -= onLog;
                }
            })
            {
                PostWorkCallBack = e =>
                {
                    var result = (ExecuteStepsResult) e.Result;
                    if (result.Steps.HasFlag(Logic.Steps.MigrateToNewAttribute) && result.Successful)
                    {
                        AttributesNeedLoaded = true;
                        ExecuteMethod(LoadAttributes);
                    }
                    Enabled = true;
                },
                ProgressChanged = e =>
                {
                    var text = e.UserState.ToString();

                    if (e.ProgressPercentage != int.MinValue)
                    {
                        var state = e.UserState as Exception;
                        SetWorkingMessage(state?.Message ?? text);
                    }
                    txtLog.AppendText(text + Environment.NewLine);
                },
                AsyncArgument = new ExecuteStepsInfo
                {
                    Action = GetCurrentAction(),
                    AutoRemovePluginRegistrationAssociations = delUpdatePlugins.Checked,
                    CurrentAttribute = attribute.Value,
                    NewAttribute = GetNewAttributeType(),
                    NewAttributeName = txtNewAttributeName.Text,
                    MappingFilePath = txtMappingFile.Text,
                    Migrator = new Logic(Service, ConnectionDetail, Metadata, Settings.TempSchemaPostfix, chkMigrate.Checked, chkIgnoreUpdateErrors.Checked),
                    Steps = steps
                }
            });
        }

        private AttributeMetadata GetNewAttributeType()
        {
            if (!chkConvertAttributeType.Checked)
            {
                return null;
            }

            int? precision = GetPrecisionSetting();

            AttributeMetadata att;
            switch (cmbNewAttributeType.Text)
            {
                case "Single Line of Text":
                    att = NewTypeAttributeCreationLogic.CreateText(int.Parse(strAttTxtMaximumLength.Text), GetStringFormat(), GetStringImeMode());
                    break;
                case "Global Option Set":
                    var optionSet = (ObjectCollectionItem<OptionSetMetadata>) optAttGlobalOptionSetCmb.SelectedItem;
                    var defaultValue = (ObjectCollectionItem<int?>) optAttDefaultValueCmb.SelectedItem;
                    att = NewTypeAttributeCreationLogic.CreateOptionSet(optionSet.Value, defaultValue?.Value);
                    break;
                case "Local Option Set":
                    // TODO Read from Tab
                    att = NewTypeAttributeCreationLogic.CreateOptionSet(null);
                    break;
                case "Two Options":
                    att = NewTypeAttributeCreationLogic.CreateTwoOptions(new BooleanOptionSetMetadata(TrueOption, FalseOption), DefaultTwoOptionSetValue);
                    break;
                case "Image":
                    att = NewTypeAttributeCreationLogic.CreateImage(null);
                    break;
                case "Whole Number":
                    int inttmp;
                    int? intmin = null;
                    int? intmax = null;
                    if (int.TryParse(numAttMinTxt.Text, out inttmp))
                    {
                        intmin = inttmp;
                    }
                    if (int.TryParse(numAttMaxTxt.Text, out inttmp))
                    {
                        intmax = inttmp;
                    }
                    att = NewTypeAttributeCreationLogic.CreateWholeNumber(GetIntergerFormat(), intmin, intmax);
                    break;
                case "Floating Point Number":
                    double dbltmp;
                    double? dblmin = null;
                    double? dblmax = null;
                    if (double.TryParse(numAttMinTxt.Text, out dbltmp))
                    {
                        dblmin = dbltmp;
                    }
                    if (double.TryParse(numAttMaxTxt.Text, out dbltmp))
                    {
                        dblmax = dbltmp;
                    }
                    att = NewTypeAttributeCreationLogic.CreateFloatingPoint(dblmin, dblmax, precision);
                    break;
                case "Decimal Number":
                    decimal dectmp;
                    decimal? decmin = null;
                    decimal? decmax = null;
                    if (decimal.TryParse(numAttMinTxt.Text, out dectmp))
                    {
                        decmin = dectmp;
                    }
                    if (decimal.TryParse(numAttMaxTxt.Text, out dectmp))
                    {
                        decmax = dectmp;
                    }
                    att = NewTypeAttributeCreationLogic.CreateDecimal(decmin, decmax, precision);
                    break;
                case "Currency":
                    double curtmp;
                    double? curmin = null;
                    double? curmax = null;
                    if (double.TryParse(numAttMinTxt.Text, out curtmp))
                    {
                        curmin = curtmp;
                    }
                    if (double.TryParse(numAttMaxTxt.Text, out curtmp))
                    {
                        curmax = curtmp;
                    }
                    int? currencyPrecision = GetCurrencyPrecision();
                    att = NewTypeAttributeCreationLogic.CreateCurrency(curmin, curmax, currencyPrecision);
                    break;
                case "Multiple Lines of Text":
                    att = NewTypeAttributeCreationLogic.CreateMemo(int.Parse(strAttTxtMaximumLength.Text), GetStringImeMode());
                    break;
                case "Date and Time":
                    att = NewTypeAttributeCreationLogic.CreateDateTime();
                    break;
                case "Lookup":
                    att = NewTypeAttributeCreationLogic.CreateLookup(null);
                    break;
                case "":
                    att = null;
                    break;
                default:
                    throw new Exception("Unexpected Type: " + cmbNewAttributeType.Text);
            }

            return att;
        }

        #endregion Steps

        private void HideNewAttribute(bool isVisible)
        {
            lblNewAttribute.Visible = isVisible;
            cmbNewAttribute.Visible = isVisible;
            lblSchemaName.Visible = !isVisible;
            txtNewAttributeName.Visible = !isVisible && !chkDelete.Checked;
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
            if (chkDelete.Checked)
            {
                action = Logic.Action.Delete;
            }
            else if (item != null && string.Equals(txtNewAttributeName.Text, item.Value.SchemaName, StringComparison.OrdinalIgnoreCase))
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

        private StringFormatName GetStringFormat()
        {
            StringFormatName format;
            switch (strAttCmbFormat?.SelectedItem?.ToString() ?? "Text")
            {
                case "Email":
                    format = StringFormatName.Email;
                    break;
                case "Phone":
                    format = StringFormatName.Phone;
                    break;
                case "PhoneticGuide":
                    format = StringFormatName.PhoneticGuide;
                    break;
                case "Text":
                    format = StringFormatName.Text;
                    break;
                case "TextArea":
                    format = StringFormatName.TextArea;
                    break;
                case "TickerSymbol":
                    format = StringFormatName.TickerSymbol;
                    break;
                case "Url":
                    format = StringFormatName.Url;
                    break;
                case "VersionNumber":
                    format = StringFormatName.VersionNumber;
                    break;
                default:
                    throw new Exception("Unable to determine String Format for " + strAttCmbFormat?.SelectedText);
            }
            return format;
        }

        private Microsoft.Xrm.Sdk.Metadata.ImeMode GetStringImeMode()
        {
            var stringMode = strAttCmbImeMode?.SelectedItem?.ToString();
            Microsoft.Xrm.Sdk.Metadata.ImeMode mode;
            if (!Enum.TryParse(stringMode, out mode))
            {
                LogError("Unable to determine Ime Mode for " + stringMode);
                mode = Microsoft.Xrm.Sdk.Metadata.ImeMode.Auto;
            }
            return mode;
        }

        #region Event Handlers

        #region Number Type Settings Tab

        // ReSharper disable UnusedParameter.Local
        private void HideNumberAttributes()
        {
            numAttFormatLbl.Visible = false;
            numAttFormatCmb.Visible = false;
            numAttMaxLbl.Visible = false;
            numAttMaxTxt.Visible = false;
            numAttMinLbl.Visible = false;
            numAttMinTxt.Visible = false;
            numAttPrecisionLbl.Visible = false;
            numAttPrecisionTxt.Visible = false;
            numAttCurrencyPrecisionLbl.Visible = false;
            numAttCurrencyPrecisionCmb.Visible = false;
            numAttFormatCmb.SelectedIndex = -1;
            numAttMaxTxt.Text = "";
            numAttMinTxt.Text = "";
            numAttPrecisionTxt.Text = "";
            numAttCurrencyPrecisionCmb.SelectedIndex = -1;
        }
        
        private void SetDecimalNumberVisible()
        {
            HideNumberAttributes();
            numAttMaxLbl.Visible = true;
            numAttMaxTxt.Visible = true;
            numAttMinLbl.Visible = true;
            numAttMinTxt.Visible = true;
            numAttPrecisionLbl.Visible = true;
            numAttPrecisionTxt.Visible = true;
            numAttMinTxt.Text = DecimalAttributeMetadata.MinSupportedValue.ToString(CultureInfo.InvariantCulture);
            numAttMaxTxt.Text = DecimalAttributeMetadata.MaxSupportedValue.ToString(CultureInfo.InvariantCulture);
            numAttPrecisionTxt.Text = @"2";
        }

        private void SetCurrencyNumberVisible()
        {
            HideNumberAttributes();
            numAttMaxLbl.Visible = true;
            numAttMaxTxt.Visible = true;
            numAttMinLbl.Visible = true;
            numAttMinTxt.Visible = true;
            numAttCurrencyPrecisionLbl.Visible = true;
            numAttCurrencyPrecisionCmb.Visible = true;
            numAttMinTxt.Text = MoneyAttributeMetadata.MinSupportedValue.ToString(CultureInfo.InvariantCulture);
            numAttMaxTxt.Text = MoneyAttributeMetadata.MaxSupportedValue.ToString(CultureInfo.InvariantCulture);
            numAttCurrencyPrecisionCmb.SelectedIndex = 0;
        }

        private void SetFloatNumberVisible()
        {
            HideNumberAttributes();
            numAttMaxLbl.Visible = true;
            numAttMaxTxt.Visible = true;
            numAttMinLbl.Visible = true;
            numAttMinTxt.Visible = true;
            numAttPrecisionLbl.Visible = true;
            numAttPrecisionTxt.Visible = true;
            numAttMinTxt.Text = DoubleAttributeMetadata.MinSupportedValue.ToString(CultureInfo.InvariantCulture);
            numAttMaxTxt.Text = DoubleAttributeMetadata.MaxSupportedValue.ToString(CultureInfo.InvariantCulture);
            numAttPrecisionTxt.Text = @"2";
        }
        // ReSharper restore UnusedParameter.Local

        private void SetWholeNumberVisible()
        {
            HideNumberAttributes();
            numAttFormatLbl.Visible = true;
            numAttFormatCmb.Visible = true;
            numAttMaxLbl.Visible = true;
            numAttMaxTxt.Visible = true;
            numAttMinLbl.Visible = true;
            numAttMinTxt.Visible = true;
            numAttFormatCmb.SelectedIndex = 0;
            numAttMinTxt.Text = IntegerAttributeMetadata.MinSupportedValue.ToString(CultureInfo.InvariantCulture);
            numAttMaxTxt.Text = IntegerAttributeMetadata.MaxSupportedValue.ToString(CultureInfo.InvariantCulture);
        }

        private IntegerFormat GetIntergerFormat()
        {
            IntegerFormat format;

            switch (numAttFormatCmb.SelectedItem.ToString())
            {
                case "None":
                    format = IntegerFormat.None;
                    break;
                case "Duration":
                    format = IntegerFormat.Duration;
                    break;
                case "TimeZone":
                    format = IntegerFormat.TimeZone;
                    break;
                case "Language":
                    format = IntegerFormat.Language;
                    break;
                case "Locale":
                    format = IntegerFormat.Locale;
                    break;
                default:
                    throw new Exception("Unable to determine Integer Format for " + numAttFormatCmb.SelectedItem);
            }
            return format;
        }

        private int? GetCurrencyPrecision()
        {
            if (numAttCurrencyPrecisionCmb.SelectedIndex == -1)
            {
                return null;
            }
            else
            {
                int tmp;
                int? precision = null;
                if (int.TryParse((numAttCurrencyPrecisionCmb.SelectedItem.ToString()), out tmp))
                {
                    precision = tmp;
                }
                return precision;
            }
        }

        #endregion Number Type Settings Tab

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
                txtDisplayName.Text = item.Value.DisplayName.UserLocalizedLabel?.Label ?? item.Value.LogicalName;
                txtOldSchema.Text = txtNewAttributeName.Text;
                txtOldDisplay.Text = txtDisplayName.Text;
                txtOldAttributType.Text = GetAttributeTypeDisplayValue(item.Value);
                SetTabVisible(tabDelete, chkDelete.Checked);
            }

            btnExecuteSteps.Enabled = cmbAttributes.SelectedText == string.Empty;
        }

        private string GetAttributeTypeDisplayValue(AttributeMetadata attribute)
        {
            var displayValue = attribute.AttributeType.GetValueOrDefault().ToString();
            switch (attribute.AttributeType.GetValueOrDefault())
            {
                case AttributeTypeCode.Picklist:
                    var optionSetAtt = attribute as PicklistAttributeMetadata;
                    if (optionSetAtt?.OptionSet != null)
                    {
                        displayValue = (optionSetAtt.OptionSet.IsGlobal.GetValueOrDefault() ? "Global Option Set - " : "Local Option Set - ") + 
                            optionSetAtt.OptionSet.DisplayName.GetLocalOrDefaultText() + $" ({optionSetAtt.OptionSet.Name})";
                    }

                    break;

                case AttributeTypeCode.Boolean:
                case AttributeTypeCode.Customer:
                case AttributeTypeCode.DateTime:
                case AttributeTypeCode.Decimal:
                case AttributeTypeCode.Double:
                case AttributeTypeCode.Integer:
                case AttributeTypeCode.Lookup:
                case AttributeTypeCode.Memo:
                case AttributeTypeCode.Money:
                case AttributeTypeCode.Owner:
                case AttributeTypeCode.PartyList:
                case AttributeTypeCode.State:
                case AttributeTypeCode.Status:
                case AttributeTypeCode.String:
                case AttributeTypeCode.Uniqueidentifier:
                case AttributeTypeCode.CalendarRules:
                case AttributeTypeCode.Virtual:
                case AttributeTypeCode.BigInt:
                case AttributeTypeCode.ManagedProperty:
                case AttributeTypeCode.EntityName:
                    break;
                default:
                    throw new EnumCaseUndefinedException<AttributeTypeCode>(attribute.AttributeType.GetValueOrDefault());
            }

            return displayValue;
        }

        private void cmbAttributes_MouseEnter(object sender, EventArgs e)
        {
            if (cmbEntities.SelectedIndex > -1)
            {
                ExecuteMethod(LoadAttributes);
            }
        }

        #endregion // cmbAttributes

        private void chkConvertAttributeType_CheckedChanged(object sender, EventArgs e)
        {
            var visible = chkConvertAttributeType.Checked;
            cmbNewAttributeType.Visible = visible;
            if (visible)
            {
                if (!string.IsNullOrWhiteSpace(cmbNewAttributeType.Text))
                {
                    DisplayTabForNewAttributeSelected();
                }
            }
            else
            {
                HideAllTabs();
            }
            UpdateDisplayedSteps();
        }

        private void HideAllTabs()
        {
            SetTabVisible(tabStringAttribute, false);
            SetTabVisible(tabNumberAttribute, false);
            SetTabVisible(tabOptionSetAttribute, false);
            SetTabVisible(tabDelete, false);
        }

        private void chkDelete_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDelete.Checked)
            {
                chkConvertAttributeType.Checked = false;
                chkMigrate.Checked = false;
                HideAllTabs();
            }

            SetTabVisible(tabDelete, chkDelete.Checked);

            chkConvertAttributeType.Visible = !chkDelete.Checked;
            chkMigrate.Visible              = !chkDelete.Checked;
            lblNewAttributeGrid.Visible     = !chkDelete.Checked;
            txtNewAttributeName.Visible     = !chkDelete.Checked;
            txtDisplayName.Visible          = !chkDelete.Checked;
            cmbNewAttributeType.Visible     = !chkDelete.Checked;

            UpdateDisplayedSteps();
        }

        private void cmbNewAttribute_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = cmbNewAttribute.SelectedItem as ObjectCollectionItem<AttributeMetadata>;

            //   Hide New Attribute Name Text Field
            //   Populate the New Attribute Name Text Field
            if (item == null || !GetCurrentAction().HasFlag(Logic.Action.RemoveTemp))
            {
                return;
            }

            txtNewAttributeName.Text = item.Value.SchemaName;
        }

        private void cmbNewAttributeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayTabForNewAttributeSelected();
        }

        private void DisplayTabForNewAttributeSelected()
        {
            var stringTabVisible = false;
            var optionTabVisible = false;
            var numberTabVisible = false;
            if (cmbNewAttributeType.SelectedIndex == -1)
            {
                return;
            }
            switch (cmbNewAttributeType.Text)
            {
                case "Single Line of Text":
                    stringTabVisible = true;
                    strAttTxtMaximumLength.Text = @"100";
                    strAttCmbFormat.Text = @"Text";
                    strAttCmbFormat.Visible = true;
                    strAttLblFormat.Visible = true;
                    strAttCmbImeMode.Text = @"Auto";
                    break;
                case "Global Option Set":
                    optionTabVisible = true;
                    RetrieveOptionSets();
                    optAttGlobalOptionSetCmb.Visible = true;
                    optAttGlobalOptionSetLbl.Visible = true;
                    PnlLocalOptionSet.Visible = false;
                    break;
                case "Local Option Set":
                    optionTabVisible = true;
                    optAttGlobalOptionSetCmb.Visible = false;
                    optAttGlobalOptionSetLbl.Visible = false;
                    ShowLocalOptionSet(false);
                    break;
                case "Two Options":
                    optionTabVisible = true;
                    optAttGlobalOptionSetCmb.Visible = false;
                    optAttGlobalOptionSetLbl.Visible = false;
                    PnlLocalOptionSet.Visible = true;
                    ShowLocalOptionSet(true);
                    break;
                case "Image":

                    break;
                case "Whole Number":
                    numberTabVisible = true;
                    pNumberType.Visible = true;
                    SetWholeNumberVisible();
                    break;
                case "Floating Point Number":
                    numberTabVisible = true;
                    SetFloatNumberVisible();
                    break;
                case "Decimal Number":
                    numberTabVisible = true;
                    SetDecimalNumberVisible();
                    break;
                case "Currency":
                    numberTabVisible = true;
                    SetCurrencyNumberVisible();
                    break;
                case "Multiple Lines of Text":
                    stringTabVisible = true;
                    strAttTxtMaximumLength.Text = @"2000";
                    strAttCmbFormat.SelectedItem = "Text Area";
                    strAttCmbFormat.Visible = false;
                    strAttLblFormat.Visible = false;
                    strAttCmbImeMode.Text = @"Auto";
                    break;
                case "Date and Time":

                    break;
                case "Lookup":

                    break;
                default:
                    throw new Exception("Unepxected Type: " + cmbNewAttributeType.Text);
            }

            SetTabVisible(tabNumberAttribute, numberTabVisible);
            SetTabVisible(tabStringAttribute, stringTabVisible);
            SetTabVisible(tabOptionSetAttribute, optionTabVisible);
        }

        private void RetrieveOptionSets()
        {
            if (OptionSetsMetadata != null)
            {
                return;
            }

            WorkAsync(new WorkAsyncInfo("Retrieving OptionSets...", e => { e.Result = ((RetrieveAllOptionSetsResponse) Service.Execute(new RetrieveAllOptionSetsRequest())).OptionSetMetadata; })
            {
                PostWorkCallBack = e =>
                {
                    OptionSetsMetadata = (IEnumerable<OptionSetMetadataBase>) e.Result;
                    LoadOptionSets(OptionSetsMetadata);
                }
            });
        }

        private void LoadOptionSets(IEnumerable<OptionSetMetadataBase> optionSets)
        {
            try
            {
                optAttGlobalOptionSetCmb.BeginUpdate();
                optAttGlobalOptionSetCmb.Items.Clear();
                optAttGlobalOptionSetCmb.Text = null;

                var values = optionSets.Where(m => m.IsGlobal.GetValueOrDefault() && m is OptionSetMetadata).Select(e => new ObjectCollectionItem<OptionSetMetadata>(e.Name, (OptionSetMetadata) e)).OrderBy(r => r.DisplayName).Cast<object>().ToArray();

                optAttGlobalOptionSetCmb.Items.AddRange(values);
            }
            finally
            {
                optAttGlobalOptionSetCmb.EndUpdate();
            }
        }

        private void SetTabVisible(TabPage tab, bool visible)
        {
            if (visible)
            {
                if (!tabControl.TabPages.Contains(tab))
                {
                    tabControl.TabPages.Insert(0, tab);
                    tabControl.SelectedTab = tab;
                }
            }
            else
            {
                tabControl.TabPages.Remove(tab);
            }
        }

        private void SetReadOnlyOptionVisible()
        {
            if (chkMigrate.Checked)
            {
                chkIgnoreUpdateErrors.Visible = true;
            }
            else
            {
                chkIgnoreUpdateErrors.Visible = false;
                chkIgnoreUpdateErrors.Checked = false;
            }
        }

        private void SetMappingFileVisible()
        {
            var visible = chkMigrate.Checked;
            lblMappingFile.Visible = visible;
            txtMappingFile.Visible = visible;
            btnMappingFile.Visible = visible;
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

        private void chkMigrate_CheckedChanged(object sender, EventArgs e)
        {
            SetReadOnlyOptionVisible();
            SetMappingFileVisible();
            UpdateDisplayedSteps();
        }
        private void btnMappingFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = @"CSV files (*.csv)|*.csv";
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            txtMappingFile.Text = openFileDialog1.FileName;
        }

        private void UpdateDisplayedSteps()
        {
            var item = cmbAttributes.SelectedItem as ObjectCollectionItem<AttributeMetadata>;
            if (item == null)
            {
                return;
            }

            var action = GetCurrentAction();
            if (action.HasFlag(Logic.Action.Delete))
            {
                clbSteps.LoadItems(DeleteSteps);
            }
            else if (txtNewAttributeName.Text == item.Value.SchemaName && !action.HasFlag(Logic.Action.ChangeType))
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
                var steps = chkMigrate.Checked
                    ? StepMapper.Select(v => v.Key)
                    : StepMapper.Where(p => p.Value != Logic.Steps.MigrateDataToTemp && p.Value != Logic.Steps.MigrateDataToNewAttribute).Select(v => v.Key);

                // Change Case
                clbSteps.LoadItems(steps.ToObjectArray());
            }
            else if (action.HasFlag(Logic.Action.RemoveTemp))
            {
                // Partial Completion.  Need to allow for Migrate to Temp and Remove
                var steps = StepMapper.Where(p => (p.Value == Logic.Steps.MigrateDataToTemp && chkMigrate.Checked)
                                                  || p.Value == Logic.Steps.MigrateToNewAttribute 
                                                  || p.Value == Logic.Steps.RemoveTemp).Select(v => v.Key);
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

        public class ExecuteStepsInfo
        {
            public bool AutoRemovePluginRegistrationAssociations { get; set; }
            public Logic.Action Action { get; set; }
            public AttributeMetadata CurrentAttribute { get; set; }
            public AttributeMetadata NewAttribute { get; set; }
            public string NewAttributeName { get; set; }
            public string MappingFilePath { get; set; }
            public Logic Migrator { get; set; }
            public Logic.Steps Steps { get; internal set; }
        }

        private class ExecuteStepsResult
        {
            public bool Successful { get; set; }
            public Logic.Steps Steps { get; set; }
        }

        private void RemoveNonNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if (e.KeyChar == '.' && (((TextBox) sender).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void optAttGlobalOptionSetCmb_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var optionSet = (ObjectCollectionItem<OptionSetMetadata>) optAttGlobalOptionSetCmb.SelectedItem;
            optAttDefaultValueCmb.BeginUpdate();
            optAttDefaultValueCmb.Items.Clear();
            try
            {
                optAttGlobalOptionSetCmb.Items.Add(new ObjectCollectionItem<int?>("Unassigned Value", null));
                optAttDefaultValueCmb.Items.AddRange(optionSet.Value.Options.Select(o => new ObjectCollectionItem<int?>(o.Label.GetLocalOrDefaultText(), o.Value.GetValueOrDefault())).Cast<object>().ToArray());
            }
            finally
            {
                optAttDefaultValueCmb.EndUpdate();
            }
        }

        private void SelectAllKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                (sender as TextBox)?.SelectAll();
                e.Handled = true;
            }
        }

        private int? GetPrecisionSetting()
        {
            int tmp;
            int? number = null;
            if (int.TryParse(numAttPrecisionTxt.Text, out tmp))
            {
                number = tmp;
            }
            return number;
        }

        private void chkIgnoreUpdateErrors_CheckedChanged(object sender, EventArgs e)
        {
            if (chkIgnoreUpdateErrors.Checked)
            {
                //warn about possible data loss
                var result = MessageBox.Show(@"Selecting this option may result in significant data loss if there are read-only records or invalid data", @"Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Cancel)
                {
                    chkIgnoreUpdateErrors.Checked = false;
                }
            }
        }
    }

    [Export(typeof (IXrmToolBoxPlugin)),
        ExportMetadata("Name", "Attribute Manager"),   
        ExportMetadata("Description", "Handles Creating/Updating an attribute for an Entity."),
        ExportMetadata("SmallImageBase64", SmallImage32X32), // null for "no logo" image or base64 image content 
        ExportMetadata("BigImageBase64", LargeImage120X120), // null for "no logo" image or base64 image content 
        ExportMetadata("BackgroundColor", "White"), // Use a HTML color name
        ExportMetadata("PrimaryFontColor", "#000000"), // Or an hexadecimal code
        ExportMetadata("SecondaryFontColor", "DarkGray")]
    public class AttributeManager : PluginFactory
    {
        public override IXrmToolBoxPluginControl GetControl()
        {
            return new AttributeManagerPlugin();
        }
    }
}
