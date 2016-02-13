using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace DLaB.CrmSvcUtilExtensions
{
    public static class Extensions
    {
        #region CodeTypeDeclaration

        public static string GetFieldInitalizedValue(this CodeTypeDeclaration type, string fieldName)
        {
            var field = type.Members.OfType<CodeMemberField>().FirstOrDefault(f => f.Name == fieldName);
            if (field != null)
            {
                return ((CodePrimitiveExpression)field.InitExpression).Value.ToString();
            }

            throw new Exception("Field " + fieldName + " was not found for type " + type.Name);
        }

        /// <summary>
        /// Determines if the type inherits from one of the known Xrm OrganizationServiceContext types.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static bool IsContextType(this CodeTypeDeclaration type)
        {
            var baseType = type.BaseTypes[0].BaseType;
            return baseType == "Microsoft.Xrm.Client.CrmOrganizationServiceContext"
                   || baseType == "Microsoft.Xrm.Sdk.Client.OrganizationServiceContext";
        }

        #endregion // CodeTypeDeclaration

        #region Label

        public static string GetLocalOrDefaultText(this Label label, string defaultIfNull = null)
        {
            var local = label.UserLocalizedLabel ?? label.LocalizedLabels.FirstOrDefault();

            if (local == null)
            {
                return defaultIfNull;
            }
            else
            {
                return local.Label ?? defaultIfNull;
            }
        }

        #endregion // Label

        #region OptionSetMetadataBase

        public static List<OptionMetadata> GetOptions(this OptionSetMetadataBase metadata)
        {
            List<OptionMetadata> options;
            var booleanOptionSet = metadata as BooleanOptionSetMetadata;
            var nonBooleanOptionSet = metadata as OptionSetMetadata;
            if (booleanOptionSet != null)
            {
                options = new List<OptionMetadata>
                {
                    booleanOptionSet.FalseOption,
                    booleanOptionSet.TrueOption
                };
            }
            else if (nonBooleanOptionSet != null)
            {
                options = nonBooleanOptionSet.Options.ToList();
            }
            else
            {
                throw new NotImplementedException($"OptionSetMetadataBase was of type {metadata.GetType().FullName}.  Only BooleanOptionSetMetadata or OptionSetMetadata are implemented");
            }
            return options;
        }

        #endregion OptionSetMetadataBase
    }
}
