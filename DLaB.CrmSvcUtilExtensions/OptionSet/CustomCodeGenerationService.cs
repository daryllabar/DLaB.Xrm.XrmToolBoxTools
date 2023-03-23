using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;

namespace DLaB.ModelBuilderExtensions.OptionSet
{
    public class CustomCodeGenerationService : BaseCustomCodeGenerationService
    {
        protected override bool CreateOneFilePerCodeUnit => DLaBSettings.CreateOneFilePerOptionSet;

        private string LocalOptionSetEntityFileNameFormat => ConfigHelper.GetAppSettingOrDefault("LocalOptionSetEntityFileNameFormat", "{0}Sets.cs");
        private bool GroupLocalOptionSetsByEntity => ConfigHelper.GetAppSettingOrDefault("GroupLocalOptionSetsByEntity", false);

        protected override CodeUnit SplitByCodeUnit => CodeUnit.Enum;

        public CustomCodeGenerationService(ICodeGenerationService defaultService, IDictionary<string, string> parameters) : base(defaultService, parameters)
        {
        }

        public CustomCodeGenerationService(ICodeGenerationService defaultService, DLaBModelBuilderSettings settings) : base(defaultService, settings)
        {
        }

        protected override void UpdateFilesToWrite(List<FileToWrite> files)
        {
            if (!GroupLocalOptionSetsByEntity)
            {
                return;
            }

            var metadata = ServiceProvider.GetMetadataForLocalEnumsByName();
            var groundFilesByEntity = new Dictionary<string, FileToWrite>();
            foreach (var file in files.ToArray())
            {
                var optionSetName = Path.GetFileNameWithoutExtension(file.Path) ?? string.Empty;
                if (!metadata.TryGetValue(optionSetName, out var localOptionSet))
                {
                    continue;
                }

                files.Remove(file);
                if(groundFilesByEntity.TryGetValue(localOptionSet.Item1, out var entityFile))
                {
                    var closingNamespaceIndex = entityFile.Contents.LastIndexOf("}", StringComparison.InvariantCulture);
                    var startingNamespaceIndex = file.Contents.IndexOf("{", StringComparison.InvariantCulture);
                    var content = entityFile.Contents.Substring(0, closingNamespaceIndex) + file.Contents.Substring(startingNamespaceIndex + 1);
                    groundFilesByEntity[localOptionSet.Item1] = new FileToWrite(entityFile.Path, content);
                }
                else
                {
                    groundFilesByEntity[localOptionSet.Item1] = new FileToWrite(Path.Combine(file.Directory, string.Format(LocalOptionSetEntityFileNameFormat, localOptionSet.Item1)), file.Contents);
                }
            }

            files.AddRange(groundFilesByEntity.Values);

            base.UpdateFilesToWrite(files);
        }
    }
}
