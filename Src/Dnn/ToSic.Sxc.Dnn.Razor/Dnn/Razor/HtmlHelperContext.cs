using System.Web.Hosting;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Dnn.Razor;

internal record HtmlHelperContext
{
    public RazorComponentBase Page { get; init; }
    public DnnRazorHelper Helper { get; init; }
    public bool IsSystemAdmin { get; init; }
    public IExecutionContext ExCtx => Page.ExCtx;
}



internal record HtmlHelperContextWithPaths : HtmlHelperContext
{
    public HtmlHelperContextWithPaths() { }

    public HtmlHelperContextWithPaths(HtmlHelperContext context, string relative) : base(context)
    {
        Relative = relative;
    }

    public string Relative { get; init; }

    public string Normalized => field ??= Page.NormalizePath(Relative);
    
    public string CacheKey => field ??= Normalized.ToLowerInvariant();
    
    public string FullPath => field ??= HostingEnvironment.MapPath(Normalized);
}