using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Windows.Forms;
using DLaB.VSSolutionAccelerator.Wizard;
using DLaB.XrmToolBoxCommon;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;
using Exception = System.Exception;

namespace DLaB.VSSolutionAccelerator
{
    public partial class VsSolutionAcceleratorPlugin : DLaBPluginControlBase
    {
        public Settings Settings { get; set; }

        public VsSolutionAcceleratorPlugin()
        {
            InitializeComponent();
            if (Debugger.IsAttached)
            {
                ActionCmb.Items.Add("Generate With Default Settings");
                ActionCmb.Items.Add("Add Plugins With Default Settings");
            }

            ActionCmb.SelectedIndex = 0;
            try
            {
                UnzipTemplate();
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to unzip the template.  " + ex.Message, ex);
            }
        }

        private void UnzipTemplate()
        {
            string zipPath = Path.Combine(Paths.PluginsPath, "DLaB.VSSolutionAccelerator", "Template.zip");
            if (!File.Exists(zipPath))
            {
                return;
            }

            var tmp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            File.SetAttributes(zipPath, FileAttributes.Normal);
            File.Move(zipPath, tmp);
            DeleteDirectory(Path.GetDirectoryName(zipPath));
            Directory.CreateDirectory(zipPath);
            ZipFile.ExtractToDirectory(tmp, Path.GetDirectoryName(zipPath));
            File.Delete(tmp);
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
                foreach (var page in InitializeSolutionInfo.InitializePages())
                {
                    host.WizardPages.Add(page);
                }
                host.LoadWizard();
                if (host.ShowDialog() == DialogResult.OK)
                {
                    var results = host.SaveResults;
                    var info = InitializeSolutionInfo.InitializeSolution(results);

                    Execute(info);
                }

                host.Close();
            }
        }

        private void ShowAddAssemblyWizard()
        {
            using (var host = new WizardHost
            {
                Text = @"Add Accelerators Wizard",
                ShowFirstButton = false,
                ShowLastButton = false
            })
            {
                foreach (var page in AddProjectToSolutionInfo.InitializePages())
                {
                    host.WizardPages.Add(page);
                }
                host.LoadWizard();
                if (host.ShowDialog() == DialogResult.OK)
                {
                    var results = host.SaveResults;
                    var info = AddProjectToSolutionInfo.Create(results);
                    Execute(info);
                }

                host.Close();
            }
        }

        private void ExecuteInstallCodeSnippets()
        {
            WorkAsync(new WorkAsyncInfo("Installing Code Snippets...", (w, e) => // Work To Do Asynchronously
            {
                Logic.VisualStudio.InstallCodeSnippets(Paths.PluginsPath);
            }).WithLogger(this, TxtOutput));
        }

        private void ExecuteBttn_Click(object sender, EventArgs e)
        {
            try
            {
                switch (ActionCmb.SelectedIndex)
                {
                    case 0:
                        ShowAddAcceleratorsWizard();
                        break;
                    case 1:
                        ShowAddAssemblyWizard();
                        break;
                    case 2:
                        ExecuteInstallCodeSnippets();
                        break;
                    case 3:
                        GenerateWithDefaultSettings();
                        break;
                    case 4:
                        GenerateAddAssemblyWithDefaultSettings();
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                TxtOutput.AppendText(Environment.NewLine + ex);
            }
        }

        private void GenerateWithDefaultSettings()
        {
            var results = new object[]
            {
                new List<string>{"Y", "C:\\Temp\\AdvXTB\\Abc.Xrm\\Abc.Xrm.sln" },
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
                "Y",
                "Abc.Xrm",
                "Abc.Xrm.WorkflowCore",
                new List<string> {"Y", "Abc.Xrm.Test", "Abc.Xrm.TestCore"},
                new List<string> {"Y", "Abc.Xrm.Plugin", "0"},
                "Abc.Xrm.Plugin.Tests",
                new List<string> {"Y", "Abc.Xrm.Workflow", "1"},
                "Abc.Xrm.Workflow.Tests",
                new List<string> {"0", "0"},
            };

            var info = InitializeSolutionInfo.InitializeSolution(results);
            var solutionDir = Path.GetDirectoryName(info.SolutionPath) ?? Guid.NewGuid().ToString();
            DeleteDirectory(solutionDir);

            do
            {
                TxtOutput.AppendText("Creating Directory." + Environment.NewLine);
                Directory.CreateDirectory(solutionDir);
            } while (!Directory.Exists(solutionDir));

            File.Copy("C:\\Temp\\AdvXTB\\Abc.Xrm.sln", info.SolutionPath);
            Execute(info);
        }

        private static void DeleteDirectory(string directoryPath)
        {
            try
            {
                TryDeleteDirectory(directoryPath);
            }
            catch
            {
                var secondTryFailed = false;
                try
                {
                    Thread.Sleep(3000);
                    TryDeleteDirectory(directoryPath);
                }
                catch
                {
                    secondTryFailed = true;
                }

                if (secondTryFailed)
                {
                    throw;
                }
            }
        }

        private static void TryDeleteDirectory(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                foreach (var file in Directory.EnumerateFiles(directoryPath, "*", SearchOption.AllDirectories))
                {
                    File.Delete(file);
                }

                Directory.Delete(directoryPath, true);
            }
        }


        private void GenerateAddAssemblyWithDefaultSettings()
        {
            if (File.Exists(@"C:\Temp\AdvXTB\Abc.Xrm\Abc.Xrm.Lead.Plugin\Abc.Xrm.Lead.Plugin.csproj"))
            {
                GenerateWithDefaultSettings();
                while (!Enabled)
                {
                    Thread.Sleep(10);
                }
            }
            var results = new object[]
            {
                @"C:\Temp\AdvXTB\Abc.Xrm\Abc.Xrm.sln",
                new List<string> {"Y", "Abc.Xrm.Lead.Plugin"},
                new List<string> {"Y", "Abc.Xrm.Lead.Plugin.Tests"},
                new List<string> {"Y", "Abc.Xrm.Lead.Workflow"},
                new List<string> {"Y", "Abc.Xrm.Lead.Workflow.Tests"},
            };

            var info = AddProjectToSolutionInfo.Create(results);
            Execute(info);
        }

        private void Execute(object info)
        {
            WorkAsync(new WorkAsyncInfo("Performing requested operations...", (w, e) => // Work To Do Asynchronously
            {
                var templatePath = Path.GetFullPath(Path.Combine(Paths.PluginsPath, Settings.TemplateFolder));
                if (e.Argument is InitializeSolutionInfo solutionInfo)
                {
                    if (solutionInfo.InstallSnippets)
                    {
                        Logic.VisualStudio.InstallCodeSnippets(Paths.PluginsPath);
                    }
                    Logic.SolutionInitializer.Execute(solutionInfo, templatePath);
                }
                else if (e.Argument is AddProjectToSolutionInfo projectInfo)
                {
                    Logic.SolutionUpdater.Execute(projectInfo, templatePath);
                }
            }).WithLogger(this, TxtOutput, info));
        }

        private void ActionCmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteBttn.Enabled = ActionCmb.SelectedIndex >= 0;
        }
    }

    [Export(typeof(IXrmToolBoxPlugin)),
     ExportMetadata("Name", "Visual Studio Solution Accelerator"),
     ExportMetadata("Description", "Adds recommended isolation/accelerator projects for use with the DLaB.Xrm and XrmUnitTest framework to your Visual Studio solution."),
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