using ToSic.Eav.Logging;
using ToSic.Lib.Logging;

namespace ToSic.Eav.Core.Tests.LogTests
{
    public class LogTestBase
    {
        //public Logging.Simple.Log SL(string l) => new Logging.Simple.Log(l);
        public Log L(string l) => new Log(l);
        public LogAdapter LA(string l) => new LogAdapter(L(l));
    }
}
