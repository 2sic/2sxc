using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Services.Tokens;
using ToSic.Eav;

namespace ToSic.SexyContent.Engines.TokenEngine
{
    public class AppPropertyAccess : IPropertyAccess, ToSic.Eav.DataSources.Tokens.IPropertyAccess
    {
        private App _app;
        public string Name { get; private set; }

        public AppPropertyAccess(App app)
        {
            _app = app;
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
            return GetProperty(strPropertyName, ref PropertyNotFound);
        }

        public string GetProperty(string strPropertyName, ref bool PropertyNotFound)
        {
            if (strPropertyName == "Path")
                return _app.Path;
            if (strPropertyName == "PhysicalPath")
                return _app.PhysicalPath;

            PropertyNotFound = true;
            return string.Empty;
        }
    }
}