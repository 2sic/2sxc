using ToSic.Eav.DI;
using ToSic.Eav.Documentation;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code;
using ToSic.Sxc.Web.PageService;

namespace ToSic.Sxc.Services
{
    /// <summary>
    /// Internal special base class for services
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice]
    // TODO: MAYBE SET PRIVATE in docs, if the remaining docs still work as expected
    public abstract class ServiceForDynamicCode: ServiceWithLogDependenciesBase, INeedsDynamicCodeRoot
    {
        [PrivateApi]
        protected ServiceForDynamicCode(string logName) : base(logName)
        {
        }

        [PrivateApi]
        public virtual void ConnectToRoot(IDynamicCodeRoot codeRoot)
        {
            (Log as Log)?.LinkTo(codeRoot?.Log);                        // Link the logs
            CodeRoot = codeRoot;                                        // Remember the parent
            Log.Fn(message: $"Linked {nameof(PageService)}").Done();    // report connection in log
        }

        [PrivateApi]
        protected IDynamicCodeRoot CodeRoot;

    }
}
