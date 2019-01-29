using System;
using Microsoft.Xrm.Sdk;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm
#else
namespace Source.DLaB.Xrm
#endif
{
    /// <summary>
    /// Tracing Service guaranteed to not throw an exception
    /// </summary>
    public class ExtendedTracingService: ITracingService
    {
        private ITracingService TraceService { get; }

        /// <summary>
        /// Constructor.  
        /// </summary>
        /// <param name="service">The Real Trace Service to utilize</param>
        public ExtendedTracingService(ITracingService service) { TraceService = service; }

        /// <inheritdoc />
        public virtual void Trace(string format, params object[] args) {
            try
            {
                if (string.IsNullOrWhiteSpace(format) || TraceService == null)
                {
                    return;
                }

                TraceService.Trace(format, args);
            }
            catch (Exception ex)
            {
                try
                {
                    if (args == null)
                    {
                        // ReSharper disable once PossibleNullReferenceException
                        TraceService.Trace((format == null
                            ? "Exception occured attempting to trace null and null args."
                            : $@"Exception occured attempting to trace: ""{format}"" and null args.") + Environment.NewLine + ex);
                    }
                    else
                    {
                        try
                        {
                            format = args.Length == 0 ? format : string.Format(format ?? "NULL", args);
                            // ReSharper disable once PossibleNullReferenceException
                            TraceService.Trace($"Exception occured attempting to trace {format}.{Environment.NewLine + ex}");
                        }
                        catch
                        {
                            var argsText = $"[{string.Join(", ", args)}]";
                            // ReSharper disable once PossibleNullReferenceException
                            TraceService.Trace( $"Exception occured attempting to trace and then handle format for {format} with {argsText}.{Environment.NewLine + ex}");
                        }
                        // ReSharper disable once PossibleNullReferenceException
                    }

                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                    // Attempted to trace a message, and had an exception, and then had another exception attempting to trace the exception that occured when tracing.
                    // Better to give up rather than stopping the entire program when attempting to write a Trace message
                }
            }
        }
    }
}
