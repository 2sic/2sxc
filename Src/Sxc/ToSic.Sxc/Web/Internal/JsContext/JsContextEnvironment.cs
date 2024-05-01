using ToSic.Eav;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Web.Parameters;

namespace ToSic.Sxc.Web.Internal.JsContext;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class JsContextEnvironment(string systemRootUrl, IContextOfBlock ctx)
{
    public int WebsiteId { get; } = ctx.Site.Id;
    public string WebsiteUrl { get; } = "//" + ctx.Site.UrlRoot + "/";
    public int PageId { get; } = ctx.Page.Id;
    public string PageUrl { get; } = ctx.Page.Url;

    // ReSharper disable once InconsistentNaming
#pragma warning disable IDE1006
    public IEnumerable<KeyValuePair<string, string>> parameters { get; } = ctx.Page.Parameters?.Where(p => p.Key != OriginalParameters.NameInUrlForOriginalParameters);
#pragma warning restore IDE1006

    public int InstanceId { get; } = ctx.Module.Id;

    public string SxcVersion { get; } = EavSystemInfo.VersionWithStartUpBuild;

    public string SxcRootUrl { get; } = systemRootUrl;

    public bool IsEditable { get; } = ctx.Permissions.IsContentAdmin;
}