using System;
using System.CodeDom;
using System.Linq;
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;

namespace DLaB.ModelBuilderExtensions.Entity
{
    public class RemoveEntityTypeCodeService : ICustomizeCodeDomService
    {
        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            foreach (var type in codeUnit.GetEntityTypes()) {
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
