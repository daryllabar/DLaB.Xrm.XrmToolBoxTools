using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using DLaB.EarlyBoundGenerator.Settings;
using DLaB.XrmToolboxCommon;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;
using PropertyInterface = DLaB.XrmToolboxCommon.PropertyInterface;

namespace DLaB.EarlyBoundGenerator
{
    public partial class EarlyBoundGeneratorPlugin : DLaBPluginControlBase, PropertyInterface.IEntityMetadatas, PropertyInterface.IGlobalOptionSets, PropertyInterface.IActions
    {
        public Config Settings { get; set; }
        public IEnumerable<Entity> Actions { get; set; }
        public IEnumerable<EntityMetadata> EntityMetadatas { get; set; }
        public IEnumerable<OptionSetMetadataBase> GlobalOptionSets { get; set; }

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
            Settings = Config.Load(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            ChkAddDebuggerNonUserCode.Checked = Settings.ExtensionConfig.AddDebuggerNonUserCode;
            ChkAddFilesToProject.Checked = Settings.ExtensionConfig.AddNewFilesToProject;
            ChkCreateOneActionFile.Checked = Settings.ExtensionConfig.CreateOneFilePerAction;
            ChkCreateOneEntityFile.Checked = Settings.ExtensionConfig.CreateOneFilePerEntity;
            ChkCreateOneOptionSetFile.Checked = Settings.ExtensionConfig.CreateOneFilePerOptionSet;
            ChkIncludeCommandLine.Checked = Settings.IncludeCommandLine;
            ChkMakeReadonlyFieldsEditable.Checked = Settings.ExtensionConfig.MakeReadonlyFieldsEditable;
            ChkMaskPassword.Checked = Settings.MaskPassword;
            ChkGenerateAttributeNameConsts.Checked = Settings.ExtensionConfig.GenerateAttributeNameConsts;
            ChkGenerateAnonymousTypeConstructor.Checked = Settings.ExtensionConfig.GenerateAnonymousTypeConstructor;
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

        public override void ClosingPlugin(PluginCloseInfo info)
        {
            base.ClosingPlugin(info);
            if (info.Cancel) return;

            ConnectionDetail = null; // Don't save the Connection Details when closing.
            HydrateSettingsFromUI();
            Settings.Save();
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
            Settings.Save();

            WorkAsync(new WorkAsyncInfo("Shelling out to Command Line...",
                (w, e) => // Work To Do Asynchronously
                {
                    var settings = (Config) e.Argument;

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
                        TxtOutput.AppendText(result.Detail + Environment.NewLine);
                        summary = result.Summary;
                    }
                    // Status Update
                    if (e.ProgressPercentage == int.MinValue)
                    {
                        TxtOutput.AppendText(e.UserState + Environment.NewLine);
                    }
                    else
                    {
                        SetWorkingMessage(summary);
                    }
                }
            });
        }

