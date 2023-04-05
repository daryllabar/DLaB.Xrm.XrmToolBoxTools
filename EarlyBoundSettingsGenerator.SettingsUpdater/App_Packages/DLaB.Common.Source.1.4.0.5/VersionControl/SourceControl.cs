using System;
using System.IO;

#if DLAB_UNROOT_COMMON_NAMESPACE
namespace DLaB.Common.VersionControl
#else
namespace Source.DLaB.Common.VersionControl
#endif
{
    /// <summary>
    /// Helper Class to Checkout/Checking text to a Version Contorl System
    /// </summary>
#if DLAB_PUBLIC
    public class SourceControl
#else
    internal class SourceControl
#endif
    {
        [ThreadStatic]
        private static ISourceControlProvider _provider;

        private static ISourceControlProvider Provider => _provider ?? (_provider = new VsTfsSourceControlProvider());

        /// <summary>
        /// Sets the provider.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public static void SetProvider(ISourceControlProvider provider) { _provider = provider; }

        /// <summary>
        /// Checkouts the and update file if different.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="contents">The contents.</param>
        /// <exception cref="System.ArgumentException">File Path cannot contain a directory that is null or empty!</exception>
        public static bool CheckoutAndUpdateFileIfDifferent(string filePath, string contents)
        {
            var dir = Path.GetDirectoryName(filePath);
            if (string.IsNullOrWhiteSpace(dir))
            {
                throw new ArgumentException($"File Path \"{filePath}\" contains a directory that is null or empty!", nameof(filePath));
            }

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, contents);
                Provider.Add(filePath);
                return true;
            }

            return Provider.CheckoutAndUpdateIfDifferent(filePath, contents);
        }
    }
}
