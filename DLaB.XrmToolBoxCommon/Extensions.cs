using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using DLaB.Log;
using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk.Metadata;
using XrmToolBox.Extensibility;

namespace DLaB.XrmToolBoxCommon
{
    public static class Extensions
    {
        #region ConnectionDetail

        public static string GetUrlString(this ConnectionDetail detail)
        {
            var orgName = detail.GetOrgName();
            var onPremUrl = detail.WebApplicationUrl;
            onPremUrl = onPremUrl != null && !onPremUrl.ToLower().EndsWith(orgName.ToLower())
                ? onPremUrl + orgName
                : onPremUrl;
            var url = detail.UseOnline
                ? detail.OrganizationServiceUrl
                : onPremUrl;
            return url?.Replace(@"/XRMServices/2011/Organization.svc", string.Empty);
        }

        public static string GetOrgName(this ConnectionDetail detail)
        {
            var orgName = detail.OrganizationUrlName;
            if (string.IsNullOrWhiteSpace(orgName)
                && !string.IsNullOrWhiteSpace(detail.WebApplicationUrl))
            {
                var startIndex = detail.WebApplicationUrl.LastIndexOf('/') + 1;
                var length = detail.WebApplicationUrl.IndexOf('.') - startIndex;
                if (length > 0)
                {
                    orgName = detail.WebApplicationUrl.Substring(startIndex, length);
                }
            }

            return orgName ?? string.Empty;
        }

        /// <summary>
        /// Handles returning the url for Certificate, ClientSecret and OAuth with MFA
        /// </summary>
        public static string GetNonUserConnectionString(this ConnectionDetail detail)
        {
            switch (detail.NewAuthType)
            {
                case Microsoft.Xrm.Tooling.Connector.AuthenticationType.Certificate:
                    return $"AuthType=Certificate;Url={detail.GetUrlString()};ThumbPrint={detail.Certificate.Thumbprint};ClientId={detail.AzureAdAppId};";

                case Microsoft.Xrm.Tooling.Connector.AuthenticationType.ClientSecret:
                    return $"AuthType=ClientSecret;Url={detail.GetUrlString()};ClientId={detail.AzureAdAppId};ClientSecret={detail.GetClientSecret()};";

                case Microsoft.Xrm.Tooling.Connector.AuthenticationType.OAuth:
                    if (detail.UseMfa)
                    {
                        var path = Path.Combine(Path.GetTempPath(), detail.ConnectionId.Value.ToString("B"));

                        return $"AuthType=OAuth;Username={detail.UserName};Url={detail.GetUrlString()};AppId={detail.AzureAdAppId};RedirectUri={detail.ReplyUrl};TokenCacheStorePath={path};LoginPrompt=Auto";
                    }
                    break;
            }
            return null;
        }

        /// <summary>
        /// Attempts to lookup the user password.  First by reflection of the userPassword, then by the old public property, then by a config value, then by crying uncle and prompting the user for the password 
        /// </summary>
        /// <returns></returns>
        public static string GetUserPassword(this ConnectionDetail connection)
        {
            return ExtractEncryptedValueFromConnectionDetail(connection, connection.PasswordIsEmpty, "userPassword", "UserPassword", "EarlyBoundGenerator.CrmSvcUtil.UserPassword");
        }

        /// <summary>
        /// Attempts to lookup the client password.  First by reflection of the clientPassword, then by a config value, then by crying uncle and prompting the user for the password 
        /// </summary>
        /// <returns></returns>
        public static string GetClientSecret(this ConnectionDetail connection)
        {
            return ExtractEncryptedValueFromConnectionDetail(connection, connection.ClientSecretIsEmpty, "clientSecret", "EarlyBoundGenerator.CrmSvcUtil.ClientSecret");
        }

