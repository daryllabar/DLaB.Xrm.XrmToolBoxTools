using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLaB.ModelBuilderExtensions.Tests
{
    internal class CrmSvcUtilParameters
    {
        internal bool NoLogo { get; set; }
        internal string Language { get; set; }
        internal string Url { get; set; }
        internal string OutputFile { get; set; }
        internal string Namespace { get; set; }
        public bool UseInteractiveLogin { get; set; }
        public string ConnectionString { get; set; }
        public bool UseOAuth { get; set; }
        internal string UserName { get; set; }
        internal string Password { get; set; }
        internal string Domain { get; set; }
        internal string ServiceContextName { get; set; }
        internal string MessageNamespace { get; set; }
        internal bool ShowHelp { get; set; }
        internal string CodeCustomizationService { get; set; }
        internal string CodeWriterFilterService { get; set; }
        internal string CodeWriterMessageFilterService { get; set; }
        internal string MetadataProviderService { get; set; }
        internal string CodeGenerationService { get; set; }
        internal string NamingService { get; set; }
        internal bool Private { get; set; }
        internal bool GenerateCustomActions { get; set; }
        internal bool GenerateMessages { get; set; }
        internal string DeviceID { get; set; }
        internal string DevicePassword { get; set; }
        public string ConnectionProfileName { get; set; }

        public static CrmSvcUtilParameters CreateForEntities(string outputFile)
        {
            return new CrmSvcUtilParameters
            {
                Url = "https://MyTest.api.crm.dynamics.com/XRMServices/2011/Organization.svc",
                Namespace = "EarlyBound.Xrm.Entities",
                OutputFile = outputFile,
                ServiceContextName = "CrmContext", 
                CodeCustomizationService = "DLaB.ModelBuilderExtensions.Entity.CustomizeCodeDomService,DLaB.ModelBuilderExtensions",
                CodeGenerationService = "DLaB.ModelBuilderExtensions.Entity.CustomCodeGenerationService,DLaB.ModelBuilderExtensions",
                CodeWriterFilterService = "DLaB.ModelBuilderExtensions.Entity.CodeWriterFilterService,DLaB.ModelBuilderExtensions",
                MetadataProviderService = "DLaB.ModelBuilderExtensions.Entity.MetadataProviderService,DLaB.ModelBuilderExtensions",
                NamingService = "DLaB.ModelBuilderExtensions.NamingService,DLaB.ModelBuilderExtensions",
                UserName = "MyUser@MyTest.microsoftonline.com",
                Password = "***********"
            };
        }

        //public static object GetParamters()
        //{
            
        //}
    }
}
