﻿using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.WebApi.Adam
{
    public class AdamServices<TFolderId, TFileId>: MyServicesBase
    {
        public LazySvc<AdamContext<TFolderId, TFileId>> AdamState { get; }
        public IContextResolver CtxResolver { get; }
        public Generator<AdamItemDtoMaker<TFolderId, TFileId>> AdamDtoMaker { get; }

        public AdamServices(
            Generator<AdamItemDtoMaker<TFolderId, TFileId>> adamDtoMaker,
            LazySvc<AdamContext<TFolderId, TFileId>> adamState,
            IContextResolver ctxResolver)
        {
            ConnectServices(
                AdamDtoMaker = adamDtoMaker,
                AdamState = adamState,
                CtxResolver = ctxResolver
            );
        }
    }
}