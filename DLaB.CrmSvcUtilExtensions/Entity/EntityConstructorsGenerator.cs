using Microsoft.Crm.Services.Utility;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLaB.CrmSvcUtilExtensions.Entity
{
    internal class EntityConstructorsGenerator : ICustomizeCodeDomService
    {
        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            var metadataService = (IMetadataProviderService)services.GetService(typeof(IMetadataProviderService));
            var metadata = metadataService.LoadMetadata();
            var types = codeUnit.Namespaces[0].Types;

            foreach (var type in types.Cast<CodeTypeDeclaration>().
                                 Where(type => type.IsClass && !type.IsContextType()))
            {
                AddEntityConstructors(type);
            }
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

                var paramsToConstructor = constructor.GetParameters();

                // default constructor already there
                if (paramsToConstructor.Length <= 1)
                    continue;

                foreach (var param in paramsToConstructor)
                {
                    if(param.Name == "entityName")
                    {
                        //codeConstructor.Parameters.Add(new CodeParameterDeclarationExpression(param.ParameterType, param.Name));
                        //codeConstructor.BaseConstructorArgs.Add(new CodeArgumentReferenceExpression(param.Name));
                        codeConstructor.BaseConstructorArgs.Add(new CodeArgumentReferenceExpression("EntityLogicalName"));
                    }
                    else
                    {
                        codeConstructor.Parameters.Add(new CodeParameterDeclarationExpression(param.ParameterType, param.Name));
                        codeConstructor.BaseConstructorArgs.Add(new CodeArgumentReferenceExpression(param.Name));
                    }
                }

                entityClass.Members.Add(codeConstructor);
            }
        }

    }
}
