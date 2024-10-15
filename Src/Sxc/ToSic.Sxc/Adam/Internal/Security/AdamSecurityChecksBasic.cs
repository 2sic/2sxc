namespace ToSic.Sxc.Adam.Internal;

/// <summary>
/// This is a simple AdamSecurityChecks which doesn't know much about the environment but works to get started.
/// 
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class AdamSecurityChecksBasic(AdamSecurityChecksBase.MyServices services)
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

    public override bool CanEditFolder(Eav.Apps.Assets.IAsset item)
        => AdamContext.Context.Permissions.IsSiteAdmin
        || AdamContext.Context.Permissions.IsContentAdmin
        || AdamContext.Context.Permissions.IsContentEditor;
}