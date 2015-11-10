using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DLaB.Xrm;
using DLaB.Xrm.Entities;
using McTools.Xrm.Connection;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

namespace DLaB.AttributeManager
{
    public partial class Logic
    {
        public delegate void LogHandler(string text);
        public event LogHandler OnLog;

        public EntityMetadata Metadata { get; set; }
        public bool MigrateData { get; set; }
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
            MigrateToTemp = 2,
            RemoveExistingAttribute = 4,
            CreateNewAttribute = 8,
            MigrateToNewAttribute = 16,
            RemoveTemp = 32
        }

        [Flags]
        public enum Action
        {
            Rename = 1,
            ChangeCase = 2,
            RemoveTemp = 4,
            ChangeType = 8
        }

        public Logic(IOrganizationService service, ConnectionDetail connectionDetail, EntityMetadata metadata, string tempPostFix, bool migrateData)
        {
            SupportsExecuteMultipleRequest = connectionDetail.OrganizationMajorVersion >= Crm2013 ||
                                             (connectionDetail.OrganizationMajorVersion >= Crm2011 && int.Parse(connectionDetail.OrganizationVersion.Split('.')[3]) >= Rollup12);
            Service = service;
            TempPostfix = tempPostFix;
            MigrateData = migrateData;
            ValidLanguageCodes = GetValidLanguageCodes();
            Metadata = metadata;
        }

        private HashSet<int> GetValidLanguageCodes()
        {
            var resp = (RetrieveAvailableLanguagesResponse)Service.Execute(new RetrieveAvailableLanguagesRequest());
            return new HashSet<int>(resp.LocaleIds);
        }

