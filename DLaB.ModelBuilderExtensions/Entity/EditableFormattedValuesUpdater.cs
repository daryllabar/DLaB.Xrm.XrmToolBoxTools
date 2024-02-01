using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using System;
using System.CodeDom;

namespace DLaB.ModelBuilderExtensions.Entity
{
    /*
     * Analyzes entity properties, and check to see if the get is a Formatted Value Property, and the Set is a SetAttribute

    public string CreatedByName
    {
        [System.Diagnostics.DebuggerNonUserCode()]
        get
        {
            if (this.FormattedValues.Contains("createdby"))
            {
                return this.FormattedValues["createdby"];
            }
            else
            {
                return default(string);
            }
        }
        [System.Diagnostics.DebuggerNonUserCode()]
        set
        {
            this.SetAttributeValue("createdbyname", value);
        }
    }

    * which should have it's set updated to:
    
        [System.Diagnostics.DebuggerNonUserCode()]
        set
        {
           this.FormattedValues["creatdby"] = value;
        }
 */

    public class EditableFormattedValuesUpdater : ICustomizeCodeDomService
    {
        #region Implementation of ICustomizeCodeDomService

        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            foreach (var type in codeUnit.GetEntityTypes())
            {
                foreach (var member in type.Members)
                {
                    if (!(member is CodeMemberProperty property)
                        || !IsSetFunctionACallToSetAttributeValue(property))
                    {
                        continue;
                    }
                    var logicalName = GetAttributeNameForFormattedValuesIndexer(property);
                    if (string.IsNullOrWhiteSpace(logicalName))
                    {
                        continue;
                    }

                    // Convert this:
                    // this.SetAttributeValue("createdbyname", value);
                    // to this:
                    // this.FormattedValues["creatdby"] = value;

                    // Assign the value to the indexed property
                    property.SetStatements[0] = new CodeAssignStatement( // =
                        new CodeIndexerExpression( // [<>]
                            new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "FormattedValues"), // this.FormattedValues
                            new CodePrimitiveExpression(logicalName) // Key ("createdby" in this case)
                        ),
                        new CodePropertySetValueReferenceExpression() // value
                    );
                }
            }
        }

        private static bool IsSetFunctionACallToSetAttributeValue(CodeMemberProperty property)
        {
            return property.HasSet
                && property.SetStatements.Count == 1
                && property.SetStatements[0] is CodeExpressionStatement setStatement
                && setStatement.Expression is CodeMethodInvokeExpression setValue
                && setValue.Method.MethodName == "SetAttributeValue";
        }

        private static string GetAttributeNameForFormattedValuesIndexer(CodeMemberProperty property)
        {
            if (property.HasGet
                   && property.GetStatements.Count == 1
                   && property.GetStatements[0] is CodeConditionStatement getStatement
                   && getStatement.TrueStatements.Count == 1
                   && getStatement.TrueStatements[0] is CodeMethodReturnStatement getValue
                   && getValue.Expression is CodeIndexerExpression indexer
                   && indexer.TargetObject is CodePropertyReferenceExpression target
                   && target.PropertyName == "FormattedValues"
                   && indexer.Indices.Count == 1
                   && indexer.Indices[0] is CodePrimitiveExpression nameExpression)
            {
                return nameExpression.Value.ToString();
            }
            return null;
        }

        #endregion
    }
}
