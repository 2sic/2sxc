using ToSic.Eav.LookUp;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.LookUp;

/// <summary>
/// Look up things in app-settings, app-resources etc.
/// </summary>
[InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class LookUpInAppProperty(string name, IApp app) : LookUpBase(name, "LookUp in App Properties - mainly path")
{
    #region Internal stuff to be able to supply sub-properties

    private ILookUp Settings
    {
        get
        {
            if (_settings != null || app.Settings == null) return _settings;
            var dynEnt = app.Settings as IDynamicEntity;
            return _settings = new LookUpInEntity("appsettings", dynEnt?.Entity, dynEnt?.Cdf.Dimensions);
        }
    }
    private ILookUp _settings;

    private ILookUp Resources
    {
        get
        {
            if (_resources != null || app.Resources == null) return _resources;
            var dynEnt = app.Resources as IDynamicEntity;
            return _resources = new LookUpInEntity("appresources", dynEnt?.Entity, dynEnt?.Cdf.Dimensions);
        }
    }
    private ILookUp _resources;


    #endregion

    /// <inheritdoc/>
    public override string Get(string key, string strFormat)
    {
        key = key.ToLowerInvariant();
        switch (key)
        {
            case "path":
                return app.Path;
            case "physicalpath":
                return app.PhysicalPath;
            // Maybe someday: also retrieve metadata like Folder, Name, Version
        }

        var subToken = CheckAndGetSubToken(key);

        if (!subToken.HasSubtoken) return string.Empty;

        var subProvider = subToken.Source switch
        {
            "settings" => Settings,
            "resources" => Resources,
            _ => null
        };

        return subProvider?.Get(subToken.Rest, strFormat) ?? string.Empty;
    }

}