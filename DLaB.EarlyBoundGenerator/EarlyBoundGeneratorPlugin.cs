using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DLaB.EarlyBoundGenerator.Settings;
using DLaB.Log;
using DLaB.XrmToolBoxCommon;
using DLaB.XrmToolBoxCommon.AppInsightsHelper;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Metadata;
using XrmToolBox;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;
using AuthenticationType = Microsoft.Xrm.Tooling.Connector.AuthenticationType;
using PropertyInterface = DLaB.XrmToolBoxCommon.PropertyInterface;

namespace DLaB.EarlyBoundGenerator
{
    public partial class EarlyBoundGeneratorPlugin : DLaBPluginControlBase, PropertyInterface.IEntityMetadatas, PropertyInterface.IGlobalOptionSets, PropertyInterface.IActions, IGetEditorSetting
    {
        public EarlyBoundGeneratorConfig Settings { get; set; }
        public IEnumerable<Entity> SdkMessages { get; set; }
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
            TxtOutput.AppendText("You are running V1 of the XTB Early Bound Generator.  If you Please consider upgrading to V2, which is much faster and has many more features!" + Environment.NewLine);
            HydrateUiFromSettings(ConnectionSettings.FullSettingsPath);
            Telemetry.Enabled = Options.Instance.AllowLogUsage ?? true;
            // Totally security by obscurity
            var key = new Guid(Convert.FromBase64String("F5YHOkZqRky" + "Ns3GffS9Rxw=="));
            Telemetry.InitAiConnection($"InstrumentationKey={key};IngestionEndpoint=https://westus2-1.in.applicationinsights.azure.com/;LiveEndpoint=https://westus2.livediagnostics.monitor.azure.com/");
            FormLoaded = true;
        }

