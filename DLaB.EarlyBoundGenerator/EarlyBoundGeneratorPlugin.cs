using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows.Forms;
using DLaB.EarlyBoundGenerator.Settings;
using DLaB.XrmToolboxCommon;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Metadata;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;
using PropertyInterface = DLaB.XrmToolboxCommon.PropertyInterface;

namespace DLaB.EarlyBoundGenerator
{
    public partial class EarlyBoundGeneratorPlugin : DLaBPluginControlBase, PropertyInterface.IEntityMetadatas, PropertyInterface.IGlobalOptionSets, PropertyInterface.IActions
    {
        public EarlyBoundGeneratorConfig Settings { get; set; }
        public IEnumerable<Entity> Actions { get; set; }
        public IEnumerable<EntityMetadata> EntityMetadatas { get; set; }
        public IEnumerable<OptionSetMetadataBase> GlobalOptionSets { get; set; }
        public ConnectionSettings ConnectionSettings { get; set; }
        private bool SkipSaveSettings { get; set; }
        private bool FormLoaded { get; set; }

        #region XrmToolBox Menu Interfaces

        #region IPayPalPlugin

        public override string DonationDescription => "Support Development for the Early Bound Generator!";

        #endregion IPayPalPlugin

        #region IHelpPlugin

        public override string HelpUrl => "https://github.com/daryllabar/DLaB.Xrm.XrmToolBoxTools/wiki/Early-Bound-Generator";

        #endregion IHelpPlugin

        #endregion XrmToolBox Menu Interfaces

        private const int Crm2013 = 6;

        public EarlyBoundGeneratorPlugin()
        {
            FormLoaded = false;
            InitializeComponent();
        }

        private void SetAddFilesToProjectVisibility()
        {
            ChkAddFilesToProject.Visible = ChkCreateOneActionFile.Checked || ChkCreateOneEntityFile.Checked || ChkCreateOneOptionSetFile.Checked;
        }

        private void EarlyBoundGenerator_Load(object sender, EventArgs e)
        {
            if (ConnectionDetail != null)
            {
                DisplayActionsIfSupported(false);
            }

            SetConnectionSettingOnLoad();
            HydrateUiFromSettings(ConnectionSettings.FullSettingsPath);
            FormLoaded = true;
        }

