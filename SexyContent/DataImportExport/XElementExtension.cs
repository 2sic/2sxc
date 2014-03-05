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

        public static void AddAttribute(this XElement element, XName name, object value)
        {
            if (value == null)
            {
                value = "";
            }
            element.Add(new XAttribute(name, value));
        }

        public static void AddElement(this XElement element, XName name, object value)
        {
            element.Add(new XElement(name, value));
        }

        public static void AddElementFirst(this XElement element, XName name, object value)
        {
            element.AddFirst(new XElement(name, value));
        }

        /// <summary>
        /// Add content of an EAV value as a child of this element.
        /// </summary>
        public static void AddEavValueElement(this XElement element, XName name, EavValue value, ResourceReferenceOption resourceReferenceOption)
        {
            if (resourceReferenceOption.IsResolve())
            {
                element.AddElement(name, value.ResolveValueReference());
            }
            else
            {
                element.AddElement(name, value.Value);
            }
        }
    }
}