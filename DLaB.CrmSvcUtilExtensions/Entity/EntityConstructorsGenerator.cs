using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using System;
using System.CodeDom;

namespace DLaB.ModelBuilderExtensions.Entity
{
    internal class EntityConstructorsGenerator : ICustomizeCodeDomService
    {
        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            foreach (var type in codeUnit.GetEntityTypes())
            {
                AddEntityConstructors(type);
            }
        }
  

        private static void AddEntityConstructors(CodeTypeDeclaration entityClass)
        {
            var entityConstructors = typeof(Microsoft.Xrm.Sdk.Entity).GetConstructors();
            var position = 1;

            foreach (var constructor in entityConstructors)
            {
                var paramsToConstructor = constructor.GetParameters();

                // default constructor already there
                if (paramsToConstructor.Length <= 1)
                    continue;

                var codeConstructor = new CodeConstructor
                {
                    Attributes = System.CodeDom.MemberAttributes.Public,
                };

                foreach (var param in paramsToConstructor)
                {
                    if(param.Name == "entityName")
                    {
                        codeConstructor.BaseConstructorArgs.Add(new CodeArgumentReferenceExpression("EntityLogicalName"));
                    }
                    else
                    {
                        codeConstructor.Parameters.Add(new CodeParameterDeclarationExpression(param.ParameterType, param.Name));
                        codeConstructor.BaseConstructorArgs.Add(new CodeArgumentReferenceExpression(param.Name));
                    }
                }

                entityClass.Members.Insert(position, codeConstructor);
                position++;
            }
        }

    }
}
