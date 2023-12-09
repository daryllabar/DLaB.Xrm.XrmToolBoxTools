using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Source.DLaB.Common.Exceptions;
using Source.DLaB.Xrm;
using DLaB.Xrm.Entities;
using McTools.Xrm.Connection;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Label = Microsoft.Xrm.Sdk.Label;

namespace DLaB.AttributeManager
{
    public partial class Logic
    {
        public delegate void LogHandler(string text);
        public event LogHandler OnLog;

        public EntityMetadata Metadata { get; set; }
        public bool MigrateData { get; set; }
        public bool IgnoreUpdateErrors { get; set; }
        public bool SupportsExecuteMultipleRequest { get; set; }
        public string TempPostfix { get; private set; }
        public IOrganizationService Service { get; private set; }
        public HashSet<int> ValidLanguageCodes { get; private set; }
        private const int Crm2013 = 6;
        private const int Crm2011 = 5;
        private const int Rollup12 = 3218;


        [Flags]
        public enum Steps
        {
            CreateTemp = 1,
            MigrateDataToTemp = 2,
            MigrateToTemp = 4,
            RemoveExistingAttribute = 8,
            CreateNewAttribute = 16,
            MigrateDataToNewAttribute = 32,
            MigrateToNewAttribute = 64,
            RemoveTemp = 128,
            MigrationToTempRequired = 256
        }

        [Flags]
        public enum Action
        {
            Rename = 1,
            ChangeCase = 2,
            RemoveTemp = 4,
            ChangeType = 8,
            Delete = 16
        }

        public Logic(IOrganizationService service, ConnectionDetail connectionDetail, EntityMetadata metadata, string tempPostFix, bool migrateData)
        {
            SupportsExecuteMultipleRequest = connectionDetail.OrganizationMajorVersion >= Crm2013 ||
                                             (connectionDetail.OrganizationMajorVersion >= Crm2011 && int.Parse(connectionDetail.OrganizationVersion.Split('.')[3]) >= Rollup12);
            Service = new RetryOrgService(service, Trace);
            TempPostfix = tempPostFix;
            MigrateData = migrateData;
            ValidLanguageCodes = GetValidLanguageCodes();
            Metadata = metadata;
        }

        public Logic(IOrganizationService service, ConnectionDetail connectionDetail, EntityMetadata metadata, string tempPostFix, bool migrateData, bool ignoreUpdateErrors):
            this(service, connectionDetail, metadata, tempPostFix, migrateData)
        {
            IgnoreUpdateErrors = ignoreUpdateErrors;
        }

        private HashSet<int> GetValidLanguageCodes()
        {
            var resp = (RetrieveAvailableLanguagesResponse)Service.Execute(new RetrieveAvailableLanguagesRequest());
            return new HashSet<int>(resp.LocaleIds);
        }

        //public void Run(AttributeMetadata att, string newAttributeSchemaName, Steps stepsToPerform, Action actions, AttributeMetadata newAttributeType = null, Dictionary<string,string> migrationMapping = null)
        public void Run(AttributeManagerPlugin.ExecuteStepsInfo info)
        {
            var att = info.CurrentAttribute;
            var newAttributeSchemaName = info.NewAttributeName;
            var stepsToPerform = info.Steps;
            var actions = info.Action;
            var newAttributeType = info.NewAttribute;
            var migrationMapping = GetMigrationMapping(info.MappingFilePath) ?? new Dictionary<string, string>();
            var state = GetApplicationMigrationState(Service, att, newAttributeSchemaName);
            AssertValidStepsForState(att.SchemaName, newAttributeSchemaName, stepsToPerform, state, actions);
            var oldAtt = state.Old;
            var tmpAtt = state.Temp;
            var newAtt = state.New;

            switch (actions)
            {
                case Action.Delete:
                    ClearFieldDependencies(oldAtt, info.AutoRemovePluginRegistrationAssociations);
                    RemoveExisting(stepsToPerform, oldAtt);
                    break;

                case Action.RemoveTemp:
                    RemoveTemp(stepsToPerform, tmpAtt);
                    break;

                case Action.Rename:
                case Action.Rename | Action.ChangeType:
                    CreateNew(newAttributeSchemaName, stepsToPerform, oldAtt, ref newAtt, newAttributeType); // Create or Retrieve the New Attribute
                    MigrateDataToNew(stepsToPerform, oldAtt, newAtt, actions, migrationMapping);
                    MigrateToNew(stepsToPerform, oldAtt, newAtt);
                    RemoveExisting(stepsToPerform, oldAtt);
                    break;

                case Action.ChangeCase:
                case Action.ChangeCase | Action.ChangeType:
                case Action.ChangeType:
                    CreateTemp(stepsToPerform, oldAtt, ref tmpAtt, newAttributeType); // Either Create or Retrieve the Temp
                    MigrateDataToTemp(stepsToPerform, oldAtt, tmpAtt, actions, migrationMapping);
                    MigrateToTemp(stepsToPerform, oldAtt, tmpAtt);
                    RemoveExisting(stepsToPerform, oldAtt);
                    CreateNew(newAttributeSchemaName, stepsToPerform, tmpAtt, ref newAtt, newAttributeType);
                    MigrateDataToNew(stepsToPerform, tmpAtt, newAtt, actions, new Dictionary<string, string>()); // Don't reapply mapping on Migrate Date to New when Create Temp has already had it applied
                    MigrateToNew(stepsToPerform, tmpAtt, newAtt);
                    RemoveTemp(stepsToPerform, tmpAtt);
                    break;
            }
        }

        private Dictionary<string, string> GetMigrationMapping(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }
            if (!System.IO.File.Exists(path))
            {
                MessageBox.Show(@"Mapping file: ""{path}"" was not found!  No Mapping will be performed!",
                    @"No Mapping File",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return null;
            }

            var mapping = new Dictionary<string, string>();
            using (var parser = new TextFieldParser(path))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                var line = 0;
                while (!parser.EndOfData)
                {
                    line++;
                    //Processing row
                    var fields = parser.ReadFields();
                    if (fields == null || fields.Length == 0)
                    {
                        continue;
                    }
                    if (fields.Length != 2)
                    {
                        throw new Exception($@"Error parsing file: ""{path}"" on line {line}.  Expected 2 values per line, found {fields.Length}.");
                    }

                    mapping.Add(fields[0], fields[1]);
                }
            }
            return mapping;
        }

        private void ClearFieldDependencies(AttributeMetadata att, bool removePluginRegistrationAssociations)
        {
            Trace("Beginning Step: Clearing Field Dependencies");
            UpdateCharts(Service, att);
            UpdateViews(Service, att);
            UpdateForms(Service, att);
            if (removePluginRegistrationAssociations)
            {
                UpdatePluginSteps(Service, att);
                UpdatePluginImages(Service, att);
            }
            UpdateRelationships(Service, att);
            UpdateMappings(Service, att);
            UpdateWorkflows(Service, att);
            PublishAll(Service);
            AssertCanDelete(Service, att);
            Trace("Completed Step: Clearing Field Dependencies" + Environment.NewLine);
        }

