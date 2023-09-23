// ReSharper disable InconsistentNaming

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm.Plugin
#else
namespace Source.DLaB.Xrm.Plugin
#endif
	
{
    #pragma warning disable 1591
    /// <summary>
    /// This was a CRM 4 static class, but has changed for some values: Commented out unverified values
    /// </summary>
    public struct ParameterName
    {
        //public const string Assignee = "Assignee";
        //public const string AsyncOperationId = "AsyncOperationId";
        /// <summary>
        /// Contains the Entity for Retrieve in Post Operation
        /// </summary>
        public const string BusinessEntity = "BusinessEntity";
        /// <summary>
        /// Contains the EntityCollection for RetrieveMultiple in Post Operation
        /// </summary>
        public const string BusinessEntityCollection = "BusinessEntityCollection";
        //public const string CampaignActivityId = "CampaignActivityId";
        //public const string CampaignId = "CampaignId";
        //public const string ColumnSet = "columnset";
        //public const string Context = "context";
        //public const string ContractId = "ContractId";
        //public const string EmailId = "emailid";
        //public const string EndpointId = "EndpointId";
        //public const string EntityId = "EntityId";
        public const string EntityMoniker = "EntityMoniker";
        //public const string EntityName = "EntityName";
        //public const string ExchangeRate = "ExchangeRate";
        //public const string FaxId = "FaxId";
        //public const string Id = "id";
        //public const string ItemId = "ItemId";
        //public const string ListId = "ListId";
        //public const string MemberIds = "MemberIds";
        //public const string OptionalParameters = "OptionalParameters";
        //public const string PostBusinessEntity = "PostBusinessEntity";
        //public const string PostMasterBusinessEntity = "PostMasterBusinessEntity";
        //public const string PreBusinessEntity = "PreBusinessEntity";
        //public const string PreMasterBusinessEntity = "PreMasterBusinessEntity";
        //public const string PreSubordinateBusinessEntity = "PreSubordinateBusinessEntity";
        //public const string PrivilegeId = "PrivilegeId";
        //public const string Privileges = "Privileges";
        //public const string ProductId = "ProductId";
        public const string Query = "Query";
        //public const string ReturnDynamicEntities = "ReturnDynamicEntities";
        //public const string RoleId = "RoleId";
        //public const string RoleIds = "RoleIds";
        //public const string RouteType = "RouteType";
        //public const string Settings = "Settings";
        public const string State = "State";
        public const string Status = "Status";
        //public const string SubordinateId = "subordinateid";
        //public const string SubstituteId = "SubstituteId";
        public const string Target = "Target";
        //public const string TeamId = "TeamId";
        //public const string TemplateId = "TemplateId";
        //public const string TriggerAttribute = "TriggerAttribute";
        //public const string UpdateContent = "UpdateContent";
        //public const string UserId = "UserId";
        //public const string ValidationResult = "ValidationResult";
        //public const string VisualizationDefaultModuleDataDefinition = "VisualizationDefaultModuleDataDefinition";
        //public const string WorkflowId = "WorkflowId";
    }
}
