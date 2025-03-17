using System.ComponentModel;
using ToSic.Lib.Memory;
using ToSic.Sxc.Web.Internal.ClientAssets;
using ToSic.Sxc.Web.Internal.ContentSecurityPolicy;
using ToSic.Sxc.Web.Internal.PageFeatures;
using ToSic.Sxc.Web.Internal.PageService;

namespace ToSic.Sxc.Blocks.Internal.Render;

[PrivateApi]
[EditorBrowsable(EditorBrowsableState.Never)]
public record RenderResult : IRenderResult, ICanEstimateSize
{
    /// <inheritdoc />
    public string Html { get; init; }

    /// <inheritdoc />
    public bool CanCache { get; init; }

    /// <inheritdoc />
    public bool IsError { get; init; }

    /// <inheritdoc />
    public IList<IPageFeature> Features { get; init; }

    /// <inheritdoc />
    public IList<IClientAsset> Assets { get; init; }

    /// <inheritdoc />
    public IList<PagePropertyChange> PageChanges { get; init; }

    /// <inheritdoc />
    public IList<HeadChange> HeadChanges { get; init; }

    /// <inheritdoc />
    public IList<IPageFeature> FeaturesFromSettings { get; init; }

    /// <inheritdoc />
    public int? HttpStatusCode { get; init; }

    /// <inheritdoc />
    public string HttpStatusMessage { get; init; }

    /// <inheritdoc />
    public List<IDependentApp> DependentApps { get; } = [];


    public int ModuleId { get; init; }

    public IList<HttpHeader> HttpHeaders { get; init; }

    public bool CspEnabled { get; init; } = false;
    public bool CspEnforced { get; init; } = false;
    public IList<CspParameters> CspParameters { get; init; }

    public List<string> Errors { get; init; }

    /// <inheritdoc />
    public int AppId { get; init; }

    /// <summary>
    /// Cache information to report size etc. when needed
    /// </summary>
    SizeEstimate ICanEstimateSize.EstimateSize(ILog log)
    {
        var l = log.Fn<SizeEstimate>();
        var estimator = new MemorySizeEstimator(log);
        try
        {
            var size = Html?.Length ?? 0;
            var known = new SizeEstimate(size, 300, Unknown: true);
            if (Errors != null)
                known += estimator.Estimate(Errors);
            return l.Return(known);
        }
        catch
        {
            return l.ReturnAsError(new(Error: true));
        }
    }

}