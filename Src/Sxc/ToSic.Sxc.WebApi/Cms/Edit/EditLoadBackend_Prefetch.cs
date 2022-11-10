using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.WebApi.Adam;
using ToSic.Eav.WebApi.Dto;

namespace ToSic.Sxc.WebApi.Cms
{
    public partial class EditLoadBackend
    {

        public EditPrefetchDto TryToPrefectAdditionalData(int appId, EditDto editData)
        {
            return new EditPrefetchDto
            {
                Links = PrefetchLinks(appId, editData),
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

        private Dictionary<string, Dictionary<string, IEnumerable</*AdamItemDto*/object>>> PrefetchAdam(int appId, EditDto editData)
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
                        b.SelectMany(selector: bundle => bundle.Keys.Select(key => new
                            {
                                Key = key,
                                Dic = GetAdamListOfItems(appId, bundle.Set, key),
                            }))
                            // skip empty bits to avoid UI from relying on these nodes to always exist
                            .Where(r => r.Dic.Any())
                            // Step 2: Check which ones have a link reference
                            .ToDictionary(r => r.Key, r => r.Dic)
                        );

            return links;
        }

        private IEnumerable</*AdamItemDto*/object> GetAdamListOfItems(int appId, BundleWithLinkField set, string key)
        {
            var adamListMaker = GetService<IAdamTransGetItems>();
            adamListMaker.Init(appId, set.ContentTypeName, set.Guid, key, false, Log);
            var dic = adamListMaker.ItemsInField(string.Empty, false) as IEnumerable<AdamItemDto>;
            return dic;
        }
    }
    
}
