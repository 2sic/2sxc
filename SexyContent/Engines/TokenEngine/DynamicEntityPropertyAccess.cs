using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Services.Tokens;
using ToSic.Eav;

namespace ToSic.SexyContent.Engines.TokenEngine
{
    public class DynamicEntityPropertyAccess : IPropertyAccess, ToSic.Eav.DataSources.Tokens.IPropertyAccess
    {
        private DynamicEntity _entity;
        public string Name { get; private set; }

        public DynamicEntityPropertyAccess(DynamicEntity entity)
        {
            _entity = entity;
        }

        public CacheLevel Cacheability
        {
            get { return CacheLevel.notCacheable; }
        }

        /// <summary>
        /// Get Property out of NameValueCollection
        /// </summary>
        /// <param name="strPropertyName"></param>
        /// <param name="strFormat"></param>
        /// <param name="formatProvider"></param>
        /// <param name="AccessingUser"></param>
        /// <param name="AccessLevel"></param>
        /// <param name="PropertyNotFound"></param>
        /// <returns></returns>
        public string GetProperty(string strPropertyName, string strFormat, System.Globalization.CultureInfo formatProvider, UserInfo AccessingUser, Scope AccessLevel, ref bool PropertyNotFound)
        {
            // Return empty string if Entity is null
            if (_entity == null)
                return string.Empty;

            string outputFormat = strFormat == string.Empty ? "g" : strFormat;

            bool propertyNotFound;
            object valueObject = _entity.GetEntityValue(strPropertyName, out propertyNotFound);

            if (!propertyNotFound && valueObject != null)
            {
                switch (valueObject.GetType().Name)
                {
                    case "String":
                        return PropertyAccess.FormatString((string)valueObject, strFormat);
                    case "Boolean":
                        return (PropertyAccess.Boolean2LocalizedYesNo((bool)valueObject, formatProvider));
                    case "DateTime":
                    case "Double":
                    case "Single":
                    case "Int32":
                    case "Int64":
                    case "Decimal":
                        return (((IFormattable)valueObject).ToString(outputFormat, formatProvider));
                    default:
                        return PropertyAccess.FormatString(valueObject.ToString(), strFormat);
                }

                ////string value = PropertyAccess.GetObjectProperty(valueObject, strPropertyName, outputFormat, formatProvider, ref PropertyNotFound);
                //string value = string.IsNullOrEmpty(outputFormat) ? valueObject.ToString() : string.Format(formatProvider, outputFormat, valueObject);
                //PortalSecurity Security = new PortalSecurity();
                //value = Security.InputFilter(value, PortalSecurity.FilterFlag.NoScripting);
                ////return Security.InputFilter(PropertyAccess.FormatString(value, strFormat), PortalSecurity.FilterFlag.NoScripting);
                //return Security.InputFilter(value, PortalSecurity.FilterFlag.NoScripting);
            }
            else
            {
                PropertyNotFound = true;
                return string.Empty;
            }
        }

        public string GetProperty(string strPropertyName, ref bool PropertyNotFound)
        {
            return GetProperty(strPropertyName, "", System.Threading.Thread.CurrentThread.CurrentCulture, null, Scope.DefaultSettings, ref PropertyNotFound);
        }
    }
}