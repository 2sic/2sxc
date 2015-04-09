using System;
using System.Globalization;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Tokens;
using ToSic.Eav.ValueProvider;

namespace ToSic.SexyContent.Engines.TokenEngine
{
    public class AppPropertyAccess : IPropertyAccess, IValueProvider
    {
        private readonly App _app;
		public string Name { get; private set; }

		/// <summary>
		/// Constructor
		/// </summary>
        public AppPropertyAccess(string name, App app)
        {
			Name = name;
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
        public string GetProperty(string strPropertyName, string strFormat, CultureInfo formatProvider, UserInfo AccessingUser, Scope AccessLevel, ref bool PropertyNotFound)
        {
            return Get(strPropertyName, strFormat, ref PropertyNotFound);
        }

        public string Get(string strPropertyName, string strFormat, ref bool PropertyNotFound)
        {
            if (strPropertyName == "Path")
                return _app.Path;
            if (strPropertyName == "PhysicalPath")
                return _app.PhysicalPath;

            PropertyNotFound = true;
            return string.Empty;
        }
        /// <summary>
        /// Shorthand version, will return the string value or a null if not found. 
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public virtual string Get(string property)
        {
            var temp = false;
            return Get(property, "", ref temp);
        }


        public bool Has(string property)
        {
            throw new NotImplementedException();
        }

    }
}