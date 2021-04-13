using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.WebApi.Adam;

namespace ToSic.Sxc.WebApi.Cms
{
    public partial class EditLoadBackend
    {

        public EditPrefetchDto TryToPrefectAdditionalData(int appId, EditDto editData)
        {
            return new EditPrefetchDto
            {
                Links = PrefetchLinks(editData),
                Entities = PrefetchEntities(appId, editData),
                Adam = PrefetchAdam(appId, editData),
            };
        }

        private List<EntityForPickerDto> PrefetchEntities(int appId, EditDto editData)
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
                if(!entities.Any()) return new List<EntityForPickerDto>();

                var backend = _entityPickerBackend.Init(Log);
                var items = backend.GetAvailableEntities(appId, entities, null, false);
                return items.ToList();
            }
            catch
            {
                return new List<EntityForPickerDto>
                {
                    new EntityForPickerDto {Id = -1, Text = "Error occurred pre-fetching entities", Value = Guid.Empty}
                };
            }
        }

        private Dictionary<string, Dictionary<string, IEnumerable<AdamItemDto>>> PrefetchAdam(int appId, EditDto editData)
        {
            // Step 1: try to find hyperlink fields
            var bundlesHavingLinks = BundleWithLinkFields(editData, true);

            var bundlesWithAllKeys = bundlesHavingLinks.Select(set =>
            {
                var keys = new List<string>();
                var hKeys = set.HyperlinkFields?.Select(h => h.Key);
                if (hKeys != null) keys.AddRange(hKeys);
                var sKeys = set.StringFields?.Select(s => s.Key);
                if (sKeys != null) keys.AddRange(sKeys);
                return new
                {
                    Set = set,
                    Keys = keys
                };
            });

            var links = bundlesWithAllKeys
                .GroupBy(b => b.Set.Guid)
                .ToDictionary(
                    b => b.Key.ToString(),
                    b =>
                        b.SelectMany(selector: bundle =>
                            {
                                var set = bundle.Set;
                                return bundle.Keys.Select(key =>
                                {
                                    var adamListMaker = ServiceProvider.Build<IAdamTransGetItems>();
                                    adamListMaker.Init(appId, set.ContentTypeName, set.Guid, key, false, Log);
                                    return new
                                    {
                                        Key = key,
                                        Dic = adamListMaker.ItemsInField(string.Empty, false) as
                                            IEnumerable<AdamItemDto>,
                                    };
                                });
                            })
                            // skip empty bits to avoid UI from relying on these nodes to always exist
                            .Where(r => r.Dic.Any())
                            // Make distinct by key - temporary disabled, as key = field and should never be duplicate
                            //.GroupBy(r => r.Key)
                            //.Select(g => g.First())
                            // Step 2: Check which ones have a link reference
                            .ToDictionary(r => r.Key, r => r.Dic)
                        );

            return links;
        }

    }
    
}
