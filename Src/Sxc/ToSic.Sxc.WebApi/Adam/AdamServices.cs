//using ToSic.Lib.DI;
//using ToSic.Lib.Services;
//using ToSic.Sxc.Adam;
//using ToSic.Sxc.Context;

//namespace ToSic.Sxc.WebApi.Adam
//{
//    // NOTE: This is the only MyServices which is not inside the class that actually needs it
//    // this is because the definition would result in a very long
//    public class AdamServices<TFolderId, TFileId>: MyServicesBase
//    {
//        public LazySvc<AdamContext<TFolderId, TFileId>> AdamState { get; }
//        public IContextResolver CtxResolver { get; }
//        public Generator<AdamItemDtoMaker<TFolderId, TFileId>> AdamDtoMaker { get; }

//        public AdamServices(
//            Generator<AdamItemDtoMaker<TFolderId, TFileId>> adamDtoMaker,
//            LazySvc<AdamContext<TFolderId, TFileId>> adamState,
//            IContextResolver ctxResolver)
//        {
//            ConnectServices(
//                AdamDtoMaker = adamDtoMaker,
//                AdamState = adamState,
//                CtxResolver = ctxResolver
//            );
//        }
//    }
//}