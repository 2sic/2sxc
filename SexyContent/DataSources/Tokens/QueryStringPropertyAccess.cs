using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Services.Tokens;

namespace ToSic.SexyContent.DataSources.Tokens
{
    public class QueryStringPropertyAccess : ToSic.Eav.DataSources.Tokens.IPropertyAccess, DotNetNuke.Services.Tokens.IPropertyAccess
    {
        public string GetProperty(string propertyName, ref bool propertyNotFound)
        {
            var context = HttpContext.Current;

            if (context == null)
            {
                propertyNotFound = true;
                return null;
            }
	        var result = context.Request.QueryString[propertyName.ToLower()];
	        if (result == null)
		        propertyNotFound = true;
            return result ?? "";
        }

        public string Name { get; private set; }

        public CacheLevel Cacheability
        {
            get { return CacheLevel.notCacheable; }
        }

        public string GetProperty(string propertyName, string format, System.Globalization.CultureInfo formatProvider, DotNetNuke.Entities.Users.UserInfo accessingUser, DotNetNuke.Services.Tokens.Scope accessLevel, ref bool propertyNotFound)
        {
            return GetProperty(propertyName, ref propertyNotFound);
        }
    }
}