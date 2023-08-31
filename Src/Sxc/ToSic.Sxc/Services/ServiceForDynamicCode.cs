using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Services
{
    /// <summary>
    /// Internal special base class for services which link to the dynamic code root
    /// </summary>
    [PrivateApi]
    public abstract class ServiceForDynamicCode: ServiceBase, INeedsDynamicCodeRoot, IHasDynamicCodeRoot, ICanDebug
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
        public void ConnectToRoot(IDynamicCodeRoot codeRoot, ILog parentLog)
        {
            // Avoid unnecessary reconnects
            if (_alreadyConnected) return;
            _alreadyConnected = true;

            // Remember the parent
            _DynCodeRoot = codeRoot;
            // Link the logs
            this.LinkLog(parentLog ?? codeRoot?.Log);
            // report connection in log
            Log.Fn(message: "Linked to Root").Done();
        }
        private bool _alreadyConnected;

        [PrivateApi]
        public IDynamicCodeRoot _DynCodeRoot { get; private set; }

        [PrivateApi]
        public virtual bool Debug { get; set; }
    }
}
