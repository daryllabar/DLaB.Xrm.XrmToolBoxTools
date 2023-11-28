using System;
using System.IO;
using DLaB.ModelBuilderExtensions;
using McTools.Xrm.Connection;
using XrmToolBox.Extensibility;

namespace DLaB.EarlyBoundGeneratorV2.Settings
{
    [Serializable]
    public class ConnectionSettings
    {
        public string SettingsPath { get; set; }

        public string FullSettingsPath => SettingsPath.RootPath(Paths.PluginsPath);

        public string SettingsDirectoryName => Path.GetDirectoryName(FullSettingsPath);

        public void Save(ConnectionDetail connectionDetail)
        {
            SettingsManager.Instance.Save(typeof(EarlyBoundGeneratorPlugin), this, connectionDetail.ConnectionName);
        }

        public static ConnectionSettings GetDefault()
        {
            return new ConnectionSettings
            {
                SettingsPath = Path.GetFullPath(Path.Combine(Paths.SettingsPath, "DLaB.EarlyBoundGeneratorV2.DefaultSettings.xml"))
            };
        }

        public static ConnectionSettings GetForConnection(ConnectionDetail connectionDetail)
        {
            // ReSharper disable once UnusedVariable
            var loadedSuccessfully = SettingsManager.Instance.TryLoad(typeof(EarlyBoundGeneratorPlugin), out ConnectionSettings localSettings, connectionDetail?.ConnectionName) ||
                                     SettingsManager.Instance.TryLoad(typeof(EarlyBoundGeneratorPlugin), out localSettings);
            return localSettings ?? GetDefault();
        }
    }
}
