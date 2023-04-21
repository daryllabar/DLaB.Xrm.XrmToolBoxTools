#if !DLAB_NO_TELEMETRY
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights;

namespace DLaB.XrmToolBoxCommon.AppInsightsHelper
{
    public static class Telemetry
    {
        private static TelemetryClient _telemetry = GetAppInsightsClient();

        public static bool Enabled { get; set; } = true;
        public static string InstrumentationKey { get; private set; }
        public static string ConnectionString { get; private set; }

        private static TelemetryClient GetAppInsightsClient()
        {
            if (string.IsNullOrEmpty(InstrumentationKey))
            {
                return null;
            }

            var config = new TelemetryConfiguration
            {
                ConnectionString = ConnectionString,
                TelemetryChannel = new Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel.ServerTelemetryChannel()
            };
            //config.TelemetryChannel = new Microsoft.ApplicationInsights.Channel.InMemoryChannel(); // Default channel

            var client = new TelemetryClient(config);
            client.Context.Component.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            client.Context.Session.Id = Guid.NewGuid().ToString();
            client.Context.User.Id = (Environment.UserName + Environment.MachineName).GetHashCode().ToString();
            client.Context.Device.OperatingSystem = Environment.OSVersion.ToString();
            return client;
        }

        public static void SetUser(string user)
        {
            _telemetry.Context.User.AuthenticatedUserId = user;
        }

        [Obsolete("Use InitAiConnection")]
        public static void InitAiConfig(string instrumentationKey)
        {
            InstrumentationKey = $"InstrumentationKey={instrumentationKey};IngestionEndpoint=https://westus2-1.in.applicationinsights.azure.com/;LiveEndpoint=https://westus2.livediagnostics.monitor.azure.com/";
            _telemetry = GetAppInsightsClient();
        }

        public static void InitAiConnection(string connectionString)
        {
            ConnectionString = connectionString;
            _telemetry = GetAppInsightsClient();
        }

        public static void TrackEvent(string eventName, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
            if (Enabled)
            {
                _telemetry?.TrackEvent(eventName, properties, metrics);
            }
        }

        public static void TrackException(Exception ex)
        {
            if (ex != null && Enabled)
            {
                var telex = new Microsoft.ApplicationInsights.DataContracts.ExceptionTelemetry(ex);
                _telemetry?.TrackException(telex);
                Flush();
            }
        }

        internal static void Flush()
        {
            _telemetry?.Flush();
        }
    }
}
#endif