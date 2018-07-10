using System;
using System.Collections.Generic;
using System.Text;

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
        public IQuery CreateForUpdate() { return new UserQuery(Id); }
    }

    public partial class SavedQuery : IQuery
    {
        public IQuery CreateForUpdate() { return new SavedQuery(Id); }
    }
}
