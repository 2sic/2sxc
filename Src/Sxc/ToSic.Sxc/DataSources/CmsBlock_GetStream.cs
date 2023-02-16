using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Lib.Logging;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.DataSources
{
    public sealed partial class CmsBlock
    {
        private ImmutableArray<IEntity> GetStream(
            IReadOnlyCollection<IEntity> contentList, 
            IEntity contentDemoEntity, 
            IReadOnlyList<IEntity> presentationList, 
            IEntity presentationDemoEntity, 
            bool isListHeader)
        {
            Log.A($"get stream content⋮{contentList.Count}, demo#{contentDemoEntity?.EntityId}, present⋮{presentationList?.Count}, presDemo#{presentationDemoEntity?.EntityId}, header:{isListHeader}");
            try
            {
                // if no template is defined, return empty list
                if (BlockConfiguration.View == null && OverrideView == null)
                {
                    Log.A("no template definition - will return empty list");
                    return ImmutableArray<IEntity>.Empty;
                }

                // Create copy of list (not in cache) because it will get modified
                var contentEntities = contentList.ToList();

                // If no Content Elements exist and type is content (means, presentationList is not null), add an empty entity (demo entry will be taken for this)
                if (contentList.Count == 0 && presentationList != null)
                {
                    Log.A("empty list, will add a null-item");
                    contentEntities.Add(null);
                }

                var entitiesToDeliver = new List<IEntity>();
                var originals = GetInStream();
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
                            contentEntity = contentDemoEntity;
                            isDemoItem = true;  // mark demo-items for demo-item detection in template #1792
                        }

                        // now check again...
                        // ...we can't deliver entities that are not delivered by base (original stream), so continue
                        if (contentEntity == null || !originals.Has(contentEntity.EntityId))
                            continue;

                        // use demo-entities where available
                        entityId = contentEntity.EntityId;

                        var presentationEntity = GetPresentationEntity(originals, presentationList, i, /*presentationDemoEntity,*/ entityId)
                            ?? presentationDemoEntity;

                        try
                        {
                            var itm = originals.One(entityId);
                            entitiesToDeliver.Add(
                                EntityInBlockDecorator.Wrap(itm, null, null, isListHeader ? -1 : i, presentationEntity, isDemoItem)
                            //{
                            //    // 2021-10-12 2dm #dropGroupId - believe this is never used anywhere. Leave comment till EOY 2021
                            //    //#if NETFRAMEWORK
                            //    // actually unclear if this is ever used, maybe for automatic serialization?
                            //    // 2021-10-12 2dm #dropGroupId - believe this is never used anywhere. Leave comment till EOY 2021
                            //    //GroupId = BlockConfiguration.Guid,
                            //    //#endif
                            //}
                            );
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("trouble adding to output-list, id was " + entityId, ex);
                        }
                        prevIdForErrorReporting = entityId;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("problems looping items - had to stop on id " + i + "; current entity is " + entityId + "; prev is " + prevIdForErrorReporting, ex);
                }

                Log.A($"stream:{(isListHeader ? "list" : "content")} - items⋮{entitiesToDeliver.Count}");
                return entitiesToDeliver.ToImmutableArray();
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading items of a module - probably the module-id is incorrect - happens a lot with test-values on visual queries.", ex);
            }
        }

        private ImmutableList<IEntity> GetInStream()
        {
            var callLog = Log.Fn<ImmutableList<IEntity>>();
            
            // Check if in not connected, in which case we must find it yourself
            if (!In.ContainsKey(Eav.Constants.DefaultStreamName))
            {
                var showDrafts = Block?.Context?.UserMayEdit ?? false;
                Log.A($"In not attached, will auto-attach with showDrafts: {showDrafts}");
                var publishing = _services.DataSourceFactory.Value.GetPublishing(this, showDrafts, Configuration.LookUpEngine);
                Attach(publishing);
            }

            return callLog.Return(In[Eav.Constants.DefaultStreamName].List.ToImmutableList());
        }

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
                throw new Exception("trouble adding presentationList of " + entityId, ex);
            }

        }
    }
}
