using ToSic.Eav.DI;
using ToSic.Eav.Documentation;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Services
{
    /// <summary>
    /// Internal special base class for services which link to the dynamic code root
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice]
    // TODO: MAYBE SET PRIVATE in docs, if the remaining docs still work as expected
    public abstract class ServiceForDynamicCode: ServiceWithLog, INeedsDynamicCodeRoot, IHasDynamicCodeRoot
    {
        [PrivateApi]
        protected ServiceForDynamicCode(string logName) : base(logName)
        {
        }

        [PrivateApi]
        public virtual void ConnectToRoot(IDynamicCodeRoot codeRoot) => ConnectToRoot(codeRoot, codeRoot?.Log);

        [PrivateApi]
        public virtual void ConnectToRoot(IDynamicCodeRoot codeRoot, ILog parentLog)
        {
            // Link the logs
            this.Init(parentLog);
            // Remember the parent
            _DynCodeRoot = codeRoot;
            // report connection in log
            Log.Fn(message: "Linked to Root").Done();
        }

        [PrivateApi]
        public IDynamicCodeRoot _DynCodeRoot { get; private set; }

    }
}