        // ReSharper disable once InconsistentNaming
        private void HydrateSettingsFromUI()
        {
            if (ConnectionDetail != null)
            {
                Settings.Domain = ConnectionDetail.UserDomain;
                Settings.Password = ConnectionDetail.GetUserPassword();
                Settings.SupportsActions = ConnectionDetail.OrganizationMajorVersion >= Crm2013;
                Settings.Url = ConnectionDetail.OrganizationServiceUrl;
                Settings.UseCrmOnline = ConnectionDetail.UseOnline;
                Settings.UserName = ConnectionDetail.UserName;
            }

            Settings.ActionOutPath = TxtActionPath.Text;
            Settings.EntityOutPath = TxtEntityPath.Text;
            if (ChkUseDeprecatedOptionSetNaming.Checked)
            {
                Settings.SetExtensionArgument(CreationType.OptionSets, CrmSrvUtilService.CodeWriterFilter, @"DLaB.CrmSvcUtilExtensions.OptionSet.CodeWriterFilterService,DLaB.CrmSvcUtilExtensions");
                Settings.SetExtensionArgument(CreationType.OptionSets, CrmSrvUtilService.NamingService, string.Empty);
            }
            else
            {
                var defaultConfig = Config.GetDefault();
                Settings.SetExtensionArgument(CreationType.OptionSets, CrmSrvUtilService.CodeWriterFilter, defaultConfig.GetExtensionArgument(CreationType.OptionSets, CrmSrvUtilService.CodeWriterFilter).Value);
                Settings.SetExtensionArgument(CreationType.OptionSets, CrmSrvUtilService.NamingService, defaultConfig.GetExtensionArgument(CreationType.OptionSets, CrmSrvUtilService.NamingService).Value);
            }

            var extensions = Settings.ExtensionConfig;
            extensions.AddDebuggerNonUserCode = ChkAddDebuggerNonUserCode.Checked;
            extensions.AddNewFilesToProject = ChkAddFilesToProject.Checked;
            extensions.CreateOneFilePerAction = ChkCreateOneActionFile.Checked;
            extensions.CreateOneFilePerEntity = ChkCreateOneEntityFile.Checked;
            extensions.CreateOneFilePerOptionSet = ChkCreateOneOptionSetFile.Checked;
            extensions.GenerateAttributeNameConsts = ChkGenerateAttributeNameConsts.Checked;
            extensions.GenerateAnonymousTypeConstructor = ChkGenerateAnonymousTypeConstructor.Checked;
            extensions.GenerateEnumProperties = ChkGenerateOptionSetEnums.Checked;
            extensions.InvalidCSharpNamePrefix = TxtInvalidCSharpNamePrefix.Text;
            extensions.MakeReadonlyFieldsEditable = ChkMakeReadonlyFieldsEditable.Checked;
            extensions.LocalOptionSetFormat = TxtOptionSetFormat.Text;
            extensions.RemoveRuntimeVersionComment = ChkRemoveRuntimeComment.Checked;
            extensions.UseXrmClient = ChkUseXrmClient.Checked;
            extensions.UseDeprecatedOptionSetNaming = ChkUseDeprecatedOptionSetNaming.Checked;
            extensions.UseTfsToCheckoutFiles = ChkUseTFS.Checked;
            Settings.IncludeCommandLine = ChkIncludeCommandLine.Checked;
            Settings.MaskPassword = ChkMaskPassword.Checked;
            Settings.Namespace = TxtNamespace.Text;
            Settings.OptionSetOutPath = TxtOptionSetPath.Text;
            Settings.ServiceContextName = string.IsNullOrWhiteSpace(TxtServiceContextName.Text) ? null : TxtServiceContextName.Text;
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
            Settings.Save();
        }

        private void BtnActionsToSkip_Click(object sender, EventArgs e)
        {
            var dialog = new SpecifyActionsDialog(this) { SpecifiedActions = Settings.ExtensionConfig.ActionsToSkip };
            if (dialog.ShowDialog() != DialogResult.OK) return;
            Settings.ExtensionConfig.ActionsToSkip = dialog.SpecifiedActions;
            Settings.Save();
        }

        private void BtnEntitesToSkip_Click(object sender, EventArgs e)
        {
            var dialog = new SpecifyEntitiesDialog(this) { SpecifiedEntities = Settings.ExtensionConfig.EntitiesToSkip };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Settings.ExtensionConfig.EntitiesToSkip = dialog.SpecifiedEntities;
                Settings.Save();
            }
        }