        private void CreateTemp(Steps stepsToPerform, AttributeMetadata oldAtt, ref AttributeMetadata tmpAtt, AttributeMetadata newAttributeType)
        {
            if (stepsToPerform.HasFlag(Steps.CreateTemp))
            {
                Trace("Beginning Step: Create Temp");
                tmpAtt = tmpAtt ?? CreateAttributeWithDifferentName(Service, oldAtt, oldAtt.SchemaName + TempPostfix, newAttributeType);
                Trace("Completed Step: Create Temp" + Environment.NewLine);
            }
        }

        private void RemoveTemp(Steps stepsToPerform, AttributeMetadata tmpAtt)
        {
            if (stepsToPerform.HasFlag(Steps.RemoveTemp))
            {
                Trace("Beginning Step: Remove Temporary Field");
                DeleteField(Service, tmpAtt);
                Trace("Completed Step: Remove Temporary Field" + Environment.NewLine);
            }
        }

        private void MigrateDataToNew(Steps stepsToPerform, AttributeMetadata tmpAtt, AttributeMetadata newAtt, Action actions, Dictionary<string, string> migrationMapping)
        {
            if (stepsToPerform.HasFlag(Steps.MigrateDataToNewAttribute))
            {
                Trace("Beginning Step: Migrate Data To New Attribute");
                CopyData(Service, tmpAtt, newAtt, actions, migrationMapping);
                Trace("Completed Step: Migrate Data To New Attribute" + Environment.NewLine);
            }
        }

        private void MigrateToNew(Steps stepsToPerform, AttributeMetadata tmpAtt, AttributeMetadata newAtt)
        {
            if (stepsToPerform.HasFlag(Steps.MigrateToNewAttribute))
            {
                Trace("Beginning Step: Migrate To New Attribute");
                MigrateAttribute(tmpAtt, newAtt);
                Trace("Completed Step: Migrate To New Attribute" + Environment.NewLine);
            }
        }

        private void CreateNew(string newAttributeSchemaName, Steps stepsToPerform, AttributeMetadata attributeToCopy, ref AttributeMetadata createdAttributeOrAttributeIfAlreadyCreated, AttributeMetadata newAttributeType)
        {
            if (stepsToPerform.HasFlag(Steps.CreateNewAttribute))
            {
                Trace("Beginning Step: Create New Attribute");
                createdAttributeOrAttributeIfAlreadyCreated = createdAttributeOrAttributeIfAlreadyCreated ?? CreateAttributeWithDifferentName(Service, attributeToCopy, newAttributeSchemaName, newAttributeType);
                Trace("Completed Step: Create New Attribute" + Environment.NewLine);
            }
        }

        private void RemoveExisting(Steps stepsToPerform, AttributeMetadata oldAtt)
        {
            if (stepsToPerform.HasFlag(Steps.RemoveExistingAttribute))
            {
                Trace("Beginning Step: Remove Existing Attribute");
                DeleteField(Service, oldAtt);
                Trace("Completed Step: Remove Existing Attribute" + Environment.NewLine);
            }
        }

        private void MigrateDataToTemp(Steps stepsToPerform, AttributeMetadata oldAtt, AttributeMetadata tmpAtt, Action actions, Dictionary<string, string> migrationMapping)
        {
            if (stepsToPerform.HasFlag(Steps.MigrateDataToTemp))
            {
                Trace("Beginning Step: Migrate Data To Temp");
                CopyData(Service, oldAtt, tmpAtt, actions, migrationMapping);
                Trace("Completed Step: Migrate Data To Temp" + Environment.NewLine);
            }
        }

        private void MigrateToTemp(Steps stepsToPerform, AttributeMetadata oldAtt, AttributeMetadata tmpAtt)
        {
            if (stepsToPerform.HasFlag(Steps.MigrateToTemp))
            {
                Trace("Beginning Step: Migrate To Temp");
                MigrateAttribute(oldAtt, tmpAtt);
                Trace("Completed Step: Migrate To Temp" + Environment.NewLine);
            }
        }

        private void MigrateAttribute(AttributeMetadata fromAtt, AttributeMetadata toAtt)
        {
            // Replace Old Attribute with Tmp Attribute
            UpdateCalculatedFields(Service, fromAtt, toAtt);
            UpdateCharts(Service, fromAtt, toAtt);
            UpdateViews(Service, fromAtt, toAtt);
            UpdateForms(Service, fromAtt, toAtt);
            UpdateWorkflows(Service, fromAtt, toAtt);
            UpdatePluginStepFilters(Service, fromAtt, toAtt);
            UpdatePluginStepImages(Service, fromAtt, toAtt);
            PublishAll(Service);
            AssertCanDelete(Service, fromAtt);
        }

        private AttributeMigrationState GetApplicationMigrationState(IOrganizationService service, AttributeMetadata att, string newSchemaName)
        {
            var entityName = att.EntityLogicalName;
            var schemaName = att.SchemaName;
            var tempSchemaName = att.SchemaName + TempPostfix;
            var state = new AttributeMigrationState();

            var metadata = ((RetrieveEntityResponse) service.Execute(new RetrieveEntityRequest {LogicalName = att.EntityLogicalName, EntityFilters = EntityFilters.Attributes})).EntityMetadata;
            
            Trace("Searching for Existing Attribute " + entityName + "." + schemaName);
            state.Old = metadata.Attributes.FirstOrDefault(a => a.SchemaName == schemaName);
            Trace("Existing Attribute {0}.{1} {2}found", entityName, schemaName, state.Old == null ? "not " : string.Empty);

            Trace("Searching for Temp Attribute " + entityName + "." + tempSchemaName);
            state.Temp = metadata.Attributes.FirstOrDefault(a => a.SchemaName == tempSchemaName);
            Trace("Temp Attribute {0}.{1} {2}found", entityName, tempSchemaName, state.Temp == null ? "not " : string.Empty);

            Trace("Searching for New Attribute " + entityName + "." + newSchemaName);
            state.New = metadata.Attributes.FirstOrDefault(a => a.SchemaName == newSchemaName);
            Trace("New Attribute {0}.{1} {2}found", entityName, newSchemaName, state.New == null ? "not " : string.Empty);

            SetDisplayName(state.Old, att.DisplayName);
            SetDisplayName(state.Temp, att.DisplayName);
            SetDisplayName(state.New, att.DisplayName);

            return state;
        }

        private void SetDisplayName(AttributeMetadata att, Label displayName)
        {
            if (att != null)
            {
                att.DisplayName = displayName;
            }
        }

