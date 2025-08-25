using ToSic.Razor.Blade;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Services.OutputCache;
using ToSic.Sxc.Sys.Render.PageContext;
using ToSic.Sxc.Sys.Render.PageFeatures;
using ToSic.Sxc.Web.Sys.ClientAssets;
using ToSic.Sxc.Web.Sys.ContentSecurityPolicy;
using ToSic.Sxc.Web.Sys.Html;
using ToSic.Sys.Memory;

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

namespace ToSic.Sxc.Render.Sys;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public record RenderResult : HybridHtmlString, IRenderResult, ICanEstimateSize
{
    #region HybridHtmlString / HybridHtmlRecord

    protected override string ToHtmlString()
    {
        return Html!;
    }

    /// <summary>
    /// Return a string for the recommended way in ASP.net to render it, which just uses a &lt;%= theRenderResult %&gt;
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return Html!;
    }

    #endregion

    /// <inheritdoc />
    public string? Html { get; init; }

    /// <inheritdoc />
    public bool CanCache { get; init; }

    /// <inheritdoc />
    public bool IsError { get; init; }

    /// <inheritdoc />
    public IList<IPageFeature>? Features { get; init; }

    /// <inheritdoc />
    public IList<ClientAsset>? Assets { get; init; }

    /// <inheritdoc />
    public IList<PagePropertyChange>? PageChanges { get; init; }

    /// <inheritdoc />
    public IList<HeadChange>? HeadChanges { get; init; }

    /// <inheritdoc />
    public IList<PageFeatureFromSettings>? FeaturesFromResources { get; init; }

    /// <inheritdoc />
    public int? HttpStatusCode { get; init; }

    /// <inheritdoc />
    public string? HttpStatusMessage { get; init; }

    /// <inheritdoc />
    public List<IDependentApp> DependentApps { get; } = [];


    public int ModuleId { get; init; }

    public IList<HttpHeader>? HttpHeaders { get; init; }

    public bool CspEnabled { get; init; } = false;
    public bool CspEnforced { get; init; } = false;

    /// <summary>
    /// CspParameter - for now, MUST be a real List, since it will be modified a few times
    /// </summary>
    public List<CspParameters>? CspParameters { get; init; }

    public List<string>? Errors { get; init; }

    /// <inheritdoc />
    public int AppId { get; init; }

    public OutputCacheSettings? OutputCacheSettings { get; init; }

    /// <summary>
    /// Cache information to report size etc. when needed
    /// </summary>
    SizeEstimate ICanEstimateSize.EstimateSize(ILog? log)
    {
        var l = log.Fn<SizeEstimate>();
        var estimator = new MemorySizeEstimator(log);
        try
        {
            var size = Html?.Length ?? 0;
            var known = new SizeEstimate(size, 300, true);
            if (Errors != null)
                known += estimator.Estimate(Errors);
            return l.Return(known);
        }
        catch
        {
            return l.ReturnAsError(new(Error: true));
        }
    }

    /// <summary>
    /// Determine if this is just a partial render result, meaning it should be treated differently by the cache.
    /// </summary>
    public bool IsPartial { get; init; }

    public List<string>? PartialActivateWip { get; init; }

    public List<(IHtmlTag Tag, bool NoDuplicates)>? PartialModuleTags { get; init; }
}