using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using System.Linq;

namespace DLaB.ModelBuilderExtensions.Action
{
    public class CustomCodeGenerationService : BaseCustomCodeGenerationService
    {
        protected override bool CreateOneFilePerCodeUnit => DLaBSettings.CreateOneFilePerAction;

        private string OutputFilePath { get; set; }

        public CustomCodeGenerationService(ICodeGenerationService defaultService, IDictionary<string, string> parameters) : base(defaultService, parameters)
        {
        }

        public CustomCodeGenerationService(ICodeGenerationService defaultService, DLaBModelBuilderSettings settings) : base(defaultService, settings)
        {
        }

        protected override void WriteInternal(IOrganizationMetadata organizationMetadata, string language, string outputFile, string targetNamespace, IServiceProvider services)
        {
            OutputFilePath = outputFile;

            base.WriteInternal(organizationMetadata, language, outputFile, targetNamespace, services);

            // Since no Actions.cs class gets created by default, they are all request / response classes, delete the file if there is no CommandLineText to be added
            if (ShouldDeleteMainFile())
            {
                File.Delete(outputFile);
            }
        }

        protected override void UpdateFilesToWrite(List<FileToWrite> files)
        {
            if (ShouldDeleteMainFile())
            {
                //Disregard `Action.cs` if we're splitting into files/code unit && CommandLineText is empty
                var outputFile = files.FirstOrDefault(file => file.Path.Equals(OutputFilePath));
                if (outputFile != null)
                {
                    files.Remove(outputFile);
                }
            }

            base.UpdateFilesToWrite(files);

        }

        private bool ShouldDeleteMainFile() => CreateOneFilePerCodeUnit && string.IsNullOrEmpty(CommandLineText);
    }
}
