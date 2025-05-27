using ToSic.Eav.LookUp;
using ToSic.Eav.LookUp.Sources;
using ToSic.Lib.LookUp;
using ToSic.Lib.LookUp.Sources;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.LookUp;

/// <summary>
/// Look up things in app-settings, app-resources etc.
/// </summary>
[InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class LookUpInAppProperty(string name, IApp app) : LookUpBase(name, "LookUp in App Properties - mainly path")
{
    #region Internal stuff to be able to supply sub-properties

    private ILookUp Settings
    {
        get
        {
            if (field != null || app.Settings == null) return field;
            var dynEnt = app.Settings as IDynamicEntity;
            return field = new LookUpInEntity("appsettings", dynEnt?.Entity, dynEnt?.Cdf.Dimensions);
        }
    }

    private ILookUp Resources
    {
        get
        {
            if (field != null || app.Resources == null) return field;
            var dynEnt = app.Resources as IDynamicEntity;
            return field = new LookUpInEntity("appresources", dynEnt?.Entity, dynEnt?.Cdf.Dimensions);
        }
    }

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

        if (!subToken.HasSubToken) return string.Empty;

        var subProvider = subToken.Source switch
        {
            "settings" => Settings,
            "resources" => Resources,
            _ => null
        };

        return subProvider?.Get(subToken.Rest, strFormat) ?? string.Empty;
    }

}