        private void HydrateUiFromSettings(string settingsPath)
        {
            try
            {
                Settings = EarlyBoundGeneratorConfig.Load(settingsPath);
                SkipSaveSettings = false;
            }
            catch (Exception ex)
            {
                TxtOutput.AppendText($"Unable to Load Settings from Config file: {settingsPath}.  {ex}");
                var result = MessageBox.Show(@"The Settings File is either empty or malformed.  Would you like to reset the file to the default settings?", @"Unable to Load Settings!", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.Yes)
                {
                    Settings = EarlyBoundGeneratorConfig.GetDefault();

                }
                else
                {
                    SkipSaveSettings = true;
                }
            }

            ChkAddDebuggerNonUserCode.Checked = Settings.ExtensionConfig.AddDebuggerNonUserCode;
            ChkAddFilesToProject.Checked = Settings.ExtensionConfig.AddNewFilesToProject;
            ChkAudibleCompletion.Checked = Settings.AudibleCompletionNotification;
            ChkCreateOneActionFile.Checked = Settings.ExtensionConfig.CreateOneFilePerAction;
            ChkCreateOneEntityFile.Checked = Settings.ExtensionConfig.CreateOneFilePerEntity;
            ChkCreateOneOptionSetFile.Checked = Settings.ExtensionConfig.CreateOneFilePerOptionSet;
            ChkEditableResponseActions.Checked = Settings.ExtensionConfig.MakeResponseActionsEditable;
            ChkIncludeCommandLine.Checked = Settings.IncludeCommandLine;
            ChkMakeReadonlyFieldsEditable.Checked = Settings.ExtensionConfig.MakeReadonlyFieldsEditable;
            ChkMaskPassword.Checked = Settings.MaskPassword;
            ChkGenerateActionAttributeNameConsts.Checked = Settings.ExtensionConfig.GenerateActionAttributeNameConsts;
            ChkGenerateAttributeNameConsts.Checked = Settings.ExtensionConfig.GenerateAttributeNameConsts;
            ChkGenerateAnonymousTypeConstructor.Checked = Settings.ExtensionConfig.GenerateAnonymousTypeConstructor;
            ChkGenerateEntityRelationships.Checked = Settings.ExtensionConfig.GenerateEntityRelationships;
            ChkGenerateOptionSetEnums.Checked = Settings.ExtensionConfig.GenerateEnumProperties;
            ChkRemoveRuntimeComment.Checked = Settings.ExtensionConfig.RemoveRuntimeVersionComment;
            ChkUseDeprecatedOptionSetNaming.Checked = Settings.ExtensionConfig.UseDeprecatedOptionSetNaming;
            ChkUseTFS.Checked = Settings.ExtensionConfig.UseTfsToCheckoutFiles;
            ChkUseXrmClient.Checked = Settings.ExtensionConfig.UseXrmClient;
            TxtActionPath.Text = Settings.ActionOutPath;
            TxtEntityPath.Text = Settings.EntityOutPath;
            TxtInvalidCSharpNamePrefix.Text = Settings.ExtensionConfig.InvalidCSharpNamePrefix;
            TxtOptionSetFormat.Text = Settings.ExtensionConfig.LocalOptionSetFormat;
            TxtNamespace.Text = Settings.Namespace;
            TxtOptionSetPath.Text = Settings.OptionSetOutPath;
            TxtServiceContextName.Text = Settings.ServiceContextName;

            // Hide or show labels based on checked preferences
            LblActionsDirectory.Visible = ChkCreateOneActionFile.Checked;
            LblActionPath.Visible = !LblActionsDirectory.Visible;
            LblEntitiesDirectory.Visible = ChkCreateOneEntityFile.Checked;
            LblEntityPath.Visible = !LblEntitiesDirectory.Visible;
            LblOptionSetsDirectory.Visible = ChkCreateOneOptionSetFile.Checked;
            LblOptionSetPath.Visible = !LblOptionSetsDirectory.Visible;
            SetAddFilesToProjectVisibility();
        }

        private void SaveSettings()
        {
            Settings.Save(ConnectionSettings.SettingsPath);
        }

        public override void ClosingPlugin(PluginCloseInfo info)
        {
            base.ClosingPlugin(info);
            HydrateSettingsFromUI();
            SaveSettings();
            if (info.Cancel || SkipSaveSettings) return;

            ConnectionDetail = null; // Don't save the Connection Details when closing.
        }

        private void Create(CreationType creationType)
        {
            if (IsFormValid(creationType))
            {
                ExecuteMethod(CreateCode, creationType);
            }
        }

        private bool IsFormValid(CreationType creationType)
        {
            var isValid =
                IsValidPath(creationType, CreationType.Entities, TxtEntityPath, ChkCreateOneEntityFile, "Entities") &&
                IsValidPath(creationType, CreationType.Entities, TxtOptionSetPath, ChkCreateOneOptionSetFile, "OptionSets") &&
                IsValidPath(creationType, CreationType.Entities, TxtActionPath, ChkCreateOneActionFile, "Actions");

            return isValid;
        }

