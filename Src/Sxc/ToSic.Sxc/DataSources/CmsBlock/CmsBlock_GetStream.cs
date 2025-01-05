using System.Collections.Immutable;
using ToSic.Eav.DataSource;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Data.Internal.Decorators;
using static ToSic.Eav.DataSource.Internal.DataSourceConstants;

namespace ToSic.Sxc.DataSources;

public sealed partial class CmsBlock
{
    private IImmutableList<IEntity> GetStream(
        IView view,
        IReadOnlyCollection<IEntity> items, 
        IEntity cDemoItem, 
        IReadOnlyList<IEntity> presList, 
        IEntity pDemoItem, 
        bool isListHeader
    ) => Log.Func($"content⋮{items.Count}, demo#{cDemoItem?.EntityId}, present⋮{presList?.Count}, presDemo#{pDemoItem?.EntityId}, header:{isListHeader}", l =>
    {
            
        try
        {
            // if no template is defined, return empty list
            if (view == null)
                return (EmptyList, "no template definition - empty list");

            // Create copy of list (not in cache) because it will get modified
            var contentEntities = items.ToList();

            // If no Content Elements exist and type is content (means, presentationList is not null), add an empty entity (demo entry will be taken for this)
            if (items.Count == 0 && presList != null)
            {
                l.A("empty list, will add a null-item");
                contentEntities.Add(null);
            }

            var entitiesToDeliver = new List<IEntity>();
            var originals = GetInOrAutoCreate();
            int i = 0, entityId = 0, prevIdForErrorReporting = 0;
            try
            {
                for (; i < contentEntities.Count; i++)
                {
                    // new 2019-09-18 trying to mark demo-items for better detection in output #1792
                    var isDemoItem = false;

                    // get the entity, if null: try to substitute with the demo item
                    var contentEntity = contentEntities[i];

                    // check if it "exists" in the in-stream. if not, then it's probably unpublished
                    // so try revert back to the demo-item (assuming it exists...)
                    if (contentEntity == null || !originals.Has(contentEntity.EntityId))
                    {
                        contentEntity = cDemoItem;
                        isDemoItem = true;  // mark demo-items for demo-item detection in template #1792
                    }

                    // now check again...
                    // ...we can't deliver entities that are not delivered by base (original stream), so continue
                    if (contentEntity == null || !originals.Has(contentEntity.EntityId))
                        continue;

                    // use demo-entities where available
                    entityId = contentEntity.EntityId;

                    var presentationEntity = GetPresentationEntity(originals, presList, i, entityId)
                                             ?? pDemoItem;

                    try
                    {
                        var itm = originals.One(entityId);
                        entitiesToDeliver.Add(EntityInBlockDecorator.Wrap(
                            entity: itm, field: null, index: isListHeader ? -1 : i, presentation: presentationEntity, isDemoItem: isDemoItem, parent: null));
                    }
                    catch (Exception ex)
                    {
                        throw new("trouble adding to output-list, id was " + entityId, ex);
                    }
                    prevIdForErrorReporting = entityId;
                }
            }
            catch (Exception ex)
            {
                throw new("problems looping items - had to stop on id " + i + "; current entity is " + entityId + "; prev is " + prevIdForErrorReporting, ex);
            }

            return (entitiesToDeliver.ToImmutableList(), $"stream:{(isListHeader ? "list" : "content")} - items⋮{entitiesToDeliver.Count}");
        }
        catch (Exception ex)
        {
            throw new("Error loading items of a module - probably the module-id is incorrect - happens a lot with test-values on visual queries.", ex);
        }
    });

    private ImmutableList<IEntity> GetInOrAutoCreate() => Log.Func(l =>
    {
        // Check if in not connected, in which case we must find it yourself
        if (!In.ContainsKey(StreamDefaultName))
        {
            l.A("In not attached, will auto-attach");
            var publishing = _services.DataSourceFactory.Value.CreateDefault(new DataSourceOptions(appIdentity: this, lookUp: Configuration.LookUpEngine));
            Attach(publishing);
        }

        return (In[StreamDefaultName].List.ToImmutableList(), "ok");
    });

    private static IEntity GetPresentationEntity(IReadOnlyCollection<IEntity> originals, IReadOnlyList<IEntity> presItems, int itemIndex, int entityId)
    {
        try
        {
            if (presItems == null) return null;

            // Try to find presentationList entity
            var presentationId =
                presItems.Count - 1 >= itemIndex && presItems[itemIndex] != null &&
                originals.Has(presItems[itemIndex].EntityId)
                    ? presItems[itemIndex].EntityId
                    : new int?();

            // If there is no presentationList entity, take default entity
            if (presentationId.HasValue) return originals.One(presentationId.Value);

            // 2021-10-12 2dm - believe this is an unnecessary loop to re-retrieve the demo item, will short-circuit
            return null;
            //var demoPresentationId = demo != null && originals.Has(demo.EntityId)
            //    ? demo.EntityId
            //    : new int?();

            //return demoPresentationId.HasValue ? originals.One(demoPresentationId.Value) : null;
        }
        catch (Exception ex)
        {
            throw new("trouble adding presentationList of " + entityId, ex);
        }

    }
}