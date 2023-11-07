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
        //private readonly LazySvc<JsonSerializer> _jsonSerializerGenerator;
        private readonly EntityPickerApi _entityPickerBackend;
        private readonly Generator<HyperlinkBackend<int, int>> _hyperlinkBackendGenerator;
        private readonly Generator<IAdamTransGetItems> _adamTransGetItems;

        public EditLoadPrefetchHelper(
            //LazySvc<JsonSerializer> jsonSerializerGenerator,
            Generator<HyperlinkBackend<int, int>> hyperlinkBackend,
            Generator<IAdamTransGetItems> adamTransGetItems,
            EntityPickerApi entityPickerBackend
            ) : base(Constants.SxcLogName + ".Prefetch")
        {
            ConnectServices(
                //_jsonSerializerGenerator = jsonSerializerGenerator,
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
            });


        private List<EntityForPickerDto> PrefetchEntities(int appId, EditDto editData)
        {
            var l = Log.Fn<List<EntityForPickerDto>>();
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
                    })
                    .ToList();

                var entities = bundlesHavingEntities.SelectMany(set
                        => set.Entity.SelectMany(e
                            => e.Value?.SelectMany(entityAttrib => entityAttrib.Value)))
                    .Where(guid => guid != null)
                    .Select(guid => guid.ToString())
                    // Step 2: Check which ones have a link reference
                    .ToArray();

                // stop here if nothing found, otherwise the backend will return all entities
                if (!entities.Any()) return l.Return(new List<EntityForPickerDto>(), "none found");

                var items = _entityPickerBackend.GetForEntityPicker(appId, entities, null, false, allowFromAllScopes: true);
                return l.Return(items, $"{items.Count}");
            }
            catch
            {
                return l.Return(new List<EntityForPickerDto>
                {
                    new EntityForPickerDto {Id = -1, Text = "Error occurred pre-fetching entities", Value = Guid.Empty}
                }, "error");
            }
        }
    }
}
