using System.IO;

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
    public class NoSourceControlProvider : ISourceControlProvider
#else
    internal class NoSourceControlProvider: ISourceControlProvider
#endif
    {
        /// <summary>
        /// Adds the file to be added to source control.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public void Add(string filePath) { }

        /// <summary>
        /// Checks the files out.  Returns the output text.
        /// </summary>
        /// <param name="fileNames">The file paths.</param>
        public string Checkout(params string[] fileNames)
        {
            return string.Empty;
        }

        /// <summary>
        /// Returns true if the file was unchanged and an undo operation was performed
        /// </summary>
        /// <param name="filePath"></param>
        public bool UndoCheckoutIfUnchanged(string filePath)
        {
            return false;
        }

        /// <summary>
        /// Returns true if the file was unchanged and so it was checked out
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="contents">The contents.</param>
        /// <returns></returns>
        public bool CheckoutAndUpdateIfDifferent(string filePath, string contents)
        {
            File.WriteAllText(filePath, contents);
            return true;
        }
    }
}
