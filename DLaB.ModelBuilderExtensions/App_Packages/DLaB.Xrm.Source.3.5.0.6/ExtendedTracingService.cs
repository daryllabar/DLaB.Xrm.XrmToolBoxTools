using System;
using System.Text;
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
    public class ExtendedTracingService : IMaxLengthTracingService
    {
        private ITracingService TraceService { get; }
        /// <summary>
        /// The max length of the trace log.
        /// </summary>
        public int MaxTraceLength { get; }
        private StringBuilder TraceHistory { get; }


        /// <summary>
        /// Constructor.  
        /// </summary>
        /// <param name="service">The Real Trace Service to utilize</param>
        /// <param name="maxTraceLength">The max length of the trace log.</param>
        public ExtendedTracingService(ITracingService service, int maxTraceLength = 10240)
        {
            TraceService = service;
            MaxTraceLength = maxTraceLength;
            TraceHistory = new StringBuilder();
        }

        /// <inheritdoc />
        public virtual void Trace(string format, params object[] args)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(format) || TraceService == null)
                {
                    return;
                }

                if (args.Length == 0)
                {
                    TraceHistory.AppendLine(format);
                }
                else
                {
                    TraceHistory.AppendFormat(format, args);
                    TraceHistory.AppendLine();
                }
                
                // Preserve Trace Formatting for Unit Test Traceability
                TraceService.Trace(format, args);
            }
            catch (Exception ex)
            {
                AttemptToTraceTracingException(format, args, ex);
            }
        }

        /// <inheritdoc />
        public void RetraceMaxLength()
        {
            if (TraceHistory.Length <= MaxTraceLength)
            {
                return;
            }

            var trace = TraceHistory.ToString().Trim();
            if (trace.Length <= MaxTraceLength)
            {
                // WhiteSpace 
                Trace(trace);
                return;
            }

            //Assume the three traces will each add new lines, which are 2 characters each, so 6
            var maxLength = MaxTraceLength - 6;
            if (maxLength <= 0)
            {
                return;
            }

            var snip = Environment.NewLine + "..." + Environment.NewLine;
            var startLength = maxLength / 2 - snip.Length; // Subtract snip from start
            if (startLength <= 0)
            {
                // Really short MaxTraceLength, don't do anything
                return;
            }
            Trace(trace.Substring(0, startLength));
            Trace(snip);
            Trace(trace.Substring(trace.Length - (maxLength - (startLength + snip.Length))));
        }

        private void AttemptToTraceTracingException(string format, object[] args, Exception ex)
        {
            try
            {
                if (args == null)
                {
                    TraceService.Trace((format == null
                                           ? "Exception occured attempting to trace null and null args."
                                           : $@"Exception occured attempting to trace: ""{format}"" and null args.") + Environment.NewLine + ex);
                }
                else
                {
                    try
                    {
                        format = args.Length == 0 ? format : string.Format(format ?? "NULL", args);
                        TraceService.Trace($"Exception occured attempting to trace {format}.{Environment.NewLine + ex}");
                    }
                    catch
                    {
                        var argsText = $"[{string.Join(", ", args)}]";
                        TraceService.Trace($"Exception occured attempting to trace and then handle format for {format} with {argsText}.{Environment.NewLine + ex}");
                    }
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