        private static bool IsValidPath(CreationType creationType, CreationType creationTypeToCheck, TextBox textPath, CheckBox chk, string name)
        {
            if (creationType != creationTypeToCheck && creationType != CreationType.All)
            {
                return true;
            }
            var isValid = true;
            var containsExtension = !string.IsNullOrWhiteSpace(Path.GetExtension(textPath.Text));
            // Validate Path
            if (chk.Checked)
            {
                if (containsExtension)
                {
                    MessageBox.Show(name + @" path must be a directory!", @"Invalid " + name + @" Path", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    isValid = false;
                }
            }
            else
            {
                if (!containsExtension)
                {
                    MessageBox.Show(name + @" path must be a file!", @"Invalid " + name + @" Path", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    isValid = false;
                }
            }
            return isValid;
        }

        public void CreateCode(CreationType creationType)
        {
            EnableForm(false);

            HydrateSettingsFromUI();
            if (!SkipSaveSettings)
            {
                SaveSettings();
            }

            WorkAsync(new WorkAsyncInfo("Shelling out to Command Line...",
                (w, e) => // Work To Do Asynchronously
                {
                    var settings = (EarlyBoundGeneratorConfig) e.Argument;

                    var generator = new Logic(settings);
                    Logic.LogHandler onLog = m => w.ReportProgress(0, m);
                    generator.OnLog += onLog;
                    try
                    {
                        switch (creationType)
                        {
                            case CreationType.Actions:
                                w.ReportProgress(0, "Executing for Actions");
                                generator.CreateActions();
                                break;
                            case CreationType.All:
                                w.ReportProgress(0, "Executing for All");
                                generator.ExecuteAll();
                                break;
                            case CreationType.Entities:
                                w.ReportProgress(0, "Executing for Entities");
                                generator.CreateEntities();
                                break;
                            case CreationType.OptionSets:
                                w.ReportProgress(0, "Executing for OptionSets");
                                generator.CreateOptionSets();
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(creationType));
                        }
                        w.ReportProgress(99, "Creation Complete!");
                    }
                    catch (InvalidOperationException ex)
                    {
                        w.ReportProgress(int.MinValue, ex.Message);
                    }
                    catch (Exception ex)
                    {
                        w.ReportProgress(int.MinValue, ex.ToString());
                    }
                    finally
                    {
                        generator.OnLog -= onLog;
                    }
                })
            {
                AsyncArgument = Settings,
                PostWorkCallBack = e => // Creation has finished.  Cleanup
                {
                    var result = e.Result as Logic.LogMessageInfo;
                    if (result != null)
                    {
                        TxtOutput.AppendText(result.Detail + Environment.NewLine);
                    }
                    EnableForm(true);
                },
                ProgressChanged = e => // Logic wants to display an update
                {
                    string summary;
                    var result = e.UserState as Logic.LogMessageInfo;
                    if (result == null)
                    {
                        summary = e.UserState.ToString();
                    }
                    else
                    {
                        if (result.Detail != null)
                        {
                            TxtOutput.AppendText(result.Detail + Environment.NewLine);
                        }
                        summary = result.Summary;
                    }
                    // Status Update
                    if (e.ProgressPercentage == int.MinValue)
                    {
                        TxtOutput.AppendText(e.UserState + Environment.NewLine);
                    }
                    else
                    {
                        if (summary != null)
                        {
                            SetWorkingMessage(summary);
                        }
                    }
                }
            });
        }

        // ReSharper disable once InconsistentNaming
        private void HydrateSettingsFromUI()
        {
            if (ConnectionDetail != null)
            {   
                TxtOutput.AppendText("CRM Authentication Type Detected: " + ConnectionDetail.AuthType + Environment.NewLine);
                Settings.AuthType = ConnectionDetail.AuthType;
                Settings.Domain = ConnectionDetail.UserDomain;
                Settings.Password = ConnectionDetail.GetUserPassword();
                Settings.SupportsActions = ConnectionDetail.OrganizationMajorVersion >= Crm2013;
                Settings.UseConnectionString = Settings.UseConnectionString || Settings.AuthType == AuthenticationProviderType.ActiveDirectory;
                Settings.UseCrmOnline = ConnectionDetail.UseOnline;
                Settings.UserName = ConnectionDetail.UserName;
                Settings.Url = GetUrlString();

                if (Settings.UseConnectionString && string.IsNullOrWhiteSpace(Settings.Password))
                {
                    // Fix for https://github.com/daryllabar/DLaB.Xrm.XrmToolBoxTools/issues/43
                    // Difficulties with Early Bound Generator #43

                    var askForPassowrd = new PasswordDialog(this);
                    Settings.Password = askForPassowrd.ShowDialog(this) == DialogResult.OK ? askForPassowrd.Password : "UNKNOWN";
                }
                if (ConnectionDetail.AuthType == AuthenticationProviderType.ActiveDirectory && string.IsNullOrWhiteSpace(Settings.UserName))
                {
                    Settings.UserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                }
            }

            Settings.ActionOutPath = TxtActionPath.Text;
            Settings.EntityOutPath = TxtEntityPath.Text;
            Settings.RootPath = Path.GetDirectoryName(Path.GetFullPath(TxtSettingsPath.Text));
            if (ChkUseDeprecatedOptionSetNaming.Checked)
            {
                Settings.SetExtensionArgument(CreationType.OptionSets, CrmSrvUtilService.CodeWriterFilter, @"DLaB.CrmSvcUtilExtensions.OptionSet.CodeWriterFilterService,DLaB.CrmSvcUtilExtensions");
                Settings.SetExtensionArgument(CreationType.OptionSets, CrmSrvUtilService.NamingService, string.Empty);
            }
            else
            {
                var defaultConfig = EarlyBoundGeneratorConfig.GetDefault();
                Settings.SetExtensionArgument(CreationType.OptionSets, CrmSrvUtilService.CodeWriterFilter, defaultConfig.GetExtensionArgument(CreationType.OptionSets, CrmSrvUtilService.CodeWriterFilter).Value);
                Settings.SetExtensionArgument(CreationType.OptionSets, CrmSrvUtilService.NamingService, defaultConfig.GetExtensionArgument(CreationType.OptionSets, CrmSrvUtilService.NamingService).Value);
            }

            var extensions = Settings.ExtensionConfig;
            extensions.AddDebuggerNonUserCode = ChkAddDebuggerNonUserCode.Checked;
            extensions.AddNewFilesToProject = ChkAddFilesToProject.Checked;
            extensions.CreateOneFilePerAction = ChkCreateOneActionFile.Checked;
            extensions.CreateOneFilePerEntity = ChkCreateOneEntityFile.Checked;
            extensions.CreateOneFilePerOptionSet = ChkCreateOneOptionSetFile.Checked;
            extensions.GenerateActionAttributeNameConsts = ChkGenerateActionAttributeNameConsts.Checked;
            extensions.GenerateAttributeNameConsts = ChkGenerateAttributeNameConsts.Checked;
            extensions.GenerateAnonymousTypeConstructor = ChkGenerateAnonymousTypeConstructor.Checked;
            extensions.GenerateEntityRelationships = ChkGenerateEntityRelationships.Checked;
            extensions.GenerateEnumProperties = ChkGenerateOptionSetEnums.Checked;
            extensions.InvalidCSharpNamePrefix = TxtInvalidCSharpNamePrefix.Text;
            extensions.MakeReadonlyFieldsEditable = ChkMakeReadonlyFieldsEditable.Checked;
            extensions.MakeResponseActionsEditable = ChkEditableResponseActions.Checked;
            extensions.LocalOptionSetFormat = TxtOptionSetFormat.Text;
            extensions.RemoveRuntimeVersionComment = ChkRemoveRuntimeComment.Checked;
            extensions.UseXrmClient = ChkUseXrmClient.Checked;
            extensions.UseDeprecatedOptionSetNaming = ChkUseDeprecatedOptionSetNaming.Checked;
            extensions.UseTfsToCheckoutFiles = ChkUseTFS.Checked;
            Settings.AudibleCompletionNotification = ChkAudibleCompletion.Checked;
            Settings.IncludeCommandLine = ChkIncludeCommandLine.Checked;
            Settings.MaskPassword = ChkMaskPassword.Checked;
            Settings.Namespace = TxtNamespace.Text;
            Settings.OptionSetOutPath = TxtOptionSetPath.Text;
            Settings.ServiceContextName = string.IsNullOrWhiteSpace(TxtServiceContextName.Text) ? null : TxtServiceContextName.Text;
        }

        private string GetUrlString()
        {
            var url = ConnectionDetail.OrganizationServiceUrl;
            return Settings.UseConnectionString
                ? url.Replace(@"/XRMServices/2011/Organization.svc", string.Empty)
                :  url;

            //if (auth == AuthenticationProviderType.ActiveDirectory)
            //{
            //    return url;
            //}
            //int start;
            //var prefix = url.SubstringByString(0, "//", out start) + "//";
            //if (start < 0)
            //{
            //    return url;
            //}
            //var end = url.IndexOf(".", start, StringComparison.Ordinal);
            //if (end < 0)
            //{
            //    return url;
            //}
            //return prefix + ConnectionDetail.OrganizationUrlName + url.Substring(end);
        }

        private void EnableForm(bool enable)
        {
            BtnCreateEntities.Enabled = enable;
            BtnCreateOptionSets.Enabled = enable;
            BtnCreateAll.Enabled = enable;
            groupBox1.Enabled = enable;
            tabControl1.Enabled = enable;
        }

        private void ChkIncludeCommandLine_CheckedChanged(object sender, EventArgs e)
        {
            ChkMaskPassword.Visible = ChkIncludeCommandLine.Checked;
        }

        private void ChkGenerateOptionSetEnums_CheckedChanged(object sender, EventArgs e)
        {
            BtnEnumMappings.Visible = ChkGenerateOptionSetEnums.Checked;
            BtnUnmappedProperties.Visible = ChkGenerateOptionSetEnums.Checked;
        }

        #region Create Button Click Events

        private void BtnCreateActions_Click(object sender, EventArgs e)
        {
            Create(CreationType.Actions);
        }

        private void BtnCreateEntities_Click(object sender, EventArgs e)
        {
            Create(CreationType.Entities);
        }

        private void BtnCreateBoth_Click(object sender, EventArgs e)
        {
            Create(CreationType.All);
        }

        private void BtnCreateOptionSets_Click(object sender, EventArgs e)
        {
            Create(CreationType.OptionSets);
        }

        #endregion // Create Button Click Events

        private void BtnSpecifyAttributeNames_Click(object sender, EventArgs e)
        {
            var dialog = new SpecifyAttributeNamesDialog(this) {
                ConfigValue = Settings.ExtensionConfig.EntityAttributeSpecifiedNames
            };
            var result = dialog.ShowDialog();
            if (result != DialogResult.OK) return;
            Settings.ExtensionConfig.EntityAttributeSpecifiedNames = dialog.ConfigValue;
            SaveSettings();
        }

        private void BtnActionsToSkip_Click(object sender, EventArgs e)
        {
            var dialog = new SpecifyActionsDialog(this) { SpecifiedActions = Settings.ExtensionConfig.ActionsToSkip };
            if (dialog.ShowDialog() != DialogResult.OK) return;
            Settings.ExtensionConfig.ActionsToSkip = dialog.SpecifiedActions;
            SaveSettings();
        }

        private void BtnEntitesToSkip_Click(object sender, EventArgs e)
        {
            var dialog = new SpecifyEntitiesDialog(this) { SpecifiedEntities = Settings.ExtensionConfig.EntitiesToSkip };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Settings.ExtensionConfig.EntitiesToSkip = dialog.SpecifiedEntities;
                SaveSettings();
            }
        }

