using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Crm.Services.Utility;

namespace DLaB.CrmSvcUtilExtensions.OptionSet
{
    public class CustomCodeGenerationService : BaseCustomCodeGenerationService
    {
        public CustomCodeGenerationService(ICodeGenerationService service) : base(service) {}

        protected override string CommandLineText => ConfigHelper.GetAppSettingOrDefault("OptionSetCommandLineText", string.Empty);

        protected override bool CreateOneFilePerCodeUnit => ConfigHelper.GetAppSettingOrDefault("CreateOneFilePerOptionSet", false);

        private string LocalOptionSetEntityFileNameFormat => ConfigHelper.GetAppSettingOrDefault("LocalOptionSetEntityFileNameFormat", "{0}Sets.cs");
        private bool GroupLocalOptionSetsByEntity => ConfigHelper.GetAppSettingOrDefault("GroupLocalOptionSetsByEntity", false);

        protected override CodeUnit SplitByCodeUnit => CodeUnit.Enum;

        protected override void UpdateFilesToWrite(List<FileToWrite> files)
        {
            if (!GroupLocalOptionSetsByEntity)
            {
                return;
            }

            base.UpdateFilesToWrite(files);
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
        }
    }
}
