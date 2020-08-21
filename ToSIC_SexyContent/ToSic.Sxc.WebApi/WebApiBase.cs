using ToSic.Eav;
using ToSic.Eav.Logging;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.WebApi.App;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.WebApi
{
    public abstract class WebApiBase: HasLog
    {
        protected WebApiBase(string logName) : base(logName)
        {
        }

        internal AppOfRequest AppFinder => Factory.Resolve<AppOfRequest>().Init(Log);

        /// <summary>
        /// used for API calls to get the current app
        /// </summary>
        /// <returns></returns>
        internal IApp GetApp(int appId, IBlockBuilder optionalBuilder) => Factory.Resolve<Apps.App>().Init(appId, Log, optionalBuilder);

    }
}
