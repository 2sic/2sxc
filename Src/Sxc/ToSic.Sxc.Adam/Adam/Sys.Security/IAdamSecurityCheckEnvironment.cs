namespace ToSic.Sxc.Adam.Sys.Security;



public interface IAdamSecurityCheckEnvironment
{
    /// <summary>
    /// Our version here just gives an ok - so that the site doesn't block this extension.
    /// Note that internally we'll still check against dangerous extensions, so this would just be an extra layer of protection,
    /// which isn't used in the basic implementation. 
    /// </summary>
    /// <returns></returns>
    bool SiteAllowsExtension(IAdamSecurityCheckService security, string fileName);

    bool CanEditFolder(IAdamSecurityCheckService security, Eav.Apps.Assets.IAsset? item);
}