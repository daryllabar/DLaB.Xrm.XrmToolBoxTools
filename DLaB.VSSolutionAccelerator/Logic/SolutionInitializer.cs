using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using DLaB.Log;

namespace DLaB.VSSolutionAccelerator.Logic
{
    public class SolutionInitializer : SolutionEditor
    {
        public SolutionInitializer(string solutionPath, string templateDirectory, string strongNamePath = null, string nugetPath = null)
            : base(solutionPath, templateDirectory, strongNamePath, nugetPath)
        {
        }

        public Dictionary<string, ProjectInfo> GetProjectInfos(InitializeSolutionInfo info)
        {
            var projects = new Dictionary<string, ProjectInfo>();
            AddSharedCommonProject(projects, info);
            AddSharedWorkflowProject(projects, info);
            if (info.ConfigureXrmUnitTest)
            {
                AddSharedTestCoreProject(projects, info);
                AddBaseTestProject(projects, info);
            }
            if (info.CreatePlugin)
            {
                AddPlugin(projects, info);
                if (info.ConfigureXrmUnitTest)
                {
                    AddPluginTest(projects, info);
                }
            }
            if (info.CreateWorkflow)
            {
                AddWorkflow(projects, info);
                if (info.ConfigureXrmUnitTest)
                {
                    AddWorkflowTest(projects, info);
                }
            }

            AddNugetPostUpdateCommandsToProjects(info.XrmPackage.Version, projects);
            return projects;
        }

        private void AddSharedCommonProject(Dictionary<string, ProjectInfo> projects, InitializeSolutionInfo info)
        {
            var project = CreateDefaultSharedProjectInfo(
                ProjectInfo.Keys.Common,
                info.SharedCommonProject);

            var projItems = project.Files.First(f => f.Name.EndsWith(".projitems"));
            projItems.Removals.Add("$(MSBuildThisFileDirectory)Entities");
            projItems.RemovalsToSkip.Add(@"$(MSBuildThisFileDirectory)Entities\Actions\xyz");
            projects.Add(project.Key, project);
        }

        private void AddSharedWorkflowProject(Dictionary<string, ProjectInfo> projects, InitializeSolutionInfo info)
        {
            var project = CreateDefaultSharedProjectInfo(
                ProjectInfo.Keys.WorkflowCommon,
                info.SharedCommonWorkflowProject,
                "Xyz.Xrm.Workflow",
                info.RootNamespace + ".Workflow");
            projects.Add(project.Key, project);
        }

        private void AddSharedTestCoreProject(Dictionary<string, ProjectInfo> projects, InitializeSolutionInfo info)
        {
            var project = CreateDefaultSharedProjectInfo(
                ProjectInfo.Keys.TestCore,
                info.SharedTestCoreProject, 
                ProjectInfo.Keys.Test,
                info.TestBaseProject);

            projects.Add(project.Key, project);
        }

        private void AddBaseTestProject(Dictionary<string, ProjectInfo> projects, InitializeSolutionInfo info)
        {
            var project = CreateDefaultProjectInfo(
                ProjectInfo.Keys.Test,
                info.TestBaseProject,
                "v4.6.2",
                info.SharedCommonProject);
            project.Files.Add(new ProjectFile
            {
                Name = @"Assumptions\Entity Xml\Product_Install.xml",
                Replacements = new Dictionary<string, string>
                {
                    {"Xyz.Xrm", info.RootNamespace}
                }
            });
            projects.Add(project.Key, project);
        }

        private ProjectInfo CreateDefaultSharedProjectInfo(string key, string name, string originalNamespace = null, string newNamespace = null)
        {
            Logger.AddDetail($"Configuring Project {name} based on {key}.");
            originalNamespace = originalNamespace ?? key;
            newNamespace = newNamespace ?? name;
            var id = Guid.NewGuid();
            var project = new ProjectInfo
            {
                Key = key,
                Id = id,
                Type = ProjectInfo.ProjectType.SharedProj,
                NewDirectory = Path.Combine(OutputBaseDirectory, name),
                Name = name,
                Files = new List<ProjectFile>
                {
                    new ProjectFile
                    {
                        Name = name + ".projitems",
                        Replacements = new Dictionary<string, string>
                        {
                            {ProjectInfo.IdByKey[key], id.ToString()},
                            {$"<Import_RootNamespace>{originalNamespace}</Import_RootNamespace>", $"<Import_RootNamespace>{newNamespace}</Import_RootNamespace>"}
                        },
                    },
                    new ProjectFile
                    {
                        Name = name + ".shproj",
                        Replacements = new Dictionary<string, string>
                        {
                            {ProjectInfo.IdByKey[key], id.ToString()},
                            { key +".projitems", name + ".projitems"}
                        }
                    },
                }
            };
            return project;
        }

