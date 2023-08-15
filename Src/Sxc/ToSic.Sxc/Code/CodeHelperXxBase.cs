using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Code
{
    public class CodeHelperXxBase: ServiceBase
    {
        protected CodeHelperXxBase(IDynamicCodeRoot codeRoot, bool isRazor, string codeFileName, string logName) : base(logName)
        {
            CodeRoot = codeRoot;
            this.LinkLog(codeRoot.Log);
            IsRazor = isRazor;
            CodeFileName = codeFileName;
        }
        protected readonly IDynamicCodeRoot CodeRoot;
        protected readonly bool IsRazor;
        protected readonly string CodeFileName;


        public IDevTools DevTools => _devTools.Get(() => new DevTools(IsRazor, CodeFileName, Log));
        private readonly GetOnce<IDevTools> _devTools = new GetOnce<IDevTools>();

    }
}
