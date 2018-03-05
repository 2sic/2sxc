using System.Collections.Specialized;
using System.Globalization;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Services.Tokens;
using ToSic.Eav.ValueProvider;

namespace ToSic.SexyContent.Engines.TokenEngine
{
    /// <summary>
    /// Copied from Form and List Module
    /// </summary>
    public class FilteredNameValueCollectionPropertyAccess : BaseValueProvider/*, IPropertyAccess*/
    {
	    readonly NameValueCollection _nameValueCollection;
        public FilteredNameValueCollectionPropertyAccess(string name, NameValueCollection list)
        {
            Name = name;
            _nameValueCollection = list;
        }

        //public CacheLevel Cacheability => CacheLevel.notCacheable;

            // 2018-03-05 slimming down interface, probably not used
        ///// <summary>
        ///// Get Property out of NameValueCollection
        ///// </summary>
        ///// <param name="strPropertyName"></param>
        ///// <param name="strFormat"></param>
        ///// <param name="formatProvider"></param>
        ///// <param name="AccessingUser"></param>
        ///// <param name="AccessLevel"></param>
        ///// <param name="PropertyNotFound"></param>
        ///// <returns></returns>
        //public string GetProperty(string strPropertyName, string strFormat, CultureInfo formatProvider, UserInfo AccessingUser, Scope AccessLevel, ref bool PropertyNotFound)
        //{
        //    if (_nameValueCollection == null)
        //        return string.Empty;
        //    var value = _nameValueCollection[strPropertyName];
        //    //string OutputFormat = null;
        //    //if (strFormat == string.Empty)
        //    //{
        //    //    OutputFormat = "g";
        //    //}
        //    //else
        //    //{
        //    //    OutputFormat = string.Empty;
        //    //}
        //    if (value != null)
        //    {
        //        var Security = new PortalSecurity();
        //        value = Security.InputFilter(value, PortalSecurity.FilterFlag.NoScripting);
        //        return Security.InputFilter(PropertyAccess.FormatString(value, strFormat), PortalSecurity.FilterFlag.NoScripting);
        //    }
	       // PropertyNotFound = true;
	       // return string.Empty;
        //}

        public override string Get(string property, string format, ref bool propertyNotFound)
        {
            if (_nameValueCollection == null)
                return string.Empty;
            return FormatString(_nameValueCollection[property], format);
        }

        public override bool Has(string property)
        {
            throw new System.NotImplementedException();
        }
    }
}