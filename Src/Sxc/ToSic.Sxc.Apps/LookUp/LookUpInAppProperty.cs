using ToSic.Eav.LookUp;
using ToSic.Eav.LookUp.Sources;
using ToSic.Sxc.Apps.Sys;

namespace ToSic.Sxc.LookUp;

/// <summary>
/// Look up things in app-settings, app-resources etc.
/// </summary>
[InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class LookUpInAppProperty(string name, IAppForCode app, string?[]? dimensions) : LookUpBase(name, "LookUp in App Properties - mainly path")
{
    #region Internal stuff to be able to supply sub-properties

    private ILookUp? Settings
    {
        get
        {
            if (field != null || app.SettingsEntity is not {} entity)
                return field;
            // ReSharper disable once StringLiteralTypo
            return field = new LookUpInEntity("appsettings", entity, dimensions);
        }
    }

    private ILookUp? Resources
    {
        get
        {
            if (field != null || app.ResourcesEntity is not {} entity)
                return field;
            // ReSharper disable once StringLiteralTypo
            return field = new LookUpInEntity("appresources", entity, dimensions);
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
                return app.AppPaths.Path;
            // ReSharper disable once StringLiteralTypo
            case "physicalpath":
                return app.AppPaths.PhysicalPath;
            // Maybe someday: also retrieve metadata like Folder, Name, Version
        }

        var subToken = LookUpHelpers.CheckAndGetSubToken(key);

        if (!subToken.HasSubToken)
            return string.Empty;

        var subProvider = subToken.Source switch
        {
            "settings" => Settings,
            "resources" => Resources,
            _ => null
        };

        return subProvider?.Get(subToken.Rest ?? string.Empty, strFormat) ?? string.Empty;
    }

}