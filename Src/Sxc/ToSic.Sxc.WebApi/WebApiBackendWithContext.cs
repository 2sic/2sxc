//using System;
//using ToSic.Eav.Logging;
//using ToSic.Sxc.Apps;
//using ToSic.Sxc.Context;


//namespace ToSic.Sxc.WebApi
//{
//    public abstract class BlockWebApiBackendWithContext<T>: WebApiBackendBase<BlockWebApiBackendWithContext<T>> where T: class
//    {
//        //private readonly Lazy<CmsManager> _cmsManagerLazy;
//        private readonly IContextResolver _ctxResolver;
//        protected IContextOfApp Context;
//        //protected CmsManager CmsManager;

//        protected BlockWebApiBackendWithContext(IServiceProvider sp, Lazy<CmsManager> cmsManagerLazy, IContextResolver ctxResolver, string logName) : base(sp, logName)
//        {
//            //_cmsManagerLazy = cmsManagerLazy;
//            _ctxResolver = ctxResolver;
//        }

//        //public T Init(string appName, IContextOfApp context, ILog parentLog)
//        //{
//        //    Log.LinkTo(parentLog);

//        //    // if app-path specified, use that app, otherwise use from context
//        //    Context = _ctxResolver.AppNameRouteBlock(appName);

//        //    //CmsManager = context.AppState == null ? null : _cmsManagerLazy.Value.Init(context.AppState, context.UserMayEdit, Log);

//        //    return this as T;
//        //}

//    }
//}
