using System;
using System.Globalization;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Tokens;
using ToSic.Eav.ValueProvider;

namespace ToSic.SexyContent.Engines.TokenEngine
{
    public class AppPropertyAccess : BaseValueProvider, IPropertyAccess
    {
        //private readonly DotNetNuke.Services.Tokens.TokenReplace dnnTokenReplace  = new TokenReplace(Scope.NoSettings);
        private readonly App _app;

        #region Internal stuff to be able to supply sub-properties
        private IValueProvider _settings;

        private IValueProvider Settings
        {
            get
            {
                if(_settings == null)
                    if (_app.Settings != null)
                        _settings = new DynamicEntityPropertyAccess("appsettings", _app.Settings);
                return _settings;

            }
        }

        private DynamicEntityPropertyAccess _resources;
        private IValueProvider Resources
        {
            get
            {
                if(_resources == null)
                    if (_app.Resources != null)
                        _resources = new DynamicEntityPropertyAccess("appresources", _app.Resources);
                return _resources;

            }
        }


        #endregion

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

        public override string Get(string strPropertyName, string strFormat, ref bool PropertyNotFound)
        {
            strPropertyName = strPropertyName.ToLower();
            if (strPropertyName == "path")
                return _app.Path;
            if (strPropertyName == "physicalpath")
                return _app.PhysicalPath;

            var subToken = CheckAndGetSubToken(strPropertyName);

            if (subToken.HasSubtoken)
            {
                var subProvider =
                    (subToken.Source == "settings")
                        ? Settings
                        : (subToken.Source == "resources") ? Resources : null;
                if (subProvider != null)
                    return subProvider.Get(subToken.Rest, strFormat, ref PropertyNotFound);
            }

            // Maybe someday: also retrieve metadata like Folder, Name, Version
            // var found = base.Get(strPropertyName, strFormat, ref PropertyNotFound);

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


        public override bool Has(string property)
        {
            throw new NotImplementedException();
        }

    }
}