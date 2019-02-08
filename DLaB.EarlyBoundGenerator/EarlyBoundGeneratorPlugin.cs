using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DLaB.EarlyBoundGenerator.Settings;
using DLaB.Log;
using DLaB.XrmToolBoxCommon;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Metadata;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;
using PropertyInterface = DLaB.XrmToolBoxCommon.PropertyInterface;

namespace DLaB.EarlyBoundGenerator
{
    public partial class EarlyBoundGeneratorPlugin : DLaBPluginControlBase, PropertyInterface.IEntityMetadatas, PropertyInterface.IGlobalOptionSets, PropertyInterface.IActions
    {
        public EarlyBoundGeneratorConfig Settings { get; set; }
        public IEnumerable<Entity> Actions { get; set; }
        // ReSharper disable once IdentifierTypo
        public IEnumerable<EntityMetadata> EntityMetadatas { get; set; }
        public IEnumerable<OptionSetMetadataBase> GlobalOptionSets { get; set; }
        public ConnectionSettings ConnectionSettings { get; set; }
        private bool SkipSaveSettings { get; set; }
        private bool FormLoaded { get; set; }
        private SettingsMap SettingsMap { get; set; }

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

        private void EarlyBoundGenerator_Load(object sender, EventArgs e)
        {
            if (ConnectionDetail != null)
            {
                DisplayActionsIfSupported();
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
                Settings.CrmSvcUtilRealtiveRootPath = Paths.PluginsPath;
                SettingsMap = new SettingsMap(this, Settings){SettingsPath = settingsPath};
                PropertiesGrid.SelectedObject = SettingsMap;
                SkipSaveSettings = false;
            }
            catch (Exception ex)
            {
                TxtOutput.AppendText($"Unable to Load Settings from Config file: {settingsPath}.  {ex}");
                var result = MessageBox.Show(@"The Settings File is either empty or malformed.  Would you like to reset the file to the default settings?", @"Unable to Load Settings!", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.Yes)
                {
                    Settings = EarlyBoundGeneratorConfig.GetDefault();
                    Settings.CrmSvcUtilRealtiveRootPath = Paths.PluginsPath;
                }
                else
                {
                    SkipSaveSettings = true;
                }
            }
        }

        private void SaveSettings()
        {
            SettingsMap.PushChanges();
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
            SettingsMap.PushChanges();
            var isValid =
                IsValidPath(creationType, CreationType.Entities, SettingsMap.EntityOutPath, SettingsMap.CreateOneFilePerEntity, "Entities") &&
                IsValidPath(creationType, CreationType.Entities, SettingsMap.OptionSetOutPath, SettingsMap.CreateOneFilePerOptionSet, "OptionSets") &&
                IsValidPath(creationType, CreationType.Entities, SettingsMap.ActionOutPath, SettingsMap.CreateOneFilePerAction, "Actions") &&
                IsNamespaceDifferentThanContext();

            return isValid;
        }

