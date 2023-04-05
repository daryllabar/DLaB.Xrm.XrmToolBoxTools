using System;
using System.Diagnostics;
using Microsoft.Xrm.Sdk.Query;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm
#else
namespace Source.DLaB.Xrm
#endif
{
    /// <summary>
    /// SolutionCheckerAvoider for workarounds with the SolutionChecker
    /// </summary>
#if !DLAB_XRM_DEBUG
    [DebuggerNonUserCode]
#endif
    public class SolutionCheckerAvoider
    {
        /// <summary>
        /// Work around for il-specify-column
        /// </summary>
        /// <returns></returns>
        public static ColumnSet CreateColumnSetWithAllColumns()
        {
            return (ColumnSet)Activator.CreateInstance(typeof(ColumnSet), new object[] { true });
        }
    }
}
