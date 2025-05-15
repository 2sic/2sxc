using ToSic.Eav.Code;
using ToSic.Sxc.Services;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Code.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class IHasKitExtensions
{
    internal static TServiceKit GetKit<TServiceKit>(object codeRoot) where TServiceKit : ServiceKit
        => codeRoot switch
        {
            // if it has the exact kit version, return it
            IHasKit<TServiceKit> { Kit: not null } withKit => withKit.Kit,

            // Unexpected - but old fallback: just generate a new one
            ICanGetService cgs => cgs.GetService<TServiceKit>(),
            
            _ => throw new($"GetKit: {codeRoot.GetType().Name} doesn't implement IHasKit or IExCtxGetKit")
        };


    internal static /*IExecutionContext*/ void SetCompatibility(this IExecutionContext codeRoot, int compatibility)
    {
        codeRoot.GetCdf().SetCompatibilityLevel(compatibility);
        //return codeRoot;
    }

}