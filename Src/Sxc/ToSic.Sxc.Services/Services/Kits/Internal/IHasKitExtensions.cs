using ToSic.Sxc.Services;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Code.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class IHasKitExtensions
{
    public static TServiceKit GetKit<TServiceKit>(this ICodeAllApiService codeRoot) where TServiceKit : ServiceKit
        => codeRoot switch
        {
            // if it has the exact kit version, return it
            IHasKit<TServiceKit> { Kit: not null } withKit => withKit.Kit,

            // if it has a service that can provide the kit, return it
            IExCtxGetKit cas => cas.GetKit<TServiceKit>(),

            // Unexpected - but old fallback: just generate a new one
            _ => codeRoot.GetService<TServiceKit>()
        };


    internal static ICodeApiService SetCompatibility(this ICodeApiService codeRoot, int compatibility)
    {
        codeRoot.Cdf.SetCompatibilityLevel(compatibility);
        return codeRoot;
    }
    //internal static ICodeApiService SetCompatibility(this ICodeApiService codeRoot, int compatibility)
    //{
    //    codeRoot.Cdf.SetCompatibilityLevel(compatibility);
    //    return codeRoot;
    //}
}