        private void BtnOptionSetsToSkip_Click(object sender, EventArgs e)
        {
            var dialog = new SpecifyOptionSetsDialog(this) { OptionSets = Settings.ExtensionConfig.OptionSetsToSkip };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Settings.ExtensionConfig.OptionSetsToSkip = dialog.OptionSets;
                SaveSettings();
            }
        }

        private void BtnEnumMappings_Click(object sender, EventArgs e)
        {
            var dialog = new AttributesToEnumMapperDialog(this) { ConfigValue = Settings.ExtensionConfig.PropertyEnumMappings };
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                Settings.ExtensionConfig.PropertyEnumMappings = dialog.ConfigValue;
                SaveSettings();
            }
        }

        private void BtnUnmappedProperties_Click(object sender, EventArgs e)
        {
            var dialog = new SpecifyAttributesDialog (this) { ConfigValue = Settings.ExtensionConfig.UnmappedProperties };
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                Settings.ExtensionConfig.UnmappedProperties = dialog.ConfigValue;
                SaveSettings();
            }
        }

        private void BtnOpenActionPathDialog_Click(object sender, EventArgs e)
        {
            openFileDialog1.SetCsFilePath(TxtActionPath, ConnectionSettings.SettingsDirectoryName);
        }

        private void BtnOpenEntityPathDialog_Click(object sender, EventArgs e)
        {
            openFileDialog1.SetCsFilePath(TxtEntityPath, ConnectionSettings.SettingsDirectoryName);
        }

        private void BtnOpenOptionSetPathDialog_Click(object sender, EventArgs e)
        {
            openFileDialog1.SetCsFilePath(TxtOptionSetPath, ConnectionSettings.SettingsDirectoryName);
        }
        private void BtnOpenSettingsPathDialog_Click(object sender, EventArgs e)
        {
            openFileDialog1.SetXmlFilePath(TxtSettingsPath);
        }

        private void ChkCreateOneActionFile_CheckedChanged(object sender, EventArgs e)
        {
            LblActionsDirectory.Visible = ChkCreateOneActionFile.Checked;
            LblActionPath.Visible = !ChkCreateOneActionFile.Checked;
            SetAddFilesToProjectVisibility();

            ConditionallyAddRemoveExtension(TxtActionPath, "Actions.cs", ChkCreateOneActionFile.Checked);
        }

        private void ChkCreateOneOptionSetFile_CheckedChanged(object sender, EventArgs e)
        {
            LblOptionSetsDirectory.Visible = ChkCreateOneOptionSetFile.Checked;
            LblOptionSetPath.Visible = !ChkCreateOneOptionSetFile.Checked;
            SetAddFilesToProjectVisibility();

            ChkAddFilesToProject.Visible = !ChkCreateOneEntityFile.Checked;

            ConditionallyAddRemoveExtension(TxtOptionSetPath, "OptionSets.cs", ChkCreateOneOptionSetFile.Checked);
        }

        private void ChkUseDeprecatedOptionSetNaming_CheckedChanged(object sender, EventArgs e)
        {
            LblOptionSetFormat.Visible = !ChkUseDeprecatedOptionSetNaming.Checked;
            TxtOptionSetFormat.Visible = !ChkUseDeprecatedOptionSetNaming.Checked;
        }

        private static void ConditionallyAddRemoveExtension(TextBox textBox, string singleClassFileName, bool @checked)
        {
            var fileName = textBox.Text;
            bool hasExtension = Path.GetExtension(fileName) != string.Empty;

            if (@checked && hasExtension)
            {
                // Remove Extension
                textBox.Text = Path.GetDirectoryName(fileName);
            }
            else if (!@checked && !hasExtension)
            {
                // Add Actions.cs
                textBox.Text = Path.Combine(fileName, singleClassFileName);
            }
        }

        private void ChkCreateOneEntityFile_CheckedChanged(object sender, EventArgs e)
        {
            LblEntitiesDirectory.Visible = ChkCreateOneEntityFile.Checked;
            LblEntityPath.Visible = !ChkCreateOneEntityFile.Checked;
            SetAddFilesToProjectVisibility();

            ConditionallyAddRemoveExtension(TxtEntityPath, "Entities.cs", ChkCreateOneEntityFile.Checked);
        }

        private void actionsTab_Enter(object sender, EventArgs e)
        {
            ExecuteMethod(DisplayActionsIfSupported, true);
        }

        private void TxtSettingsPath_TextChanged(object sender, EventArgs e)
        {
            if (!FormLoaded)
            {
                return;
            }
            var file = Path.GetFullPath(TxtSettingsPath.Text);
            if (File.Exists(file))
            {
                SetConnectionSettingOnSettingsFileChanged();
            }
            else
            {
                MessageBox.Show($@"File ""{file}"" Not Found!  Unable to Update the Settings");
            }

        }

        public void DisplayActionsIfSupported(bool displayErrorIfUnsupported)
        {
            if (ConnectionDetail.OrganizationMajorVersion < Crm2013)
            {
                if (displayErrorIfUnsupported)
                {
                    MessageBox.Show(@"Your version of CRM doesn't support Actions!");
                }
                tabControl1.TabPages.Remove(actionsTab);
            }
            else if(!tabControl1.TabPages.Contains(actionsTab))
            {
                tabControl1.TabPages.Add(actionsTab);
            }
        }

        private void SetConnectionSettingOnLoad()
        {
            // Has Connection
            // - Use Connection's Setting, or default
            ConnectionSettings = ConnectionSettings.GetForConnection(ConnectionDetail) ?? ConnectionSettings.GetDefault();
            TxtSettingsPath.Text = ConnectionSettings.SettingsPath;
        }

        private void SetConnectionSettingOnConnectionChanged()
        {
            if (ConnectionDetail == null)
            {
                // No Connection Specified, leave Connection Settings unchanged
                return;
            }

            var localSettings = ConnectionSettings.GetForConnection(ConnectionDetail);

            if (localSettings == null)
            {
                // New Connection did not have a settings file associated with it, use current
                ConnectionSettings.Save(ConnectionDetail);
            }
            else
            {
                // New Connection Did Have a Connection Settings, load settings
                ConnectionSettings = localSettings;
                TxtSettingsPath.TextChanged -= TxtSettingsPath_TextChanged;
                TxtSettingsPath.Text = ConnectionSettings.SettingsPath;
                TxtSettingsPath.TextChanged += TxtSettingsPath_TextChanged;
                HydrateUiFromSettings(ConnectionSettings.FullSettingsPath);
            }
        }

        private void SetConnectionSettingOnSettingsFileChanged()
        {
            ConnectionSettings = new ConnectionSettings
            {
                SettingsPath = TxtSettingsPath.Text
            };

            if (ConnectionDetail != null)
            {
                ConnectionSettings.Save(ConnectionDetail);
            }
            if (File.Exists(ConnectionSettings.FullSettingsPath))
            {
                HydrateUiFromSettings(ConnectionSettings.FullSettingsPath);
            }
        }

        private void EarlyBoundGenerator_ConnectionUpdated(object sender, ConnectionUpdatedEventArgs e)
        {
            if (!FormLoaded)
            {
                return;
            }
            EntityMetadatas = null;
            GlobalOptionSets = null;
            Actions = null;
            DisplayActionsIfSupported(false);
            SetConnectionSettingOnConnectionChanged();
        }
    }

    [Export(typeof(IXrmToolBoxPlugin)),
     ExportMetadata("Name", "Early Bound Generator"),
     ExportMetadata("Description", "Adds advanced features and configuration to the generation of Early Bound CRM Entities."),
     ExportMetadata("SmallImageBase64", SmallImage32X32), // null for "no logo" image or base64 image content 
     ExportMetadata("BigImageBase64", LargeImage120X120), // null for "no logo" image or base64 image content 
     ExportMetadata("BackgroundColor", "White"), // Use a HTML color name
     ExportMetadata("PrimaryFontColor", "#000000"), // Or an hexadecimal code
     ExportMetadata("SecondaryFontColor", "DarkGray")]
    public class EarlyBoundGenerator : PluginFactory
    {
        public override IXrmToolBoxPluginControl GetControl()
        {
            return new EarlyBoundGeneratorPlugin();
        }
    }
}
