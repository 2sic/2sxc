using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code.CodeHelpers
{
    public class CodeHelperBase : ServiceForDynamicCode
    {
        protected CodeHelperBase(string logName) : base(logName)
        {
        }

        public override void ConnectToRoot(IDynamicCodeRoot codeRoot)
        {
            // Do base work
            base.ConnectToRoot(codeRoot);
            var l = Log.Fn(message: "connected");
            // Make sure the Code-Log is reset, in case it was used before this call
            _codeLog.Reset();
            l.Done();
        }


        #region CodeLog / Html Helper

        public ICodeLog CodeLog => _codeLog.Get(() => new CodeLog(Log));
        private readonly GetOnce<ICodeLog> _codeLog = new GetOnce<ICodeLog>();

        #endregion
    }
}
