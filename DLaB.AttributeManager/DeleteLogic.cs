using DLaB.Xrm.Entities;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Source.DLaB.Common;
using Source.DLaB.Xrm;
using System;
using System.Linq;

namespace DLaB.AttributeManager
{
    partial class Logic
    {
        private void UpdateCharts(IOrganizationService service, AttributeMetadata att)
        {
            foreach (var chart in GetSystemChartsWithAttribute(service, att))
            {
                Trace("Updating Chart " + chart.Name);
                chart.DataDescription = RemoveFieldFromFetchXml(chart.DataDescription, att.LogicalName);
                service.Update(chart);
            }

            foreach (var chart in GetUserChartsWithAttribute(service, att))
            {
                Trace("Updating Chart " + chart.Name);
                chart.DataDescription = RemoveFieldFromFetchXml(chart.DataDescription, att.LogicalName);
                service.Update(chart);
            }
        }

        private void UpdateViews(IOrganizationService service, AttributeMetadata att)
        {
            foreach (var query in GetViewsWithAttribute(service, att))
            {
                Trace("Updating View " + query.Name);
                query.FetchXml = RemoveFieldFromFetchXml(query.FetchXml, att.LogicalName);

                if (query.LayoutXml != null)
                {
                    query.LayoutXml = RemoveFieldFromFetchXml(query.LayoutXml, att.LogicalName);
                }
                service.Update(query);
            }
        }

        private void UpdateForms(IOrganizationService service, AttributeMetadata att)
        {
            /*
             * <row>
             *   <cell id="{056d159e-9144-d809-378b-9e04a7626953}" showlabel="true" locklevel="0">
             *     <labels>
             *       <label description="Points" languagecode="1033" />
             *     </labels>
             *     <control id="new_points" classid="{4273EDBD-AC1D-40d3-9FB2-095C621B552D}" datafieldname="new_points" disabled="true" />
             *   </cell>
             * </row>
             */

            foreach (var form in GetFormsWithAttribute(service, att))
            {
                Trace("Updating Form " + form.Name);
                var xml = form.FormXml;
                var dataFieldStart = "datafieldname=\"" + att.LogicalName + "\"";
                var index = xml.IndexOf(dataFieldStart, StringComparison.OrdinalIgnoreCase);
                while (index >= 0)
                {
                    index = xml.LastIndexOf("<cell ", index, StringComparison.OrdinalIgnoreCase);
                    var cellEnd = xml.IndexOf("</cell>", index, StringComparison.OrdinalIgnoreCase) + "</cell>".Length;
                    xml = xml.Remove(index, cellEnd - index);

                    index = xml.IndexOf(dataFieldStart, index, StringComparison.OrdinalIgnoreCase);
                }
                form.FormXml = xml;
                service.Update(form);
            }
        }

        private void UpdatePluginSteps(IOrganizationService service, AttributeMetadata att)
        {
            foreach (var step in GetPluginStepsContainingAtt(service, att))
            {
                var filter = RemoveValueFromCsvValues(step.FilteringAttributes, att.LogicalName);
                Trace("Updating {0} - \"{1}\" to \"{2}\"", step.Name, step.FilteringAttributes, filter);
                service.Update(new SdkMessageProcessingStep
                {
                    Id = step.Id,
                    FilteringAttributes = filter
                });
            }
        }

        private void UpdatePluginImages(IOrganizationService service, AttributeMetadata att)
        {
            Trace("Looking up Plugin Images");
            foreach (var image in GetPluginImagesContainingAtt(service, att))
            {
                var attributes = RemoveValueFromCsvValues(image.Attributes1, att.LogicalName);
                Trace("Updating {0} - \"{1}\" to \"{2}\"", image.Name, image.Attributes1, attributes);
                service.Update(new SdkMessageProcessingStepImage
                {
                    Id = image.Id,
                    Attributes1 = attributes,
                    SdkMessageProcessingStepId = image.SdkMessageProcessingStepId
                });
            }
        }

