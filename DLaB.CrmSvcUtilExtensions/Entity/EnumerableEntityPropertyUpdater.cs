using System;
using System.CodeDom;
using Microsoft.Crm.Services.Utility;

namespace DLaB.ModelBuilderExtensions.Entity
{
    public class EnumerableEntityPropertyUpdater : ICustomizeCodeDomService
    {
        #region Implementation of ICustomizeCodeDomService

        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            foreach (CodeTypeDeclaration type in codeUnit.Namespaces[0].Types)
            {
                if (!type.IsClass || type.IsContextType() || type.IsBaseEntityType()) { continue; }

                foreach (var member in type.Members)
                {
                    if (!(member is CodeMemberProperty property)
                        || !IsIEnumerableEntitySpecificProperty(property))
                    {
                        continue;
                    }

                    // Find the If Statement First
                    foreach (var get in property.GetStatements)
                    {
                        if (get is CodeConditionStatement ifStatement)
                        {
                            // Now change
                            // return System.Linq.Enumerable.Cast<Hc.Merlin.Entities.ActivityParty>(collection.Entities);
                            // To be
                            // return System.Linq.Enumerable.Select<Microsoft.Xrm.Sdk.Entity, Hc.Merlin.Entities.ActivityParty>(collection.Entities, e => e.ToEntity<Hc.Merlin.Entities.ActivityParty>());

                            ifStatement.TrueStatements.Clear();

                            ifStatement.TrueStatements.Add(
                                new CodeMethodReturnStatement(
                                    new CodeMethodInvokeExpression(
                                        new CodeMethodReferenceExpression(
                                            new CodeTypeReferenceExpression("System.Linq.Enumerable"),
                                            "Select",
                                            new CodeTypeReference(typeof(Microsoft.Xrm.Sdk.Entity)),
                                            property.Type.TypeArguments[0]
                                        ),
                                        new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("collection"), "Entities"),
                                        new CodeSnippetExpression($"e => e.ToEntity<{property.Type.TypeArguments[0].BaseType}>()")
                                    )));

                            break;
                        }
                    }
                }
            }
        }

        private static bool IsIEnumerableEntitySpecificProperty(CodeMemberProperty property)
        {
            // By default this check will work
            return property.Type.BaseType.EndsWith("IEnumerable`1")
                && property.Type.TypeArguments[0].BaseType != "Microsoft.Xrm.Sdk.Entity";
        }

        #endregion
    }
}
