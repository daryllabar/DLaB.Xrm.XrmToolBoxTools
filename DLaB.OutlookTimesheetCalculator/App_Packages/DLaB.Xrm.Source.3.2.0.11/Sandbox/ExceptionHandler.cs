using System;
using System.Diagnostics;
#if DLAB_UNROOT_COMMON_NAMESPACE
using DLaB.Common;
#else
using Source.DLaB.Common;
#endif

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm.Sandbox
#else
namespace Source.DLaB.Xrm.Sandbox
#endif
{
    /// <summary>
    /// Exception Handler For Exceptions when executing in Sandbox Isolation Mode
    /// </summary>
#if !DLAB_XRM_DEBUG
    [DebuggerNonUserCode]
#endif
    public class ExceptionHandler
    {
        /// <summary>
        /// Determines whether the given exception can be thrown in sandbox mode.
        /// Throws a "Sandbox-Safe" Exception if it can't
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static bool AssertCanThrow(Exception ex)
        {
            var currentException = ex;
            var canThrow = true;

            // While the Exception Types are still valid to be thrown, loop through all inner exceptions, checking for validity
            while (canThrow && currentException != null)
            {
                if (IsValidToBeThrown(currentException))
                {
                    currentException = currentException.InnerException;
                }
                else
                {
                    canThrow = false;
                }
            }

            if (canThrow)
            {
                return true;
            }

            var exceptionMessage = ex.ToStringWithCallStack();

            // ReSharper disable once InvertIf - I like it better this way
            if (IsValidToBeThrown(ex))
            {
                // Attempt to throw the exact Exception Type
                var ctor = ex.GetType().GetConstructor(new[] { typeof(string) });
                if (ctor != null)
                {
                    throw (Exception) ctor.Invoke(new object[] { exceptionMessage });
                }
            }

            throw new Exception(exceptionMessage);
        }

        /// <summary>
        /// Determines whether the specified ex is valid to be thrown.
        /// Current best guess is that it is not 
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        private static bool IsValidToBeThrown(Exception ex)
        {
            var assembly = ex.GetType().Assembly.FullName.ToLower();
            return assembly.StartsWith("mscorlib,") || assembly.StartsWith("microsoft.xrm.sdk,") || assembly.StartsWith("system.");
        }
    }
}
