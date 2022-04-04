using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Crm.Services.Utility;
using System.Linq;

namespace DLaB.CrmSvcUtilExtensions.Action
{
    public class CustomCodeGenerationService : BaseCustomCodeGenerationService
    {
        public CustomCodeGenerationService(ICodeGenerationService service) : base(service) { }

        protected override string CommandLineText => ConfigHelper.GetAppSettingOrDefault("ActionCommandLineText", string.Empty);

        protected override bool CreateOneFilePerCodeUnit => ConfigHelper.GetAppSettingOrDefault("CreateOneFilePerAction", false);

        private string OutputFilePath { get; set; }

        protected override void WriteInternal(IOrganizationMetadata organizationMetadata, string language, string outputFile, string targetNamespace, IServiceProvider services)
        {
            OutputFilePath = outputFile;

            base.WriteInternal(organizationMetadata, language, outputFile, targetNamespace, services);
            // Since no Actions.cs class gets created by default, they are all request / response classes, delete the file if there is no CommandLineText to be added
            if (!ShouldWriteOutputFile())
            {
                File.Delete(outputFile);
            }
        }

        protected override void UpdateFilesToWrite(List<FileToWrite> files)
        {
            base.UpdateFilesToWrite(files);

            //Disregard `Action.cs` if we're splitting into files/code unit && CommandLineText is empty
            if (!ShouldWriteOutputFile())
            {
                var outputFile = files.Where(file => file.Path.Equals(OutputFilePath)).SingleOrDefault();
                if (outputFile != null) files.Remove(outputFile);
            }
        }

        private bool ShouldWriteOutputFile() => !CreateOneFilePerCodeUnit && !string.IsNullOrEmpty(CommandLineText);
    }
}
