using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using System;
using System.Collections.Generic;

namespace DLaB.ModelBuilderExtensions
{
    public class CodeWriterMessageFilterService : TypedServiceSettings<ICodeWriterMessageFilterService>, ICodeWriterMessageFilterService
    {
        public BlacklistLogic MessageApprover { get; set; }

        #region Constructors
        public CodeWriterMessageFilterService(ICodeWriterMessageFilterService defaultService, IDictionary<string, string> parameters) : base(defaultService, parameters)
        {
            MessageApprover = new BlacklistLogic(new HashSet<string>(DLaBSettings.MessageBlacklist), new List<string>());
        }

        public CodeWriterMessageFilterService(ICodeWriterMessageFilterService defaultService, DLaBModelBuilderSettings settings = null) : base(defaultService, settings)
        {
            MessageApprover = new BlacklistLogic(new HashSet<string>(DLaBSettings.MessageBlacklist), new List<string>());
        }

        #endregion Constructors

        #region ICodeWriterMessageFilterService Members

        public bool GenerateSdkMessage(SdkMessage sdkMessage, IServiceProvider services)
        {
            return MessageApprover.IsAllowed(sdkMessage.Name.ToLower()) && DefaultService.GenerateSdkMessage(sdkMessage, services);
        }

        public bool GenerateSdkMessagePair(SdkMessagePair sdkMessagePair, IServiceProvider services)
        {
            return DefaultService.GenerateSdkMessagePair(sdkMessagePair, services);
        }

        #endregion ICodeWriterMessageFilterService

    }
}
