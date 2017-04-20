using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Crm.Services.Utility;
using System.Reflection;
using System.Threading.Tasks;

namespace DLaB.CrmSvcUtilExtensions.Entity
{
    internal class EntityBaseClassGenerator : ICustomizeCodeDomService
    {
        public static HashSet<string> BaseEntityMembers { get; } = new HashSet<string>
        {
            "CreatedBy",
            "CreatedOn",
            "CreatedOnBehalfBy",
            "ImportSequenceNumber",
            "ModifiedBy",
            "ModifiedOnBehalfBy",
            "OverriddenCreatedOn",
            "statecode",
            "statuscode",
            "TimeZoneRuleVersionNumber",
            "UTCConversionTimeZoneCode",
            "VersionNumber"
        };

        public static HashSet<string> OrgEntityMembers { get; } = new HashSet<string>(BaseEntityMembers.Union(new []
        {
            "OrganizationId",
        }));

        public static HashSet<string> UserEntityMembers { get; } = new HashSet<string>(BaseEntityMembers.Union(new[]
        {
            "OwnerId",
            "OwningBusinessUnit",
            "OwningTeam",
            "OwningUser"
        }));

        public static string BaseEntityName => ConfigHelper.GetAppSettingOrDefault("BaseEntityClassName", "EarlyBoundEntity");
        public static string OrgEntityName => ConfigHelper.GetAppSettingOrDefault("OrgEntityClassName", "OrgEntityName");
        public static string UserEntityName => ConfigHelper.GetAppSettingOrDefault("UserEntityClassName", "UserEntityName");

        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            var types = codeUnit.Namespaces[0].Types.Cast<CodeTypeDeclaration>().ToList();
            var orgEntities = types.Where(t => TypeContainsAllMembers(t, OrgEntityMembers)).ToList();
            var userEntities = types.Where(t => TypeContainsAllMembers(t, UserEntityMembers)).ToList();

            types.Add(GetEarlyBoundEntityClassDeclaration(orgEntities.FirstOrDefault()));
            types.Add(GetOrgEntity(orgEntities.FirstOrDefault()));
            types.Add(GetUserEntity(userEntities.FirstOrDefault()));

            UpdateEntityClassesToUseBaseClass(orgEntities, OrgEntityName, OrgEntityMembers);
            UpdateEntityClassesToUseBaseClass(userEntities, UserEntityName, UserEntityMembers);
        }

        private void UpdateEntityClassesToUseBaseClass(IEnumerable<CodeTypeDeclaration> types, string baseName, IEnumerable<string> baseMemberNames)
        {
            Parallel.ForEach(types, t => 
            {
                // Set Base Class To New Class
                t.BaseTypes.Clear();
                t.BaseTypes.Add(new CodeTypeReference(baseName));

                // Remove Properties In Base
                var propertiesToRemove = t.Members.Cast<CodeTypeMember>().Where(m => baseMemberNames.Contains(m.Name)).ToList();
                propertiesToRemove.ForEach(p => t.Members.Remove(p));
            });
        }

        public static CodeTypeDeclaration GetEarlyBoundEntityClassDeclaration(CodeTypeDeclaration type)
        {
            var entityClass = new CodeTypeDeclaration(BaseEntityName)
            {
                IsClass = true,
                TypeAttributes = TypeAttributes.Public,
                BaseTypes = { new CodeTypeReference(typeof(Microsoft.Xrm.Sdk.Entity)) }
            };

            entityClass.Members.AddRange(type.Members.Cast<CodeTypeMember>().Where(p => BaseEntityMembers.Contains(p.Name)).ToArray());

            return entityClass;
        }

        public static CodeTypeDeclaration GetOrgEntity(CodeTypeDeclaration type)
        {
            var entityClass = new CodeTypeDeclaration(OrgEntityName)
            {
                IsClass = true,
                TypeAttributes = TypeAttributes.Public,
                BaseTypes = { new CodeTypeReference(BaseEntityName) }
            };

            var entityOnlyMembers = new HashSet<string>(OrgEntityMembers.Where(m => !BaseEntityMembers.Contains(m)));

            entityClass.Members.AddRange(type.Members.Cast<CodeTypeMember>().Where(p => entityOnlyMembers.Contains(p.Name)).ToArray());

            return entityClass;
        }

        public static CodeTypeDeclaration GetUserEntity(CodeTypeDeclaration type)
        {
            var entityClass = new CodeTypeDeclaration(UserEntityName)
            {
                IsClass = true,
                TypeAttributes = TypeAttributes.Public,
                BaseTypes = { new CodeTypeReference(BaseEntityName) }
            };

            var entityOnlyMembers = new HashSet<string>(UserEntityMembers.Where(m => !BaseEntityMembers.Contains(m)));

            entityClass.Members.AddRange(type.Members.Cast<CodeTypeMember>().Where(p => entityOnlyMembers.Contains(p.Name)).ToArray());

            return entityClass;
        }

        private static bool TypeContainsAllMembers(CodeTypeDeclaration t, IEnumerable<string> membersToFind)
        {
            if (!t.IsClass)
            {
                return false;
            }
            var members = new HashSet<string>(t.Members.Cast<CodeTypeMember>().Select(m => m.Name));
            return membersToFind.All(s => members.Contains(s));
        }
    }
}
