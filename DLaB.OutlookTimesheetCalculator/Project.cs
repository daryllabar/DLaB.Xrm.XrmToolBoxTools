using System;
using System.Diagnostics;

namespace DLaB.OutlookTimesheetCalculator
{
    [DebuggerDisplay("{Name}")]
    public class Project : IEquatable<Project>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsBillable { get; set; }

        #region IEquatable<Project> Members

        public bool Equals(Project other)
        {
            return other != null && other.Id == this.Id;
        }

        #endregion

        public override bool Equals(object obj)
        {
            Project other = obj as Project;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            return Id.ToString().GetHashCode();
        }
    }
}