        private void AssertValidStepsForState(string existingSchemaName, string newSchemaName, Steps stepsToPerform, AttributeMigrationState state, Action actions)
        {
            // TODO CLEAN THIS UP!
            if (actions.HasFlag(Action.ChangeType) && existingSchemaName == newSchemaName && state.Old != null && state.New != null)
            {
                Trace("Only an attribute type change has been requested.  Attempting to differentiate between existing and new.");

                if (state.Temp == null)
                {
                    Trace("No Temporary Attribute was found.  Treating New as not yet created.");
                    state.New = null;
                }
                else if (state.Old.GetType() == state.Temp.GetType())
                {
                    if (stepsToPerform.HasFlag(Steps.RemoveExistingAttribute) 
                        || stepsToPerform.HasFlag(Steps.CreateNewAttribute)
                        || stepsToPerform.HasFlag(Steps.MigrateDataToTemp)
                        || stepsToPerform.HasFlag(Steps.MigrateToTemp))
                    {
                        Trace("A Temporary Attribute was found and a request has been made to either remove the existing attribute, create a new attribute, or migrate to temp.  Treating New as not yet created.");
                        state.New = null;
                    }
                    else
                    {
                        Trace("A Temporary Attribute was found and a request has not been made to either remove the existing attribute, create a new attribute, or migrate to temp.  Treating New as already created.");
                        state.Old = null;
                    }
                }
                else
                {
                    Trace("A Temporary Attribute was found and the current Attribute Type is different.  Treating New as not yet created.");
                    state.New = null;
                }
            }

            Trace("Validating Current CRM State Before Performing Steps:");

            if (stepsToPerform.HasFlag(Steps.CreateTemp) && state.Temp != null)
            {
                throw new InvalidOperationException("Unable to Create Temp!  Temp " + state.Temp.EntityLogicalName + "." + state.Temp.LogicalName + " already exists!");
            }

            if (stepsToPerform.HasFlag(Steps.MigrateDataToTemp) || stepsToPerform.HasFlag(Steps.MigrateToTemp))
            {
                // Can only Migrate if old already exists
                if (state.Old == null)
                {
                    throw new InvalidOperationException("Unable to Migrate!  Existing Attribute " + existingSchemaName + " does not exist!");
                }

                // Can only Migrate if Tmp already exists, or temp will be created
                if (!(state.Temp != null || stepsToPerform.HasFlag(Steps.CreateTemp)))
                {
                    throw new InvalidOperationException("Unable to Migrate!  Temporary Attribute " + existingSchemaName + TempPostfix + " does not exist!");
                }
            }


            if (stepsToPerform.HasFlag(Steps.RemoveExistingAttribute))
            {
                if (state.Old == null)
                {
                    AssertInvalidState("Unable to Remove Existing Attribute! Attribute " + existingSchemaName + " does not exist!");
                }

                // Can only Remove existing if Tmp already exists, or temp will be created, or action is to delete, or if performing rename and there is a Create Or the New Already exists
                if (!(
                        state.Temp != null 
                        || stepsToPerform.HasFlag(Steps.CreateTemp) 
                        || actions.HasFlag(Action.Delete)
                        || (!string.Equals(existingSchemaName, newSchemaName, StringComparison.OrdinalIgnoreCase) 
                            && (stepsToPerform.HasFlag(Steps.CreateNewAttribute) || state.New != null))))
                {
                    AssertInvalidState("Unable to Remove Existing Attribute!  Temporary Attribute " + existingSchemaName + TempPostfix + " does not exist!");
                }

                // Can only Remove existing if Tmp will be migrated, or has been migrated, or action is delete
                if (!(
                        (actions.HasFlag(Action.ChangeCase) && stepsToPerform.HasFlag(Steps.MigrateToTemp)) 
                        || (actions.HasFlag(Action.Rename) && stepsToPerform.HasFlag(Steps.MigrateToNewAttribute))
                        || actions.HasFlag(Action.Delete)))
                {
                    try
                    {
                        AssertCanDelete(Service, state.Old);
                    }
                    catch
                    {
                        AssertInvalidState("Unable to Remove Existing!  Existing Attribute " + existingSchemaName + " has not been migrated to Temporary Attribute!");
                    }
                }
            }

            if (stepsToPerform.HasFlag(Steps.CreateNewAttribute))
            {
                if (stepsToPerform.HasFlag(Steps.MigrationToTempRequired))
                {
                    // Temp is required, Can only Create New, if Old doesn't exist, or will be removed
                    if(!(state.Old == null || stepsToPerform.HasFlag(Steps.RemoveExistingAttribute)))
                    {
                        AssertInvalidState("Unable to create new Attribute!  Old Attribute " + existingSchemaName + " still exists!");
                    }
                }


                // Can only Create Global if doesn't already exist
                if (state.New != null)
                {
                    AssertInvalidState("Unable to create new Attribute!  New Attribute " + existingSchemaName + " already exists!");
                }
            }

            if (stepsToPerform.HasFlag(Steps.MigrateDataToNewAttribute) || stepsToPerform.HasFlag(Steps.MigrateToNewAttribute))
            {
                // Can only Migrate To New if Temp Exists, or Creating a Temp, or There is a Rename and the Old Already Exists
                if (!(state.Temp != null || stepsToPerform.HasFlag(Steps.CreateTemp) || (actions.HasFlag(Action.Rename) && state.Old != null)))
                {
                    AssertInvalidState("Unable to Migrate!  Temp Attribute " + existingSchemaName + TempPostfix + " does not exist!");
                }

                // Can only Migrate if New Already exists, or New will be created
                if (!(state.New != null || stepsToPerform.HasFlag(Steps.CreateNewAttribute)))
                {
                    AssertInvalidState("Unable to Migrate!  New Attribute " + existingSchemaName + " does not exist!");
                }
            }

            if (stepsToPerform.HasFlag(Steps.RemoveTemp))
            {
                // Can Only remove Temp if it exists, or will exist
                if (!(state.Temp != null || stepsToPerform.HasFlag(Steps.CreateTemp)))
                {
                    AssertInvalidState("Unable to Remove Temp!  Temp Attribute " + existingSchemaName + TempPostfix + " does not exist!");
                }

                // Can Only remove Temp if new Attribute Already exists, or if only step is RemoveTemp 
                if (!(state.New != null || stepsToPerform.HasFlag(Steps.CreateNewAttribute) || stepsToPerform == Steps.RemoveTemp))
                {
                    AssertInvalidState("Unable to Migrate!  New Attribute " + existingSchemaName + " does not exist!");
                }

                // Can only Remove tmp if global will be migrated, or has been migrated
                if (!stepsToPerform.HasFlag(Steps.MigrateToNewAttribute))
                {
                    try
                    {
                        AssertCanDelete(Service, state.Temp);
                    }
                    catch
                    {
                        AssertInvalidState("Unable to Remove Old Attribute!  Old Attribute " + existingSchemaName + " has not been migrated to Temporary Attribute!");
                    }
                }
            }
        }

        private void AssertInvalidState(string message)
        {
            throw new InvalidOperationException(message);
        }

        private void DeleteField(IOrganizationService service, AttributeMetadata att)
        {
            Trace("Deleting Field " + att.EntityLogicalName + "." + att.LogicalName);
            service.Execute(new DeleteAttributeRequest
            {
                EntityLogicalName = att.EntityLogicalName,
                LogicalName = att.LogicalName
            });
        }

