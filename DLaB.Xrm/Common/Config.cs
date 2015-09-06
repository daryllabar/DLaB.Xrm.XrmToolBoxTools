using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace DLaB.Xrm.Common
{
    /// <summary>
    /// Config Helper Class for working with the ConfigurationManager
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// Attempts to read the setting from the config file, and Parse to get the value.
        /// If the Type doesn't contain a Parse, a cast is attempted.
        /// Any failure in the Parse will throw an exception.
        /// If the config value is null, then the default value will be used.
        /// </summary>
        /// <typeparam name="T">The type to attempt to cast the config setting to</typeparam>
        /// <param name="appSetting">The AppSetting Key</param>
        /// <param name="defaultValue">The Value to return if the config file does not contain an entry for the appSetting</param>
        /// <returns></returns>
        public static T GetAppSettingOrDefault<T>(string appSetting, T defaultValue)
        {
            T value;
            var config = ConfigurationManager.AppSettings[appSetting];
            if(config == null){
                value = defaultValue;
            }else{
                var type = typeof(T);
                var parse = type.GetMethod("Parse", new Type[] { typeof(String) });

                if(parse == null){
                    value = (T)Convert.ChangeType(config, type);
                }else{
                    value = (T)parse.Invoke(null, new Object[] {config});
                }
            }
            return value;
        }
    }
}