        private void UpdateRelationships(IOrganizationService service, AttributeMetadata att)
        {
            var noneFound = true;
            Trace("Looking up Relationships");
            foreach (var relationship in Metadata.OneToManyRelationships.Where(r => r.ReferencedAttribute == att.LogicalName || r.ReferencingAttribute == att.LogicalName))
            {
                Trace("Deleting One to Many Relationship: " + relationship.SchemaName);
                service.Execute(new DeleteRelationshipRequest
                {
                    Name = relationship.SchemaName
                });
                noneFound = false;
            }
            foreach (var relationship in Metadata.ManyToManyRelationships.Where(r => r.Entity1IntersectAttribute == att.LogicalName || r.Entity2IntersectAttribute == att.LogicalName))
            {
                Trace("Deleting Many to Many Relationship: " + relationship.SchemaName);
                service.Execute(new DeleteRelationshipRequest
                {
                    Name = relationship.SchemaName
                });
                noneFound = false;
            }
            // Think this is the actual entity itself.  
            //foreach (var relationship in Metadata.ManyToOneRelationships.Where(r => r.ReferencedAttribute == att.LogicalName || r.ReferencingAttribute == att.LogicalName))
            //{
            //    Trace("Deleting Many to One Relationship: " + relationship.SchemaName);
            //    service.Execute(new DeleteRelationshipRequest
            //    {
            //        Name = relationship.SchemaName
            //    });
            //    noneFound = false;
            //}

            if (noneFound)
            {
                Trace("No Relationships Found.");
            }
        }

        private void UpdateMappings(IOrganizationService service, AttributeMetadata att)
        {
            Trace("Looking up Mappings");
            var mappings = service.GetEntities<AttributeMap>(AttributeMap.Fields.SourceAttributeName,
                att.LogicalName,
                LogicalOperator.Or,
                AttributeMap.Fields.TargetAttributeName,
                att.LogicalName);
            foreach (var mapping in mappings.Where(m => !m.IsManaged.GetValueOrDefault() && !m.IsSystem.GetValueOrDefault()))
            {
                Trace("Deleting Attribute Mapping: " + mapping.Id);
                service.Delete(mapping.LogicalName, mapping.Id);
            }

            if (mappings.Count == 0)
            {
                Trace("No Mappings Found");
            }
        }

