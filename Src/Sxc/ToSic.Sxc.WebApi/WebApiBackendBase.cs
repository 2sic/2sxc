using System;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Apps;
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

        /// <summary>
        /// used for API calls to get the current app
        /// </summary>
        /// <returns></returns>
        internal IApp GetApp(int appId, bool showDrafts) => ServiceProvider.Build<Apps.App>().Init(ServiceProvider, appId, Log, null, showDrafts);
    }
}
