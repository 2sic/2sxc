using ToSic.Lib.Helpers;
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
            base.ConnectToRoot(codeRoot);
            // Make sure the Code-Log is reset, in case it was used before this call
            _codeLog.Reset();
        }


        #region CodeLog / Html Helper

        public ICodeLog CodeLog => _codeLog.Get(() => new CodeLog(Log));
        private readonly GetOnce<ICodeLog> _codeLog = new GetOnce<ICodeLog>();

        #endregion
    }
}
