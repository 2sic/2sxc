using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using ToSic.Eav;

namespace ToSic.SexyContent.DataImportExport
{
    public static class XElementExtension
    {
        public static bool HasChildren(this XElement element)
        {
            return element.Elements().Count() > 0;
        }

        public static void Append(this XElement element, XName name, object value)
        {
            element.Add(new XElement(name, value));
        }

        public static void Prepend(this XElement element, XName name, object value)
        {
            element.AddFirst(new XElement(name, value));
        }

        public static void AppendValue(this XElement element, XName name, EavValue value, ResourceReferenceExportOption resourceReferenceOption)
        {
            if (resourceReferenceOption.IsResolve())
            {
                element.Append(name, value.ResolveValueReference());
            }
            else
            {
                element.Append(name, value.Value);
            }
        }

        public static void AppendValueResolved(this XElement element, Entity entity, Attribute attribute, string language, string languageFallback, ResourceReferenceExportOption resourceReferenceOption)
        {
            var valueName = attribute.StaticName;
            var value = entity.GetAttributeValue(attribute, language, languageFallback);
            element.AppendValue(valueName, value, resourceReferenceOption);
        }

        public static void AppendValueReferenced(this XElement element, Entity entity, Attribute attribute, string language, string languageFallback, IEnumerable<string> languageScope, bool referenceParentLanguagesOnly, ResourceReferenceExportOption resourceReferenceOption)
        {
            var valueName = attribute.StaticName;
            var value = entity.GetAttributeValue(attribute, language);
            if (value == null)
            {
                element.Append(valueName, "=ref()");
                return;
            }

            var valueLanguage = value.GetLanguage(language);
            if (valueLanguage == null)
            {   // If no language is found, serialize the plain value
                element.AppendValue(valueName, value, resourceReferenceOption);
                return;
            }

            var valueLanguagesReferenced = value.GetLanguagesReferenced(language, true)
                                                .OrderBy(lang => lang != languageFallback)
                                                .ThenBy(lan => lan);;
            if (valueLanguagesReferenced.Count() == 0)
            {   // If the value is a head value, serialize the plain value
                element.AppendValue(valueName, value, resourceReferenceOption);
                return;
            }

            var valueLanguageReferenced = default(string);
            var valueLanguageReadOnly = value.IsLanguageReadOnly(language);
            if (referenceParentLanguagesOnly)
            {
                valueLanguageReferenced = valueLanguagesReferenced.FirstOrDefault
                    (
                        lang => languageScope.IndexOf(lang) < languageScope.IndexOf(language)
                    );
            }
            else if (valueLanguageReadOnly)
            {   // If one language is serialized, do not serialize read-write values 
                // as references
                valueLanguageReferenced = valueLanguagesReferenced.First();
            }

            if (valueLanguageReferenced == null)
            {
                element.AppendValue(valueName, value, resourceReferenceOption);
                return;
            }

            element.Append(valueName, string.Format("=ref(lang={0},readonly={1})", valueLanguageReferenced, valueLanguageReadOnly));
        }
    }
}