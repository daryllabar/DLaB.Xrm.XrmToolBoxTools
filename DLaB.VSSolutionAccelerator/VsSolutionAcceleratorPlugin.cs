using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows.Forms;
using DLaB.VSSolutionAccelerator.Wizard;
using DLaB.XrmToolBoxCommon;
using XrmToolBox.Extensibility;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using XrmToolBox.Extensibility.Interfaces;

namespace DLaB.VSSolutionAccelerator
{
    public partial class VsSolutionAcceleratorPlugin : DLaBPluginControlBase
    {
        public Settings Settings { get; set; }

        public VsSolutionAcceleratorPlugin()
        {
            InitializeComponent();
        }

        private void MyPluginControl_Load(object sender, EventArgs e)
        {
            ShowInfoNotification("This is a notification that can lead to XrmToolBox repository", new Uri("https://github.com/MscrmTools/XrmToolBox"));

            // Loads or creates the settings for the plugin
            if (!SettingsManager.Instance.TryLoad(GetType(), out Settings settings))
            {
                Settings = new Settings();

                LogWarning("Settings not found => a new settings file has been created!");
            }
            else
            {
                Settings = settings;
                LogInfo("Settings found and loaded");
            }
        }

        private void tsbSample_Click(object sender, EventArgs e)
        {
            // The ExecuteMethod method handles connecting to an
            // organization if XrmToolBox is not yet connected
            ExecuteMethod(GetAccounts);
        }

