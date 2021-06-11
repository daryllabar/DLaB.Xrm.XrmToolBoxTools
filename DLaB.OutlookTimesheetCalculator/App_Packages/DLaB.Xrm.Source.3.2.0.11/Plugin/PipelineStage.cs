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
    /// The Pipeline stage of the Plugin
    /// </summary>
    public enum PipelineStage
    {
        /// <summary>
        /// The pre validation - 10
        /// </summary>
        PreValidation = 10,
        /// <summary>
        /// The pre operation - 20
        /// </summary>
        PreOperation = 20,
        /// <summary>
        /// The post operation - 40
        /// </summary>
        PostOperation = 40
    }

}
