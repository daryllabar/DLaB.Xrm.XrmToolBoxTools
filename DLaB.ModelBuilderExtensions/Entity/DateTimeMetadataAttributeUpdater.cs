using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.CodeDom;
using System.Linq;

namespace DLaB.ModelBuilderExtensions.Entity
{
    /// <summary>
    /// Adds the DateTimeMetadataAttribute to all generated DateTime properties, defining the Format and the
    /// TimeZone Adjustment (behavior) of the column.
    /// </summary>
    public class DateTimeMetadataAttributeUpdater : ICustomizeCodeDomService
    {
        #region ICustomizeCodeDomService Members

        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            var attributesByEntity = ((IMetadataProviderService)services.GetService(typeof(IMetadataProviderService)))
                .LoadMetadata(services).Entities
                .ToDictionary(k => k.LogicalName, v => v.Attributes.ToDictionary(k => k.LogicalName));

            foreach (var type in codeUnit.GetEntityTypes())
            {
                var logicalName = type.GetEntityLogicalName();
                if (!attributesByEntity.TryGetValue(logicalName ?? string.Empty, out var attributes))
                {
                    continue;
                }

                foreach (var member in type.Members)
                {
                    if (!(member is CodeMemberProperty property)
                        || !IsDateTimeProperty(property)
                        || !attributes.TryGetValue(property.GetLogicalName() ?? string.Empty, out var metadata)
                        || !(metadata is DateTimeAttributeMetadata dateTime))
                    {
                        continue;
                    }

                    property.CustomAttributes.Add(CreateAttribute(dateTime));
                }
            }
        }

        #endregion

        private static bool IsDateTimeProperty(CodeMemberProperty property)
        {
            const string dateTime = "System.DateTime";
            return property.Type.BaseType == dateTime
                   || (property.Type.TypeArguments.Count == 1 && property.Type.TypeArguments[0].BaseType == dateTime);
        }

        private static CodeAttributeDeclaration CreateAttribute(DateTimeAttributeMetadata metadata)
        {
            return new CodeAttributeDeclaration(new CodeTypeReference(DateTimeMetadataAttributeGenerator.AttributeClassName),
                new CodeAttributeArgument(CreateEnumValue(DateTimeMetadataAttributeGenerator.FormatEnumName, GetFormat(metadata))),
                new CodeAttributeArgument(CreateEnumValue(DateTimeMetadataAttributeGenerator.TimeZoneEnumName, GetTimeZoneAdjustment(metadata))));
        }

        private static CodeFieldReferenceExpression CreateEnumValue(string enumName, string value)
        {
            return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(enumName), value);
        }

        private static string GetFormat(DateTimeAttributeMetadata metadata)
        {
            return metadata.Format == DateTimeFormat.DateOnly
                ? "DateOnly"
                : "DateAndTime";
        }

        private static string GetTimeZoneAdjustment(DateTimeAttributeMetadata metadata)
        {
            var behavior = metadata.DateTimeBehavior?.Value;
            if (behavior == DateTimeBehavior.TimeZoneIndependent.Value)
            {
                return "TimeZoneIndependent";
            }

            return behavior == DateTimeBehavior.DateOnly.Value
                ? "DateOnly"
                : "UserLocal";
        }
    }
}
