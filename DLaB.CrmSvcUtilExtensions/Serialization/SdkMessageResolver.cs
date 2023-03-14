using System;
using System.Runtime.Serialization;
using System.Xml;
using Microsoft.Crm.Services.Utility;

namespace DLaB.ModelBuilderExtensions.Serialization
{
    internal class SdkMessageResolver : DataContractResolver
    {
        public const string CrmSvcUtilNamespace = "http://schemas.datacontract.org/2004/07/Microsoft.Crm.Services.Utility";

        public struct Names
        {
            public const string SdkMessages = "SdkMessages";
            public const string SdkMessage = "SdkMessage";
            public const string SdkMessagePair = "SdkMessagePair";
            public const string SdkMessageRequest = "SdkMessageRequest";
            public const string SdkMessageRequestField = "SdkMessageRequestField";
            public const string SdkMessageResponse = "SdkMessageResponse";
            public const string SdkMessageResponseField = "SdkMessageResponseField";            
        }


        public override bool TryResolveType(Type type,
                                            Type declaredType,
                                            DataContractResolver knownTypeResolver,
                                            out XmlDictionaryString typeName,
                                            out XmlDictionaryString typeNamespace)
        {
            if (type == typeof(SdkMessages))
            {
                var dictionary = new XmlDictionary();
                typeName = dictionary.Add(Names.SdkMessages);
                typeNamespace = dictionary.Add(CrmSvcUtilNamespace);
                return true; // indicating that this resolver knows how to handle "SdkMessages"
            }
            if (type == typeof(SdkMessage))
            {
                var dictionary = new XmlDictionary();
                typeName = dictionary.Add(Names.SdkMessage);
                typeNamespace = dictionary.Add(CrmSvcUtilNamespace);
                return true; // indicating that this resolver knows how to handle "SdkMessages"
            }
            if (type == typeof(SdkMessagePair))
            {
                var dictionary = new XmlDictionary();
                typeName = dictionary.Add(Names.SdkMessagePair);
                typeNamespace = dictionary.Add(CrmSvcUtilNamespace);
                return true; // indicating that this resolver knows how to handle "SdkMessages"
            }
            if (type == typeof(SdkMessageRequest))
            {
                var dictionary = new XmlDictionary();
                typeName = dictionary.Add(Names.SdkMessageRequest);
                typeNamespace = dictionary.Add(CrmSvcUtilNamespace);
                return true; // indicating that this resolver knows how to handle "SdkMessages"
            }
            if (type == typeof(SdkMessageRequestField))
            {
                var dictionary = new XmlDictionary();
                typeName = dictionary.Add(Names.SdkMessageRequestField);
                typeNamespace = dictionary.Add(CrmSvcUtilNamespace);
                return true; // indicating that this resolver knows how to handle "SdkMessages"
            }
            if (type == typeof(SdkMessageResponse))
            {
                var dictionary = new XmlDictionary();
                typeName = dictionary.Add(Names.SdkMessageResponse);
                typeNamespace = dictionary.Add(CrmSvcUtilNamespace);
                return true; // indicating that this resolver knows how to handle "SdkMessages"
            }
            if (type == typeof(SdkMessageResponseField))
            {
                var dictionary = new XmlDictionary();
                typeName = dictionary.Add(Names.SdkMessageResponseField);
                typeNamespace = dictionary.Add(CrmSvcUtilNamespace);
                return true; // indicating that this resolver knows how to handle "SdkMessages"
            }

            // Defer to the known type resolver
            var value =  knownTypeResolver.TryResolveType(type, declaredType, knownTypeResolver, out typeName, out typeNamespace);
            if (!value)
            {
                value = true;
            }
            return value;
        }

        public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver)
        {
            if (typeNamespace == CrmSvcUtilNamespace)
            {
                switch (typeName)
                {
                    case Names.SdkMessages:
                        return typeof(SdkMessages);
                    case Names.SdkMessage:
                        return typeof(SdkMessage);
                    case Names.SdkMessagePair:
                        return typeof(SdkMessagePair);
                    case Names.SdkMessageRequest:
                        return typeof(SdkMessageRequest);
                    case Names.SdkMessageRequestField:
                        return typeof(SdkMessageRequestField);
                    case Names.SdkMessageResponse:
                        return typeof(SdkMessageResponse);
                    case Names.SdkMessageResponseField:
                        return typeof(SdkMessageResponseField);
                }
            }
            // Defer to the known type resolver
            return knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType, knownTypeResolver);
        }

    }
}
