using System;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.WebApi.App;

namespace ToSic.Sxc.WebApi
{
    public abstract class BlockWithAppWebApiBackendBase<T>: BlockWebApiBackendBase<T> where T: class
    {
        protected BlockWithAppWebApiBackendBase(Lazy<CmsManager> cmsManagerLazy, string logName) : base(cmsManagerLazy, logName) { }

        internal AppOfRequest AppFinder => ServiceProvider.Build<AppOfRequest>().Init(Log);

        /// <summary>
        /// used for API calls to get the current app
        /// </summary>
        /// <returns></returns>
        internal IApp GetApp(int appId, IBlock optionalBlock) => ServiceProvider.Build<Apps.App>().Init(ServiceProvider, appId, Log, optionalBlock);

    }
}
