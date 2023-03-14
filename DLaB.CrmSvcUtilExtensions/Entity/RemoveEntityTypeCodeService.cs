using System;
using System.CodeDom;
using System.Linq;
using Microsoft.Crm.Services.Utility;

namespace DLaB.ModelBuilderExtensions.Entity
{
    public class RemoveEntityTypeCodeService : ICustomizeCodeDomService
    {
        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            var types = codeUnit.Namespaces[0].Types;

            foreach (var type in types.Cast<CodeTypeDeclaration>().
                                       Where(type => type.IsClass && !type.IsContextType())) {
                RemoveEntityTypeCodeField(type);
            }
        }

        private void RemoveEntityTypeCodeField(CodeTypeDeclaration type)
        {
            var field = type.Members.OfType<CodeMemberField>().FirstOrDefault(f => f.Name == "EntityTypeCode");
            if (field != null)
            {
                type.Members.Remove(field);
            }
        }
    }
}
