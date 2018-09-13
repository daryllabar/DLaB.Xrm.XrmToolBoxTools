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
            "ModifiedOn",
            "ModifiedOnBehalfBy",
            "OverriddenCreatedOn",
            "OnPropertyChanged",
            "OnPropertyChanging",
            "PropertyChanged",
            "PropertyChanging",
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
        public static string OrgEntityName => ConfigHelper.GetAppSettingOrDefault("OrgEntityClassName", "OrganizationOwnedEntity");
        public static string UserEntityName => ConfigHelper.GetAppSettingOrDefault("UserEntityClassName", "UserOwnedEntity");

        private bool MultiSelectCreated { get; }

        public EntityBaseClassGenerator(bool multiSelectCreated)
        {
            MultiSelectCreated = multiSelectCreated;
        }

        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            var typesCollection = codeUnit.Namespaces[0].Types;
            var types = typesCollection.Cast<CodeTypeDeclaration>().ToList();
            var orgEntities = types.Where(t => TypeContainsAllMembers(t, OrgEntityMembers)).ToList();
            var userEntities = types.Where(t => TypeContainsAllMembers(t, UserEntityMembers)).ToList();

            typesCollection.Add(GetEarlyBoundEntityClassDeclaration(orgEntities.FirstOrDefault()));
            typesCollection.Add(GetOrgEntity(orgEntities.FirstOrDefault()));
            typesCollection.Add(GetUserEntity(userEntities.FirstOrDefault()));

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

        public CodeTypeDeclaration GetEarlyBoundEntityClassDeclaration(CodeTypeDeclaration type)
        {
            var entityClass = new CodeTypeDeclaration(BaseEntityName)
            {
                IsClass = true,
                TypeAttributes = TypeAttributes.Public | TypeAttributes.Abstract,
                BaseTypes = {
                    new CodeTypeReference(typeof(Microsoft.Xrm.Sdk.Entity)),
                    new CodeTypeReference(typeof(System.ComponentModel.INotifyPropertyChanging)),
                    new CodeTypeReference(typeof(System.ComponentModel.INotifyPropertyChanged))
                }
            };

            AddEntityConstructors(entityClass);

            entityClass.Members.AddRange(type.Members.Cast<CodeTypeMember>().Where(p => BaseEntityMembers.Contains(p.Name)).Select(GetCodeTypeMember).ToArray());

            if (CustomizeCodeDomService.GenerateEnumProperties)
            {
                entityClass.Members.AddRange(EnumPropertyGenerator.CreateGetEnumMethods(MultiSelectCreated));
            }
            return entityClass;
        }

        private static CodeTypeMember GetCodeTypeMember(CodeTypeMember p)
        {
            if ((p.Name == "OnPropertyChanged" || p.Name == "OnPropertyChanging") 
                && p.Attributes.HasFlag(System.CodeDom.MemberAttributes.Private))
            {
                // ReSharper disable BitwiseOperatorOnEnumWithoutFlags
                p.Attributes = (p.Attributes & ~System.CodeDom.MemberAttributes.Private) | System.CodeDom.MemberAttributes.Family;
                // ReSharper restore BitwiseOperatorOnEnumWithoutFlags
            }
            return p;
        }

        private static void AddEntityConstructors(CodeTypeDeclaration entityClass)
        {
            var entityConstructors = typeof(Microsoft.Xrm.Sdk.Entity).GetConstructors();

            foreach (var constructor in entityConstructors)
            {
                var codeConstructor = new CodeConstructor
                {
                    Attributes = System.CodeDom.MemberAttributes.Public,
                };

                foreach (var param in constructor.GetParameters())
                {
                    codeConstructor.Parameters.Add(new CodeParameterDeclarationExpression(param.ParameterType, param.Name));
                    codeConstructor.BaseConstructorArgs.Add(new CodeArgumentReferenceExpression(param.Name));
                }

                entityClass.Members.Add(codeConstructor);
            }
        }

        public static CodeTypeDeclaration GetOrgEntity(CodeTypeDeclaration type)
        {
            var entityClass = new CodeTypeDeclaration(OrgEntityName)
            {
                IsClass = true,
                TypeAttributes = TypeAttributes.Public,
                BaseTypes = { new CodeTypeReference(BaseEntityName) }
            };

            AddEntityConstructors(entityClass);

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

            AddEntityConstructors(entityClass);

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