        private void GetAccounts()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Getting accounts",
                Work = (worker, args) =>
                {
                    args.Result = Service.RetrieveMultiple(new QueryExpression("account")
                    {
                        TopCount = 50
                    });
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    var result = args.Result as EntityCollection;
                    if (result != null)
                    {
                        MessageBox.Show($"Found {result.Entities.Count} accounts");
                    }
                }
            });
        }

        private void VsSolutionAcceleratorPlugin_OnCloseTool(object sender, EventArgs e)
        {
            SettingsManager.Instance.Save(GetType(), Settings);
        }

        private void ShowAddAcceleratorsWizard()
        {
            using (var host = new WizardHost
            {
                Text = @"Add Accelerators Wizard",
                ShowFirstButton = false,
                ShowLastButton = false
            })
            {
                //host.WizardCompleted += new WizardHost.WizardCompletedEventHandler(host_WizardCompleted);
                host.WizardPages.Add(GenericPage.Create(new PathQuestionInfo("What Solution?")
                {
                    Filter = "Solution Files (*.sln)|*.sln",
                    Description = "This Wizard will walk through the process of adding isolation/plugin/workflow/testing projects based on the DLaB/XrmUnitTest framework, adding the projects to the solution defined here."
                }));
                host.WizardPages.Add(GenericPage.Create(new TextQuestionInfo("What is the root NameSpace?")
                {
                    DefaultResponse = "YourCompanyNameOrAbbreviation.Xrm",
                    Description = "This is the root namespace that will the Plugin and (if desired) Early Bound Entities will be appended to."
                }));
                host.WizardPages.Add(NuGetVersionSelectorPage.Create(
                    "What version of the SDK?",
                    PackageLister.CoreXrmAssemblies,
                    "This will determine the NuGet packages referenced and the version of the .Net Framework to use."));
                host.WizardPages.Add(GenericPage.Create(new ConditionalYesNoQuestionInfo("Do you want to use the Early Bound Generator To Create Early Bound Entities?")
                {
                    Yes = new PathQuestionInfo("What is the path to the Early Bound Generator Settings.xml file?")
                    {
                        Filter = "EBG Setting File (*.xml)|*.xml",
                        DefaultResponse = Path.GetFullPath(Path.Combine(Paths.SettingsPath, "DLaB.EarlyBoundGenerator.DefaultSettings.xml")),
                        Description = "The selected settings file will be moved to the folder of the solution, and configured to place the output of the files in the appropriate folders." 
                                      + Environment.NewLine
                                      + "The Early Bound Generator will also be triggered upon completion to generated the Early Bound classes."
                    },
                    Description = "Configures the output paths of the Early Bound Generator to generate files in the appropriate shared project within the solution."
                        + Environment.NewLine
                        + "This requires the XrmToolBox Early Bound Generator to be installed."
                }));
                host.WizardPages.Add(GenericPage.Create(new TextQuestionInfo("What do you want the name of the shared common assembly to be?")
                {
                    DefaultResponse = "SaveResults[1]",
                    Description = "This will be the name of a shared C# project that will be used to store common code including plugin logic and early bound entities if applicable"
                }));

                host.WizardPages.Add(GenericPage.Create(new TextQuestionInfo("What do you want the name of the shared common workflow assembly to be?")
                {
                    DefaultResponse = "SaveResults[1].Workflow",
                    Description = "This will be the name of a shared C# project that contains references to the workflow code.  It would only be required by assemblies containing a workflow."
                }));
                host.WizardPages.Add(GenericPage.Create(new ConditionalYesNoQuestionInfo("Do you want to use XrmUnitTest for unit testing?")
                {
                    Yes = new TextQuestionInfo("What do you want the Test Settings project to be called?")
                    {
                        DefaultResponse = "SaveResults[1].Test",
                        Description = "The Test Settings project will contain the single test settings config file and assumption xml files."
                    },
                    Yes2 = new TextQuestionInfo("What do you want the shared core test project to be called?")
                    {
                        DefaultResponse = "SaveResults[1].TestCore",
                        Description = "The shared Test Project will contain all other shared test code (Assumption Definitions, Builders, Test Base Class, etc)"
                    },
                    Description = "This will add the appropriate NuGet References and create the appropriate isolation projects."
                }));
                host.WizardPages.Add(GenericPage.Create(new ConditionalYesNoQuestionInfo("Do you want to create a Plugin Project?")
                {
                    Yes = new TextQuestionInfo("What should the plugin project be called?")
                    {
                        DefaultResponse = "SaveResults[1].Plugin",
                        Description = "The name and default namespace for the plugin project."
                    },
                    Description = "This will add a new plugin project to the solution and wire up the appropriate references."
                }));
                host.WizardPages.Add(GenericPage.Create(new ConditionalYesNoQuestionInfo("Do you want to create a Workflow Project?")
                {
                    Yes = new TextQuestionInfo("What should the workflow project be called??")
                    {
                        DefaultResponse = "SaveResults[1].Workflow",
                        Description = "The name and default namespace for the workflow project."
                    },
                    Description = "This will add a new workflow project to the solution and wire up the appropriate references."
                }));
                host.LoadWizard();
                if (host.ShowDialog() == DialogResult.OK)
                {
                    var results = host.SaveResults;

                }

                host.Close();
            }
        }

        private void ExecuteBttn_Click(object sender, EventArgs e)
        {
            switch (ActionCmb.SelectedIndex)
            {
                case 0:
                    ShowAddAcceleratorsWizard();
                    break;
                case 1:
                    var results = new object[]
                    {
                        "C:\\Temp\\AdvXTB\\Abc.Xrm\\Abc.Xrm.sln",
                        "Abc.Xrm",
                        new NuGetPackage
                        {
                            Id = "Microsoft.CrmSdk.CoreAssemblies",
                            LicenseUrl = "http://download.microsoft.com/download/E/1/8/E18C0FAD-FEC8-44CD-9A16-98EDC4DAC7A2/LicenseTerms.docx",
                            Name = "Microsoft Dynamics 365 SDK core assemblies",
                            Version = new Version("9.0.2.5"),
                            VersionText = "9.0.2.5",
                            XrmToolingClient = false
                        },
                        new List<string> {"Y", "C:\\Users\\daryl.labar\\Documents\\GitHub\\DLaB.Xrm.XrmToolBoxTools\\DLaB.VSSolutionAccelerator\\bin\\Debug\\Settings\\DLaB.EarlyBoundGenerator.DefaultSettings.xmlC:\\Users\\daryl.labar\\Documents\\GitHub\\DLaB.Xrm.XrmToolBoxTools\\DLaB.VSSolutionAccelerator\\bin\\Debug\\Settings\\DLaB.EarlyBoundGenerator.DefaultSettings.xml"},
                        "Abc.Xrm",
                        "Abc.Xrm.Workflow",
                        new List<string> {"Y", "Abc.Xrm.Test", "Abc.Xrm.TestCore" },
                        new List<string> {"Y", "Abc.Xrm.Plugin"},
                        new List<string> {"Y", "Abc.Xrm.Workflow"}
                    };

                    Logic.Execute(InitializeSolutionInfo.InitializeSolution(results));

                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void ActionCmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteBttn.Enabled = ActionCmb.SelectedIndex >= 0;
        }
    }

    [Export(typeof(IXrmToolBoxPlugin)),
     ExportMetadata("Name", "VS Solution Accelerator"),
     ExportMetadata("Description", "Creates recommended isolation assemblies for use with the DLaB.Xrm and XrmUnitTest framework."),
     ExportMetadata("SmallImageBase64", SmallImage32X32), // null for "no logo" image or base64 image content 
     ExportMetadata("BigImageBase64", LargeImage120X120), // null for "no logo" image or base64 image content 
     ExportMetadata("BackgroundColor", "White"), // Use a HTML color name
     ExportMetadata("PrimaryFontColor", "#000000"), // Or an hexadecimal code
     ExportMetadata("SecondaryFontColor", "DarkGray")]
    public class VsSolutionAccelerator : PluginFactory
    {
        public override IXrmToolBoxPluginControl GetControl()
        {
            return new VsSolutionAcceleratorPlugin();
        }
    }
}