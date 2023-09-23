using System;
using System.Diagnostics;
using Microsoft.Xrm.Sdk;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm.Plugin
#else
namespace Source.DLaB.Xrm.Plugin
#endif
	
{
    /// <summary>
    /// Trace Time that starts when the timer when the object is created, and stops it when it is disposed.
    /// </summary>
#if !DLAB_XRM_DEBUG
    [DebuggerNonUserCode]
#endif
    public class TraceTimer : IDisposable
    {
        /// <summary>
        /// Gets the tracing service.
        /// </summary>
        /// <value>
        /// The tracing service.
        /// </value>
        public ITracingService TracingService { get; private set; }
        /// <summary>
        /// Gets the timer.
        /// </summary>
        /// <value>
        /// The timer.
        /// </value>
        public Stopwatch Timer { get; private set; }
        /// <summary>
        /// Gets the end message format.
        /// </summary>
        /// <value>
        /// The end message format.
        /// </value>
        public string EndMessageFormat { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceTimer"/> class.
        /// </summary>
        /// <param name="traceService">The trace service.</param>
        public TraceTimer(ITracingService traceService) : this(traceService, "Starting Timer", "Timer Ended ({0,7:F3} seconds)")
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceTimer"/> class.
        /// </summary>
        /// <param name="traceService">The trace service.</param>
        /// <param name="message">The message.</param>
        public TraceTimer(ITracingService traceService, string message): this(traceService, "Starting Timer: " + message, "Timer Ended ({0,7:F3} seconds): " + message)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceTimer"/> class.
        /// </summary>
        /// <param name="traceService">The trace service.</param>
        /// <param name="startMessage">The start message.</param>
        /// <param name="endMessageFormat">The end message format.</param>
        public TraceTimer(ITracingService traceService, string startMessage, string endMessageFormat)
        {
            Timer = new Stopwatch();
            TracingService = traceService;
            EndMessageFormat = endMessageFormat;
            traceService.Trace(startMessage);
            Timer.Start();
        }

        private void Stop()
        {
            Timer.Stop();
            try
            {
                TracingService.Trace(string.Format(EndMessageFormat, Timer.ElapsedMilliseconds/1000D));
            }
            catch
            {
                TracingService.Trace($"Unable to trace time for end message format: {EndMessageFormat}.  Are you missing a {{0}}? Timer: {Timer.ElapsedMilliseconds}" );
            }
        }

        #region IDisposible

        private bool _disposed;


        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="TraceTimer"/> class.
        /// </summary>
        ~TraceTimer()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // free other managed objects that implement IDisposable
            }
            try
            {
                // release any unmanaged objects
                // set the object references to null
                Stop();

                EndMessageFormat = null;
                TracingService = null;
                Timer = null;
            }
            catch
            {
                // This is only called from the finalizer and should never throw and exception
            }
            finally
            {
                _disposed = true;
            }
        }

        #endregion IDisposible
    }
}
