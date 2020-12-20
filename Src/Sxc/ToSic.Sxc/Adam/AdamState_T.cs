using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Adam
{
    public class AdamState<TFolderId, TFileId>: AdamState
    {
        internal AdamAppContext<TFolderId, TFileId> AdamAppContext => _adamAppContext.Value;
        private readonly Lazy<AdamAppContext<TFolderId, TFileId>> _adamAppContext;

        public AdamState(Lazy<AdamAppContext<TFolderId, TFileId>> adamAppContext, IServiceProvider serviceProvider): base(serviceProvider, "Adm.StatTT")
        {
            _adamAppContext = adamAppContext;
        }

        internal AdamOfBase<TFolderId, TFileId> ContainerContext;

        public override AdamState Init(IContextOfApp context, string contentType, string fieldName, Guid entityGuid, bool usePortalRoot, ILog parentLog)
        {
            Log.Add("PrepCore(...)");
            AdamAppContext.Init(context, 10, Log);
            ContainerContext = usePortalRoot
                ? new AdamOfSite<TFolderId, TFileId>(AdamAppContext) as AdamOfBase<TFolderId, TFileId>
                : new AdamOfField<TFolderId, TFileId>(AdamAppContext, entityGuid, fieldName);

            return base.Init(context, contentType, fieldName, entityGuid, usePortalRoot, parentLog);
        }


        // temp
        public override AppRuntime AppRuntime => AdamAppContext.AppRuntime;

    }
}