        private void AssertCanDelete(IOrganizationService service, AttributeMetadata attribute)
        {
            Trace("Checking for Delete Dependencies for " + attribute.EntityLogicalName + "." + attribute.LogicalName);
            var depends = (RetrieveDependenciesForDeleteResponse)service.Execute(new RetrieveDependenciesForDeleteRequest
            {
                ComponentType = (int)ComponentType.Attribute,
                ObjectId = attribute.MetadataId.GetValueOrDefault()
            });

            var errors = new List<string>();
            foreach (var d in depends.EntityCollection.ToEntityList<Dependency>())
            {
                var type = (ComponentType)d.DependentComponentType.GetValueOrDefault();
                var dependentId = d.DependentComponentObjectId.GetValueOrDefault();
                var err = type + " " + dependentId;
                switch (type) {
                    case ComponentType.Attribute:
                        var req = new RetrieveAttributeRequest
                        {
                            MetadataId = dependentId
                        };
                        var dependent = ((RetrieveAttributeResponse)service.Execute(req)).AttributeMetadata;
                       
                        err = $"{err} ({dependent.EntityLogicalName + " : " + dependent.DisplayName.GetLocalOrDefaultText()}";
                        break;

                    case ComponentType.EntityRelationship:
                        var response =
                            (RetrieveRelationshipResponse)service.Execute(new RetrieveRelationshipRequest { MetadataId = dependentId });
                        Trace("Entity Relationship / Mapping {0} must be manually removed/added", response.RelationshipMetadata.SchemaName);
                        break;

                    case ComponentType.SDKMessageProcessingStep:
                        var step = service.GetEntity<SdkMessageProcessingStep>(dependentId);
                        err = $"{type} {step.Name}({dependentId}) - {step.Description}";
                        break;

                    case ComponentType.SDKMessageProcessingStepImage:
                        var image = service.GetEntity<SdkMessageProcessingStepImage>(dependentId);
                        var imageStep = service.GetEntity<SdkMessageProcessingStep>(image.SdkMessageProcessingStepId.Id);
                        err = $"{type} {imageStep.Name}.{image.Name}({dependentId}) - {image.ImageTypeEnum}";
                        break;

                    case ComponentType.SavedQueryVisualization:
                        var sqv = service.GetEntity<SavedQueryVisualization>(dependentId);
                            err = $"{err} ({sqv.Name} - {sqv.CreatedBy.Name})";
                        break;

                    case ComponentType.SavedQuery:
                        var sq = service.GetEntity<SavedQuery>(dependentId);
                            err = $"{err} ({sq.Name} - {sq.CreatedBy.Name})";
                        break;

                    case ComponentType.Workflow:
                        var workflow = service.GetEntity<Workflow>(d.DependentComponentObjectId.GetValueOrDefault());
                        err = err + " " + workflow.Name + " (" + workflow.CategoryEnum + ")";
                            break;
                }

                errors.Add(err);
            }

            if (errors.Count > 0)
            {
                throw new Exception("Dependencies found: " + Environment.NewLine + "\t" + string.Join(Environment.NewLine + "\t", errors));
            }
        }

        private void UpdateCalculatedFields(IOrganizationService service, AttributeMetadata from, AttributeMetadata to)
        {
            Trace("Checking for Calculated Field Dependencies");
            var depends = ((RetrieveDependenciesForDeleteResponse)service.Execute(new RetrieveDependenciesForDeleteRequest
            {
                ComponentType = (int)ComponentType.Attribute,
                ObjectId = from.MetadataId.GetValueOrDefault()
            })).EntityCollection.ToEntityList<Dependency>().Where(d => d.DependentComponentTypeEnum == ComponentType.Attribute).ToList();

            if (!depends.Any())
            {
                Trace("No Calculated Dependencies Found");
                return;
            }

            foreach (var dependency in depends)
            {
                Trace($"Retrieving Dependent Attribute {dependency.DependentComponentObjectId}");
                var att = ((RetrieveAttributeResponse) Service.Execute(new RetrieveAttributeRequest
                {
                    MetadataId = dependency.DependentComponentObjectId.GetValueOrDefault()
                })).AttributeMetadata;
                Trace($"Updating Dependent Attribute: {att.DisplayName.GetLocalOrDefaultText()}");

                var response = UpdateFormulaDefintionLogic.Update(att, from, to);
                if (!response.HasFormula)
                {
                    Trace("Dependency does not have a Formula Definition.  Unable to Remove Dependency.");
                }

                Trace($"Updating FormulatDefinition from {response.CurrentForumla} to {response.NewFormula}.");
                Service.Execute(new UpdateAttributeRequest
                {
                    Attribute = att,
                    EntityName = att.EntityLogicalName
                });
                Trace($"Successfully Removed Dependency for Dependent Attribute: {att.DisplayName.GetLocalOrDefaultText()}");
            }

           
        }

        private void UpdateCharts(IOrganizationService service, AttributeMetadata from, AttributeMetadata to)
        {
            var found = false;
            Trace("Checking for Chart Dependencies");
            foreach (var chart in GetSystemChartsWithAttribute(service, from))
            {
                Trace("Updating Chart " + chart.Name);
                chart.DataDescription = ReplaceFetchXmlAttribute(chart.DataDescription, from.LogicalName, to.LogicalName);
                service.Update(chart);
                found = true;
            }

            foreach (var chart in GetUserChartsWithAttribute(service, from))
            {
                Trace("Updating Chart " + chart.Name);
                chart.DataDescription = ReplaceFetchXmlAttribute(chart.DataDescription, from.LogicalName, to.LogicalName);
                service.Update(chart);
                found = true;
            }
            Trace(found
                ? "Finished updating Chart Dependencies"
                : "No Chart Dependencies Found");
        }

        private List<SavedQueryVisualization> GetSystemChartsWithAttribute(IOrganizationService service, AttributeMetadata from)
        {
            var qe = QueryExpressionFactory.Create<SavedQueryVisualization>(q => new
            {
                q.Id,
                q.DataDescription
            });

            AddFetchXmlCriteria(qe, SavedQueryVisualization.Fields.DataDescription, from.EntityLogicalName, from.LogicalName);

            Trace("Retrieving System Charts with Query: " + qe.GetSqlStatement());
            return service.GetEntities(qe);
        }

        private List<UserQueryVisualization> GetUserChartsWithAttribute(IOrganizationService service, AttributeMetadata from)
        {
            var qe = QueryExpressionFactory.Create<UserQueryVisualization>(q => new
            {
                q.Id,
                q.DataDescription
            });

            AddFetchXmlCriteria(qe, UserQueryVisualization.Fields.DataDescription, from.EntityLogicalName, from.LogicalName);

            Trace("Retrieving System Charts with Query: " + qe.GetSqlStatement());
            return service.GetEntities(qe);
        }

        private List<SystemForm> GetFormsWithAttribute(IOrganizationService service, AttributeMetadata att)
        {
            var qe = QueryExpressionFactory.Create<SystemForm>(q => new
            {
                q.Id,
                q.Name,
                q.FormXml
            },
                SystemForm.Fields.ObjectTypeCode, att.EntityLogicalName,
                new ConditionExpression("formxml", ConditionOperator.Like, "%<control %datafieldname=\"" + att.LogicalName + "\"%"));

            Trace("Retrieving Forms with Query: " + qe.GetSqlStatement());
            return service.GetEntities(qe);
        }

