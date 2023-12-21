using DLaB.EarlyBoundGeneratorV2.Settings;
using DLaB.Log;
using DLaB.XrmToolBoxCommon;
using DLaB.XrmToolBoxCommon.AppInsightsHelper;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Media;
using System.Windows.Forms;
using DLaB.ModelBuilderExtensions;
using XrmToolBox;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;
using PropertyInterface = DLaB.XrmToolBoxCommon.PropertyInterface;
using DLaB.XrmToolBoxCommon.Controls;

namespace DLaB.EarlyBoundGeneratorV2
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
            PropertiesGrid.FilterableGrid.PropertyValueChanged += PropertiesGrid_PropertyValueChanged;
        }

        private void EarlyBoundGenerator_Load(object sender, EventArgs e)
        {
            Logger.Instance.OnLog += OnLoadLog;
            SetConnectionSettingOnLoad();
            HydrateUiFromSettings(ConnectionSettings.FullSettingsPath);
            Telemetry.Enabled = Options.Instance.AllowLogUsage ?? true;
            // Totally security by obscurity
            var key = new Guid(Convert.FromBase64String("F5YHOkZqRky" + "Ns3GffS9Rxw=="));
            Telemetry.InitAiConnection($"InstrumentationKey={key};IngestionEndpoint=https://westus2-1.in.applicationinsights.azure.com/;LiveEndpoint=https://westus2.livediagnostics.monitor.azure.com/");
            FormLoaded = true;
            Logger.Instance.OnLog -= OnLoadLog;
        }

        private void OnLoadLog(LogMessageInfo info)
        {
            TxtOutput.AppendText(info.Detail + Environment.NewLine);
            if (!string.IsNullOrWhiteSpace(info.ModalMessage))
            {
                MessageBox.Show(info.ModalMessage);
            }
        }

        private void HydrateUiFromSettings(string settingsPath)
        {
            try
            {
                Logger.AddDetail("Loading settings from " + settingsPath);
                HydrateUiFromSettings(EarlyBoundGeneratorConfig.Load(settingsPath), settingsPath);
            }
            catch (Exception ex)
            {
                TxtOutput.AppendText($"Unable to Load Settings from Config file: {settingsPath}.  {ex}");
                var result = MessageBox.Show(@"The Settings File is either empty or malformed.  Would you like to reset the file to the default settings?", @"Unable to Load Settings!", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.Yes)
                {
                    Settings = EarlyBoundGeneratorConfig.GetDefault();
                    Settings.ExtensionConfig.XrmToolBoxPluginPath = Paths.PluginsPath;
                }
                else
                {
                    SkipSaveSettings = true;
                }
            }
        }

        private void HydrateUiFromSettings(EarlyBoundGeneratorConfig config, string settingsPath)
        {
            Settings = config;
            Settings.ExtensionConfig.XrmToolBoxPluginPath = Paths.PluginsPath;
            SettingsMap = new SettingsMap(this, Settings)
            {
                SettingsPath = string.IsNullOrWhiteSpace(Settings.ExtensionConfig.OutputRelativeDirectory) 
                    ? settingsPath 
                    : Settings.ExtensionConfig.OutputRelativeDirectory
            };
            PropertiesGrid.SelectedObject = SettingsMap;
            HideUnusedCategories();
            SkipSaveSettings = false;

            if (settingsPath.EndsWith(@"MscrmTools\XrmToolBox\Settings\DLaB.EarlyBoundGeneratorV2.DefaultSettings.xml"))
            {
                SystemSounds.Beep.Play();
                SystemSounds.Beep.Play();
                Logger.AddDetail(@"******************************
* Default Settings Detected! *
******************************
Best practice is to save a copy of the default settings into the same directory as where the files will be generated, and to check it into source control.

This allows for the following:
  1. Changes to how the files are configured will be checked into control, allowing tracking of what changes have been made, and merging of users with more than one change.
  2. Allows for anyone to regenerate the files in the identical manner.
  3. Most file paths are relative to the settings file, this includes the ability to automatically add/remove files to the C# project/shared project file.

Please consider clicking the save button in the top right to save the settings where the classes are being generated in a source control.
");
            }
        }

        private void HideUnusedCategories()
        {
            foreach (var category in PropertiesGrid.FilterableGrid.GetAllGridItems()
                         .Where(i => i.Label != null
                                     && i.GridItemType == GridItemType.Category 
                                     && i.Label[0] > '4'))
            {
                category.Expanded = false;
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

        private void Create()
        {
            if (IsFormValid())
            {
                ExecuteMethod(CreateCode);
            }
        }

        private bool IsFormValid()
        {
            SettingsMap.PushChanges();
            var isValid =
                IsValidPath(SettingsMap.EntityTypesFolder, SettingsMap.CreateOneFilePerEntity, "Entities") &&
                IsValidPath(SettingsMap.OptionSetsTypesFolder, SettingsMap.CreateOneFilePerOptionSet, "OptionSets") &&
                IsValidPath(SettingsMap.MessageTypesFolder, SettingsMap.CreateOneFilePerMessage, "Actions") &&
                IsNamespaceDifferentThanContext();

            return isValid;
        }

        private static bool IsValidPath(string path, bool pathIsDirectory, string name)
        {
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

        public void CreateCode()
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
                if(MessageBox.Show($@"This version of the Early Bound Generator ({Settings.Version}) is older than the previous ran version from the settings ({Settings.SettingsVersion}).  You should probably update the plugin before running.  Are you sure you want to continue?", @"Older Version detected!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation) != DialogResult.Yes)
                {
                    EnableForm(true);
                    return;
                }
            }
            if (!SkipSaveSettings)
            {
                SaveSettings();
            }

            LogConfigSettings();
            WorkAsync(new WorkAsyncInfo("Starting Early Bound File Generation Logic...",
                (w, e) => // Work To Do Asynchronously
                {
                    var settings = (EarlyBoundGeneratorConfig) e.Argument;
                    Logger.WireUpToReportProgress(w);

                    var generator = new Logic(settings);
                    try
                    {
                        if (settings.UpdateBuilderSettingsJson)
                        {
                            w.ReportProgress(0, "Updating Builder Settings");
                            generator.UpdateBuilderSettingsJson();
                        }
                        w.ReportProgress(0, "Generating Early Bound Files");
                        generator.Create(Service);
                        w.ReportProgress(99, "Generation Complete!");
                        if (settings.UpdateBuilderSettingsJson)
                        {
                            generator.RemoveXrmToolBoxPluginPath();
                        }
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
                Settings.SupportsActions = ConnectionDetail.OrganizationMajorVersion >= Crm2013;
                Settings.UseCrmOnline = ConnectionDetail.UseOnline;
            }

            SettingsMap.PushChanges();
            var settingsDirectory = Path.GetDirectoryName(Path.GetFullPath(TxtSettingsPath.Text));
            Settings.RootPath = string.IsNullOrWhiteSpace(SettingsMap.OutputRelativeDirectory)
                ? settingsDirectory
                : SettingsMap.OutputRelativeDirectory.RootPath(settingsDirectory); ;

            var defaultConfig = EarlyBoundGeneratorConfig.GetDefault();
            defaultConfig.ExtensionConfig.XrmToolBoxPluginPath = Paths.PluginsPath;
            // Settings.SetExtensionArgument(CreationType.OptionSets, CrmSrvUtilService.CodeWriterFilter, defaultConfig.GetExtensionArgument(CreationType.OptionSets, CrmSrvUtilService.CodeWriterFilter).Value);
            // Settings.SetExtensionArgument(CreationType.OptionSets, CrmSrvUtilService.NamingService, defaultConfig.GetExtensionArgument(CreationType.OptionSets, CrmSrvUtilService.NamingService).Value);
        }

        private void LogConfigSettings()
        {
            var properties = Settings.GetType().GetProperties().Select(prop => new { Object = (object)Settings, Property = prop })
                .Union(Settings.ExtensionConfig.GetType().GetProperties().Select(prop => new { Object = (object)Settings.ExtensionConfig, Property = prop }))
                .ToDictionary(
                    k => k.Property.Name,
                    v => v.Property.GetValue(v.Object)?.ToString() ?? string.Empty);
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

            if(Telemetry.Enabled){
                TxtOutput.AppendText("Tracking Generation Event." + Environment.NewLine);
                Telemetry.TrackEvent("ConfigSettings", properties);
            }
            else
            {
                TxtOutput.AppendText("Tracking not enabled!  Please consider allowing the Xrm Tool Box to send anonymous statistics via the Configuration --> Settings -- Data Collect.  This allows reporting for which features are used and what features can be deprecated." + Environment.NewLine);
            }
        }

        private void EnableForm(bool enable)
        {
            BtnCreateAll.Enabled = enable;
            TxtSettingsPath.Enabled = enable;
            TxtOutput.Enabled = enable;
            PropertiesGrid.Enabled = enable;
        }

        #region Create Button Click Events

        private void BtnCreateBoth_Click(object sender, EventArgs e)
        {
            Create();
        }

        #endregion // Create Button Click Events

        private void BtnOpenSettingsPathDialog_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.SetXmlFilePath(TxtSettingsPath))
            {
                ValidatedSettingsPath();
            }
        }

        private void BtnRefreshSettings_Click(object sender, EventArgs e)
        {
            HydrateUiFromSettings(ConnectionSettings.FullSettingsPath);
        }

        private void BtnResetSettings_Click(object sender, EventArgs e)
        {
            HydrateUiFromSettings(EarlyBoundGeneratorConfig.GetDefault(), ConnectionSettings.FullSettingsPath);
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
            return null;
        }
    }

    [Export(typeof(IXrmToolBoxPlugin)),
     ExportMetadata("Name", "Early Bound Generator V2"),
     ExportMetadata("Description", "Adds advanced features and configuration to the generation of Early Bound Dataverse Tables."),
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
