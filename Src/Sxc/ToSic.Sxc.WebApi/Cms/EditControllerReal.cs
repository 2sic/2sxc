using System;
using System.Collections.Generic;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi.Cms;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Formats;
using ToSic.Sxc.WebApi.InPage;

namespace ToSic.Sxc.WebApi.Cms
{
    // IMPORTANT: Uses the Proxy/Real concept - see https://r.2sxc.org/proxy-controllers

    public class EditControllerReal: HasLog<EditControllerReal>, IEditController
    {
        public const string LogSuffix = "Api.CmsEdit";

        public EditControllerReal(
            LazyInitLog<EntityPickerBackend> entityBackend,
                LazyInitLog<EditLoadBackend> loadBackend,
                Lazy<EditSaveBackend> saveBackendLazy,
                LazyInitLog<HyperlinkBackend<int, int>> linkBackendLazy,
                LazyInitLog<AppViewPickerBackend> appViewPickerBackendLazy
            ) : base("Api.CmsEditRl")
        {
            _entityBackend = entityBackend.SetLog(Log);
            _loadBackend = loadBackend.SetLog(Log);
            _saveBackendLazy = saveBackendLazy;
            _linkBackendLazy = linkBackendLazy.SetLog(Log);
            _appViewPickerBackendLazy = appViewPickerBackendLazy.SetLog(Log);

        }
        private readonly LazyInitLog<EntityPickerBackend> _entityBackend;
        private readonly LazyInitLog<EditLoadBackend> _loadBackend;
        private readonly Lazy<EditSaveBackend> _saveBackendLazy;
        private readonly LazyInitLog<AppViewPickerBackend> _appViewPickerBackendLazy;
        private readonly LazyInitLog<HyperlinkBackend<int, int>> _linkBackendLazy;

        public EditDto Load(List<ItemIdentifier> items, int appId) => _loadBackend.Ready.Load(appId, items);

        public Dictionary<Guid, int> Save(EditDto package, int appId, bool partOfPage)
            => _saveBackendLazy.Value.Init(appId, Log).Save(package, partOfPage);

        public IEnumerable<EntityForPickerDto> EntityPicker(
            int appId,
            string[] items,
            string contentTypeName = null)
            => _entityBackend.Ready.GetAvailableEntities(appId, items, contentTypeName);


        public LinkInfoDto LinkInfo(string link, int appId, string contentType = default, Guid guid = default, string field = default)
            => _linkBackendLazy.Ready.LookupHyperlink(appId, link, contentType, guid, field);

        // TODO: we will need to make simpler implementation
        public bool Publish(int id)
            => _appViewPickerBackendLazy.Ready.Publish(id);
    }
}