        public static void Execute(InitializeSolutionInfo info, string templateDirectory, string strongNamePath = null)
        {
            Logger.AddDetail($"Starting to process solution '{info.SolutionPath}' using templates from '{templateDirectory}'");
            CreateSolution(info);
            var logic = new SolutionInitializer(info.SolutionPath, templateDirectory, strongNamePath);
            logic.Projects = logic.GetProjectInfos(info);
            logic.CreateProjects(info.RootNamespace);
            UpdateSolution(info, logic);
            logic.ExecuteNuGetRestoreForSolution();
            UpdateEarlyBoundConfigOutputPaths(info);
        }

        private static void UpdateSolution(InitializeSolutionInfo info, SolutionInitializer logic)
        {
            Logger.Show("Updating Solution");
            IEnumerable<string> solution = File.ReadAllLines(logic.SolutionPath);
            solution = SolutionFileEditor.AddMissingProjects(solution, logic.Projects.Values);
            if (info.IncludeCodeGenerationFiles)
            {
                var parser = new SolutionFileParser(solution);
                var codeGenPath = Path.Combine(logic.TemplateDirectory, "CodeGeneration");
                var outputPath = Path.Combine(Path.GetDirectoryName(info.SolutionPath) ?? "", "CodeGeneration");
                Directory.CreateDirectory(outputPath);
                var files = new List<string>();
                foreach (var file in Directory.GetFiles(codeGenPath))
                {
                    Logger.AddDetail($"Adding {file} to the solution...");
                    var name = "CodeGeneration\\" + Path.GetFileName(file);
                    var outputFile = Path.Combine(outputPath, Path.GetFileName(file));
                    files.Add($"\t\t{name} = {name}");
                    if (File.Exists(outputFile))
                    {
                        Logger.AddDetail($"File '{outputFile}' already existing.  Skipping Adding");
                        continue;
                    }
                    File.Copy(file, outputFile);
                }
                parser.Projects.Add($@"Project(""{{2150E333-8FDC-42A3-9474-1A3956D46DE8}}"") = ""Code Generation"", ""Code Generation"", ""{{{Guid.NewGuid()}}}""
	ProjectSection(SolutionItems) = preProject
{string.Join(Environment.NewLine, files)}
	EndProjectSection
EndProject");
                File.WriteAllLines(logic.SolutionPath, parser.GetSolution());
            }
            else
            {
                File.WriteAllLines(logic.SolutionPath, solution);
            }
        }

        private static void CreateSolution(InitializeSolutionInfo info)
        {
            if (info.CreateSolution)
            {
                if (!info.SolutionPath.EndsWith(".sln"))
                {
                    info.SolutionPath += ".sln";
                }

                Directory.CreateDirectory(Path.GetDirectoryName(info.SolutionPath)??"");
                File.WriteAllText(info.SolutionPath, $@"

Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio 15
VisualStudioVersion = 15.0.28307.329
MinimumVisualStudioVersion = 10.0.40219.1
Global
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
	GlobalSection(ExtensibilityGlobals) = postSolution
		SolutionGuid = {{{Guid.NewGuid().ToString().ToUpper()}}}
	EndGlobalSection
EndGlobal
");
            }
        }

        private static void UpdateEarlyBoundConfigOutputPaths(InitializeSolutionInfo info)
        {
            if (!info.ConfigureEarlyBound)
            {
                return;
            }
            var settings = EarlyBoundGenerator.Settings.EarlyBoundGeneratorConfig.GetDefault();
            var settingsDirectory = Path.Combine(Path.GetDirectoryName(info.SolutionPath)?? "", info.SharedCommonProject, "Entities");
            // ReSharper disable StringLiteralTypo
            settings.ExtensionConfig.ActionsToSkip = "AcceptProposedBooking|AcceptTeamRecommendation|AddInvoiceLineDetails|applyworktemplate|applyworktemplateforresources|ApprovalStatusApprove|ApprovalStatusReject|AssignGenericResource|AutoGenerateProjectTeam|batchentityCUD|BookingResource|BookingResourceRequirement|BulkCreatePredecessorsForTask|BulkDeletePredecessorsForTask|CancelBookings|CloseAllOpportunityQuotes|CloseQuoteAsLost|CloseQuoteAsWon|CompleteResourceRequest|CopyProject|CopyRelatedProjectEntitiesFromTemplate|CopyWbsToProject|CorrectInvoice|CreateContractLineDetailsFromEstimate|CreateContractSpecificPriceList|createinvoicefrominvoiceschedule|CreateQuoteFromOpportunity|CreateQuoteLineDetailsFromEstimate|CreateQuoteSpecificPriceList|createrequestfromrequirement|CreateSharepointDocumentLocation|CreateTaskBasedEstimatesForProject|CreateTemplateFromProject|CreateEstimateLines|CreateEstimatesForProjectTask|CreateExtensionRequirement|DeleteEstimateLines|DeleteEstimatesForProjectTask|EnableSharePoint|EnableLinkedInDataValidation|ExpenseApproveAction|ExpenseEntriesApprove|ExpenseEntriesPendingApproval|ExpenseEntriesRecall|ExpenseEntriesReject|ExpenseEntriesSubmit|ExpenseRejectAction|ExpenseSubmitAction|ExportActual|FetchProjectCalendarWorkHours|FieldServiceSystemAction|FulfillResourceDemand|GDPROptoutContact|GDPROptoutLead|GDPROptoutUser|GenerateContractLineInvoiceSchedule|GenerateContractLineScheduleOfValues|GenerateQuoteLineScheduleOfValues|GenerateQuoteLineInvoiceSchedule|GetACIMarsConnectorStatus|GetDocumentManagementSettings|GetDocumentStorePath|GetOfficeGroupForEntity|GetProjectMapForContractLine|GetProjectMapForQuoteLine|GetProjectTaskCategories|GetResourceAvailability|GetResourceDemandTimeLine|GetRIProvisioningStatus|GetRITenantEndpoint|GetBookingDetailsByResource|GetCollectionData|GetContractLineChargeability|GetDataForContractPerformance|GetDataForRadialGauge|GetGenericResourceDetails|GetLegalAcceptanceStatus|GetMyChangedSkills|GetNotesAnalysis|GetPrice|GetProcessNotes|GetProductLine|GetProductLines|GetProjectCoparticipation|GetProjectCurrencies|GetProjectDetails|GetProjects|GetProjectsForContract|GetProjectsForQuote|GetQuoteLineChargeability|GetRecordUsers|GetResourceAvailabilitySummary|GetResourceBookingByProject|GetResourceBookingDetails|GetResourceBookingFormParameters|GetResourcePopupDetails|GetResources|GetSIPackageStatus|GetSummaryBookings|GetTalkingPoints|GetTimelineData|GetTransactionUnitPrices|GetUserTimeZoneName|IndentWBSTask|InvoicePaid|InvoiceRecalculate|InvokeServiceStoredProc|IsLinkedInDataValidationEnabled|IsProjectTemplatesView|JoinProjectTeam|LoadFactTableEstimate|LogFindWorkEvent|MarketingListMetadataUpdate|MarketingMetadataUpdate|MarkIntegrationJobAsFailedAsync|MoveProject|MoveDownWBSTask|MoveUpWBSTask|MSProject_ExportToProject|MSProject_LinkToProject|MSProject_PublishToExistingProject|MSProject_ReadFromExistingProject|MSProject_ReadProjectTeamMembers|MSProject_UnlinkFromProject|NewInvoiceContract|OutdentWBSTask|PerformNotesAnalysisAction|PostInvoice|PostJournal|ProjectTeamMemberSignupprocessaccept|ProjectTeamMemberSignUpProcess|ProjectTeamUpdateMembershipStatus|ProvisionSharePointDocumentLibraries|QueryExchange|ReadEstimateLines|RecommendWork|RefreshBusinessProcessStage|RejectProposedBooking|RemoveMarketingListMembersByIds|ResAssignResourcesForTask|ResGetResourceDetail|ResourceAssignmentDetailUpdate|FpsAction|GeocodeAddress|RetrieveDistanceMatrix|RetrieveResourceAvailability|ResourceReservationCancel|ResourceSubstitution|ResourceUtilization|ResourceUtilizationChart|RetrieveKPIvaluesfromDCI|RetrieveTypeValuesFromDCI|SaveProjectLineTasks|SetFeatureStatus|SetSharePointDocumentStatus|SetTeamsDocumentStatus|SetDefaultRole|SetLegalAcceptanceStatus|SetTalkingPointLikedStatus|StartRIProvisioning|SubmitCategoriesAndPriceLists|SubmitRolesAndPriceLists|TimeEntriesApprove|TimeEntriesCopyPaste|TimeEntriesPaste|TimeEntriesPendingApproval|TimeEntriesRecall|TimeEntriesReject|TimeEntriesSubmit|TrackExchangeActivity|Updatefeatureconfig|updateprojecttask|updateremainingresourcerequirement|UpdateRITenantInfo|UpdateAllEstimatesForProject|UpdateChangedSkills|UpdateEstimateLineDetails|UpdateEstimateLines|UpgradeTelemetry|ValidateFixedPriceLineTotals|IsSharePointEnabled";
            // ReSharper restore StringLiteralTypo
            settings.ExtensionConfig.CreateOneFilePerAction = true;
            settings.ExtensionConfig.GenerateActionAttributeNameConsts = true;
            settings.ActionOutPath = settings.ExtensionConfig.CreateOneFilePerAction ? @"Actions" : @"Actions.cs";
            settings.ExtensionConfig.CreateOneFilePerEntity = true;
            settings.ExtensionConfig.GenerateEnumProperties = true;
            settings.ExtensionConfig.GenerateAttributeNameConsts = true;
            settings.ExtensionConfig.EntitiesWhitelist = "account|businessunit|competitor|contact|lead|product|site|systemuser";
            settings.EntityOutPath = settings.ExtensionConfig.CreateOneFilePerEntity ? @"Entities" : @"Entities.cs";
            settings.ExtensionConfig.CreateOneFilePerOptionSet = true;
            settings.OptionSetOutPath = settings.ExtensionConfig.CreateOneFilePerOptionSet ? @"OptionSets" : @"OptionSets.cs";
            settings.Namespace = $"{info.SharedCommonProject}.Entities";
            settings.ServiceContextName = "CrmContext";
            settings.Version = "1.2019.3.12"; // Set to minimum required version of the EBG since the Default version will be the version of the VSSolutionAccelerator, not the EBG
            var settingsPath = Path.Combine(settingsDirectory, "EBG." + info.RootNamespace + ".Settings.xml");
            Directory.CreateDirectory(Path.GetDirectoryName(settingsPath)??"");
            settings.Save(Path.Combine(settingsDirectory, settingsPath));
            RunCopyToClipboard(settingsPath);
            Logger.AddDetail(@"Now you should generate your Early Bound Entities for your Org!");
            Logger.AddDetail($@"Open the Early Bound Generator XrmToolBox plugin, connect to your org, and then set the Settings Path to ""{settingsPath}"" (which has been already been copied to your clipboard for your convenience) and generate your entities." + Environment.NewLine);
            Logger.AddDetail($@"These settings should be checked into TFS and should be the settings used by all individuals on your project plugin for generating entities!");
            MessageBox.Show(@"Please refer to the instructions in the text box for generating your early bound entities.", @"Generate Early Bound Entities!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Work around for this error: Current thread must be set to single thread apartment (STA) mode before OLE calls can be made. Ensure that your Main function has STAThreadAttribute marked on it.
        /// </summary>
        /// <param name="text"></param>
        private static void RunCopyToClipboard(string text)
        {
            var thread = new Thread(param => { CopyToClipboard((string)param); });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start(text);
        }

        private static void CopyToClipboard(string text)
        {
            Clipboard.SetText(text);
        }

        /// <summary>
        /// Used for Unit Testing
        /// </summary>
        /// <param name="projectKey"></param>
        /// <param name="info"></param>
        public void CreateProject(string projectKey, InitializeSolutionInfo info)
        {
            Projects[projectKey].CopyFromAndUpdate(TemplateDirectory, info.RootNamespace);
        }
    }
}
