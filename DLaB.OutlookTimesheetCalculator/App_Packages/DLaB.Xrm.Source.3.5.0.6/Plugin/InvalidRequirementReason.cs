using System.Collections.Generic;
using System.Linq;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm.Plugin
#else
namespace Source.DLaB.Xrm.Plugin
#endif
{
    /// <summary>
    /// The reason a requirement was invalid
    /// </summary>
    public class InvalidRequirementReason
    {
        /// <summary>
        /// If the requirement was an Any requirement
        /// </summary>
        public bool IsAny { get; set; }

        /// <summary>
        /// The columns that did not meet the requirement
        /// </summary>
        public IReadOnlyList<string> Columns { get; set; }

        /// <summary>
        /// The type of Column requirement
        /// </summary>
        public ColumnRequirementCheck RequirementType { get; set; }

        /// <summary>
        /// The context entity
        /// </summary>
        public ContextEntity ContextEntity { get; set; }

        /// <summary>
        /// The Column reason
        /// </summary>
        public InvalidColumnReason ColumnReason { get; set; }

        /// <summary>
        /// Returns a mapping to the formatted column if defined
        /// </summary>
        /// <param name="mapping">The Mapping</param>
        public List<string> MapFormattedColumn(IDictionary<string, string> mapping)
        {
            return Columns.Select(c => mapping.TryGetValue(c, out var formatted)
                ? formatted
                : c).ToList();
        }
    }
}
