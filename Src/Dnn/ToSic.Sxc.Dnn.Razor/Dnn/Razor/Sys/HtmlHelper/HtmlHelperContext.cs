using System.Diagnostics.CodeAnalysis;
using System.Web.Hosting;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Dnn.Razor.Sys;

internal record HtmlHelperContext
{
    public required RazorComponentBase Page { get; init; }
    public required DnnRazorHelper Helper { get; init; }
    public required bool IsSystemAdmin { get; init; }
    public IExecutionContext ExCtx => Page.ExCtx;
}



internal record HtmlHelperContextWithPaths : HtmlHelperContext
{

    [SetsRequiredMembers]
    public HtmlHelperContextWithPaths(HtmlHelperContext context, string relative)
        : base(context)
    {
        Relative = relative;
    }

    public required string Relative { get; init; }

    public string Normalized => field ??= Page.NormalizePath(Relative);
    
    public string CacheKey => field ??= Normalized.ToLowerInvariant();
    
    public string FullPath => field ??= HostingEnvironment.MapPath(Normalized);
}