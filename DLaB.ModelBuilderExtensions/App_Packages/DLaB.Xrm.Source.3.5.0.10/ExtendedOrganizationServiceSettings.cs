using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm
#else
namespace Source.DLaB.Xrm
#endif
{
    /// <summary>
    /// The Settings for the ExtendedOrganizationServiceSettings
    /// </summary>
#if !DLAB_XRM_DEBUG
    [DebuggerNonUserCode]
#endif
    public class ExtendedOrganizationServiceSettings
    {
        #region Properties

        private string _timeStartMessageFormat;
        private string _timeEndMessageFormat;

        /// <summary>
        /// All Fetch Xml or QueryExpressions (SQL Equivalent) made with IOrganziationServices executed will be traced
        /// </summary>
        public bool LogDetailedRequests { get; set; }

        /// <summary>
        /// All Requests made with IOrganizationServices will be timed.
        /// </summary>
        public bool TimeRequests { get; set; }

        /// <summary>
        /// The format of the message to use when logging the start of a request.  Must include "{0}" for the name of the request.
        /// Defaults to "Starting Timer for {0}"
        /// </summary>
        public string TimeStartMessageFormat
        {
            get => _timeStartMessageFormat;
            set
            {
                var key = Guid.NewGuid().ToString();
                const string errMsg = "The format of the TimeStartMessageFormat must include \"{0}\" for the name of the request.";
                try
                {
                    var tmp = string.Format(value, key);
                    if (!tmp.Contains(key))
                    {
                        throw new FormatException(errMsg);
                    }
                }
                catch (FormatException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new FormatException(errMsg, ex);
                }

                _timeStartMessageFormat = value;
            }
        }

        /// <summary>
        /// The format of the message to use when logging the end of a request.  Must include "{0}" for the elapsed seconds of the request.
        /// Defaults to "Timer Ended ({0,7:F3} seconds)"
        /// </summary>
        public string TimeEndMessageFormat
        {
            get => _timeEndMessageFormat;
            set
            {
                var key = Guid.NewGuid().ToString();
                const string errMsg = "The format of the TimeEndMessageFormat must include \"{0}\" for the elapsed seconds of the request.";
                try
                {
                    string.Format(value, key);
                }
                catch (FormatException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new FormatException(errMsg, ex);
                }

                _timeEndMessageFormat = value;
            }
        }

        #endregion Properties

        /// <inheritdoc />
        public ExtendedOrganizationServiceSettings()
        {
            LogDetailedRequests = true;
            TimeRequests = true;
            TimeStartMessageFormat = "Starting Timer for {0}";
            TimeEndMessageFormat = "Timer Ended ({0,7:F3} seconds)";
        }
    }
}