        private void UpdateWorkflows(IOrganizationService service, AttributeMetadata att)
        {
            Trace("Checking for Workflow Dependencies");
            var depends = ((RetrieveDependenciesForDeleteResponse)service.Execute(new RetrieveDependenciesForDeleteRequest
            {
                ComponentType = (int)ComponentType.Attribute,
                ObjectId = att.MetadataId.GetValueOrDefault()
            })).EntityCollection.ToEntityList<Dependency>().Where(d => d.DependentComponentTypeEnum == ComponentType.Workflow).ToList();

            if (!depends.Any())
            {
                Trace("No Workflow Dependencies Found");
                return;
            }

            foreach (var workflow in service.GetEntitiesById<Workflow>(depends.Select(d => d.DependentComponentObjectId.GetValueOrDefault())))
            {
                var workflowToUpdate = new Workflow { Id = workflow.Id };
                Trace("Updating {0} - {1} ({2})", workflow.CategoryEnum.ToString(), workflow.Name, workflow.Id);
                workflowToUpdate.Xaml = RemoveParentXmlNodesWithTagValue(workflow.Xaml, "mxswa:ActivityReference AssemblyQualifiedName=\"Microsoft.Crm.Workflow.Activities.StepComposite,", "mcwb:Control", "DataFieldName", att.LogicalName, "mxswa:ActivityReference");
                var unsupportedXml = RemoveXmlNodesWithTagValue(workflow.Xaml, "mxswa:GetEntityProperty", "Attribute", att.LogicalName);
                if (workflowToUpdate.Xaml != unsupportedXml)
                {
                    throw new NotImplementedException("Attribute is used in a Business Rules Get Entity Property.  This is unsupported for manual deletion.  Delete the Business Rule " + workflow.Name + " manually to be able to delete the attribute.");
                }
                
                var activate = workflow.StateCode == WorkflowState.Activated;
                if (activate)
                {
                    service.Execute(new SetStateRequest
                    {
                        EntityMoniker = workflow.ToEntityReference(),
                        State = new OptionSetValue((int)WorkflowState.Draft),
                        Status = new OptionSetValue((int)Workflow_StatusCode.Draft)
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
                        Trace("Deleting Trigger {0} for Workflow", trigger.Id);
                        service.Delete(ProcessTrigger.EntityLogicalName, trigger.Id);
                    }

                    if (workflow.TriggerOnUpdateAttributeList != null)
                    {
                        var newValue = RemoveValueFromCsvValues(workflow.TriggerOnUpdateAttributeList, att.LogicalName);
                        if (newValue != workflow.TriggerOnUpdateAttributeList)
                        {
                            Trace("Updating workflow {0} Trigger On Update Filter - \"{1}\" to \"{2}\"", workflow.Name, workflow.TriggerOnUpdateAttributeList, newValue);
                            workflowToUpdate.TriggerOnUpdateAttributeList = newValue;
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

        private static string RemoveFieldFromFetchXml(string xml, string attName)
        {
            xml = RemoveXmlNodesWithTagValue(xml, "condition", "attribute", attName);
            xml = RemoveXmlNodesWithTagValue(xml, "attribute", "name", attName);
            xml = RemoveXmlNodesWithTagValue(xml, "cell", "name", attName); // Layout Xml from Views
            xml = RemoveXmlNodesWithTagValue(xml, "order", "attribute", attName);
            return xml;
        }

        private static string RemoveXmlNodesWithTagValue(string xml, string nodeName, string tagName, string tagValue)
        {
            var nodeStart = $"<{nodeName} ";
            const string nodeEnd = "/>";
            var nodeFullEnd = $"</{nodeName}>";

            foreach (var node in xml.SubstringAllByString(nodeStart, nodeFullEnd)
                        .Where(c => c.Contains($"{tagName}=\"{tagValue}\"", StringComparison.OrdinalIgnoreCase)).ToArray())
            {
                xml = xml.Replace(nodeStart + node + nodeFullEnd, string.Empty);
            }

            foreach (var node in xml.SubstringAllByString(nodeStart, nodeEnd)
                                    .Where(c => !c.ContainsIgnoreCase(nodeFullEnd) && c.Contains($"{tagName}=\"{tagValue}\"")).ToArray())
            {
                xml = xml.Replace(nodeStart + node + nodeEnd, string.Empty);
            }

            return xml;
        }
        private static string RemoveParentXmlNodesWithTagValue(string xml, string parentNodeName, string nodeName, string tagName, string tagValue, string parentNodeEndName = null)
        {
            var parentNodeStart = $"<{parentNodeName} ";
            var parentNodeEnd= $"</{parentNodeEndName ?? parentNodeName}>";
            var nodeStart = $"<{nodeName} ";
            const string nodeEnd = "/>";
            var nodeFullEnd = $"</{nodeName}>";

            foreach (var parentNode in xml.SubstringAllByString(parentNodeStart, parentNodeEnd)
                                          .Where(n => n.SubstringAllByString(nodeStart, nodeEnd)
                                                       .Any(c => !c.ContainsIgnoreCase(nodeFullEnd) && c.Contains($"{tagName}=\"{tagValue}\""))
                                                      ||
                                                      n.SubstringAllByString(nodeStart, nodeFullEnd)
                                                       .Any(c => !c.ContainsIgnoreCase(nodeEnd) && c.Contains($"{tagName}=\"{tagValue}\""))).ToArray())
            {
                xml = xml.Replace(parentNodeStart + parentNode + parentNodeEnd, string.Empty);
            }

            return xml;
        }
    }
}
