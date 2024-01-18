using ToSic.Lib.Logging;
using ToSic.Sxc.Code;

namespace ToSic.Eav.Core.Tests.LogTests
{
    public class LogTestBase
    {
        //public Logging.Simple.Log SL(string l) => new Logging.Simple.Log(l);
        public Log L(string l) => new(l);
        internal CodeLog LA(string l) => new(L(l));
    }
}