        private void UpdateForms(IOrganizationService service, AttributeMetadata from, AttributeMetadata to)
        {
            /*
             * <row>
             *   <cell id="{056d159e-9144-d809-378b-9e04a7626953}" showlabel="true" locklevel="0">
             *     <labels>
             *       <label description="Points" languagecode="1033" />
             *     </labels>
             *     <control id="new_points" classid="{4273EDBD-AC1D-40d3-9FB2-095C621B552D}" datafieldname="new_points" disabled="true" />
             *   </cell>
             *   <cell id="{056d159e-9144-d809-378b-9e04a7626953}" showlabel="true" locklevel="0">
             *     <labels>
             *       <label description="Points" languagecode="1033" />
             *     </labels>
             *     <control id="header_new_points" classid="{4273EDBD-AC1D-40d3-9FB2-095C621B552D}" datafieldname="new_points" disabled="true" />
             *   </cell>
             * </row>
             */
            var found = false;
            Trace("Checking for Form Dependencies");
            foreach (var form in GetFormsWithAttribute(service, from))
            {
                Trace("Updating Form " + form.Name);
                var fromDataFieldStart = "datafieldname=\"" + from.LogicalName + "\"";
                var fromControlStart = "<control id=\"";
                const string classIdStart = "classid=\"{";
                var xml = form.FormXml;
                var dataFieldIndex = xml.IndexOf(fromDataFieldStart, StringComparison.OrdinalIgnoreCase);
                if (dataFieldIndex < 0)
                {
                    break;
                }
                var index = xml.LastIndexOf(fromControlStart, dataFieldIndex, StringComparison.OrdinalIgnoreCase);
                while (index >= 0)
                {
                    index = xml.IndexOf(classIdStart, index, StringComparison.OrdinalIgnoreCase) + classIdStart.Length;
                    var classIdEnd = xml.IndexOf("}",index, StringComparison.OrdinalIgnoreCase);
                    xml = xml.Remove(index, classIdEnd - index).
                                                Insert(index, GetClassId(to));

                    dataFieldIndex = xml.IndexOf(fromDataFieldStart, ++dataFieldIndex, StringComparison.OrdinalIgnoreCase);
                    if (dataFieldIndex < 0)
                    {
                        break;
                    }
                    index = xml.LastIndexOf(fromControlStart, dataFieldIndex, StringComparison.OrdinalIgnoreCase);
                }
                form.FormXml = xml.Replace(fromControlStart + from.LogicalName + "\"", fromControlStart + to.LogicalName + "\"")
                                  .Replace(fromDataFieldStart, "datafieldname=\"" + to.LogicalName + "\"");
                service.Update(form);
                found = true;
            }
            Trace(found
                ? "Finished updating Form Dependencies"
                : "No Form Dependencies Found");
        }

        private string GetClassId(AttributeMetadata att)
        {
            switch (att.AttributeType.GetValueOrDefault(AttributeTypeCode.String))
            {
                case AttributeTypeCode.Boolean:
                    return "B0C6723A-8503-4FD7-BB28-C8A06AC933C2"; // CheckBoxControl
                case AttributeTypeCode.DateTime:
                    return "5B773807-9FB2-42DB-97C3-7A91EFF8ADFF"; // DateTimeControl
                case AttributeTypeCode.Decimal:
                    return "C3EFE0C3-0EC6-42BE-8349-CBD9079DFD8E"; // DecimalControl
                case AttributeTypeCode.Double:
                    return "0D2C745A-E5A8-4C8F-BA63-C6D3BB604660"; // FloatControl
                case AttributeTypeCode.Integer:
                    return "C6D124CA-7EDA-4A60-AEA9-7FB8D318B68F"; // IntegerControl 
                case AttributeTypeCode.Lookup:
                    return "270BD3DB-D9AF-4782-9025-509E298DEC0A"; // LookupControl
                case AttributeTypeCode.Money:
                    return "533B9E00-756B-4312-95A0-DC888637AC78"; // MoneyControl
                case AttributeTypeCode.PartyList:
                    return "CBFB742C-14E7-4A17-96BB-1A13F7F64AA2"; // PartyListControl
                case AttributeTypeCode.Picklist:
                    return "3EF39988-22BB-4F0B-BBBE-64B5A3748AEE"; // PickListControl
                case AttributeTypeCode.Status:
                    return "5D68B988-0661-4DB2-BC3E-17598AD3BE6C"; // PicklistStatusControl
                case AttributeTypeCode.String:
                    var format = ((StringAttributeMetadata) att).Format.GetValueOrDefault();
                    switch (format)
                    {
                        case StringFormat.Email:
                            return "ADA2203E-B4CD-49BE-9DDF-234642B43B52"; // EmailAddressControl
                        case StringFormat.Text:
                            return "4273EDBD-AC1D-40D3-9FB2-095C621B552D"; // TextBoxControl
                        case StringFormat.TextArea:
                            return "E0DECE4B-6FC8-4A8F-A065-082708572369"; // TextAreaControl
                        case StringFormat.Url:
                            return "71716B6C-711E-476C-8AB8-5D11542BFB47"; // UrlControl
                        case StringFormat.TickerSymbol:
                            return "1E1FC551-F7A8-43AF-AC34-A8DC35C7B6D4"; // TickerControl
                        case StringFormat.Phone:
                            return "8C10015A-B339-4982-9474-A95FE05631A5"; // PhoneNumberControl
                        case StringFormat.PhoneticGuide:
                        case StringFormat.VersionNumber:
                            throw new NotImplementedException("Unable to determine the Control ClassId for StringAttribute.Formt of " + format);
                        default:
                            throw new EnumCaseUndefinedException<StringFormat>(format);
                    }
                case AttributeTypeCode.BigInt:
                    return "C6D124CA-7EDA-4A60-AEA9-7FB8D318B68F"; // IntegerControl
                case AttributeTypeCode.Memo:
                    return "E0DECE4B-6FC8-4A8F-A065-082708572369"; // TextAreaControl
                case AttributeTypeCode.EntityName:
                case AttributeTypeCode.Virtual:
                case AttributeTypeCode.CalendarRules:
                case AttributeTypeCode.Customer:
                case AttributeTypeCode.ManagedProperty:
                case AttributeTypeCode.Owner:
                case AttributeTypeCode.State:
                case AttributeTypeCode.Uniqueidentifier:
                    throw new NotImplementedException("Unable to determine the Control ClassId for AttributeTypeCode." + att.AttributeType);
                default:
                    throw new EnumCaseUndefinedException<AttributeTypeCode>(att.AttributeType.GetValueOrDefault(AttributeTypeCode.String));
            }
        }

