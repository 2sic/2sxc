using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
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

        /// <summary>
        /// Connect to CodeRoot and it's log
        /// </summary>
        /// <param name="codeRoot"></param>
        [PrivateApi]
        public virtual void ConnectToRoot(IDynamicCodeRoot codeRoot) => ConnectToRoot(codeRoot, null);

        /// <summary>
        /// Connect to CodeRoot and a custom log
        /// </summary>
        /// <param name="codeRoot"></param>
        /// <param name="parentLog"></param>
        [PrivateApi]
        public virtual void ConnectToRoot(IDynamicCodeRoot codeRoot, ILog parentLog)
        {
            // Avoid unnecessary reconnects
            if (_alreadyConnected) return;
            _alreadyConnected = true;

            // Link the logs
            this.Init(parentLog ?? codeRoot?.Log);
            // Remember the parent
            _DynCodeRoot = codeRoot;
            // report connection in log
            Log.Fn(message: "Linked to Root").Done();
        }
        private bool _alreadyConnected;

        [PrivateApi]
        public IDynamicCodeRoot _DynCodeRoot { get; private set; }

    }
}
