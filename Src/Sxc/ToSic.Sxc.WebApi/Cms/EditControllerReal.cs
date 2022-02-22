using System;
using System.Collections.Generic;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Formats;

namespace ToSic.Sxc.WebApi.Cms
{
    public class EditControllerReal: HasLog<EditControllerReal>
    {
        public EditControllerReal(
            LazyInitLog<EntityPickerBackend> entityBackend,
                LazyInitLog<EditLoadBackend> loadBackend,
                Lazy<EditSaveBackend> saveBackendLazy,
                LazyInitLog<HyperlinkBackend<int, int>> linkBackendLazy
            ) : base("Api.EditRl")
        {
            _entityBackend = entityBackend.SetLog(Log);
            _loadBackend = loadBackend.SetLog(Log);
            _saveBackendLazy = saveBackendLazy;
            _linkBackendLazy = linkBackendLazy.SetLog(Log);

        }
        private readonly LazyInitLog<EntityPickerBackend> _entityBackend;
        private readonly LazyInitLog<EditLoadBackend> _loadBackend;
        private readonly Lazy<EditSaveBackend> _saveBackendLazy;
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

    }
}