        private void UpdatePluginStepFilters(IOrganizationService service, AttributeMetadata att, AttributeMetadata to)
        {
            Trace("Checking for Plugin Step Filters Dependencies");
            var steps = GetPluginStepsContainingAtt(service, att);
            foreach (var step in steps)
            {
                var filter = ReplaceCsvValues(step.FilteringAttributes, att.LogicalName, to.LogicalName);
                Trace("Updating {0} - \"{1}\" to \"{2}\"", step.Name, step.FilteringAttributes, filter);
                service.Update(new SdkMessageProcessingStep
                {
                    Id = step.Id,
                    FilteringAttributes = filter
                });
            }
            Trace(steps.Count > 0
                ? "Finished updating Plugin Step Filter Dependencies"
                : "No Plugin Step Filter Dependencies Found");
        }

        private List<SdkMessageProcessingStep> GetPluginStepsContainingAtt(IOrganizationService service, AttributeMetadata att)
        {
            var qe = QueryExpressionFactory.Create<SdkMessageProcessingStep>();
            qe.AddLink<SdkMessageFilter>(SdkMessageFilter.Fields.SdkMessageFilterId)
                .WhereEqual(SdkMessageFilter.Fields.PrimaryObjectTypeCode, att.EntityLogicalName);
            AddConditionsForValueInCsv(qe.Criteria, SdkMessageProcessingStep.Fields.FilteringAttributes, att.LogicalName);
            qe.WhereIn(
                SdkMessageProcessingStep.Fields.Stage,
                (int) SdkMessageProcessingStep_Stage.Postoperation,
                (int) SdkMessageProcessingStep_Stage.Preoperation,
                (int) SdkMessageProcessingStep_Stage.Prevalidation);

            Trace("Checking for Plugin Registration Step Filtering Attribute Dependencies with Query: " + qe.GetSqlStatement());

            return service.GetEntities(qe);
        }

        private static void AddConditionsForValueInCsv(FilterExpression filter, string fieldName, string value)
        {
            filter.WhereEqual(
                new ConditionExpression(fieldName, ConditionOperator.Like, $"%,{value},%"),
                LogicalOperator.Or,
                new ConditionExpression(fieldName, ConditionOperator.Like, $"{value},%"),
                LogicalOperator.Or,
                new ConditionExpression(fieldName, ConditionOperator.Like, $"%,{value}"),
                LogicalOperator.Or,
                fieldName, value
                );
        }

        private static string ReplaceCsvValues(string value, string from, string to)
        {
            var values = value.Split(',');
            var index = Array.IndexOf(values, from);
            while (index >= 0)
            {
                values[index] = to;
                index = Array.IndexOf(values, from);
            }
            return string.Join(",", values);
        }

        private static string RemoveValueFromCsvValues(string csv, string value)
        {
            var values = csv.Split(',').Where(s => s != value);
            return string.Join(",", values);
        }

        private void UpdatePluginStepImages(IOrganizationService service, AttributeMetadata att, AttributeMetadata to)
        {
            Trace("Checking for Plugin Step Image Dependencies");
            var images = GetPluginImagesContainingAtt(service, att);
            foreach (var image in images)
            {
                var filter = ReplaceCsvValues(image.Attributes1, att.LogicalName, to.LogicalName);
                Trace("Updating {0} - \"{1}\" to \"{2}\"", image.Name, image.Attributes1, filter);
                service.Update(new SdkMessageProcessingStepImage
                {
                    Id = image.Id,
                    Attributes1 = filter,
                    SdkMessageProcessingStepId = image.SdkMessageProcessingStepId
                });
            }

            Trace(images.Count > 0
                ? "Finished updating Plugin Step Image Dependencies"
                : "No Plugin Step Image Dependencies Found");
        }

        private List<SdkMessageProcessingStepImage> GetPluginImagesContainingAtt(IOrganizationService service, AttributeMetadata att)
        {
            var qe = QueryExpressionFactory.Create<SdkMessageProcessingStepImage>();
            qe.AddLink<SdkMessageProcessingStep>(SdkMessageProcessingStepImage.Fields.SdkMessageProcessingStepId)
                .AddLink<SdkMessageFilter>(SdkMessageProcessingStep.Fields.SdkMessageFilterId)
                .WhereEqual(SdkMessageFilter.Fields.PrimaryObjectTypeCode, att.EntityLogicalName);
            AddConditionsForValueInCsv(qe.Criteria, SdkMessageProcessingStepImage.Fields.Attributes1, att.LogicalName);

            Trace("Checking for Plugin Registration Step Images Attribute Dependencies with Query: " + qe.GetSqlStatement());
            return service.GetEntities(qe);
        }

        private void UpdateWorkflows(IOrganizationService service, AttributeMetadata att, AttributeMetadata to)
        {
            Trace("Checking for Workflow Dependencies");
            var depends = ((RetrieveDependenciesForDeleteResponse) service.Execute(new RetrieveDependenciesForDeleteRequest
            {
                ComponentType = (int) ComponentType.Attribute,
                ObjectId = att.MetadataId.GetValueOrDefault()
            })).EntityCollection.ToEntityList<Dependency>().Where(d => d.DependentComponentTypeEnum == ComponentType.Workflow).ToList();

            if (!depends.Any())
            {
                Trace("No Workflow Dependencies Found");
                return;
            }

            foreach (var workflow in service.GetEntitiesById<Workflow>(depends.Select(d => d.DependentComponentObjectId.GetValueOrDefault())))
            {
                var workflowToUpdate = new Workflow
                {
                    Id = workflow.Id
                };
                Trace("Updating {0} - {1} ({2})", workflow.CategoryEnum.ToString(), workflow.Name, workflow.Id);
                var xml = UpdateBusinessProcessFlowClassId(workflow.Xaml, att, to);
                workflowToUpdate.Xaml = xml.Replace("\"" + att.LogicalName + "\"", "\"" + to.LogicalName + "\"");
                var activate = workflow.StateCode == WorkflowState.Activated;
                if (activate)
                {
                    service.Execute(new SetStateRequest()
                    {
                        EntityMoniker = workflow.ToEntityReference(),
                        State = new OptionSetValue((int) WorkflowState.Draft),
                        Status = new OptionSetValue((int) Workflow_StatusCode.Draft)
                    });
                }

                try
                {
                    var triggers = service.GetEntities<ProcessTrigger>(ProcessTrigger.Fields.ProcessId,
                        workflow.Id,
                        ProcessTrigger.Fields.ControlName,
                        att.LogicalName);

                    foreach (var trigger in triggers)
                    {
                        Trace("Updating Trigger {0} for Workflow", trigger.Id);
                        service.Update(new ProcessTrigger
                        {
                            Id = trigger.Id,
                            ControlName = to.LogicalName
                        });
                    }

                    if (workflow.TriggerOnUpdateAttributeList != null)
                    {
                        var onUpdateFilters = ReplaceCsvValues(workflow.TriggerOnUpdateAttributeList, att.LogicalName, to.LogicalName);
                        if (onUpdateFilters != workflow.TriggerOnUpdateAttributeList)
                        {
                            Trace("Updating {0} On Update Filter - \"{1}\" to \"{2}\"", workflow.Name, workflow.TriggerOnUpdateAttributeList, onUpdateFilters);
                            workflowToUpdate.TriggerOnUpdateAttributeList = onUpdateFilters;
                        }
                    }

                    service.Update(workflowToUpdate);
                }
                finally
                {
                    if (activate)
                    {
                        service.Execute(new SetStateRequest()
                        {
                            EntityMoniker = workflow.ToEntityReference(),
                            State = new OptionSetValue((int) WorkflowState.Activated),
                            Status = new OptionSetValue((int) Workflow_StatusCode.Activated)
                        });
                    }
                }
            }
        }

