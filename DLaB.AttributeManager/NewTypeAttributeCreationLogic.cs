using System;
using Microsoft.Xrm.Sdk.Metadata;

namespace DLaB.AttributeManager
{
    public class NewTypeAttributeCreationLogic
    {
        public static AttributeMetadata CreateText(int? maxLength = null, StringFormatName formatName = null, ImeMode? imeMode = ImeMode.Auto, string yomiOf = null, string formulaDefinition = null)
        {            
            maxLength = maxLength != null ? maxLength : (formulaDefinition != null ? 4000 : 100);

            return new StringAttributeMetadata
            {
                FormatName = formatName ?? StringFormatName.Text,
                ImeMode = imeMode,
                MaxLength = maxLength,
                YomiOf = yomiOf,
                FormulaDefinition = formulaDefinition
            };
        }

        public static AttributeMetadata CreateMemo(int? maxLength = null, ImeMode? imeMode = ImeMode.Auto)
        {
            return new MemoAttributeMetadata
            {
                ImeMode = imeMode,
                MaxLength = maxLength ?? 2000
            };
        }

        public static AttributeMetadata CreateOptionSet(OptionSetMetadata optionSet, int? defaultFormValue = null, string formulaDefinition = null)
        {
            if (optionSet.IsGlobal.GetValueOrDefault())
            {   
                // Can't send Global Option Set Options
                optionSet.Options.Clear();
            }

            return new PicklistAttributeMetadata
            {
                FormulaDefinition = formulaDefinition,
                DefaultFormValue = defaultFormValue,
                OptionSet = optionSet
            };
        }

        public static AttributeMetadata CreateTwoOptions(BooleanOptionSetMetadata optionSet, bool? defaultValue = false, string formulaDefinition = null)
        {
            return new BooleanAttributeMetadata
            {
                DefaultValue = defaultValue,
                FormulaDefinition = formulaDefinition,
                OptionSet = optionSet
            };
        }

        public static AttributeMetadata CreateImage(bool? isPrimaryImage)
        {
            return new ImageAttributeMetadata
            {
                IsPrimaryImage = isPrimaryImage,
            };
        }

        public static AttributeMetadata CreateWholeNumber(IntegerFormat? format = IntegerFormat.None, int? minValue = IntegerAttributeMetadata.MinSupportedValue, int? maxValue = IntegerAttributeMetadata.MaxSupportedValue, string formulaDefinition = null)
        {
            return new IntegerAttributeMetadata
            {
                Format = format,
                MaxValue = maxValue,
                MinValue = minValue,
                FormulaDefinition = formulaDefinition
            };
        }

        public static AttributeMetadata CreateFloatingPoint(double? minValue = 0, double? maxValue = 1000000000, int? precision = 2, ImeMode? mode = ImeMode.Auto)
        {
            if (precision < DoubleAttributeMetadata.MinSupportedPrecision || precision > DoubleAttributeMetadata.MaxSupportedPrecision)
            {
                throw new ArgumentOutOfRangeException(nameof(precision), "Precision is out of Range!");
            }

            if (minValue < DoubleAttributeMetadata.MinSupportedValue)
            {
                throw new ArgumentOutOfRangeException(nameof(minValue), "MinValue is out of Range!");
            }

            if (maxValue > DoubleAttributeMetadata.MaxSupportedValue)
            {
                throw new ArgumentOutOfRangeException(nameof(maxValue), "MaxValue is out of Range!");
            }

            return new DoubleAttributeMetadata
            {
                ImeMode = mode,
                MaxValue = maxValue,
                MinValue = minValue,
                Precision = precision
            };
        }

        public static AttributeMetadata CreateDecimal(decimal? minValue = -100000000000, decimal? maxValue = 100000000000, int? precision = 2, ImeMode? mode = ImeMode.Auto, string formulaDefinition = null)
        {
            if (precision < DecimalAttributeMetadata.MinSupportedPrecision || precision > DecimalAttributeMetadata.MaxSupportedPrecision)
            {
                throw new ArgumentOutOfRangeException(nameof(precision), "Precision is out of Range!");
            }

            if (minValue < (decimal)DecimalAttributeMetadata.MinSupportedValue)
            {
                throw new ArgumentOutOfRangeException(nameof(minValue), "MinValue is out of Range!");
            }

            if (maxValue > (decimal)DecimalAttributeMetadata.MaxSupportedValue)
            {
                throw new ArgumentOutOfRangeException(nameof(maxValue), "MaxValue is out of Range!");
            }

            return new DecimalAttributeMetadata
            {
                MaxValue = maxValue,
                MinValue = minValue,
                Precision = precision,
                ImeMode = mode,
                FormulaDefinition = formulaDefinition
            };
        }

        public static AttributeMetadata CreateCurrency(double? minValue = -922337203685477, double? maxValue = 922337203685477, int? precision = 2, int? precisionSource = null, ImeMode? mode = ImeMode.Auto, string calculationOf = null, string formulaDefinition = null)
        {
            return new MoneyAttributeMetadata
            {
                ImeMode = mode,
                MaxValue = maxValue,
                MinValue = minValue,
                Precision = precision,
                PrecisionSource = precisionSource,
                CalculationOf = calculationOf,
                FormulaDefinition = formulaDefinition
            }; 
        }

        public static AttributeMetadata CreateDateTime(DateTimeFormat? format = DateTimeFormat.DateOnly, ImeMode? mode = ImeMode.Auto, string formulaDefinition = null)
        {
            return new DateTimeAttributeMetadata
            {
                Format = format,
                ImeMode = mode,
                FormulaDefinition = formulaDefinition
            };
        }

        public static AttributeMetadata CreateLookup(string[] targets)
        {
            return new LookupAttributeMetadata
            {
                Targets = targets
            };
        }
    }
}