        private static string ExtractEncryptedValueFromConnectionDetail(ConnectionDetail connection, bool isEmpty, string privateFieldName, string ebgAppSetting, string oldPublicProperty = null)
        {
            try
            {
                if (isEmpty)
                {
                    return string.Empty;
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
                // Probably a previous version of the XTB.  Attempt to soldier on...
            }

            var field = connection.GetType().GetField(privateFieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null)
            {
                // Please Tanguy, be nice and never change this!
                // \_/
                //  |._
                //  |'."-._.-""--.-"-.__.-'/
                //  |  \                  (
                //  |   |                  )
                //  |   |                 /
                //  |  /                 /
                //  |.'                 (
                //  |.-"-.__.-""-.__.-"-.)
                //  |
                //  |
                //  | ^ White Flag ^
                return Decrypt((string)field.GetValue(connection), "MsCrmTools", "Tanguy 92*", "SHA1", 2, "ahC3@bCa2Didfc3d", 256);
            }

            if(oldPublicProperty != null)
            {
                // Lookup Old Public Property
                var prop = connection.GetType().GetProperty(oldPublicProperty, BindingFlags.Instance | BindingFlags.Public);
                if (prop != null)
                {
                    return (string)prop.GetValue(connection);
                }
            }

            // Lookup Config Value
            var password = ConfigurationManager.AppSettings[ebgAppSetting];
            if (!string.IsNullOrWhiteSpace(password))
            {
                return password;
            }

            MessageBox.Show($"Unable to find \"{ebgAppSetting}\" in app.config.");

            // Ask User for value
            while (string.IsNullOrWhiteSpace(password))
            {
                password = privateFieldName == "clientSecret"
                    ? Prompt.ShowDialog("Please enter your client secret:", "Enter Client Secret")
                    : Prompt.ShowDialog("Please enter your password:", "Enter Password");
            }
            return password;
        }

        public static string Decrypt(string cipherText, string passPhrase, string saltValue, string hashAlgorithm, int passwordIterations, string initVector, int keySize)
        {
            var bytes1 = Encoding.ASCII.GetBytes(initVector);
            var bytes2 = Encoding.ASCII.GetBytes(saltValue);
            var buffer = Convert.FromBase64String(cipherText);
            var bytes3 = new PasswordDeriveBytes(passPhrase, bytes2, hashAlgorithm, passwordIterations).GetBytes(keySize / 8);
            var rijndaelManaged = new RijndaelManaged {Mode = CipherMode.CBC};
            var decryptor = rijndaelManaged.CreateDecryptor(bytes3, bytes1);
            var memoryStream = new MemoryStream(buffer);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            var numArray = new byte[buffer.Length];
            var count = cryptoStream.Read(numArray, 0, numArray.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(numArray, 0, count);
        }

        #endregion ConnectionDetail

        #region IEnumerable<EntityMetadata>

        public static object[] ToObjectCollectionArray(this IEnumerable<EntityMetadata> entities)
        {
            return entities.
                Select(e => new ObjectCollectionItem<EntityMetadata>(e.GetDisplayNameWithLogical(), e)).
                OrderBy(r => r.DisplayName).Cast<object>().ToArray();
        }

        /// <summary>
        /// Gets the text value of the di.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        private static string GetDisplayNameWithLogical(this EntityMetadata entity)
        {
            return entity.DisplayName.GetLocalOrDefaultText(entity.SchemaName) + " (" + entity.LogicalName + ")";
        }

        /// <summary>
        /// Gets the local or default text.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="defaultIfNull">The default if null.</param>
        /// <returns></returns>
        private static string GetLocalOrDefaultText(this Microsoft.Xrm.Sdk.Label label, string defaultIfNull = null)
        {
            var local = label.UserLocalizedLabel ?? label.LocalizedLabels.FirstOrDefault();

            if (local == null)
            {
                return defaultIfNull;
            }

            return local.Label ?? defaultIfNull;
        }


        #endregion // IEnumerable<EntityMetadata>

        #region OpenFileDialog

        public static void SetCsFilePath(this OpenFileDialog dialog, TextBox textBox, string rootPath = null)
        {
            dialog.DefaultExt = "cs";
            dialog.Filter = @"C# files|*.cs";
            SetInitialDirectory(dialog, textBox, rootPath);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox.Text = dialog.FileName;
            }
        }

        private static void SetInitialDirectory(FileDialog dialog, TextBox textBox, string rootPath)
        {
            if (!string.IsNullOrWhiteSpace(textBox.Text))
            {
                dialog.InitialDirectory =
                    Path.GetDirectoryName(string.IsNullOrWhiteSpace(rootPath)
                        ? Path.GetFullPath(textBox.Text)
                        : Path.GetFullPath(Path.Combine(rootPath, textBox.Text)));
            }
        }

        public static bool SetXmlFilePath(this OpenFileDialog dialog, TextBox textBox, string rootPath = null)
        {
            dialog.DefaultExt = "xml";
            dialog.Filter = @"Xml files|*.xml";
            SetInitialDirectory(dialog, textBox, rootPath);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox.Text = dialog.FileName;
                return true;
            }

            return false;
        }

        public static bool SetXmlFilePath(this SaveFileDialog dialog, TextBox textBox, string rootPath = null)
        {
            dialog.DefaultExt = "xml";
            dialog.Filter = @"Xml files|*.xml";
            SetInitialDirectory(dialog, textBox, rootPath);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox.Text = dialog.FileName;
                return true;
            }

            return false;
        }

        #endregion OpenFileDialog

        #region WorkAsyncInfo

        public static WorkAsyncInfo WithLogger(this WorkAsyncInfo info, PluginControlBase plugin, TextBox output, object asyncArgument = null, string successMessage = "Finished Successfully!", int? successPercent = 99)
        {
            plugin.Enabled = false;
            var oldWork = info.Work;
            info.Work = (w, args) =>
            {
                Logger.WireUpToReportProgress(w);
                try
                {
                    oldWork(w, args);
                    if (successPercent.HasValue)
                    {
                        w.ReportProgress(successPercent.Value, successMessage);
                    }
                    else
                    {
                        plugin.Enabled = true;
                    }
                }
                catch (Exception ex)
                {
                    w.ReportProgress(int.MinValue, ex.ToString());
                }
                finally
                {
                    Logger.UnwireFromReportProgress(w);
                }

            };
            info.AsyncArgument = asyncArgument;

            info.PostWorkCallBack = e => // Creation has finished.  Cleanup
            {
                Logger.DisplayLog(e, output);
                plugin.Enabled = true;
            };
            info.ProgressChanged = e => // Logic wants to display an update
            {
                Logger.DisplayLog(e, plugin.SetWorkingMessage, output);
            };
            return info;
        }

        #endregion WorkAsyncInfo
    }
}