        private void BtnOptionSetsToSkip_Click(object sender, EventArgs e)
        {
            var dialog = new SpecifyOptionSetsDialog(this) { OptionSets = Settings.ExtensionConfig.OptionSetsToSkip };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Settings.ExtensionConfig.OptionSetsToSkip = dialog.OptionSets;
                Settings.Save();
            }
        }

        private void BtnEnumMappings_Click(object sender, EventArgs e)
        {
            var dialog = new AttributesToEnumMapperDialog(this) { ConfigValue = Settings.ExtensionConfig.PropertyEnumMappings };
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                Settings.ExtensionConfig.PropertyEnumMappings = dialog.ConfigValue;
                Settings.Save();
            }
        }

        private void BtnUnmappedProperties_Click(object sender, EventArgs e)
        {
            var dialog = new SpecifyAttributesDialog (this) { ConfigValue = Settings.ExtensionConfig.UnmappedProperties };
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                Settings.ExtensionConfig.UnmappedProperties = dialog.ConfigValue;
                Settings.Save();
            }
        }

        #region HelpText

        private void BtnActionsToSkip_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Allows for the ability to specify Actions to not generate.";
        }

        private void BtnCreateActions_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Generates the Activity Classes.";
        }

        private void BtnCreateAll_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Asynchronously generates the Entites, Option Sets, and Actions (if available), with their perspective settings.";
        }

        private void BtnCreateEntities_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Generates the Entity Classes.";
        }

        private void BtnCreateOptionSets_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Generates the Enums for the Option Sets.";
        }

        private void BtnEntitesToSkip_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Allows for the ability to specify Entities to not generate.";
        }

        private void BtnEnumMappings_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Manually specifies an enum mapping for an OptionSetValue Property on an entity.";
        }

        private void BtnOptionSetsToSkip_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Allows for the ability to specify OptionSets to not generate.";
        }

        private void BtnSpecifyAttributeNames_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Allows for the ability to specify the capitalization of an attribute on an entity.";
        }

        private void BtnUnmappedProperties_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Allows for the ability to specify an OptionSetValue Property of an entity that doesn't have an enum mapping.";
        }

        private void ChkAddDebuggerNonUserCode_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Specifies that the DebuggerNonUserCodeAttribute should be applied to all generated properties and methods.";
        }

        private void ChkAddFilesToProject_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Adds any files that don't exist to the first project file found in the hierarchy of the output path.";
        }

        private void ChkCreateOneActionFile_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Specifies that each Activity will be created in its own file.";
        }

        private void ChkCreateOneEntityFile_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Specifies that each Entity will be created in its own file.";
        }

        private void ChkCreateOneOptionSetFile_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Specifies that each Enum will be created in its own file.";
        }

        private void ChkGenerateAttributeNameConsts_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Adds a Struct to each Entity class that contains the Logical Names of all attributes for the Entity.";
        }

        private void ChkGenerateAnonymousTypeConstructor_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Adds an Object Constructor to each early bound entity class to simplify LINQ Projections (http://stackoverflow.com/questions/27623542).";
        }

        private void ChkGenerateOptionSetEnums_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Adds an additional property to each early bound entity class, for each optionset property it normally contains, with Enum postfixed to the existing optionset name.";
        }

        private void ChkIncludeCommandLine_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Specifies whether to include the command line in the early bound class used to generate it.";
        }

        private void ChkMakeReadonlyFieldsEditable_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Defines that Entities should be created with editable createdby, createdon, modifiedby, modifiedon, owningbusinessunit, owningteam, and owninguser properties. Helpful for writing linq statements where those attributes are wanting to be returned in the select.";
        }

        private void ChkMaskPassword_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Masks the password in the command line.";
        }

        private void ChkRemoveRuntimeComment_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Removes the ""//   Runtime Version:X.X.X.X"" comment from the header of generated files.  This helps to alleviate unnecessary differences that pop up when the classes are generated from machines with different .Net Framework updates installed.";
        }

        private void ChkUseDeprecatedOptionSetNaming_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Creates Local OptionSets Using the Deprecated Naming Convention. prefix_oobentityname_prefix_attribute";
        }

        private void ChkUseTFS_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Will use TFS to attempt to check out the early bound classes.";
        }

        private void ChkUseXrmClient_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Specifies the Service Context should inherit from CrmOrganizationServiceContext, and conversly, Entities from Xrm.Client.Entity." + Environment.NewLine +
                @"This results in a dependence on Microsoft.Xrm.Client.dll that must be accounted for during plugins and workflows since it isn't included with CRM by default:" + Environment.NewLine +
                @"http://develop1.net/public/post/MicrosoftXrmClient-Part-1.aspx .";
        }

        private void TxtOptionSetFormat_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"The Format of Local Option Sets where {0} is the Entity Schema Name, and {1} is the Attribute Schema Name.  The format Specified in the SDK is {0}{1}, but the default is {0}_{1}, but used to be prefix_{0}_{1}(all lower case)";
        }

        private void TxtServiceContextName_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Specifies that the name of the Generated CRM Context.";
        }

        private void ShowActionPathText(object sender, EventArgs e)
        {
            TxtHelp.Text = ChkCreateOneActionFile.Checked ? 
                @"Since ""Create One File Per Action"" is checked, this needs to be a file path that ends in "".cs""" : 
                @"Since ""Create One File Per Action"" is not checked, this needs to be a path to a directory.";
        }

        private void ShowEntityPathText(object sender, EventArgs e)
        {
            TxtHelp.Text = ChkCreateOneEntityFile.Checked ? 
                @"Since ""Create One File Per Entity"" is checked, this needs to be a file path that ends in "".cs""" : 
                @"Since ""Create One File Per Entity"" is not checked, this needs to be a path to a directory.";
        }

        #endregion // HelpText

        private void BtnOpenActionPathDialog_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                TxtActionPath.Text = openFileDialog1.FileName;
            }
        }

        private void BtnOpenEntityPathDialog_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                TxtEntityPath.Text = openFileDialog1.FileName;
            }
        }

        private void BtnOpenOptionSetPathDialog_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                TxtOptionSetPath.Text = openFileDialog1.FileName;
            }
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

        private void EarlyBoundGenerator_ConnectionUpdated(object sender, ConnectionUpdatedEventArgs e)
        {
            EntityMetadatas = null;
            GlobalOptionSets = null;
            Actions = null;
            DisplayActionsIfSupported(false);
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
