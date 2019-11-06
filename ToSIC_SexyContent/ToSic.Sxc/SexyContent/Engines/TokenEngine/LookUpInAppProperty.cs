using System;
using ToSic.Eav.LookUp;

namespace ToSic.SexyContent.Engines.TokenEngine
{
    internal class LookUpInAppProperty : LookUpBase
    {
        private readonly App _app;

        #region Internal stuff to be able to supply sub-properties
        private ILookUp _settings;

        private ILookUp Settings
        {
            get
            {
                if(_settings == null)
                    if (_app.Settings != null)
                        _settings = new LookUpInDynamicEntity("appsettings", _app.Settings);
                return _settings;

            }
        }

        private LookUpInDynamicEntity _resources;
        private ILookUp Resources
        {
            get
            {
                if(_resources == null)
                    if (_app.Resources != null)
                        _resources = new LookUpInDynamicEntity("appresources", _app.Resources);
                return _resources;

            }
        }


        #endregion

        /// <summary>
		/// Constructor
		/// </summary>
        public LookUpInAppProperty(string name, App app)
        {
            Name = name; 
			_app = app;
        }

        public override string Get(string key, string strFormat, ref bool PropertyNotFound)
        {
            key = key.ToLower();
            if (key == "path")
                return _app.Path;
            if (key == "physicalpath")
                return _app.PhysicalPath;

            var subToken = CheckAndGetSubToken(key);

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
            // var found = base.Get(key, strFormat, ref PropertyNotFound);

            PropertyNotFound = true;
            return string.Empty;
        }

        ///// <summary>
        ///// Shorthand version, will return the string value or a null if not found. 
        ///// </summary>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //public virtual string Get(string key)
        //{
        //    var temp = false;
        //    return Get(key, "", ref temp);
        //}


        public override bool Has(string key)
        {
            throw new NotImplementedException();
        }

    }
}