using System;
using System.Collections.Generic;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Eav.WebApi.Cms;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Formats;
using ToSic.Lib.Services;
using ToSic.Sxc.WebApi.InPage;

namespace ToSic.Sxc.WebApi.Cms
{
    // IMPORTANT: Uses the Proxy/Real concept - see https://r.2sxc.org/proxy-controllers

    public class EditControllerReal: ServiceBase, IEditController
    {
        public const string LogSuffix = "Edit";

        public EditControllerReal(
            LazySvc<EntityPickerBackend> entityBackend,
                LazySvc<EditLoadBackend> loadBackend,
                LazySvc<EditSaveBackend> saveBackendLazy,
                LazySvc<HyperlinkBackend<int, int>> linkBackendLazy,
                LazySvc<AppViewPickerBackend> appViewPickerBackendLazy
            ) : base("Api.EditRl")
        {
            ConnectServices(
                _entityBackend = entityBackend,
                _loadBackend = loadBackend,
                _saveBackendLazy = saveBackendLazy,
                _linkBackendLazy = linkBackendLazy,
                _appViewPickerBackendLazy = appViewPickerBackendLazy
            );

        }
        private readonly LazySvc<EntityPickerBackend> _entityBackend;
        private readonly LazySvc<EditLoadBackend> _loadBackend;
        private readonly LazySvc<EditSaveBackend> _saveBackendLazy;
        private readonly LazySvc<AppViewPickerBackend> _appViewPickerBackendLazy;
        private readonly LazySvc<HyperlinkBackend<int, int>> _linkBackendLazy;

        public EditDto Load(List<ItemIdentifier> items, int appId) => _loadBackend.Value.Load(appId, items);

        public Dictionary<Guid, int> Save(EditDto package, int appId, bool partOfPage)
            => _saveBackendLazy.Value.Init(appId).Save(package, partOfPage);

        public IEnumerable<EntityForPickerDto> EntityPicker(
            int appId,
            string[] items,
            string contentTypeName = null
        // 2dm 2023-01-22 #maybeSupportIncludeParentApps
            //bool? includeParentApps = null
            )
            => _entityBackend.Value.GetForEntityPicker(appId, items, contentTypeName/*, includeParentApps == true*/);


        public LinkInfoDto LinkInfo(string link, int appId, string contentType = default, Guid guid = default, string field = default)
            => _linkBackendLazy.Value.LookupHyperlink(appId, link, contentType, guid, field);

        // TODO: we will need to make simpler implementation
        public bool Publish(int id)
            => _appViewPickerBackendLazy.Value.Publish(id);
    }
}
