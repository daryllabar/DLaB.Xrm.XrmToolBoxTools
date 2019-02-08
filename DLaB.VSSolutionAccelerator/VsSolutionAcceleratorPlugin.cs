using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows.Forms;
using DLaB.Log;
using DLaB.VSSolutionAccelerator.Wizard;
using DLaB.XrmToolBoxCommon;
using XrmToolBox.Extensibility;
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
                foreach (var page in InitializeSolutionInfo.InitializePages(Paths.SettingsPath))
                {
                    host.WizardPages.Add(page);
                }
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
                        "Abc.Xrm.WorkflowCore",
                        new List<string> {"Y", "Abc.Xrm.Test", "Abc.Xrm.TestCore" },
                        new List<string> {"Y", "Abc.Xrm.Plugin", "0"},
                        "Abc.Xrm.Plugin.Tests",
                        new List<string> {"Y", "Abc.Xrm.Workflow", "1"},
                        "Abc.Xrm.Workflow.Tests"
                    };

                    var info = InitializeSolutionInfo.InitializeSolution(results);
                    var solutionDir = Path.GetDirectoryName(info.SolutionPath) ?? Guid.NewGuid().ToString();
                    if (Directory.Exists(solutionDir))
                    {
                        foreach (var file in Directory.EnumerateFiles(solutionDir, "*", SearchOption.AllDirectories))
                        {
                            File.Delete(file);
                        }
                        Directory.Delete(solutionDir, true);
                    }
                    do
                    {
                        TxtOutput.AppendText("Creating Directory." + Environment.NewLine);
                        Directory.CreateDirectory(solutionDir);
                    }
                    while (!Directory.Exists(solutionDir));

                    File.Copy("C:\\Temp\\AdvXTB\\Abc.Xrm.sln", info.SolutionPath);
                    Execute(info);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void Execute(InitializeSolutionInfo info)
        {
            Enabled = false;
            WorkAsync(new WorkAsyncInfo("Adding Accelerator Libraries...", (w, e) => // Work To Do Asynchronously
            {
                var arg = (InitializeSolutionInfo)e.Argument;

                Logger.WireUpToReportProgress(w);
                try
                {
                    Logic.Logic.Execute(arg, Path.GetFullPath(Path.Combine(Paths.PluginsPath, "DLaB.VSSolutionAccelerator")));
                    w.ReportProgress(99, "Finished Successfully!");
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
                AsyncArgument = info,
                PostWorkCallBack = e => // Creation has finished.  Cleanup
                {
                    Logger.DisplayLog(e, TxtOutput);
                    Enabled = true;
                },
                ProgressChanged = e => // Logic wants to display an update
                {
                    Logger.DisplayLog(e, SetWorkingMessage, TxtOutput);
                }
            });
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
    public class VsSolutionAccelerator : PluginFactory, INoConnectionRequired
    {
        public override IXrmToolBoxPluginControl GetControl()
        {
            return new VsSolutionAcceleratorPlugin();
        }
    }
}