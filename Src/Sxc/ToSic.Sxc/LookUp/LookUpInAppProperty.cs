using ToSic.Eav.LookUp;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.LookUp
{
    /// <summary>
    /// Look up things in app-settings, app-resources etc.
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
    internal class LookUpInAppProperty : LookUpBase
    {
        private readonly IApp _app;

        #region Internal stuff to be able to supply sub-properties

        private ILookUp Settings
        {
            get
            {
                if (_settings != null || _app.Settings == null) return _settings;
                var dynEnt = _app.Settings as IDynamicEntity;
                return _settings = new LookUpInEntity("appsettings", dynEnt?.Entity, dynEnt?._Dependencies.Dimensions);
            }
        }
        private ILookUp _settings;

        private ILookUp Resources
        {
            get
            {
                if (_resources != null || _app.Resources == null) return _resources;
                var dynEnt = _app.Resources as IDynamicEntity;
                return _resources = new LookUpInEntity("appresources", dynEnt?.Entity, dynEnt?._Dependencies.Dimensions);
            }
        }
        private ILookUp _resources;


        #endregion

        /// <summary>
		/// Constructor
		/// </summary>
        public LookUpInAppProperty(string name, IApp app)
        {
            Name = name; 
			_app = app;
        }

        /// <inheritdoc/>
        public override string Get(string key, string strFormat)
        {
            key = key.ToLowerInvariant();
            if (key == "path")
                return _app.Path;
            if (key == "physicalpath")
                return _app.PhysicalPath;

            var subToken = CheckAndGetSubToken(key);

            if (subToken.HasSubtoken)
            {
                var subProvider =
                    subToken.Source == "settings"
                        ? Settings
                        : subToken.Source == "resources" ? Resources : null;
                if (subProvider != null)
                    return subProvider.Get(subToken.Rest, strFormat);
            }

            // Maybe someday: also retrieve metadata like Folder, Name, Version
            // var found = base.Get(key, strFormat, ref notFound);

            return string.Empty;
        }

    }
}