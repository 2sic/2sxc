using ToSic.Eav.Web.Http.Sys;
using ToSic.Sxc.Web.Sys.Parameters;

namespace ToSic.Sxc.Context.Sys.Page;

public static class PageHttpExtensions
{
    public static IParameters ExtractParameters(this IHttp http)
        => new Parameters { Nvc = OriginalParameters.GetOverrideParams(http.QueryStringParams) };
}