        private void HydrateUiFromSettings(string settingsPath)
        {
            try
            {
                Settings = EarlyBoundGeneratorConfig.Load(settingsPath);
                Settings.CrmSvcUtilRelativeRootPath = Paths.PluginsPath;
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
                    Settings.CrmSvcUtilRelativeRootPath = Paths.PluginsPath;
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
                if (containsExtension && !Directory.Exists(path))
                {
                    MessageBox.Show(name + @" path must be a directory!  Did you forget to add a \\?", @"Invalid " + name + @" Path", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            var ebgVersion = new Version(Settings.Version);
            var settingsVersion = new Version(Settings.SettingsVersion);
            if (settingsVersion.Major > ebgVersion.Major)
            {
                MessageBox.Show($@"This version of the Early Bound Generator ({Settings.Version}) is not compatible with the previous ran version from the settings ({Settings.SettingsVersion}).  Please Update to the matching version of the tool before running again.", @"Newer Major Settings Version Detected!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                EnableForm(true);
                return;
            }
            if (ebgVersion < settingsVersion)
            {
                if (MessageBox.Show($@"This version of the Early Bound Generator ({Settings.Version}) is older than the previous ran version from the settings ({Settings.SettingsVersion}).  You should probably update the plugin before running.  Are you sure you want to continue?", @"Older Version detected!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation) != DialogResult.Yes)
                {
                    EnableForm(true);
                    return;
                }
            }
            if (!SkipSaveSettings)
            {
                SaveSettings();
            }

            LogConfigSettings(creationType);
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
                if (ConnectionDetail.UseOnline)
                {
                    TxtOutput.AppendText("You are using an older, slower version of the the Early Bound Generator.  Please consider installing the Early Bound Generator V2 from the XrmToolBox plugin store!" + Environment.NewLine);
                }

                TxtOutput.AppendText("CRM Authentication Type Detected: " + ConnectionDetail.AuthType + Environment.NewLine);
                Settings.Domain = GetUserDomain();
                Settings.Password = ConnectionDetail.GetUserPassword();
                Settings.SupportsActions = ConnectionDetail.OrganizationMajorVersion >= Crm2013;
                Settings.UseCrmOnline = ConnectionDetail.UseOnline;
                Settings.UserName = ConnectionDetail.UserName;
                Settings.Url = ConnectionDetail.GetUrlString();

                if (Settings.UseConnectionString
                    && string.IsNullOrWhiteSpace(Settings.Password)
                    && ConnectionDetail.NewAuthType != AuthenticationType.Certificate
                    && ConnectionDetail.NewAuthType != AuthenticationType.ClientSecret
                    && ConnectionDetail.NewAuthType != AuthenticationType.OAuth)
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
                if (string.IsNullOrWhiteSpace(Settings.ConnectionString))
                {
                    // Load any non-username/password situations via connection string #268
                    Settings.ConnectionString = ConnectionDetail.GetNonUserConnectionString();
                    if (!string.IsNullOrWhiteSpace(Settings.ConnectionString))
                    {
                        Settings.UseConnectionString = true;
                    }
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
                defaultConfig.CrmSvcUtilRelativeRootPath = Paths.PluginsPath;
                Settings.SetExtensionArgument(CreationType.OptionSets, CrmSrvUtilService.CodeWriterFilter, defaultConfig.GetExtensionArgument(CreationType.OptionSets, CrmSrvUtilService.CodeWriterFilter).Value);
                Settings.SetExtensionArgument(CreationType.OptionSets, CrmSrvUtilService.NamingService, defaultConfig.GetExtensionArgument(CreationType.OptionSets, CrmSrvUtilService.NamingService).Value);
            }
        }

        private void LogConfigSettings(CreationType creationType)
        {
            var properties = Settings.ExtensionConfig.GetType().GetProperties().ToDictionary(
                k => k.Name,
                v => v.GetValue(Settings.ExtensionConfig)?.ToString() ?? string.Empty);
            foreach (var kvp in properties.ToList())
            {
                var name = kvp.Key.ToLower();
                if (name.EndsWith("list")
                    || name.EndsWith("toskip"))
                {
                    properties.Add(kvp.Key + "Count", string.IsNullOrWhiteSpace(kvp.Value) 
                        ? "0"
                        : kvp.Value.Split('|').Length.ToString());
                }
            }

            properties["AudibleCompletionNotification"] = Settings.AudibleCompletionNotification.ToString();
            properties["CrmSvcUtilRelativePath"] = Settings.CrmSvcUtilRelativePath;
            properties["IncludeCommandLine"] = Settings.IncludeCommandLine.ToString();
            properties["MaskPassword"] = Settings.MaskPassword.ToString();
            properties["SupportsActions"] = Settings.SupportsActions.ToString();
            properties["UseCrmOnline"] = Settings.UseCrmOnline.ToString();
            properties["Version"] = Settings.Version;
            properties["CreationType"] = creationType.ToString();

            if(Telemetry.Enabled){
                TxtOutput.AppendText($"Tracking {creationType} Generation Event." + Environment.NewLine);
                Telemetry.TrackEvent("ConfigSettings", properties);
            }
            else
            {
                TxtOutput.AppendText("Tracking not enabled!  Please consider allowing the Xrm Tool Box to send anonymous statistics via the Configuration --> Settings -- Data Collect.  This allows reporting for which features are used and what features can be deprecated." + Environment.NewLine);
            }
        }

        private string GetUserDomain()
        {
            if (string.IsNullOrWhiteSpace(ConnectionDetail.UserDomain))
            {
                return ConnectionDetail.UseOnline
                    ? ConnectionDetail.UserDomain
                    : ConnectionDetail.UserName?.Split('\\').FirstOrDefault(); // Domain\UserName
            }
            else
            {
                return ConnectionDetail.UserDomain;
            }
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

        private void BtnSaveSettingsPathDialog_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.SetXmlFilePath(TxtSettingsPath))
            {
                SaveSettingsToNewFile();
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

        private void SaveSettingsToNewFile()
        {
            ConnectionSettings.SettingsPath = TxtSettingsPath.Text;
            SaveSettings();
            if (ConnectionDetail != null)
            {
                ConnectionSettings.Save(ConnectionDetail);
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
            SdkMessages = null;
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

        public string GetEditorSetting(EditorSetting key)
        {
            if (key == EditorSetting.WorkflowlessActions)
            {
                return Settings.WorkflowlessActions;
            }

            return null;
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