        private string UpdateBusinessProcessFlowClassId(string xml, AttributeMetadata att, AttributeMetadata to)
        {
            var fromDataFieldStart = "DataFieldName=\"" + att.LogicalName + "\"";
            var fromControlStart = "<mcwb:Control ";
            const string classIdStart = "ClassId=\"";
            var dataFieldIndex = xml.IndexOf(fromDataFieldStart, StringComparison.OrdinalIgnoreCase);
            if (dataFieldIndex < 0)
            {
                return xml;
            }
            var index = xml.LastIndexOf(fromControlStart, dataFieldIndex, StringComparison.OrdinalIgnoreCase);
            while (index >= 0)
            {
                index = xml.IndexOf(classIdStart, index, StringComparison.OrdinalIgnoreCase) + classIdStart.Length;
                var classIdEnd = xml.IndexOf("\" ", index, StringComparison.OrdinalIgnoreCase);
                xml = xml.Remove(index, classIdEnd - index).
                          Insert(index, GetClassId(to));

                dataFieldIndex = xml.IndexOf(fromDataFieldStart, ++dataFieldIndex, StringComparison.OrdinalIgnoreCase);
                if (dataFieldIndex < 0)
                {
                    break;
                }
                index = xml.LastIndexOf(fromControlStart, dataFieldIndex, StringComparison.OrdinalIgnoreCase);
            }
            return xml;
        }

        private void UpdateViews(IOrganizationService service, AttributeMetadata from, AttributeMetadata to)
        {
            var found = false;
            Trace("Checking for View Dependencies");
            foreach (var query in GetViewsWithAttribute(service, from)
                .Union(GetUserViewsWithAttribute(service, from)))
            {
                Trace($"Updating {query.LogicalName} {query.Name} {query.Id}");
                var toUpdate = query.CreateForUpdate();
                toUpdate.FetchXml = ReplaceFetchXmlAttribute(query.FetchXml, from.LogicalName, to.LogicalName);

                if (query.LayoutXml != null)
                {
                    toUpdate.LayoutXml = ReplaceFetchXmlAttribute(query.LayoutXml, from.LogicalName, to.LogicalName, true);
                }
                service.Update((Entity)toUpdate);
                found = true;
            }
            Trace(found
                ? "Finished updating View Dependencies"
                : "No View Dependencies Found");
        }

        private IEnumerable<IQuery> GetViewsWithAttribute(IOrganizationService service, AttributeMetadata from)
        {
            var qe = QueryExpressionFactory.Create<SavedQuery>(q => new
            {
                q.Id,
                q.Name,
                q.QueryType,
                q.FetchXml,
                q.LayoutXml,
                q.ReturnedTypeCode
            });

            AddFetchXmlCriteria(qe, SavedQuery.Fields.FetchXml, from.EntityLogicalName, from.LogicalName);

            Trace("Retrieving Views with Query: " + qe.GetSqlStatement());
           
            return service.GetEntities(qe);
        }

        private IEnumerable<IQuery> GetUserViewsWithAttribute(IOrganizationService service, AttributeMetadata from)
        {
            var qe = QueryExpressionFactory.Create<UserQuery>(q => new
            {
                q.Id,
                q.Name,
                q.QueryType,
                q.FetchXml,
                q.LayoutXml,
                q.ReturnedTypeCode
            });

            AddFetchXmlCriteria(qe, UserQuery.Fields.FetchXml, from.EntityLogicalName, from.LogicalName);

            Trace("Retrieving User Views with Query: " + qe.GetSqlStatement());
            return service.GetEntities(qe);
        }

        private string ReplaceFetchXmlAttribute(string xml, string from, string to, bool nameOnly = false)
        {
            xml = xml.Replace($"name=\"{from}\"", $"name=\"{to}\"");
            if (nameOnly)
            {
                return xml;
            }
            return xml.Replace($"attribute=\"{from}\"", $"attribute=\"{to}\"");
        }

        private static void AddFetchXmlCriteria(QueryExpression qe, string fieldName, string entityName, string attributeName)
        {
            qe.WhereEqual(
                // Look for fetch xml that has the given entity and attribute with the entity as the main entity of the fetch. 
                new ConditionExpression(fieldName, ConditionOperator.Like, $"%<entity%name=\"{entityName}\">%name=\"{attributeName}\"%</entity>%"),
                LogicalOperator.Or,
                new ConditionExpression(fieldName, ConditionOperator.Like, $"%<entity%name=\"{entityName}\">%attribute=\"{attributeName}\"%</entity>%"),
                LogicalOperator.Or,
                // Look for fetch Xml that ahs the given entity and attribute as a link entity
                new ConditionExpression(fieldName, ConditionOperator.Like, $"%<link-entity%name=\"{entityName}\"%>%name=\"{attributeName}\"%>%</link-entity>%"),
                LogicalOperator.Or,
                new ConditionExpression(fieldName, ConditionOperator.Like, $"%<link-entity%name=\"{entityName}\"%>%attribute=\"{attributeName}\"%>%</link-entity>%"));

        }

        private void CopyData(IOrganizationService service, AttributeMetadata from, AttributeMetadata to, Action actions, Dictionary<string,string> migrationMapping)
        {
            if (!MigrateData) { return; }

            var total = GetRecordCount(service, from);
            var count = 0;

            var watch = new Stopwatch();
            watch.Start(); 
            Trace("Copying data from {0} to {1}", from.LogicalName, to.LogicalName);
            var requests = new OrganizationRequestCollection();

            var qe = new QueryExpression(from.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(from.LogicalName, to.LogicalName)
            };
            qe.WhereEqual(
                new ConditionExpression(from.LogicalName, ConditionOperator.NotNull),
                LogicalOperator.Or,
                new ConditionExpression(from.LogicalName, ConditionOperator.Null),
                new ConditionExpression(to.LogicalName, ConditionOperator.NotNull));

            int updatesSuccess = 0;
            // Grab from and to, and only update if not equal.  This is to speed things up if it has failed part way through
            foreach (var entity in service.GetAllEntities<Entity>(qe))
            {
                if (count++%100 == 0 || count == total)
                {
                    if (requests.Any())
                    {
                        updatesSuccess += PerformUpdates(service, requests);
                    }

                    Trace("Copying {0} / {1}", count, total);
                    requests.Clear();
                }

                var value = entity.GetAttributeValue<object>(from.LogicalName);
                if (actions.HasFlag(Action.ChangeType))
                {
                    // If they types have changed, or going from a local to global option set or vis-a-versa
                    if (from.GetType() != to.GetType() || from is PicklistAttributeMetadata)
                    {
                        value = CopyValue(from, to, value, migrationMapping);
                    }
                }
                // If not changing the type, don't need to do a mapping.
                //else if(migrationMapping.Count > 0)
                //{
                //    value = MapValue(value, migrationMapping);
                //}

                var toValue = entity.GetAttributeValue<object>(to.LogicalName);

                if (value != null)
                {
                    if (value.Equals(toValue)) { continue; }

                    entity.Attributes[to.LogicalName] = value;
                    requests.Add(new UpdateRequest
                    {
                        Target = entity
                    });
                }
                else if (toValue != null)
                {
                    entity.Attributes[to.LogicalName] = null;
                    requests.Add(new UpdateRequest
                    {
                        Target = entity
                    });
                }
            }

            if (requests.Any())
            {
                updatesSuccess += PerformUpdates(service, requests);
            }

            watch.Stop();
            Trace("Data Migration Complete. Total {0} records processed in {1} seconds.", total, watch.ElapsedMilliseconds / 1000);
            if (IgnoreUpdateErrors)
            {
                Trace("{0} updated sucessfully", updatesSuccess);
            }
        }

