using ToSic.Sxc.Services;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Code.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class IHasKitExtensions
{
    internal static TServiceKit GetKit<TServiceKit>(IExecutionContext exCtx) where TServiceKit : ServiceKit
        => exCtx switch
        {
            // if it has the exact kit version, return it
            IHasKit<TServiceKit> { Kit: not null } withKit => withKit.Kit,

            // Unexpected - but old fallback: just generate a new one
            ICanGetService cgs => cgs.GetService<TServiceKit>(),
            
            _ => throw new($"GetKit: {exCtx.GetType().Name} doesn't implement IHasKit or IExCtxGetKit")
        };


    internal static /*IExecutionContext*/ void SetCompatibility(this IExecutionContext codeRoot, int compatibility)
    {
        codeRoot.GetCdf().SetCompatibilityLevel(compatibility);
        //return codeRoot;
    }

}