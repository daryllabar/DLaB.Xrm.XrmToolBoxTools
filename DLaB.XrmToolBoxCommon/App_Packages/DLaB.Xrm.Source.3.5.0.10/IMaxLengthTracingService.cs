using Microsoft.Xrm.Sdk;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm
#else
namespace Source.DLaB.Xrm
#endif
{
    /// <summary>
    /// Interface to handle tracing to a trace log that has a max length of characters.  
    /// </summary>
    public interface IMaxLengthTracingService: ITracingService
    {
        /// <summary>
        /// If the max length of the trace has been exceeded, the most important parts of the trace are retraced.
        /// If the max length of the trace has not been exceeded, then nothing is done.
        /// </summary>
        void RetraceMaxLength();
    }
}
