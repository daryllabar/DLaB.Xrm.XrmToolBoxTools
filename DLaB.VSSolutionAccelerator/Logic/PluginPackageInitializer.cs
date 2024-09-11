using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using DLaB.Log;
using DLaB.Xrm.Entities;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using XrmToolBox.Extensibility;
using System.Web.UI.WebControls.WebParts;
using Source.DLaB.Xrm;

namespace DLaB.VSSolutionAccelerator.Logic
{
    public class PluginPackageInitializer
    {
        public static void InstallCodeSnippets(string pluginPath)
        {
            var codeGenPath = Path.GetFullPath(Path.Combine(pluginPath, Settings.TemplateFolder, "CodeGeneration"));
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var vsDirectories = Directory.GetDirectories(documentsPath, "Visual Studio *");
            if (vsDirectories.Length == 0)
            {
                Logger.Show("Unable to find any Visual Studio directories in " + documentsPath);
            }

            foreach (var vs in vsDirectories)
            {
                foreach (var file in Directory.GetFiles(codeGenPath, "*.snippet"))
                {
                    var snippetFolder = Path.Combine(vs, "Code Snippets", "Visual C#", "My Code Snippets");
                    if (!Directory.Exists(snippetFolder))
                    {
                        continue;
                    }
                    var newFile = Path.Combine(snippetFolder, Path.GetFileName(file));
                    if (File.Exists(newFile))
                    {
                        Logger.AddDetail($"File {newFile} already exists!  Skipping installing snippet.");
                    }
                    else
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(newFile) ?? "");
                        Logger.AddDetail($"Installing snippet '{newFile}'...");
                        File.Copy(file, newFile);
                    }
                }
            }
        }

        public static void SetPluginPackageId(IOrganizationService service, SolutionEditorInfo solutionInfo, List<Solution> solutions)
        {
            if (!solutionInfo.CreatePlugin)
            {
                return;
            }
            var solution = solutions.First(s => s.Id == solutionInfo.PluginPackage.SolutionId);
            solutionInfo.PluginPackage.PackageId = GeneratePluginPackageId(service, solution, solutionInfo.PluginName);
        }

        private static string GeneratePluginPackageId(IOrganizationService service, Solution solution, string pluginName)
        {
            Logger.AddDetail("Creating Package...");
            var pluginPackage = InstantiatePackage(solution, pluginName);
            if(pluginPackage == null)
            {
                return null;
            }
            var id = service.Create(pluginPackage);

            Logger.AddDetail("Looking up Solution Component Definition to solution...");

            // Find the right component type for the current environment
            var scd = service.RetrieveMultiple(new QueryExpression("solutioncomponentdefinition")
            {
                ColumnSet = new ColumnSet("solutioncomponenttype"),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression("primaryentityname", ConditionOperator.Equal, "pluginpackage")
                    }
                }
            }).Entities.FirstOrDefault();

            if (scd == null)
            {
                throw new Exception("Unable to find the solution component type for table pluginpackage");
            }

            Logger.AddDetail("Adding Package to Solution...");

            service.Execute(new AddSolutionComponentRequest
            {
                AddRequiredComponents = false,
                ComponentId = id,
                ComponentType = scd.GetAttributeValue<int>("solutioncomponenttype"),
                SolutionUniqueName = solution.UniqueName
            });

            return id.ToString();
        }

        private static Entity InstantiatePackage(Solution solution, string pluginName)
        {
            var packagePath = Path.Combine(Paths.PluginsPath, Settings.TemplateFolder, "DefaultPluginPackage.nupkg");
            if (!File.Exists(packagePath))
            {
                MessageBox.Show($@"File not found: {packagePath}!  Unable to configure plugin package for Dev Deploy.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            var package = new Entity("pluginpackage")
            {
                ["solutionid"] = solution.Id
            };
            using (var p = Package.Open(packagePath, FileMode.Open))
            {
                foreach (var part in p.GetParts())
                {
                    if (!part.Uri.ToString().EndsWith(".nuspec"))
                    {
                        continue;
                    }

                    using (var stream = part.GetStream())
                    {
                        var xReader = new XmlTextReader(stream);
                        var doc = new XmlDocument();
                        doc.Load(xReader);

                        var nsmgr = new XmlNamespaceManager(doc.NameTable);
                        nsmgr.AddNamespace("ns", doc.DocumentElement.NamespaceURI);

                        var metadata = doc.SelectSingleNode("ns:package/ns:metadata", nsmgr);

                        if (metadata == null)
                        {
                            MessageBox.Show(@"Package metadata not found!\r\n\r\nCould not find the package/metadata node in " + part.Uri, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return null;
                        }

                        var version = metadata.SelectSingleNode("ns:version", nsmgr)?.InnerText;
                        if (version == null)
                        {
                            MessageBox.Show(@"Package metadata not found!\r\n\r\nCould not find the package/metadata/version node in " + part.Uri, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return null;
                        }

                        var name = solution.GetAliasedEntity<Publisher>().CustomizationPrefix + "_" + pluginName;
                        package["name"] = name;
                        package["version"] = version;
                        package["uniquename"] = name;
                    }
                }
            }

            using (var reader = new FileStream(packagePath, FileMode.Open))
            {
                using (var ms = new MemoryStream())
                {
                    reader.CopyTo(ms);

                    package["content"] = Convert.ToBase64String(ms.ToArray());
                }
            }

            return package;
        }
    }
}
