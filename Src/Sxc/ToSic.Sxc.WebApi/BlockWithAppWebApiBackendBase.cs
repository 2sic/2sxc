using ToSic.Eav;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.WebApi.App;

namespace ToSic.Sxc.WebApi
{
    internal abstract class BlockWithAppWebApiBackendBase<T>: BlockWebApiBackendBase<T> where T: class
    {
        protected BlockWithAppWebApiBackendBase(string logName) : base(logName) { }

        internal AppOfRequest AppFinder => Factory.Resolve<AppOfRequest>().Init(Log);

        /// <summary>
        /// used for API calls to get the current app
        /// </summary>
        /// <returns></returns>
        internal IApp GetApp(int appId, IBlock optionalBlock) => Factory.Resolve<Apps.App>().Init(appId, Log, optionalBlock);

    }
}
