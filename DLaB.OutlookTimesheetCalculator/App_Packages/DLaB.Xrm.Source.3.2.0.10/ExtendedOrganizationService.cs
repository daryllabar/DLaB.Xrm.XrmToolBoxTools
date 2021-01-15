using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
#if DLAB_UNROOT_COMMON_NAMESPACE
using DLaB.Common;
#else
using Source.DLaB.Common;
#endif

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm
#else
namespace Source.DLaB.Xrm
#endif
{
    /// <summary>
    /// An IOrganizationService Wrapper that utilizes ExtendedOrganizationServiceSettings and an ITracingService to potentially log every request, timing it as well as parsing the queries into Sql
    /// </summary>
#if !DLAB_XRM_DEBUG
    [DebuggerNonUserCode]
#endif
    public class ExtendedOrganizationService : IOrganizationService
    {
        private ExtendedOrganizationServiceSettings Settings { get; }
        private IOrganizationService Service { get; }
        private ITracingService TraceService { get; }

        /// <summary>
        /// Constructor for determining if statements are timed and or logged.
        /// </summary>
        /// <param name="service">IOrganziationService to wrap.</param>
        /// <param name="trace">Tracing Service Required</param>
        /// <param name="settings">Settings</param>
        public ExtendedOrganizationService(IOrganizationService service, ITracingService trace, ExtendedOrganizationServiceSettings settings = null)
        {
            Service = service;
            Settings = settings ?? new ExtendedOrganizationServiceSettings();
            TraceService = trace ?? throw new ArgumentNullException(nameof(trace));
        }

        #region Implementation of IOrganizationService

        /// <inheritdoc />
        public Guid Create(Entity entity)
        {
            var message = Settings.LogDetailedRequests
                ? $"Create Request for {entity.LogicalName} with Id {entity.Id} and Attributes {entity.ToStringAttributes()}"
                : "Create Request";
            if (Settings.TimeRequests)
            {
                var timer = new Stopwatch();
                try
                {
                    TraceStart(message);
                    timer.Start();
                    return Service.Create(entity);
                }
                finally
                {
                    TraceEnd(timer);
                }
            }

            TraceExecute(message);
            return Service.Create(entity);
        }

        /// <inheritdoc />
        public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
        {
            var message = Settings.LogDetailedRequests
                ? $"Retrieve Request for {entityName} with id {id} and Columns {string.Join(", ", columnSet.Columns)}"
                : "Retrieve Request";

            if (Settings.TimeRequests)
            {
                var timer = new Stopwatch();
                try
                {
                    TraceStart(message);
                    timer.Start();
                    return Service.Retrieve(entityName, id, columnSet);
                }
                finally
                {
                    TraceEnd(timer);
                }
            }

            TraceExecute(message);
            return Service.Retrieve(entityName, id, columnSet);
        }

        /// <inheritdoc />
        public void Update(Entity entity)
        {
            var message = Settings.LogDetailedRequests
                ? $"Update Request for {entity.LogicalName} with Id {entity.Id} and Attributes { entity.ToStringAttributes()}"
                : "Update Request";
            if (Settings.TimeRequests)
            {
                var timer = new Stopwatch();
                try
                {
                    TraceStart(message);
                    timer.Start();
                    Service.Update(entity);
                    return;
                }
                finally
                {
                    TraceEnd(timer);
                }
            }

            TraceExecute(message);
            Service.Update(entity);
        }

        /// <inheritdoc />
        public void Delete(string entityName, Guid id)
        {
            var message = Settings.LogDetailedRequests
                ? $"Delete Request for {entityName} with Id {id}"
                : "Delete Request";
            if (Settings.TimeRequests)
            {
                var timer = new Stopwatch();
                try
                {
                    TraceStart(message);
                    timer.Start();
                    Service.Delete(entityName, id);
                    return;
                }
                finally
                {
                    TraceEnd(timer);
                }
            }

            TraceExecute(message);
            Service.Delete(entityName, id);
        }

        /// <inheritdoc />
        public OrganizationResponse Execute(OrganizationRequest request)
        {
            var message = Settings.LogDetailedRequests
                ? GetDetailedMessage(request)
                : $"Execute Request {request.RequestName}";

            if (Settings.TimeRequests)
            {
                var timer = new Stopwatch();
                try
                {
                    TraceStart(message);
                    timer.Start();
                    return Service.Execute(request);
                }
                finally
                {
                    TraceEnd(timer);
                }
            }

            TraceExecute(message);
            return Service.Execute(request);
        }

        /// <inheritdoc />
        public void Associate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            var message = Settings.LogDetailedRequests
                ? $"Associate Request for {entityName}, with Id {entityId}, Relationship {relationship.SchemaName}, and Related Entities {relatedEntities.Select(e => e.ToStringDebug()).ToCsv()}."
                : "Associate Request";
            if (Settings.TimeRequests)
            {
                var timer = new Stopwatch();
                try
                {
                    TraceStart(message);
                    timer.Start();
                    Service.Associate(entityName, entityId, relationship, relatedEntities);
                    return;
                }
                finally
                {
                    TraceEnd(timer);
                }
            }

            TraceExecute(message);
            Service.Associate(entityName, entityId, relationship, relatedEntities);
        }

        /// <inheritdoc />
        public void Disassociate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            var message = Settings.LogDetailedRequests
                ? $"Disassociate Request for {entityName}, with Id {entityId}, Relationship {relationship.SchemaName}, and Related Entities {relatedEntities.ToStringDebug(new StringDebugInfo(singleLine:true))}."
                : "Disassociate Request";
            if (Settings.TimeRequests)
            {
                var timer = new Stopwatch();
                try
                {
                    TraceStart(message);
                    timer.Start();
                    Service.Disassociate(entityName, entityId, relationship, relatedEntities);
                    return;
                }
                finally
                {
                    TraceEnd(timer);
                }
            }

            TraceExecute(message);
            Service.Disassociate(entityName, entityId, relationship, relatedEntities);
        }

        /// <inheritdoc />
        public EntityCollection RetrieveMultiple(QueryBase query)
        {
            var message = Settings.LogDetailedRequests
                ? "Retrieve Multiple Request " + GetDetailedMessage(query)
                : "Retrieve Multiple Request";

            if (Settings.TimeRequests)
            {
                var timer = new Stopwatch();
                try
                {
                    TraceStart(message);
                    timer.Start();
                    if (Settings.LogDetailedRequests)
                    {
                        var results = Service.RetrieveMultiple(query);
                        TraceService.Trace("Returned: " + results.Entities.Count);
                        return results;
                    }
                    return Service.RetrieveMultiple(query);
                }
                finally
                {
                    TraceEnd(timer);
                }
            }

            TraceExecute(message);
            if (Settings.LogDetailedRequests)
            {
                var results = Service.RetrieveMultiple(query);
                TraceService.Trace("Returned: " + results.Entities.Count);
                return results;
            }
            return Service.RetrieveMultiple(query);
        }

        #endregion

        /// <summary>
        /// Gets the detailed message for the request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual string GetDetailedMessage(OrganizationRequest request)
        {
            var message = $"Execute Request for {request.RequestName} with ";
            switch (request.RequestName)
            {
                case "RetrieveMultiple":
                    var query = request["Query"] as QueryBase;
                    message += GetDetailedMessage(query);

                    break;
                default:
                    message += request.Parameters.ToStringDebug("Parameters", new StringDebugInfo(singleLine:true)) + ".";
                    break;
            }

            return message;
        }

        private string GetDetailedMessage(QueryBase query)
        {
            string message;
            switch (query)
            {
                case QueryExpression qe:
                    message = $"Query Expression: {qe.GetSqlStatement()}";
                    break;
                case FetchExpression fe:
                    message = $"Fetch Expression: {fe.Query}";
                    break;
                case QueryByAttribute ba:
                    message =
                        $"Query By Attribute for {ba.EntityName} with attributes {string.Join(", ", ba.Attributes)} and values {string.Join(", ", ba.Values)} and Columns {string.Join(", ", ba.ColumnSet.Columns)}";
                    break;
                default:
                    message = $"Unknown Query Base {query.GetType().FullName}";
                    break;
            }

            return message;
        }

        private void TraceStart(string request)
        {
            TraceService.Trace(Settings.TimeStartMessageFormat, request);
        }

        private void TraceEnd(Stopwatch timer)
        {
            timer.Stop();
            TraceService.Trace(Settings.TimeEndMessageFormat, timer.ElapsedMilliseconds/1000D);
        }

        private void TraceExecute(string message)
        {
            if (Settings.LogDetailedRequests)
            {
                TraceService.Trace("Executing " + message);
            }
        }
    }
}
