#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm.Plugin
#else
namespace Source.DLaB.Xrm.Plugin
#endif
{
    /// <summary>
    /// The reason a column is determined to not be valid
    /// </summary>
    public enum InvalidColumnReason
    {
        /// <summary>
        /// The column was missing from the entity or target.
        /// </summary>
        Missing,
        /// <summary>
        /// The column was null when it was required to not be null.
        /// </summary>
        Null,
        /// <summary>
        /// The column is required to be a value that it isn't.
        /// </summary>
        UnspecifiedValue,
        /// <summary>
        /// The column is required to be changed in the target but wasn't.
        /// </summary>
        UnchangedValue,
        /// <summary>
        /// The column was not null when it was required to be null.
        /// </summary>
        NotNull
    }
}