        private int PerformUpdates(IOrganizationService service, OrganizationRequestCollection requests)
        {
            int countFaults = 0;
            if (SupportsExecuteMultipleRequest)
            {
                var response = (ExecuteMultipleResponse) service.Execute(new ExecuteMultipleRequest
                {
                    Settings = new ExecuteMultipleSettings
                    {
                        ContinueOnError = IgnoreUpdateErrors,
                        ReturnResponses = IgnoreUpdateErrors
                    },
                    Requests = requests
                });

                if (response.IsFaulted && !IgnoreUpdateErrors)
                {
                    var fault = response.Responses.First().Fault;
                    while (fault.InnerFault != null)
                    {
                        fault = fault.InnerFault;
                    }

                    var errorDetails = string.Empty;
                    if (fault.ErrorDetails.ContainsKey("CallStack"))
                    {
                        errorDetails = Environment.NewLine + fault.ErrorDetails["CallStack"];
                    }

                    errorDetails += string.Format("{0}{0}TRACE TEXT:{0}{1}", Environment.NewLine, fault.TraceText);

                    throw new Exception(fault.Message + errorDetails);
                }
                else
                {
                    var faultResponses = response.Responses.Where(r => r.Fault != null);
                    countFaults = faultResponses.Count();
                }
            }
            else
            {
                foreach (var request in requests)
                {
                    try
                    {
                        service.Update(((UpdateRequest)request).Target);
                    } catch
                    {
                        if (IgnoreUpdateErrors)
                        {
                            countFaults ++;
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }
            return requests.Count() - countFaults;
        }

        private long GetRecordCount(IOrganizationService service, AttributeMetadata from)
        {
            try
            {
                Trace("Retrieving Entity Count for {0}", from.EntityLogicalName);
                return ((RetrieveTotalRecordCountResponse) service.Execute(new RetrieveTotalRecordCountRequest
                {
                    EntityNames = new[] {from.EntityLogicalName}
                })).EntityRecordCountCollection.Values.First();
            }
            catch(Exception ex)
            {
                Trace("Error " + ex);
                Trace("Attempting to determine record count via legacy method");
                return GetRecordCountLegacy(service, from);
            }
        }
        private long GetRecordCountLegacy(IOrganizationService service, AttributeMetadata from)
        {
            Trace("Retrieving {0} id attribute name", from.EntityLogicalName);
            var response = (RetrieveEntityResponse) service.Execute(new RetrieveEntityRequest
            {
                LogicalName = from.EntityLogicalName,
                EntityFilters = EntityFilters.Entity
            });

            Trace("Determining record count (accurate only up to 50000)");
            var xml = string.Format(@"
            <fetch distinct='false' mapping='logical' aggregate='true'> 
                <entity name='{0}'> 
                   <attribute name='{1}' alias='{1}_count' aggregate='count'/> 
                </entity> 
            </fetch>", from.EntityLogicalName, response.EntityMetadata.PrimaryIdAttribute);

            int total;
            try
            {
                var resultEntity = service.RetrieveMultiple(new FetchExpression(xml)).Entities.First();
                total = resultEntity.GetAliasedValue<int>(response.EntityMetadata.PrimaryIdAttribute + "_count");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("AggregateQueryRecordLimit exceeded"))
                {
                    total = 50000;
                }
                else
                {
                    throw;
                }
            }

            return total;
        }

        private AttributeMetadata CreateAttributeWithDifferentName(IOrganizationService service, AttributeMetadata existingAtt, string newSchemaName, AttributeMetadata newAttributeType)
        {
            Trace("Creating Attribute " + existingAtt.EntityLogicalName + "." + newSchemaName);
            AttributeMetadata clone;
            try
            {
                clone = CreateAttributeWithDifferentNameInternal(service, (dynamic) existingAtt, newSchemaName, newAttributeType);
            }
            catch
            {
                Trace("Error Creating Attribute " + existingAtt.EntityLogicalName + "." + newSchemaName);
                throw;
            }
            
            PublishEntity(service, existingAtt.EntityLogicalName);

            return clone;
        }

        private void RemoveInvalidLanguageLocalizedLabels(Label label)
        {
            if (label == null)
            {
                return;
            }

            var labelsToRemove = label.LocalizedLabels.Where(local => !ValidLanguageCodes.Contains(local.LanguageCode)).ToList();

            if (label.UserLocalizedLabel != null && !ValidLanguageCodes.Contains(label.UserLocalizedLabel.LanguageCode))
            {
                Trace("UserLocalizedLabel was invalid.  Removing Localization Label '{0}' for language code '{1}'", label.UserLocalizedLabel.Label, label.UserLocalizedLabel.LanguageCode);
                label.UserLocalizedLabel = null;
            }

            foreach (var local in labelsToRemove)
            {
                Trace("Removing Localization Label '{0}' for language code '{1}'", local.Label, local.LanguageCode);
                label.LocalizedLabels.Remove(local);
            }

            labelsToRemove.Clear();
        }

        private void SetEntityLogicalName(AttributeMetadata att, string entityLogicalName)
        {
            var prop = att.GetType().GetProperty("EntityLogicalName");
            prop?.SetValue(att, entityLogicalName);
        }

        private void PublishEntity(IOrganizationService service, string logicalName)
        {
            Trace("Publishing Entity " + logicalName);
            service.Execute(new PublishXmlRequest
            {
                ParameterXml = $"<importexportxml><entities><entity>{logicalName}</entity></entities></importexportxml>"
            });
        }

        private void PublishAll(IOrganizationService service)
        {
            Trace("Publishing All");
            service.Execute(new PublishAllXmlRequest());
        }

        private void Trace(string message)
        {
            Debug.Assert(OnLog != null, "OnLog != null");
            OnLog?.Invoke(message);
        }

        private void Trace(string messageFormat, params object[] args)
        {
            Debug.Assert(OnLog != null, "OnLog != null");
            OnLog?.Invoke(string.Format(messageFormat, args));
        }

        private class AttributeMigrationState
        {
            public AttributeMetadata Old { get; set; }
            public AttributeMetadata Temp { get; set; }
            public AttributeMetadata New { get; set; }
        }
    }
}
