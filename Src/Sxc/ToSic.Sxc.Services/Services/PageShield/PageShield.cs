using ToSic.Sxc.Context;
using ToSic.Sxc.Services.Sys;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sxc.Sys.Render.PageContext;
using ToSic.Sxc.Web.Sys.PageServiceShared;

namespace ToSic.Sxc.Services.PageShield;

internal class PageShield(IPageServiceShared pageServiceShared)
    : ServiceWithContext("Sxc.OutCac", connect: [pageServiceShared]), IPageShield
{
    private const string AllowedUrlParameters = "AllowedUrlParameters";

    private IPageServiceSharedInternal PssInternal => (IPageServiceSharedInternal)pageServiceShared;

    public string? Allow(string urlParameters)
    {
        PssInternal.PageSpecs.AddCsv(AllowedUrlParameters, urlParameters);
        return null;
    }

    public string ParametersAllowed =>
        PssInternal.PageSpecs.Get(AllowedUrlParameters) ?? "";

    public bool ParametersAreValid => ParametersUnexpected == "";

    public string ParametersUnexpected
    {
        get
        {
            var page = ExCtxOrNull?.GetBlock().Context.Page;
            if (page == null)
                return "";
            var list = PssInternal.PageSpecs.GetUnexpected(AllowedUrlParameters, page.Parameters);
            return string.Join(",", list);
        }
    }

    public IParameters Parameters =>
        (ExCtxOrNull?.GetBlock().Context.Page.Parameters ?? new Context.Sys.Parameters())
        .Filter(ParametersAllowed);
}