        public void Run(AttributeMetadata att, string newAttributeSchemaName, Steps stepsToPerform, Action actions, AttributeMetadata newAttributeType = null)
        {
            var state = GetApplicationMigrationState(Service, att, newAttributeSchemaName, actions);
            AssertValidStepsForState(att.SchemaName, newAttributeSchemaName, stepsToPerform, state, actions);
            var oldAtt = state.Old;
            var tmpAtt = state.Temp;
            var newAtt = state.New;

            switch (actions)
            {
                case Action.RemoveTemp:
                    RemoveTemp(stepsToPerform, tmpAtt);
                    break;

                case Action.Rename:
                case Action.Rename | Action.ChangeType:
                    CreateNew(newAttributeSchemaName, stepsToPerform, oldAtt, ref newAtt, newAttributeType); // Create or Retrieve the New Attribute
                    MigrateToNew(stepsToPerform, oldAtt, newAtt, actions);
                    RemoveExisting(stepsToPerform, oldAtt);
                    break;

                case Action.ChangeCase:
                case Action.ChangeCase | Action.ChangeType:
                case Action.ChangeType:
                    CreateTemp(stepsToPerform, oldAtt, ref tmpAtt, newAttributeType); // Either Create or Retrieve the Temp
                    MigrateToTemp(stepsToPerform, oldAtt, tmpAtt, actions);
                    RemoveExisting(stepsToPerform, oldAtt);
                    CreateNew(newAttributeSchemaName, stepsToPerform, tmpAtt, ref newAtt, newAttributeType);
                    MigrateToNew(stepsToPerform, tmpAtt, newAtt, actions);
                    RemoveTemp(stepsToPerform, tmpAtt);
                    break;
            }
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

        private void MigrateToNew(Steps stepsToPerform, AttributeMetadata tmpAtt, AttributeMetadata newAtt, Action actions)
        {
            if (stepsToPerform.HasFlag(Steps.MigrateToNewAttribute))
            {
                Trace("Beginning Step: Migrate To New Attribute");
                MigrateAttribute(tmpAtt, newAtt, actions);
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

        private void MigrateToTemp(Steps stepsToPerform, AttributeMetadata oldAtt, AttributeMetadata tmpAtt, Action actions)
        {
            if (stepsToPerform.HasFlag(Steps.MigrateToTemp))
            {
                Trace("Beginning Step: Migrate To Temp");
                MigrateAttribute(oldAtt, tmpAtt, actions);
                Trace("Completed Step: Migrate To Temp" + Environment.NewLine);
            }
        }

        private void MigrateAttribute(AttributeMetadata fromAtt, AttributeMetadata toAtt, Action actions)
        {
            // Replace Old Attribute with Tmp Attribute
            CopyData(Service, fromAtt, toAtt, actions);
            UpdateCharts(Service, fromAtt, toAtt);
            UpdateViews(Service, fromAtt, toAtt);
            UpdateForms(Service, fromAtt, toAtt);
            UpdateWorkflows(Service, fromAtt, toAtt);
            PublishEntity(Service, fromAtt.EntityLogicalName);
            AssertCanDelete(Service, fromAtt);
        }

        private AttributeMigrationState GetApplicationMigrationState(IOrganizationService service, AttributeMetadata att, string newSchemaName, Action actions)
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

            return state;
        }

        private void AssertValidStepsForState(string existingSchemaName, string newSchemaName, Steps stepsToPerform, AttributeMigrationState state, Action actions)
        {
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
                    if (stepsToPerform.HasFlag(Steps.RemoveExistingAttribute) || stepsToPerform.HasFlag(Steps.CreateNewAttribute) || stepsToPerform.HasFlag(Steps.MigrateToTemp))
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

            Trace("Validating Current CRM State Before Preforming Steps:");

            if (stepsToPerform.HasFlag(Steps.CreateTemp) && state.Temp != null)
            {
                throw new InvalidOperationException("Unable to Create Temp!  Temp " + state.Temp.EntityLogicalName + "." + state.Temp.LogicalName + " already exists!");
            }

            if (stepsToPerform.HasFlag(Steps.MigrateToTemp))
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

                // Can only Remove existing if Tmp already exists, or temp will be created, or if performing rename and there is a Create
                if (!(state.Temp != null || stepsToPerform.HasFlag(Steps.CreateTemp) || (!string.Equals(existingSchemaName, newSchemaName, StringComparison.OrdinalIgnoreCase) && stepsToPerform.HasFlag(Steps.CreateNewAttribute))))
                {
                    AssertInvalidState("Unable to Remove Existing Attribute!  Temporary Attribute " + existingSchemaName + TempPostfix + " does not exist!");
                }

                // Can only Remove existing if Tmp will be migrated, or has been migrated
                if (!((actions.HasFlag(Action.ChangeCase) && stepsToPerform.HasFlag(Steps.MigrateToTemp)) || (actions.HasFlag(Action.Rename) && stepsToPerform.HasFlag(Steps.MigrateToNewAttribute))))
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
                // Can only Create Global if Local does not exist or will be removed
                if (!(state.Old == null || stepsToPerform.HasFlag(Steps.RemoveExistingAttribute)))
                {
                    AssertInvalidState("Unable to create new Attribute!  Old Attribute " + existingSchemaName + " still exists!");
                }

                // Can only Create Global if doesn't already exist
                if (state.New != null)
                {
                    AssertInvalidState("Unable to create new Attribute!  New Attribute " + existingSchemaName + " already exists!");
                }
            }

            if (stepsToPerform.HasFlag(Steps.MigrateToNewAttribute))
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
                ComponentType = (int)componenttype.Attribute,
                ObjectId = attribute.MetadataId.GetValueOrDefault()
            });

            var errors = new List<string>();
            foreach (var d in depends.EntityCollection.ToEntityList<Dependency>())
            {
                var type = (componenttype)d.DependentComponentType.GetValueOrDefault();
                var dependentId = d.DependentComponentObjectId.GetValueOrDefault();
                var err = type + " " + dependentId;
                switch (type) {
                    case componenttype.EntityRelationship:
                        var response =
                            (RetrieveRelationshipResponse)service.Execute(new RetrieveRelationshipRequest { MetadataId = dependentId });
                        Trace("Entity Relationship {0} must be manually removed/added", response.RelationshipMetadata.SchemaName);
                        break;
                    case componenttype.SavedQueryVisualization:
                        var sqv = service.GetEntity<SavedQueryVisualization>(dependentId);
                            err = $"{err} ({sqv.Name} - {sqv.CreatedBy.Name})";
                        break;

                    case componenttype.SavedQuery:
                        var sq = service.GetEntity<SavedQuery>(dependentId);
                            err = $"{err} ({sq.Name} - {sq.CreatedBy.Name})";
                        break;
                }

                errors.Add(err);
            }

