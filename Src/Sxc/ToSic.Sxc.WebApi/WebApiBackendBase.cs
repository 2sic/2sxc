using ToSic.Eav;
using ToSic.Eav.Logging;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.WebApi.App;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.WebApi
{
    public abstract class WebApiBackendBase<T>: HasLog<T> where T : class
    {
        protected WebApiBackendBase(string logName) : base(logName)
        {
        }

        internal AppOfRequest AppFinder => Factory.Resolve<AppOfRequest>().Init(Log);

        /// <summary>
        /// used for API calls to get the current app
        /// </summary>
        /// <returns></returns>
        internal IApp GetApp(int appId, IBlock optionalBlock) => Factory.Resolve<Apps.App>().Init(appId, Log, optionalBlock);

    }
}
