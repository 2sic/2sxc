using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Tokens;

namespace ToSic.SexyContent.Engines.TokenEngine
{
    public class AppPropertyAccess : IPropertyAccess, ToSic.Eav.PropertyAccess.IPropertyAccess
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
        public string GetProperty(string strPropertyName, string strFormat, System.Globalization.CultureInfo formatProvider, UserInfo AccessingUser, Scope AccessLevel, ref bool PropertyNotFound)
        {
            return GetProperty(strPropertyName, strFormat, ref PropertyNotFound);
        }

        public string GetProperty(string strPropertyName, string strFormat, ref bool PropertyNotFound)
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