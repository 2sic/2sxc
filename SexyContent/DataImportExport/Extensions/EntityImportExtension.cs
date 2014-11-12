using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToSic.Eav.Import;

namespace ToSic.SexyContent.DataImportExport.Extensions
{
    internal static class EntityImportExtension
    {
        /// <summary>
        /// Get values of an attribute in all languages, for example Tobi (German) and Toby (English) of 
        /// the attribute Name.
        /// </summary>
        public static IEnumerable<IValueImportModel> GetAttributeValues(this Entity entity, string valueName)
        {
            return entity.Values.Where(item => item.Key == valueName).Select(item => item.Value).FirstOrDefault();
        }

        /// <summary>
        /// Get the value of an attribute in the language specified.
        /// </summary>
        public static IValueImportModel GetAttributeValue(this Entity entity, string valueName, string valueLanguage)
        {
            var values = entity.GetAttributeValues(valueName);
            if (values == null)
            {
                return null;
            }
            return values.Where(value => value.ValueDimensions.Any(dimension => dimension.DimensionExternalKey == valueLanguage)).FirstOrDefault();
        }

        /// <summary>
        /// Add a value to the attribute specified. To do so, set the name, type and string of the value, as 
        /// well as some language properties.
        /// </summary>
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

        private static IValueImportModel GetValueModel(string valueString, string valueType, string valueLanguage, bool valueRedOnly, bool resolveHyperlink, Entity entity)
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
                        string valueReference;
                        if (string.IsNullOrEmpty(valueString))
                        {
                            valueReference = valueString;
                        }
                        else if (!resolveHyperlink)
                        {
                            valueReference = valueString;
                        }
                        else
                        {
                            valueReference = GetFileReference(valueString, valueString);
                            if (valueReference == valueString)
                            {   // Maybe it is a page and not a file
                                valueReference = GetTabReference(valueString, valueString);
                            }
                        }
                        valueModel = new ValueImportModel<string>(entity) { Value = valueReference };
                    }
                    break;

                default:
                    {   // String
                        valueModel = new ValueImportModel<string>(entity) { Value = HttpUtility.HtmlDecode(valueString) };
                    }
                    break;
            }

            valueModel.AppendLanguageReference(valueLanguage, valueRedOnly);
            return valueModel;
        }

        private static string GetFileReference(string filePath, string fallbackValue = null)
        {
            var portalInfo = PortalController.GetCurrentPortalSettings();
            var fileInfo = FileManager.Instance.GetFile(portalInfo.PortalId, filePath);
            if (fileInfo != null)
            {
                return "File:" + fileInfo.FileId;
            }
            return fallbackValue;
        }

        private static string GetTabReference(string tabPath, string fallbackValue = null)
        {
            var portalInfo = PortalController.GetCurrentPortalSettings();
            var tabController = new TabController();
            var tabCollection = tabController.GetTabsByPortal(portalInfo.PortalId);
            var tabInfo = tabCollection.Select(tab => tab.Value)
                                       .Where(tab => tab.TabPath == tabPath)
                                       .FirstOrDefault();
            if (tabInfo != null)
            {
                return "Page:" + tabInfo.TabID;
            }
            return fallbackValue;
        }
    }
}