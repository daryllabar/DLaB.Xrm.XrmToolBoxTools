using System;
using System.ComponentModel.Composition;
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
                host.WizardPages.Add(GenericPage.CreatePathQuestion(new PathQuestionInfo("What Solution?")
                {
                    Filter = "Solution Files (*.sln)|*.sln",
                    Description = "This Wizard will walk through the process of adding isolation/plugin/workflow/testing projects based on the DLaB/XrmUnitTest framework, adding the projects to the solution defined here."
                }));
                host.WizardPages.Add(GenericPage.CreateTextQuestion(new TextQuestionInfo("What is the root NameSpace?")
                {
                    DefaultResponse = "YourCompanyNameOrAbbreviation.Xrm",
                    Description = "This is the root namespace that will the Plugin and (if desired) Early Bound Entities will be appended to."
                }));
                host.WizardPages.Add(NuGetVersionSelectorPage.Create(
                    "What version of the SDK?",
                    PackageLister.CoreXrmAssemblies,
                    "This will determine the NuGet packages referenced and the version of the .Net Framework to use."));
                
                host.LoadWizard();
                if (host.ShowDialog() == DialogResult.OK)
                {
                    var results = host.Results;

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