        private static bool IsValidPath(CreationType creationType, CreationType creationTypeToCheck, string path, bool pathIsDirectory, string name)
        {
            if (creationType != creationTypeToCheck && creationType != CreationType.All)
            {
                return true;
            }
            var isValid = true;
            var containsExtension = !string.IsNullOrWhiteSpace(Path.GetExtension(path));
            // Validate Path
            if (pathIsDirectory)
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

        private bool IsNamespaceDifferentThanContext()
        {
            var isValid = true;
            if (Settings.ServiceContextName == Settings.Namespace)
            {
                MessageBox.Show(@"The Service Context can not be the same name as the Namespace!", @"Service Context / Namespace Conflict!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                isValid = false;
            }

            return isValid;
        }

        public void CreateCode(CreationType creationType)
        {
            EnableForm(false);

            HydrateSettingsFromUI();
            if (new Version(Settings.Version) < new Version(Settings.SettingsVersion))
            {
                if(MessageBox.Show($@"This version of the Early Bound Generator ({Settings.Version}) is older than the previous ran version from the settings ({Settings.SettingsVersion}).  You should probably update the plugin before running.  Are you sure you want to continue?", @"Older Version detected", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation) != DialogResult.Yes)
                {
                    EnableForm(true);
                    return;
                }
            }
            if (!SkipSaveSettings)
            {
                SaveSettings();
            }

            WorkAsync(new WorkAsyncInfo("Shelling out to Command Line...",
                (w, e) => // Work To Do Asynchronously
                {
                    var settings = (EarlyBoundGeneratorConfig) e.Argument;

                    var generator = new Logic(settings);
                    Logger.WireUpToReportProgress(w);
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
                        Logger.UnwireFromReportProgress(w);
                    }
                })
            {
                AsyncArgument = Settings,
                PostWorkCallBack = e => // Creation has finished.  Cleanup
                {
                    Logger.DisplayLog(e, TxtOutput);
                    EnableForm(true);
                },
                ProgressChanged = e => // Logic wants to display an update
                {
                    Logger.DisplayLog(e, SetWorkingMessage, TxtOutput);
                }
            });
        }

        // ReSharper disable once InconsistentNaming
        private void HydrateSettingsFromUI()
        {
            if (ConnectionDetail != null)
            {   
                TxtOutput.AppendText("CRM Authentication Type Detected: " + ConnectionDetail.AuthType + Environment.NewLine);
                Settings.Domain = GetUserDomain();
                Settings.Password = ConnectionDetail.GetUserPassword();
                Settings.SupportsActions = ConnectionDetail.OrganizationMajorVersion >= Crm2013;
                Settings.UseConnectionString = Settings.UseConnectionString; // #151 || Settings.AuthType == AuthenticationProviderType.ActiveDirectory;
                Settings.UseCrmOnline = ConnectionDetail.UseOnline;
                Settings.UserName = ConnectionDetail.UserName;
                Settings.Url = GetUrlString();

                if (Settings.UseConnectionString && string.IsNullOrWhiteSpace(Settings.Password))
                {
                    // Fix for https://github.com/daryllabar/DLaB.Xrm.XrmToolBoxTools/issues/43
                    // Difficulties with Early Bound Generator #43

                    var askForPassword = new PasswordDialog(this);
                    Settings.Password = askForPassword.ShowDialog(this) == DialogResult.OK ? askForPassword.Password : "UNKNOWN";
                }
                if (ConnectionDetail.AuthType == AuthenticationProviderType.ActiveDirectory && string.IsNullOrWhiteSpace(Settings.UserName))
                {
                    Settings.UserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                }
            }

            SettingsMap.PushChanges();
            Settings.RootPath = Path.GetDirectoryName(Path.GetFullPath(TxtSettingsPath.Text));
            if (SettingsMap.UseDeprecatedOptionSetNaming)
            {
                Settings.SetExtensionArgument(CreationType.OptionSets, CrmSrvUtilService.CodeWriterFilter, @"DLaB.CrmSvcUtilExtensions.OptionSet.CodeWriterFilterService,DLaB.CrmSvcUtilExtensions");
                Settings.SetExtensionArgument(CreationType.OptionSets, CrmSrvUtilService.NamingService, string.Empty);
            }
            else
            {
                var defaultConfig = EarlyBoundGeneratorConfig.GetDefault();
                defaultConfig.CrmSvcUtilRealtiveRootPath = Paths.PluginsPath;
                Settings.SetExtensionArgument(CreationType.OptionSets, CrmSrvUtilService.CodeWriterFilter, defaultConfig.GetExtensionArgument(CreationType.OptionSets, CrmSrvUtilService.CodeWriterFilter).Value);
                Settings.SetExtensionArgument(CreationType.OptionSets, CrmSrvUtilService.NamingService, defaultConfig.GetExtensionArgument(CreationType.OptionSets, CrmSrvUtilService.NamingService).Value);
            }
        }

        private string GetUserDomain()
        {
            if (string.IsNullOrWhiteSpace(ConnectionDetail.UserDomain))
            {
                return ConnectionDetail.UseOnline
                    ? ConnectionDetail.UserDomain
                    : ConnectionDetail.UserName.Split('\\').FirstOrDefault(); // Domain\UserName
            }
            else
            {
                return ConnectionDetail.UserDomain;
            }
        }
        private string GetUrlString()
        {
            var orgName = ConnectionDetail.OrganizationUrlName;
            var onPremUrl = ConnectionDetail.WebApplicationUrl;
            onPremUrl = onPremUrl != null && !onPremUrl.ToLower().EndsWith(orgName.ToLower())
                ? onPremUrl + orgName
                : onPremUrl;
            var url = ConnectionDetail.UseOnline 
                ? ConnectionDetail.OrganizationServiceUrl
                : onPremUrl;
            return Settings.UseConnectionString
                ? url?.Replace(@"/XRMServices/2011/Organization.svc", string.Empty)
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
            BtnCreateActions.Enabled = enable;
            BtnCreateEntities.Enabled = enable;
            BtnCreateOptionSets.Enabled = enable;
            BtnCreateAll.Enabled = enable;
            TxtSettingsPath.Enabled = enable;
            TxtOutput.Enabled = enable;
            PropertiesGrid.Enabled = enable;
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

        private void BtnOpenSettingsPathDialog_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.SetXmlFilePath(TxtSettingsPath))
            {
                ValidatedSettingsPath();
            }
        }

        private void TxtSettingsPath_Leave(object sender, EventArgs e)
        {
            ValidatedSettingsPath();
        }

        private void ValidatedSettingsPath()
        {
            if (!FormLoaded)
            {
                return;
            }
            var file = Path.GetFullPath(TxtSettingsPath.Text);
            if (ConnectionSettings.FullSettingsPath == file)
            {
                // No Change.  Exit
                return;
            }
            if (File.Exists(file))
            {
                SetConnectionSettingOnSettingsFileChanged();
            }
            else
            {
                MessageBox.Show($@"File ""{file}"" Not Found!  Unable to Update the Settings");
            }
        }

        public void DisplayActionsIfSupported()
        {
            BtnCreateActions.Enabled = ConnectionDetail.OrganizationMajorVersion >= Crm2013;
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
                TxtSettingsPath.Text = ConnectionSettings.SettingsPath;
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
            DisplayActionsIfSupported();
            SetConnectionSettingOnConnectionChanged();
        }

        private void TxtOutput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                ((TextBox) sender)?.SelectAll();
            }
        }

        private void PropertiesGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            SettingsMap.OnPropertyValueChanged(s, e);
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
