using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Dto;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.WebApi.Adam;

namespace ToSic.Sxc.WebApi.Cms
{
    public partial class EditLoadPrefetchHelper: ServiceBase
    {
        private readonly EntityPickerApi _entityPickerBackend;
        private readonly Generator<HyperlinkBackend<int, int>> _hyperlinkBackendGenerator;
        private readonly Generator<IAdamTransGetItems> _adamTransGetItems;

        public EditLoadPrefetchHelper(
            Generator<HyperlinkBackend<int, int>> hyperlinkBackend,
            Generator<IAdamTransGetItems> adamTransGetItems,
            EntityPickerApi entityPickerBackend
            ) : base(Constants.SxcLogName + ".Prefetch")
        {
            ConnectServices(
                _adamTransGetItems = adamTransGetItems,
                _hyperlinkBackendGenerator = hyperlinkBackend,
                _entityPickerBackend = entityPickerBackend
            );
        }

        public EditPrefetchDto TryToPrefectAdditionalData(int appId, EditDto editData) => Log.Func(() =>
            new EditPrefetchDto
            {
                Links = PrefetchLinks(appId, editData),
                Entities = PrefetchEntities(appId, editData),
                Adam = PrefetchAdam(appId, editData),
                SettingsEntities = PrefetchSettingsEntities(),
            });


        // TODO: MAKE PRIVATE WHEN ALL HAS MOVED
        private List<EntityForPickerDto> PrefetchEntities(int appId, EditDto editData) => Log.Func(() =>
        {
            try
            {
                // Step 1: try to find entity fields
                var bundlesHavingEntities = editData.Items
                    // Only these with entity fields
                    .Where(b => b.Entity?.Attributes?.Entity?.Any() ?? false)
                    .Select(b => new
                    {
                        b.Entity.Guid,
                        b.Entity.Attributes.Entity
                    });

                var entities = bundlesHavingEntities.SelectMany(set
                        => set.Entity.SelectMany(e
                            => e.Value?.SelectMany(entityAttrib => entityAttrib.Value)))
                    .Where(guid => guid != null)
                    .Select(guid => guid.ToString())
                    // Step 2: Check which ones have a link reference
                    .ToArray();

                // stop here if nothing found, otherwise the backend will return all entities
                if (!entities.Any()) return new List<EntityForPickerDto>();

                var items = _entityPickerBackend.GetForEntityPicker(appId, entities, null, false);
                return items.ToList();
            }
            catch
            {
                return new List<EntityForPickerDto>
                {
                    new EntityForPickerDto {Id = -1, Text = "Error occurred pre-fetching entities", Value = Guid.Empty}
                };
            }
        });
    }
}
