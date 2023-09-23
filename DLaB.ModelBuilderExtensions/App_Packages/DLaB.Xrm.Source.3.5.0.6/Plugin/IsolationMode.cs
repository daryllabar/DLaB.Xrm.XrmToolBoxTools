using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm.Plugin
#else
namespace Source.DLaB.Xrm.Plugin
#endif
	
{
    /// <summary>
    /// Isolation Mode of the Plugin Assembly
    /// </summary>
    public enum IsolationMode
    {
        /// <summary>
        /// No Isolation Mode
        /// </summary>
        None = 1,
        /// <summary>
        /// Sandboxed Isoloation Mode
        /// </summary>
        Sandbox = 2
    }
}
