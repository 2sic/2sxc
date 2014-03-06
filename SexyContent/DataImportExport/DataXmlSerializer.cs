using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using ToSic.Eav;

namespace ToSic.SexyContent.DataImportExport
{
    public class DataXmlSerializer
    {
        /// <summary>
        /// Serialize data from a 2SexyContent content-type to an XML string.
        /// </summary>
        /// <param name="applicationId">ID of the application the content-type belongs to</param>
        /// <param name="contentTypeId">ID of the content-type</param>
        /// <param name="dimensionSerializeId">ID of the dimenson to be serialized, or -1 for all dimensions</param>
        /// <param name="dimensionFallbackId">ID of the default dimension</param>
        /// <param name="languageMissingOption">How to handle missing languages</param>
        /// <param name="languageReferenceOption">How to handle language references</param>
        /// <param name="resourceReferenceOption">How th handle file / page references</param>
        /// <returns>XML string</returns>
        public string Serialize(int? applicationId, int contentTypeId, int dimensionSerializeId, int dimensionFallbackId, LanguageMissingOption languageMissingOption, LanguageReferenceOption languageReferenceOption, ResourceReferenceOption resourceReferenceOption)
        {
            var contentContext = new SexyContent(true, new int?(), applicationId).ContentContext;
            var contentType = contentContext.GetAttributeSet(contentTypeId);
            if (contentType == null)
            {   // Invalid content type specified
                return null;
            }

            var dimensionFallback = contentContext.GetDimension(dimensionFallbackId);
            var dimensionsAll = contentContext.Dimensions.Where(dim => dim.Active)
                                                         .OrderByDescending(dim => dim.DimensionID == dimensionFallbackId)
                                                         .ThenBy(dim => dim.ExternalKey)
                                                         .ToList();
            var dimensions = new List<Dimension>();
            var dimensionsSerializeAll = dimensionSerializeId < 0;
            if (dimensionsSerializeAll)
            {
                dimensions.AddRange(dimensionsAll);
            }
            else
            {
                dimensions.AddRange(dimensionsAll.Where(dim => dim.DimensionID == dimensionSerializeId));
            }

            var rootElement = new XElement(XElementName.Root);
            foreach (var entity in contentType.Entities)
            {
                foreach (var dimension in dimensions)
                {
                    var entityElement = new XElement(XElementName.Entity);
                    var attributes = contentType.GetAttributes();
                    foreach (var attribute in attributes)
                    {
                        var value = entity.GetAttributeValue(attribute, dimension, dimensionFallback);
                        if (value == null)
                        {   // No value
                            continue;
                        }

                        if (languageReferenceOption.IsResolve())
                        {   // Serialize value as clear text
                            entityElement.AddEavValueElement(attribute.StaticName, value, resourceReferenceOption);
                            continue;
                        }

                        var valueDimension = value.GetValueDimension(dimension);
                        if (valueDimension == null)
                        {
                            entityElement.AddElement(attribute.StaticName, "=ref()");
                            continue;
                        }

                        var valueReferenceDimensions = value.GetValueReferenceDimensions(dimension).OrderBy(item => dimensionsAll.IndexOf(item.Dimension)).ToList();
                        if (valueReferenceDimensions.Count() == 0)
                        {   // No reference to another dimension (may be a head value)
                            entityElement.AddEavValueElement(attribute.StaticName, value, resourceReferenceOption);
                            continue;
                        }

                        var valueReferenceDimension = default(ValueDimension);
                        if (dimensionsSerializeAll)
                        {   // Ensure that we only serialize value references when the referenced value was serialize 
                            // in clear text ones 
                            var dimensionIndex = dimensions.IndexOf(dimension);
                            valueReferenceDimension = valueReferenceDimensions.FirstOrDefault(item => 
                                {
                                    var itemDimensionIndex = dimensions.IndexOf(item.Dimension);
                                    return -1 < itemDimensionIndex && itemDimensionIndex < dimensionIndex;
                                });
                        }
                        else if (valueDimension.ReadOnly)
                        {   
                            valueReferenceDimension = valueReferenceDimensions.First();
                        }
                        else
                        {   // If only one dimension is serialized, do not serialize read-write values as references
                            valueReferenceDimension = null;
                        }
                        if (valueReferenceDimension == null)
                        {
                            entityElement.AddEavValueElement(attribute.StaticName, value, resourceReferenceOption);
                        }
                        else
                        {
                            entityElement.AddElement(attribute.StaticName, string.Format("=ref(lang={0},readonly={1})", valueReferenceDimension.Dimension.ExternalKey, valueDimension.ReadOnly));
                        }
                    }

                    if (entityElement.HasChildren() || languageMissingOption.IsCreate())
                    {
                        entityElement.AddElementFirst(XElementName.EntityGuid, entity.EntityGUID);
                        entityElement.AddElementFirst(XElementName.EntityDimension, dimension.ExternalKey);
                        rootElement.Add(entityElement);
                    }
                }
            }

            var document = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), rootElement);
            return document.ToString();
        }
    }
}