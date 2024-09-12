using DLaB.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace DLaB.VSSolutionAccelerator.Logic
{
    public class SolutionInitializer : SolutionEditor
    {
        public SolutionInitializer(string solutionPath, string templateDirectory, string strongNamePath = null, NuGetSettings nuGetSettings = null)
            : base(solutionPath, templateDirectory, strongNamePath, nuGetSettings)
        {
        }

        public Dictionary<string, ProjectInfo> GetProjectInfos(InitializeSolutionInfo info)
        {
            var projects = new Dictionary<string, ProjectInfo>();
            info.CreateCommonWorkflowProject = info.CreateWorkflow;

            AddSharedCommonProject(projects, info);
            AddBaseTestProject(projects, info);
            AddPlugin(projects, info);
            AddPluginTest(projects, info);
            AddWorkflow(projects, info);
            AddSharedWorkflowProject(projects, info, info.RootNamespace + ".Workflow");
            AddWorkflowTest(projects, info);
            //AddNugetPostUpdateCommandsToProjects(info.XrmPackage.Version, projects);
            return projects;
        }

        private void AddSharedCommonProject(Dictionary<string, ProjectInfo> projects, InitializeSolutionInfo info)
        {
            var project = CreateDefaultProjectInfo(
                ProjectInfo.Keys.Common,
                info.SharedCommonProject,
                info);

            project.HasDevDeployBuild = true;
            projects.Add(project.Key, project);
        }

        private void AddBaseTestProject(Dictionary<string, ProjectInfo> projects, InitializeSolutionInfo info)
        {
            if (!info.ConfigureXrmUnitTest)
            {
                return;
            }
            
            var project = CreateDefaultProjectInfo(ProjectInfo.Keys.Test, info.TestBaseProject, info);
            projects.Add(project.Key, project);
        }

        public static void Execute(InitializeSolutionInfo info, string templateDirectory, string strongNamePath = null, NuGetSettings nuGetSettings = null)
        {
            Logger.AddDetail($"Starting to process solution '{info.SolutionPath}' using templates from '{templateDirectory}'");
            CreateSolution(info);
            var logic = new SolutionInitializer(info.SolutionPath, templateDirectory, strongNamePath, nuGetSettings);
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
# Visual Studio Version 17
VisualStudioVersion = 17.10.35201.131
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
            var settings = EarlyBoundGeneratorV2.Settings.EarlyBoundGeneratorConfig.GetDefault();
            settings.EntityTypesFolder = settings.ExtensionConfig.CreateOneFilePerEntity ? @"Entities" : @"Entities.cs";
            // ReSharper disable StringLiteralTypo
            settings.ExtensionConfig.ActionsToSkip = "AcceptProposedBooking|AcceptTeamRecommendation|AddInvoiceLineDetails|applyworktemplate|applyworktemplateforresources|ApprovalStatusApprove|ApprovalStatusReject|AssignGenericResource|AutoGenerateProjectTeam|batchentityCUD|BookingResource|BookingResourceRequirement|BulkCreatePredecessorsForTask|BulkDeletePredecessorsForTask|CancelBookings|CloseAllOpportunityQuotes|CloseQuoteAsLost|CloseQuoteAsWon|CompleteResourceRequest|CopyProject|CopyRelatedProjectEntitiesFromTemplate|CopyWbsToProject|CorrectInvoice|CreateContractLineDetailsFromEstimate|CreateContractSpecificPriceList|createinvoicefrominvoiceschedule|CreateQuoteFromOpportunity|CreateQuoteLineDetailsFromEstimate|CreateQuoteSpecificPriceList|createrequestfromrequirement|CreateSharepointDocumentLocation|CreateTaskBasedEstimatesForProject|CreateTemplateFromProject|CreateEstimateLines|CreateEstimatesForProjectTask|CreateExtensionRequirement|DeleteEstimateLines|DeleteEstimatesForProjectTask|EnableSharePoint|EnableLinkedInDataValidation|ExpenseApproveAction|ExpenseEntriesApprove|ExpenseEntriesPendingApproval|ExpenseEntriesRecall|ExpenseEntriesReject|ExpenseEntriesSubmit|ExpenseRejectAction|ExpenseSubmitAction|ExportActual|FetchProjectCalendarWorkHours|FieldServiceSystemAction|FulfillResourceDemand|GDPROptoutContact|GDPROptoutLead|GDPROptoutUser|GenerateContractLineInvoiceSchedule|GenerateContractLineScheduleOfValues|GenerateQuoteLineScheduleOfValues|GenerateQuoteLineInvoiceSchedule|GetACIMarsConnectorStatus|GetDocumentManagementSettings|GetDocumentStorePath|GetOfficeGroupForEntity|GetProjectMapForContractLine|GetProjectMapForQuoteLine|GetProjectTaskCategories|GetResourceAvailability|GetResourceDemandTimeLine|GetRIProvisioningStatus|GetRITenantEndpoint|GetBookingDetailsByResource|GetCollectionData|GetContractLineChargeability|GetDataForContractPerformance|GetDataForRadialGauge|GetGenericResourceDetails|GetLegalAcceptanceStatus|GetMyChangedSkills|GetNotesAnalysis|GetPrice|GetProcessNotes|GetProductLine|GetProductLines|GetProjectCoparticipation|GetProjectCurrencies|GetProjectDetails|GetProjects|GetProjectsForContract|GetProjectsForQuote|GetQuoteLineChargeability|GetRecordUsers|GetResourceAvailabilitySummary|GetResourceBookingByProject|GetResourceBookingDetails|GetResourceBookingFormParameters|GetResourcePopupDetails|GetResources|GetSIPackageStatus|GetSummaryBookings|GetTalkingPoints|GetTimelineData|GetTransactionUnitPrices|GetUserTimeZoneName|IndentWBSTask|InvoicePaid|InvoiceRecalculate|InvokeServiceStoredProc|IsLinkedInDataValidationEnabled|IsProjectTemplatesView|JoinProjectTeam|LoadFactTableEstimate|LogFindWorkEvent|MarketingListMetadataUpdate|MarketingMetadataUpdate|MarkIntegrationJobAsFailedAsync|MoveProject|MoveDownWBSTask|MoveUpWBSTask|MSProject_ExportToProject|MSProject_LinkToProject|MSProject_PublishToExistingProject|MSProject_ReadFromExistingProject|MSProject_ReadProjectTeamMembers|MSProject_UnlinkFromProject|NewInvoiceContract|OutdentWBSTask|PerformNotesAnalysisAction|PostInvoice|PostJournal|ProjectTeamMemberSignupprocessaccept|ProjectTeamMemberSignUpProcess|ProjectTeamUpdateMembershipStatus|ProvisionSharePointDocumentLibraries|QueryExchange|ReadEstimateLines|RecommendWork|RefreshBusinessProcessStage|RejectProposedBooking|RemoveMarketingListMembersByIds|ResAssignResourcesForTask|ResGetResourceDetail|ResourceAssignmentDetailUpdate|FpsAction|GeocodeAddress|RetrieveDistanceMatrix|RetrieveResourceAvailability|ResourceReservationCancel|ResourceSubstitution|ResourceUtilization|ResourceUtilizationChart|RetrieveKPIvaluesfromDCI|RetrieveTypeValuesFromDCI|SaveProjectLineTasks|SetFeatureStatus|SetSharePointDocumentStatus|SetTeamsDocumentStatus|SetDefaultRole|SetLegalAcceptanceStatus|SetTalkingPointLikedStatus|StartRIProvisioning|SubmitCategoriesAndPriceLists|SubmitRolesAndPriceLists|TimeEntriesApprove|TimeEntriesCopyPaste|TimeEntriesPaste|TimeEntriesPendingApproval|TimeEntriesRecall|TimeEntriesReject|TimeEntriesSubmit|TrackExchangeActivity|Updatefeatureconfig|updateprojecttask|updateremainingresourcerequirement|UpdateRITenantInfo|UpdateAllEstimatesForProject|UpdateChangedSkills|UpdateEstimateLineDetails|UpdateEstimateLines|UpgradeTelemetry|ValidateFixedPriceLineTotals|IsSharePointEnabled";
            // ReSharper restore StringLiteralTypo
            settings.ExtensionConfig.AddNewFilesToProject = false;
            settings.ExtensionConfig.CreateOneFilePerAction = true;
            settings.ExtensionConfig.CreateOneFilePerEntity = true;
            settings.ExtensionConfig.DeleteFilesFromOutputFolders = false;
            settings.ExtensionConfig.GenerateActionAttributeNameConsts = true;
            settings.ExtensionConfig.GenerateEnumProperties = true;
            settings.ExtensionConfig.GenerateAttributeNameConsts = true;
            settings.ExtensionConfig.EntitiesWhitelist = "account|businessunit|competitor|contact|lead|product|site|systemuser";
            settings.ExtensionConfig.ReplaceOptionSetPropertiesWithEnum = true;
            settings.ExtensionConfig.CreateOneFilePerOptionSet = true;
            settings.MessageTypesFolder = settings.ExtensionConfig.CreateOneFilePerAction ? @"Messages" : @"Messages.cs";
            settings.Namespace = $"{info.RootNamespace}.Entities";
            settings.OptionSetsTypesFolder = settings.ExtensionConfig.CreateOneFilePerOptionSet ? @"OptionSets" : @"OptionSets.cs";
            settings.ServiceContextName = "DataverseContext";
            settings.Version = "2.2024.9.8"; // Set to minimum required version of the EBG since the Default version will be the version of the VSSolutionAccelerator, not the EBG

            settings.Save(info.GetEarlyBoundSettingsPath());
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
