namespace ToSic.Sxc.Adam.Sys.Security;

/// <summary>
/// This is a simple AdamSecurityChecks which doesn't know much about the environment but works to get started.
/// 
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class AdamSecurityCheckEnvironmentBasic : IAdamSecurityCheckEnvironment
{
    /// <summary>
    /// Our version here just gives an ok - so that the site doesn't block this extension.
    /// Note that internally we'll still check against dangerous extensions, so this would just be an extra layer of protection,
    /// which isn't used in the basic implementation. 
    /// </summary>
    /// <returns></returns>
    public bool SiteAllowsExtension(IAdamSecurityCheckService security, string fileName) => true;

    public bool CanEditFolder(IAdamSecurityCheckService security, Eav.Apps.Assets.IAsset? item)
    {
        var permissions = security.AdamContext.Context.Permissions;
        return permissions.IsSiteAdmin
               || permissions.IsContentAdmin
               || permissions.IsContentEditor;
    }

}