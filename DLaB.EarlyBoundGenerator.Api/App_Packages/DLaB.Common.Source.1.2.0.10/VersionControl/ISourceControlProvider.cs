
#if DLAB_UNROOT_COMMON_NAMESPACE
namespace DLaB.Common.VersionControl
#else
namespace Source.DLaB.Common.VersionControl
#endif
{
    /// <summary>
    /// Source Control Provider for Interacting with Source Control Versioning
    /// </summary>
#if DLAB_PUBLIC
    public interface ISourceControlProvider
#else
    internal interface ISourceControlProvider
#endif
    {
        /// <summary>
        /// Adds the file to be added to source control.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        void Add(string filePath);

        /// <summary>
        /// Checks the files out.  Returns the output text.
        /// </summary>
        /// <param name="fileNames">The file paths.</param>
        string Checkout(params string[] fileNames);

        /// <summary>
        /// Returns true if the file was unchanged and an undo operation was performed
        /// </summary>
        /// <param name="filePath"></param>
        bool UndoCheckoutIfUnchanged(string filePath);

        /// <summary>
        /// Returns true if the file was unchanged and so it was checked out
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="contents">The contents.</param>
        /// <returns></returns>
        bool CheckoutAndUpdateIfDifferent(string filePath, string contents);
    }
}
