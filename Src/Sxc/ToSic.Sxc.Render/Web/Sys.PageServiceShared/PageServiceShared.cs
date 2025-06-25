using ToSic.Sxc.Web.Internal.ContentSecurityPolicy;
using ToSic.Sxc.Web.Internal.PageFeatures;
using ToSic.Sxc.Web.PageServiceShared.Internal;
using IFeaturesService = ToSic.Sxc.Services.IFeaturesService;

namespace ToSic.Sxc.Web.Internal.PageService;

/// <summary>
/// This controller should collect what all the <see cref="ToSic.Sxc.Services.IPageService"/> objects do, for use on the final page
/// It must be scoped, so that it's the same object across the entire page-lifecycle.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public partial class PageServiceShared(IPageFeatures pageFeatures, IFeaturesService featuresService, CspOfModule csp)
    : IPageServiceShared, IPageServiceSharedInternal, IChangeQueue
{
    internal readonly IFeaturesService FeaturesService = featuresService;
    public IPageFeatures PageFeatures { get; } = pageFeatures;
    public CspOfModule Csp { get; } = csp;

    [field: AllowNull, MaybeNull]
    public string CspEphemeralMarker => field ??= new Random().Next(100000000, 999999999).ToString();

    /// <summary>
    /// How the changes given to this object should be processed.
    /// </summary>
    [PrivateApi("not final yet, will probably change")]
    public PageChangeModes ChangeMode { get; set; } = PageChangeModes.Auto;

    [PrivateApi("not final yet")]
    protected PageChangeModes GetMode(PageChangeModes modeForAuto)
    {
        switch (ChangeMode)
        {
            case PageChangeModes.Default:
            case PageChangeModes.Auto:
                return modeForAuto;
            case PageChangeModes.Replace:
            case PageChangeModes.Append:
            case PageChangeModes.Prepend:
                return ChangeMode;
            default:
                throw new ArgumentOutOfRangeException(nameof(ChangeMode), ChangeMode, null);
        }
    }

}