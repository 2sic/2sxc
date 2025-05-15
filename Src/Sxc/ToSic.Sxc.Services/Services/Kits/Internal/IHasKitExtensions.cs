using ToSic.Eav.Code;
using ToSic.Sxc.Services;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Code.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class IHasKitExtensions
{
    public static TServiceKit GetKit<TServiceKit>(this ICodeAnyApiHelper codeRoot) where TServiceKit : ServiceKit
        => GetKit<TServiceKit>(codeRoot as object);

        //=> codeRoot switch
        //{
        //    // if it has the exact kit version, return it
        //    IHasKit<TServiceKit> { Kit: not null } withKit => withKit.Kit,

        //    // if it has a service that can provide the kit, return it
        //    IExCtxGetKit cas => cas.GetKit<TServiceKit>(),

        //    // Unexpected - but old fallback: just generate a new one
        //    ICanGetService cgs => cgs.GetService<TServiceKit>(),
            
        //    _ => throw new($"GetKit: {codeRoot.GetType().Name} doesn't implement IHasKit or IExCtxGetKit")
        //};
    internal static TServiceKit GetKit<TServiceKit>(object codeRoot) where TServiceKit : ServiceKit
        => codeRoot switch
        {
            // if it has the exact kit version, return it
            IHasKit<TServiceKit> { Kit: not null } withKit => withKit.Kit,

            // if it has a service that can provide the kit, return it
            IExCtxGetKit cas => cas.GetKit<TServiceKit>(),

            // Unexpected - but old fallback: just generate a new one
            ICanGetService cgs => cgs.GetService<TServiceKit>(),
            
            _ => throw new($"GetKit: {codeRoot.GetType().Name} doesn't implement IHasKit or IExCtxGetKit")
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