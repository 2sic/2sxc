namespace ToSic.Sxc.Adam.Sys.Security;

/// <summary>
/// This is a simple AdamSecurityChecks which doesn't know much about the environment but works to get started.
/// 
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class AdamSecurityChecksBasic(AdamSecurityChecksBase.Dependencies services)
    : AdamSecurityChecksBase(services, LogScopes.Base)
{
    /// <summary>
    /// Our version here just gives an ok - so that the site doesn't block this extension.
    /// Note that internally we'll still check against dangerous extensions, so this would just be an extra layer of protection,
    /// which isn't used in the basic implementation. 
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public override bool SiteAllowsExtension(string fileName) => true;

    public override bool CanEditFolder(Eav.Apps.Assets.IAsset? item)
    {
        var permissions = AdamContext.Context.Permissions;
        return permissions.IsSiteAdmin
               || permissions.IsContentAdmin
               || permissions.IsContentEditor;
    }
}