using System;
using System.Linq;
using DLaB.ModelBuilderExtensions.Entity;
using Microsoft.Crm.Services.Utility;
using System.IO;
using System.IO.Compression;

namespace DLaB.ModelBuilderExtensions.Tests.Metadata
{
    public class Provider : IMetadataProviderService
    {
        private readonly IOrganizationMetadata _metadata;

        public Provider(string fileName)
        {
            var startupPath = AppDomain.CurrentDomain.BaseDirectory;
            var pathItems = startupPath.Split(Path.DirectorySeparatorChar);
            var projectPath = string.Join(Path.DirectorySeparatorChar.ToString(), pathItems.Take(pathItems.Length - 2));
            var path = Path.Combine(projectPath, "Metadata", fileName);
            UnzipMetadata(path);
            _metadata = MetadataProviderService.DeserializeMetadata(path);
        }

        public IOrganizationMetadata LoadMetadata() { return _metadata; }

        private void UnzipMetadata(string xmlPath)
        {
            var zipPath = Path.ChangeExtension(xmlPath, "zip");
            if (!File.Exists(zipPath))
            {
                if (File.Exists(xmlPath))
                {
                    return;
                }
                throw new FileNotFoundException($"Unable to find metadata xml file {xmlPath} or zip file {zipPath}.", xmlPath);
            }

            //Don't Unzip if the XML file if same or newer
            if (File.Exists(xmlPath))
            {
                if (File.GetLastWriteTime(xmlPath) >= File.GetLastWriteTime(zipPath) || File.GetCreationTime(xmlPath) >= File.GetLastWriteTime(zipPath))
                {
                    return;
                }
            }

            ZipFile.ExtractToDirectory(zipPath, Path.GetDirectoryName(zipPath));
        }
    }
}
