using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Services.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class IHasKitExtensions
{
    internal static TServiceKit GetKit<TServiceKit>(IExecutionContext exCtx) where TServiceKit : ServiceKit
        => exCtx switch
        {
            // if it has the exact kit version, return it
            IHasKit<TServiceKit> { Kit: not null } withKit => withKit.Kit,

            // Situation where the IHasKit would have a different version of the service kit...
            ICanGetService cgs => cgs.GetService<TServiceKit>(),
            
            _ => throw new($"GetKit: {exCtx.GetType().Name} doesn't implement IHasKit or IExCtxGetKit")
        };


    internal static void SetCompatibility(this IExecutionContext codeRoot, int compatibility)
    {
        codeRoot.GetCdf().SetCompatibilityLevel(compatibility);
    }

}