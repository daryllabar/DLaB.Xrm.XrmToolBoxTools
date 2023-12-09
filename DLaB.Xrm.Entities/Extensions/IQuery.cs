using System;
using Microsoft.Xrm.Sdk;

namespace DLaB.Xrm.Entities
{
    public interface IQuery
    {
        Guid Id { get; set; }
        string Name { get; set; }

        int? QueryType { get; set; }
        string FetchXml { get; set; }
        string LayoutXml { get; set; }
        string LogicalName { get; set; }

        IQuery CreateForUpdate();
    }

    public partial class UserQuery : IQuery
    {
        public IQuery CreateForUpdate() { return new UserQuery { Id = Id, ReturnedTypeCode = ReturnedTypeCode }; }
    }

    public partial class SavedQuery : IQuery
    {
        public IQuery CreateForUpdate() { return new SavedQuery {Id = Id, ReturnedTypeCode = ReturnedTypeCode }; }
    }

    // ReSharper disable once InconsistentNaming
    public static class IQueryExtensions{
        public static Guid Create(this IOrganizationService service, IQuery entity) { return service.Create((Entity) entity); }
        public static void Update(this IOrganizationService service, IQuery entity) { service.Update((Entity) entity); }
    }
}
