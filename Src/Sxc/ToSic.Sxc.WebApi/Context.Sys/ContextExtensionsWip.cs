using IContextGetServiceWip = ToSic.Sxc.Context.Sys.IContextCompleteService;

namespace ToSic.Sxc.Context.Sys;

/// <summary>
/// Temporary solution.
/// 
/// 1. In DNN the context which allows initializing with AppId should always happen at the entry Controller/Method
/// 2. In Oqtane, that is not implemented yet.
/// 3. Since it's not clear what services deeper down the stack might request this,
///    we still have places where explicitly broadcast it from within deeper code.
/// 4. But this can be removed, once Oqtane has been updated.
/// </summary>
/// <remarks>
/// TODO: @2dm change to not changing the state once Oqtane has been updated
/// </remarks>
public static class ContextExtensionsWip
{
//#if NETCOREAPP
    
    //public static IContextOfApp DnnGetAppStateOqtSetAndGet(this IContextGetServiceWip ctxService, int appId)
    //{
    //    // Temporary workaround - DNN should already have the context prepared in the real controller, Oqtane probably not yet
    //    return ctxService.ShareAppStateOptimized(appId);
    //}

//#else

    public static IContextOfApp DnnGetAppStateOqtSetAndGet(this IContextGetServiceWip ctxService, int appId)
    {
        // Temporary workaround - DNN should already have the context prepared in the real controller, Oqtane probably not yet
        return ctxService.AppContextOrNull ?? throw new InvalidOperationException("AppContext is null");
    }

//#endif
}
