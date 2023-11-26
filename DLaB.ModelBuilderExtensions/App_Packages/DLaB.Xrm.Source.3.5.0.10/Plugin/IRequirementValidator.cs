#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm.Plugin
#else
namespace Source.DLaB.Xrm.Plugin
#endif
{
    /// <summary>
    /// Used to validate requirements
    /// </summary>
    public interface IRequirementValidator
    {
        /// <summary>
        /// The reason why the requirement was not met.  SkipExecution must be called first.
        /// </summary>
        InvalidRequirementReason Reason { get; }

        /// <summary>
        /// Returns true if the context does not meet the requirements for execution
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        bool SkipExecution(IExtendedPluginContext context);
    }
}
