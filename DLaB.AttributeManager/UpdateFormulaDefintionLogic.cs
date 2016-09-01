using Microsoft.Xrm.Sdk.Metadata;

namespace DLaB.AttributeManager
{
    internal class UpdateFormulaDefintionLogic
    {
        internal static UpdateFormulaResponse Update(dynamic att, AttributeMetadata from, AttributeMetadata to) { return UpdateInternal(att, from, to); }
        // ReSharper disable UnusedParameter.Local
        private static UpdateFormulaResponse UpdateInternal(AttributeMetadata att, AttributeMetadata from, AttributeMetadata to) { return new UpdateFormulaResponse(); }
        // ReSharper restore UnusedParameter.Local
        private static UpdateFormulaResponse UpdateInternal(BooleanAttributeMetadata  att, AttributeMetadata from, AttributeMetadata to) { return UpdateForumlaDefinition(att, from, to); }
        private static UpdateFormulaResponse UpdateInternal(DateTimeAttributeMetadata att, AttributeMetadata from, AttributeMetadata to) { return UpdateForumlaDefinition(att, from, to); }
        private static UpdateFormulaResponse UpdateInternal(DecimalAttributeMetadata  att, AttributeMetadata from, AttributeMetadata to) { return UpdateForumlaDefinition(att, from, to); }
        private static UpdateFormulaResponse UpdateInternal(IntegerAttributeMetadata  att, AttributeMetadata from, AttributeMetadata to) { return UpdateForumlaDefinition(att, from, to); }
        private static UpdateFormulaResponse UpdateInternal(MoneyAttributeMetadata    att, AttributeMetadata from, AttributeMetadata to) { return UpdateForumlaDefinition(att, from, to); }
        private static UpdateFormulaResponse UpdateInternal(StringAttributeMetadata   att, AttributeMetadata from, AttributeMetadata to) { return UpdateForumlaDefinition(att, from, to); }

        private static UpdateFormulaResponse UpdateForumlaDefinition(dynamic att, AttributeMetadata from, AttributeMetadata to)
        {
            var response = new UpdateFormulaResponse
            {
                CurrentForumla = att.FormulaDefinition,
                NewFormula = UpdateFormula(att.FormulaDefinition, from.LogicalName, to.LogicalName)
            };
            att.FormulaDefinition = response.NewFormula;
            return response;
        }

        private static string UpdateFormula(string formula, string fromName, string toName)
        {
            return formula.Replace($"Attribute=\"{fromName}\"", $"Attribute=\"{toName}\"");
        }
    }

    internal class UpdateFormulaResponse
    {
        internal bool HasFormula => CurrentForumla != null;
        internal string CurrentForumla { get; set; }
        internal string NewFormula { get; set; }
    }
}
