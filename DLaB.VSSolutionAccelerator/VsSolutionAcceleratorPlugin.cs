using DLaB.VSSolutionAccelerator.Wizard;
using DLaB.XrmToolBoxCommon;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Windows.Forms;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;
using Exception = System.Exception;

namespace DLaB.VSSolutionAccelerator
{
    public partial class VsSolutionAcceleratorPlugin : DLaBPluginControlBase, IMessageBusHost
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
            var zipPath = Path.Combine(Paths.PluginsPath, "DLaB.VSSolutionAccelerator", "Template.zip");
            var zipDirectory = Path.GetDirectoryName(zipPath) ?? "UnableToGetZipDirectory";
            if (!File.Exists(zipPath))
            {
                return;
            }

            var tmp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            File.SetAttributes(zipPath, FileAttributes.Normal);
            File.Move(zipPath, tmp);
            DeleteDirectory(zipDirectory);
            Directory.CreateDirectory(zipDirectory);
            ZipFile.ExtractToDirectory(tmp, zipDirectory);
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
            Settings.Initialize();
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
            var results = new AddAllWizardResults
            {
                P0AddToExistingSolution = false,
                P0SolutionPath = "C:\\Temp\\VSA\\Acme.Dataverse.sln",
                P1Namespace = "Acme.Dataverse",
                P2EarlyBound = true,
                P3SharedCommonAssemblyName = "Acme.Dataverse",
                P4SharedWorkflowProjectName = "Acme.Dataverse.WorkflowCore",
                P5UseXrmUnitTest = true, P5TestSettingsProjectName = "Acme.Dataverse.Test",
                P6CreatePluginProject = true, P6PluginProjectName = "Acme.Dataverse.Plugin", P6IncludeExamples = true,
                P7CompanyName = "Acme", P7PluginDescription = "Default Description For Plugin", P7PacAuthName = "Daryl Dev",
                P8PluginTestProjectName = "Acme.Dataverse.Plugin.Tests",
                P9CreateWorkflowProject = true, P9WorkflowProjectName = "Acme.Dataverse.Workflow", P9IncludeExamples = true,
                P10WorkflowTestProjectName = "Acme.Dataverse.Workflow.Tests",
                P11InstallCodeSnippets = true, P11IncludeCodeGen = true

            }.GetResults();

            var info = InitializeSolutionInfo.InitializeSolution(results);
            var solutionDir = Path.GetDirectoryName(info.SolutionPath) ?? Guid.NewGuid().ToString();
            DeleteDirectory(solutionDir);

            do
            {
                TxtOutput.AppendText("Creating Directory." + Environment.NewLine);
                Directory.CreateDirectory(solutionDir);
            } while (!Directory.Exists(solutionDir));

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
            if (File.Exists(@"C:\Temp\VSA\Acme.Dataverse.Lead.Plugin\Acme.Dataverse.Lead.Plugin.csproj"))
            {
                GenerateWithDefaultSettings();
                while (!Enabled)
                {
                    Thread.Sleep(10);
                }
            }
            var results = new AddPluginWorkflowWizardResults
            {
                P0SolutionPath = @"C:\Temp\VSA\Acme.Dataverse.sln",
                P1CreatePluginProject = true, P1PluginProjectName = "Acme.Dataverse.Lead.Plugin",
                P2CreatePluginXrmUnitTest = true, P2PluginTestProjectName = "Acme.Dataverse.Lead.Plugin.Tests",
                P3CreateWorkflowProject = true, P3WorkflowProjectName = "Acme.Dataverse.Lead.Workflow",
                P4CreateWorkflowXrmUnitTest = true, P4WorkflowTestProjectName ="Acme.Dataverse.Lead.Workflow.Tests",
            }.GetResults();

            var info = AddProjectToSolutionInfo.Create(results);
            Execute(info);
        }

        private void Execute(object info)
        {
            WorkAsync(new WorkAsyncInfo("Performing requested operations...", (w, e) => // Work To Do Asynchronously
            {
                var templatePath = Path.GetFullPath(Path.Combine(Paths.PluginsPath, Settings.TemplateFolder));
                var nuGetSettings = new Logic.NuGetSettings(templatePath)
                {
                    Sources = Settings.NugetSourcesList
                };
                if (e.Argument is InitializeSolutionInfo solutionInfo)
                {
                    if (solutionInfo.InstallSnippets)
                    {
                        Logic.VisualStudio.InstallCodeSnippets(Paths.PluginsPath);
                    }
                    Logic.SolutionInitializer.Execute(solutionInfo, templatePath, nuGetSettings: nuGetSettings);
                    if (solutionInfo.ConfigureEarlyBound)
                    {
                        e.Result = solutionInfo.GetEarlyBoundSettingsPath();
                    }
                }
                else if (e.Argument is AddProjectToSolutionInfo projectInfo)
                {
                    Logic.SolutionUpdater.Execute(projectInfo, templatePath, nuGetSettings: nuGetSettings);
                }
            }).WithLogger(this, TxtOutput, info, onComplete: e =>
            {
                if (e.Result is string path)
                {
                    MessageBox.Show(@"The Early Bound Generator will now be opened in order to generate the early bound entities for your project.  Click the ""Generate"" button in the Early Bound Generator to generate your entities.", @"Generate Early Bound Entities!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    OpenEarlyBoundGeneratorWithSettings(path);
                }
            }));
        }

        private void ActionCmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteBttn.Enabled = ActionCmb.SelectedIndex >= 0;
        }

        public event EventHandler<MessageBusEventArgs> OnOutgoingMessage;

        public void OnIncomingMessage(MessageBusEventArgs message)
        {
            if (message.SourcePlugin != "Visual Studio Solution Accelerator")
            {
                return;
            }
            throw new NotImplementedException();
        }

        private void OpenEarlyBoundGeneratorWithSettings(string path)
        {
            if (OnOutgoingMessage is null)
            {
                var message = @"No events are registered on the OnOutgoingMessage event!  Unable to open the Early Bound Generator!";
                MessageBox.Show(message, @"Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                TxtOutput.AppendText(Environment.NewLine + message);
                return;
            }

            var request = new Dictionary<string, object>{
                { "path", path }
            };
            OnOutgoingMessage(this, new MessageBusEventArgs("Early Bound Generator V2")
            {
                TargetArgument = request
            });
        }
    }
}