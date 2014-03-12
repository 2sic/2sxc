using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToSic.Eav.Import;

namespace ToSic.SexyContent.DataImportExport
{
    public static class EntityImportExtension
    {
        public static IEnumerable<IValueImportModel> GetAttributeValues(this Entity entity, string valueName)
        {
            return entity.Values.Where(item => item.Key == valueName).Select(item => item.Value).FirstOrDefault();
        }

        public static IValueImportModel GetAttributeValue(this Entity entity, string valueName, string valueLanguage)
        {
            var values = entity.GetAttributeValues(valueName);
            if (values == null)
            {
                return null;
            }
            return values.Where(value => value.ValueDimensions.Any(dimension => dimension.DimensionExternalKey == valueLanguage)).FirstOrDefault();
        }

        public static IValueImportModel AppendAttributeValue(this Entity entity, string valueName, string valueString, string valueType, string valueLanguage, bool valueReadOnly, bool resolveHyperlink)
        {
            var valueModel = GetValueModel(valueString, valueType, valueLanguage, valueReadOnly, resolveHyperlink, entity);
            
            var entityValue = entity.Values.Where(item => item.Key == valueName).Select(item => item.Value).FirstOrDefault();
            if (entityValue == null)
            {
                entity.Values.Add(valueName, valueModel.ToList());
            }
            else
            {
                entity.Values[valueName].Add(valueModel);
            }
            return valueModel;
        }

        private static IValueImportModel GetValueModel(string valueString, string valueType, string valueLanguages, bool valueRedOnly, bool resolveHyperlink, Entity entity)
        {
            IValueImportModel valueModel;
            switch (valueType)
            {
                case "Boolean":
                    {
                        valueModel = new ValueImportModel<bool?>(entity)
                        {
                            Value = string.IsNullOrEmpty(valueString) ? null : new Boolean?(Boolean.Parse(valueString))
                        };
                    }
                    break;

                case "Number":
                    {
                        valueModel = new ValueImportModel<decimal?>(entity)
                        {
                            Value = string.IsNullOrEmpty(valueString) ? null : new Decimal?(Decimal.Parse(valueString))
                        };
                    }
                    break;

                case "DateTime":
                    {
                        valueModel = new ValueImportModel<DateTime?>(entity)
                        {
                            Value = string.IsNullOrEmpty(valueString) ? null : new DateTime?(DateTime.Parse(valueString))
                        };
                    }
                    break;

                case "Hyperlink":
                    {
                        if (resolveHyperlink)
                        {
                            // TODO2tk: Resolve links
                            // valueString = Ask DNN for reference
                        }
                        valueModel = new ValueImportModel<string>(entity) { Value = valueString };
                    }
                    break;

                default:
                    {   // String
                        valueModel = new ValueImportModel<string>(entity) { Value = valueString };
                    }
                    break;
            }

            valueModel.AppendValueReference(valueLanguages, valueRedOnly);
            return valueModel;
        }
    }
}