            if (errors.Count > 0)
            {
                throw new Exception("Dependencies found: " + Environment.NewLine + "\t" + string.Join(Environment.NewLine + "\t", errors));
            }
        }

        private void UpdateCharts(IOrganizationService service, AttributeMetadata from, AttributeMetadata to)
        {
            var fromValue = "name=\"" + from.LogicalName + "\"";
            var toValue = "name=\"" + to.LogicalName + "\"";
            var entityAttributeLikeExpression = $"%<entity name=\"{@from.EntityLogicalName}\">%{fromValue}%";

            Trace("Retrieving System Charts with cond: " + fromValue);
            var systemCharts = service.GetEntities<SavedQueryVisualization>(c => new { c.Id, c.DataDescription },
                new ConditionExpression(SavedQueryVisualization.Fields.DataDescription, ConditionOperator.Like, entityAttributeLikeExpression));

            foreach (var chart in systemCharts)
            {
                Trace("Updating Chart " + chart.Name);
                chart.DataDescription = chart.DataDescription.Replace(fromValue, toValue);
                service.Update(chart);
            }

            Trace("Retrieving User Charts with cond: " + fromValue);
            var userCharts = service.GetEntities<UserQueryVisualization>(c => new { c.Id, c.DataDescription },
                new ConditionExpression(UserQueryVisualization.Fields.DataDescription, ConditionOperator.Like, entityAttributeLikeExpression));

            foreach (var chart in userCharts)
            {
                Trace("Updating Chart " + chart.Name);
                chart.DataDescription = chart.DataDescription.Replace(fromValue, toValue);
                service.Update(chart);
            }
        }

        private void UpdateForms(IOrganizationService service, AttributeMetadata from, AttributeMetadata to)
        {
            Trace("Retrieving Forms");
            var forms = service.GetEntities<SystemForm>(
                "objecttypecode", from.EntityLogicalName,
                new ConditionExpression("formxml", ConditionOperator.Like, "%<control id=\"" + from.LogicalName + "\"%"));

            foreach (var form in forms)
            {
                Trace("Updating Form " + form.Name);
                form.FormXml = form.FormXml.Replace("<control id=\"" + from.LogicalName + "\"", "<control id=\"" + to.LogicalName + "\"").
                    Replace("datafieldname=\"" + from.LogicalName + "\"", "datafieldname=\"" + to.LogicalName + "\"");
                service.Update(form);
            }
        }

        private void UpdateWorkflows(IOrganizationService service, AttributeMetadata from, AttributeMetadata to)
        {
            Trace("Checking for Workflow Dependencies");
            var depends = ((RetrieveDependenciesForDeleteResponse)service.Execute(new RetrieveDependenciesForDeleteRequest
            {
                ComponentType = (int)componenttype.Attribute,
                ObjectId = from.MetadataId.GetValueOrDefault()
            })).EntityCollection.ToEntityList<Dependency>().Where(d => d.DependentComponentTypeEnum == componenttype.Workflow).ToList();

            if (!depends.Any())
            {
                Trace("No Workflow Dependencies Found");
                return;
            }

            foreach (var workflow in service.GetEntitiesById<Workflow>(depends.Select(d => d.DependentComponentObjectId.GetValueOrDefault())))
            {
                workflow.Xaml = workflow.Xaml.Replace("\"" + from.LogicalName + "\"", "\"" + to.LogicalName + "\"");
                var activate = workflow.StateCode.Value == WorkflowState.Activated;
                if (activate)
                {
                    service.Execute(new SetStateRequest()
                    {
                        EntityMoniker = workflow.ToEntityReference(),
                        State = new OptionSetValue((int) WorkflowState.Draft),
                        Status = new OptionSetValue((int) workflow_statuscode.Draft)
                    });
                }
                service.Update(workflow);
                if (activate)
                {
                    service.Execute(new SetStateRequest()
                    {
                        EntityMoniker = workflow.ToEntityReference(),
                        State = new OptionSetValue((int)WorkflowState.Activated),
                        Status = new OptionSetValue((int)workflow_statuscode.Activated)
                    });
                }
            }
        }

        private void UpdateViews(IOrganizationService service, AttributeMetadata from, AttributeMetadata to)
        {
            Trace("Retrieving Views");
            var queries = service.GetEntities<SavedQuery>(q => new { q.Id, q.Name, q.QueryType, q.FetchXml, q.LayoutXml },
                new ConditionExpression("fetchxml", ConditionOperator.Like, "%<entity name=\"" + from.EntityLogicalName + "\">%name=\"" + from.LogicalName + "\"%"),
                LogicalOperator.Or,
                new ConditionExpression("fetchxml", ConditionOperator.Like, "%<entity name=\"" + from.EntityLogicalName + "\">%attribute=\"" + from.LogicalName + "\"%"));

            foreach (var query in queries)
            {
                Trace("Updating View " + query.Name);
                query.FetchXml = query.FetchXml.Replace("name=\"" + from.LogicalName + "\"", "name=\"" + to.LogicalName + "\"");
                query.FetchXml = query.FetchXml.Replace("attribute=\"" + from.LogicalName + "\"", "attribute=\"" + to.LogicalName + "\"");
                if (query.LayoutXml != null)
                {
                    query.LayoutXml = query.LayoutXml.Replace("name=\"" + from.LogicalName + "\"", "name=\"" + to.LogicalName + "\"");
                }
                service.Update(query);
            }
        }

        private void CopyData(IOrganizationService service, AttributeMetadata from, AttributeMetadata to, Action actions)
        {
            if (!MigrateData) { return; }

            var total = GetRecordCount(service, from);
            var count = 0;

            Trace("Copying data from {0} to {1}", from.LogicalName, to.LogicalName);
            var requests = new OrganizationRequestCollection();
            // Grab from and to, and only update if not equal.  This is to speed things up if it has failed part way through
            foreach (var entity in service.GetAllEntities<Entity>(new QueryExpression(from.EntityLogicalName) { ColumnSet = new ColumnSet(from.LogicalName, to.LogicalName) }))
            {
                if (count++ % 100 == 0 || count == total)
                {
                    if (requests.Any())
                    {
                        PerformUpdates(service, requests);
                    }

                    Trace("Copying {0} / {1}", count, total);
                    requests.Clear();
                }

                var value = entity.GetAttributeValue<Object>(from.LogicalName);
                if (actions.HasFlag(Action.ChangeType) && from.GetType() != to.GetType())
                {
                    value = CopyValue(from, to, value);
                }
                var toValue = entity.GetAttributeValue<Object>(to.LogicalName);
                
                if (value != null)
                {
                    if (value.Equals(toValue)) continue;

                    entity.Attributes[to.LogicalName] = value;
                    requests.Add(new UpdateRequest { Target = entity });
                }
                else if (toValue != null)
                {
                    entity.Attributes[to.LogicalName] = null;
                    requests.Add(new UpdateRequest { Target = entity });
                }
            }

            if (requests.Any())
            {
                PerformUpdates(service, requests);
            }

            Trace("Data Migration Complete", count, total);
        }

        private void PerformUpdates(IOrganizationService service, OrganizationRequestCollection requests)
        {
            if (SupportsExecuteMultipleRequest)
            {
                var response = (ExecuteMultipleResponse)service.Execute(
                    new ExecuteMultipleRequest
                    {
                        Settings = new ExecuteMultipleSettings
                        {
                            ContinueOnError = false,
                            ReturnResponses = false
                        },
                        Requests = requests
                    });

                if (response.IsFaulted)
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
            }
            else
            {
                foreach (var request in requests)
                {
                    service.Save(((UpdateRequest)request).Target);
                }
            }
        }

        private int GetRecordCount(IOrganizationService service, AttributeMetadata from)
        {
            Trace("Retrieving {0} id attribute name", from.EntityLogicalName);
            var response = (RetrieveEntityResponse)service.Execute(new RetrieveEntityRequest { LogicalName = from.EntityLogicalName, EntityFilters = EntityFilters.Entity });

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
                clone = CreateAttributeWithDifferentNameInternal(service, (dynamic)existingAtt, newSchemaName, newAttributeType);
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
            prop.SetValue(att, entityLogicalName);
        }

        private void PublishEntity(IOrganizationService service, string logicalName)
        {
            Trace("Publishing Entity " + logicalName);
            service.Execute(new PublishXmlRequest
            {
                ParameterXml = "<importexportxml>"
              + "    <entities>"
              + "        <entity>" + logicalName + "</entity>"
              + "    </entities>"
              + "</importexportxml>"
            });
        }

        private void Trace(string message)
        {
            Debug.Assert(OnLog != null, "OnLog != null");
            OnLog(message);
        }

        private void Trace(string messageFormat, params object[] args)
        {
            Debug.Assert(OnLog != null, "OnLog != null");
            OnLog(string.Format(messageFormat, args));
        }

        private class AttributeMigrationState
        {
            public AttributeMetadata Old { get; set; }
            public AttributeMetadata Temp { get; set; }
            public AttributeMetadata New { get; set; }
        }
    }
}
