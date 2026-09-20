using DLaB.ModelBuilderExtensions.Entity;
using FakeItEasy;
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.CodeDom;
using System.Linq;

namespace DLaB.ModelBuilderExtensions.Tests.Entity
{
    [TestClass]
    public class DateTimeMetadataAttributeUpdaterTests
    {
        [TestMethod]
        public void DateTimeProperties_Should_BeAdornedWithMetadata()
        {
            var code = CreateCode(
                CreateProperty("new_startdate", typeof(DateTime?)),
                CreateProperty("new_createdon", typeof(DateTime?)),
                CreateProperty("new_appointmenttime", typeof(DateTime?)),
                CreateProperty("new_name", typeof(string)));
            var services = CreateServices(
                CreateDateTime("new_startdate", DateTimeFormat.DateOnly, DateTimeBehavior.DateOnly),
                CreateDateTime("new_createdon", DateTimeFormat.DateAndTime, DateTimeBehavior.UserLocal),
                CreateDateTime("new_appointmenttime", DateTimeFormat.DateAndTime, DateTimeBehavior.TimeZoneIndependent),
                CreateString("new_name"));

            new DateTimeMetadataAttributeUpdater().CustomizeCodeDom(code, services);

            Assert.AreEqual("DateTimeMetadataAttribute(DateTimeFormat.DateOnly, DateTimeTimeZoneAdjustment.DateOnly)", GetMetadataAttribute(code, "new_startdate"));
            Assert.AreEqual("DateTimeMetadataAttribute(DateTimeFormat.DateAndTime, DateTimeTimeZoneAdjustment.UserLocal)", GetMetadataAttribute(code, "new_createdon"));
            Assert.AreEqual("DateTimeMetadataAttribute(DateTimeFormat.DateAndTime, DateTimeTimeZoneAdjustment.TimeZoneIndependent)", GetMetadataAttribute(code, "new_appointmenttime"));
            Assert.IsNull(GetMetadataAttribute(code, "new_name"), "Only DateTime properties should be adorned with the DateTimeMetadataAttribute!");
        }

        #region Assertion Helpers

        private static string GetMetadataAttribute(CodeCompileUnit code, string logicalName)
        {
            var property = code.GetEntityTypes()
                .SelectMany(t => t.Members.OfType<CodeMemberProperty>())
                .Single(p => p.GetLogicalName() == logicalName);
            var attribute = property.CustomAttributes.OfType<CodeAttributeDeclaration>()
                .FirstOrDefault(a => a.AttributeType.BaseType == "DateTimeMetadataAttribute");
            return attribute == null
                ? null
                : $"{attribute.AttributeType.BaseType}({string.Join(", ", attribute.Arguments.OfType<CodeAttributeArgument>().Select(GetArgumentValue))})";
        }

        private static string GetArgumentValue(CodeAttributeArgument argument)
        {
            var field = (CodeFieldReferenceExpression)argument.Value;
            return $"{((CodeTypeReferenceExpression)field.TargetObject).Type.BaseType}.{field.FieldName}";
        }

        #endregion Assertion Helpers

        #region Setup Helpers

        private static CodeCompileUnit CreateCode(params CodeMemberProperty[] properties)
        {
            var code = new CodeCompileUnit();
            var codeNamespace = new CodeNamespace("Test");
            code.Namespaces.Add(codeNamespace);
            var entity = new CodeTypeDeclaration("Contact");
            entity.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(EntityLogicalNameAttribute)), new CodeAttributeArgument(new CodePrimitiveExpression("contact"))));
            entity.Members.AddRange(properties);
            codeNamespace.Types.Add(entity);
            return code;
        }

        private static CodeMemberProperty CreateProperty(string logicalName, Type type)
        {
            var property = new CodeMemberProperty
            {
                Name = logicalName,
                Type = new CodeTypeReference(type)
            };
            property.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(AttributeLogicalNameAttribute)), new CodeAttributeArgument(new CodePrimitiveExpression(logicalName))));
            return property;
        }

        private static IServiceProvider CreateServices(params AttributeMetadata[] attributes)
        {
            var entity = new EntityMetadata { LogicalName = "contact" };
            typeof(EntityMetadata).GetProperty(nameof(EntityMetadata.Attributes))!.SetValue(entity, attributes);
            var metadata = A.Fake<IOrganizationMetadata>();
            A.CallTo(() => metadata.Entities).Returns(new[] { entity });
            var provider = A.Fake<IMetadataProviderService>();
            A.CallTo(() => provider.LoadMetadata(A<IServiceProvider>._)).Returns(metadata);
            var services = A.Fake<IServiceProvider>();
            A.CallTo(() => services.GetService(typeof(IMetadataProviderService))).Returns(provider);
            return services;
        }

        private static DateTimeAttributeMetadata CreateDateTime(string logicalName, DateTimeFormat format, DateTimeBehavior behavior)
        {
            var attribute = new DateTimeAttributeMetadata(format)
            {
                DateTimeBehavior = behavior
            };
            SetLogicalName(attribute, logicalName);
            return attribute;
        }

        private static StringAttributeMetadata CreateString(string logicalName)
        {
            var attribute = new StringAttributeMetadata();
            SetLogicalName(attribute, logicalName);
            return attribute;
        }

        private static void SetLogicalName(AttributeMetadata attribute, string logicalName)
        {
            typeof(AttributeMetadata).GetProperty(nameof(AttributeMetadata.LogicalName))!.SetValue(attribute, logicalName);
        }

        #endregion Setup Helpers
    }
}
