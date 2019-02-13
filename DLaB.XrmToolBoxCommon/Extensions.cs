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
using Source.DLaB.Xrm;
using XrmToolBox.Extensibility;

namespace DLaB.XrmToolBoxCommon
{
    public static class Extensions
    {
        /// <summary>
        /// Attempts to lookup the user password.  First by reflection of the userPassword, then by the old public property, then by a config value, then by crying uncle and prompting the user for the password 
        /// </summary>
        /// <returns></returns>
        public static string GetUserPassword(this ConnectionDetail connection)
        {
            try
            {
                if (connection.PasswordIsEmpty)
                {
                    return string.Empty;
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
                // Probably a pervious version of the XTB.  Attempt to soldier on...
            }

            var field = connection.GetType().GetField("userPassword", BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null)
            {
                return Decrypt((string) field.GetValue(connection), "MsCrmTools", "Tanguy 92*", "SHA1", 2, "ahC3@bCa2Didfc3d", 256);
            }

            // Lookup Old Public Property
            var prop = connection.GetType().GetProperty("UserPassword", BindingFlags.Instance | BindingFlags.Public);
            if (prop != null)
            {
                return (string)prop.GetValue(connection);
            }

            // Lookup Config Value
            var password = ConfigurationManager.AppSettings["EarlyBoundGenerator.CrmSvcUtil.UserPassword"];
            if (!string.IsNullOrWhiteSpace(password))
            {
                return password;
            }

            MessageBox.Show(@"Unable to find ""EarlyBoundGenerator.CrmSvcUtil.UserPassword"" in app.config.");

            // Ask User for value
            while (string.IsNullOrWhiteSpace(password))
            {
                password = Prompt.ShowDialog("Please enter your password:", "Enter Password");
            }
            return password;

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

        #region IEnumerable<EntityMetadata>

        public static object[] ToObjectCollectionArray(this IEnumerable<EntityMetadata> entities)
        {
            return entities.
                Select(e => new ObjectCollectionItem<EntityMetadata>(e.GetDisplayNameWithLogical(), e)).
                OrderBy(r => r.DisplayName).Cast<object>().ToArray();
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

        private static void SetInitialDirectory(OpenFileDialog dialog, TextBox textBox, string rootPath)
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
