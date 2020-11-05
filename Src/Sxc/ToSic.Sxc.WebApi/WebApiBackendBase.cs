using System;
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
        public IServiceProvider ServiceProvider { get; }
        protected WebApiBackendBase(IServiceProvider serviceProvider, string logName) : base(logName)
        {
            ServiceProvider = serviceProvider;
        }

        internal AppOfRequest AppFinder => Factory.Resolve<AppOfRequest>().Init(Log);

        /// <summary>
        /// used for API calls to get the current app
        /// </summary>
        /// <returns></returns>
        internal IApp GetApp(int appId, IBlock optionalBlock) => Factory.Resolve<Apps.App>().Init(appId, Log, optionalBlock);
    }
}
