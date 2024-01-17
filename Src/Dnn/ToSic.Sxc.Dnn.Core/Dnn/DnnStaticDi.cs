using Microsoft.Extensions.DependencyInjection;
using System.Web;
using ToSic.Eav.Code.InfoSystem;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Internal.Plumbing;

namespace ToSic.Sxc.Dnn;

/// <summary>
/// This is a temporary helper for Dnn 7+ to help with dependency injection which is
/// patched unto Dnn.
/// </summary>
public static class DnnStaticDi
{
    private static Func<IServiceProvider> _getGlobalDnnServiceProvider;

    public static void StaticDiReady(Func<IServiceProvider> spFunc = null) 
        => _getGlobalDnnServiceProvider = spFunc ?? throw new("Can't start Static DI for old Dnn, because the ServiceCollection is null.");

    /// <summary>
    /// This is a special internal resolver for static objects
    /// Should only be used with extreme caution, as downstream objects
    /// May need more scope-specific stuff, why may be missing
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [PrivateApi]
    [Obsolete("Avoid using at all cost - only DNN and test-code may use this!")]
    public static T StaticBuild<T>(ILog parentLog = null) => GetPageScopedServiceProvider().Build<T>(parentLog);

#pragma warning disable CS0618
    public static CodeInfoService CodeInfos => StaticBuild<CodeInfoService>();
#pragma warning restore CS0618

    /// <summary>
    /// Dictionary key for keeping the Scoped Injection Service Provider in the Http-Context
    /// </summary>
    /// <remarks>
    /// In v13 we changed key to one used in DNN9 DI instead old one "eav-scoped-serviceprovider"
    /// </remarks>
    private static readonly Type ServiceScopeKey = typeof(IServiceScope);

    public static IServiceProvider GetGlobalScopedServiceProvider() => GetGlobalServiceProvider().CreateScope().ServiceProvider;

    [PrivateApi("Very internal, to use at startup, so singletons are not lost")]
    private /*public*/ static IServiceProvider GetGlobalServiceProvider() => Sp.Get(() => _getGlobalDnnServiceProvider?.Invoke() ?? throw new("can't access global DNN service provider"));
    private static readonly GetOnce<IServiceProvider> Sp = new();

    [PrivateApi("This is just a temporary solution - shouldn't be used long term")]
    public static IServiceProvider GetPageScopedServiceProvider()
    {
        // Because 2sxc runs inside DNN as a webforms project and not asp.net core mvc, we have
        // to make sure the service-provider object is disposed correctly. If we don't do this,
        // connections to the database are kept open, and this leads to errors like "SQL timeout:
        // "All pooled connections were in use". https://github.com/2sic/2sxc/issues/1200
        // Work-around for issue https://github.com/2sic/2sxc/issues/1200
        // Scope service-provider based on request
        var httpCtx = HttpContext.Current;
        if (httpCtx == null) return GetGlobalServiceProvider().CreateScope().ServiceProvider;

        // This only runs in Dnn 7.4.2 - Dnn 9.3, because Dnn 9.4 provides this in the http context
        if (!httpCtx.Items.Contains(ServiceScopeKey))
        {
            httpCtx.Items[ServiceScopeKey] = GetGlobalServiceProvider().CreateScope();

            // Make sure service provider is disposed after request finishes
            httpCtx.AddOnRequestCompleted(context => ((IDisposable)context.Items[ServiceScopeKey])?.Dispose());
        }

        return httpCtx.Items[ServiceScopeKey] is IServiceScope scope ? scope.ServiceProvider : null;
    }

    [PrivateApi]
    public static IServiceProvider CreateModuleScopedServiceProvider()
    {
        var pageSp = GetPageScopedServiceProvider();
        var moduleSp = pageSp.CreateScope().ServiceProvider;

        // In the module scope, we initialize the scoped PageScope Accessor and give it the parent scope
        // This is necessary for it to be able to give page-scoped objects
        moduleSp.Build<PageScopeAccessor>().InitPageOfModule(pageSp);
        return moduleSp;
    }
}