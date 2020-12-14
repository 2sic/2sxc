using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.WebApi.Dto;

namespace ToSic.Sxc.WebApi.Cms
{
    public partial class EditLoadBackend
    {


        public EditPrefetchDto TryToPrefectAdditionalData(int appId, EditDto editData)
        {
            return new EditPrefetchDto
            {
                Links = PrefetchLinks(editData),
                Entities = PrefetchEntities(appId, editData)
            };
        }

        private Dictionary<string, string> PrefetchLinks(EditDto editData)
        {
            try
            {
                // Step 1: try to find hyperlinks
                var bundlesHavingLinks = editData.Items
                    // Only these with hyperlinks
                    .Where(b => b.Entity?.Attributes?.Hyperlink?.Any() ?? false)
                    .Select(b => new
                    {
                        b.Entity.Guid,
                        b.Entity.Attributes.Hyperlink
                    });

                var links = bundlesHavingLinks.SelectMany(set
                        => set.Hyperlink.SelectMany(h
                            => h.Value?.Select(linkAttrib
                                => new
                                {
                                    set.Guid,
                                    Link = linkAttrib.Value
                                })))
                    .Where(set => set != null)
                    // Step 2: Check which ones have a link reference
                    .Where(set => ValueConverterBase.CouldBeReference(set.Link))
                    .ToList();

                // Step 3: Look them up
                // Step 4: return dictionary with these
                return links.ToDictionary(
                    s => s.Link,
                    s => _valueConverter.ToValue(s.Link, s.Guid)
                );
            }
            catch
            {
                return new Dictionary<string, string>
                {
                    {"Error", "An error occurred pre-fetching the links"}
                };
            }
        }

        private List<EntityForPickerDto> PrefetchEntities(int appId, EditDto editData)
        {
            try
            {
                // Step 1: try to find hyperlinks
                var bundlesHavingEntities = editData.Items
                    // Only these with hyperlinks
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

                var backend = _entityPickerBackend.Init(Log);
                var items = backend.GetAvailableEntities(appId, entities, null, true);

                // Step 3: return
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

    }
}
