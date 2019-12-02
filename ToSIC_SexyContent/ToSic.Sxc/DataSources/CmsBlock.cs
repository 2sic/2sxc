//using DotNetNuke.Entities.Modules;

using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Query;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Data;
using ToSic.Sxc.LookUp;
using ICmsBlock = ToSic.Sxc.Blocks.ICmsBlock;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// This data-source delivers the core data for a CMS Block. <br/>
    /// It will look up the configuration in the CMS (like the Module-Settings in DNN) to determine what data is needed for the block. <br/>
    /// Usually it will then find a reference to a ContentBlock, from which it determines what content-items are assigned. <br/>
    /// It could also find that the template specifies a query, in which case it would retrieve that. <br/>
    /// <em>Was previously called ModuleDataSource</em>
    /// </summary>
    [PublicApi]
    [VisualQuery(
        GlobalName = "ToSic.Sxc.DataSources.CmsBlock, ToSic.SexyContent",
        Type = DataSourceType.Source, 
        ExpectsDataOfType = "7c2b2bc2-68c6-4bc3-ba18-6e6b5176ba02",
        HelpLink = "https://github.com/2sic/2sxc/wiki/DotNet-DataSource-ModuleDataSource",
        PreviousNames = new []{ "ToSic.SexyContent.DataSources.ModuleDataSource, ToSic.SexyContent" })]
    public sealed class CmsBlock : DataSourceBase
    {
        /// <inheritdoc />
        public override string LogId => "Sxc.CmsBDs";

        private ICmsBlock _cmsContext;

        [PrivateApi]
        public enum Settings
        {
            InstanceId
        }

        /// <summary>
        /// The instance-id of the CmsBlock (2sxc instance, DNN ModuleId). <br/>
        /// It's named Instance-Id to be more neutral as we're opening it to other platforms
        /// </summary>
        public int? InstanceId
        {
            get
            {
                EnsureConfigurationIsLoaded();
                var listIdString = Configuration["ModuleId"];
                return int.TryParse(listIdString, out var listId) ? listId : new int?();
            }
            set => Configuration["ModuleId"] = value.ToString();
        }

        [PrivateApi]
        internal ICmsBlock CmsInstance
        {
            get
            {
                if (_cmsContext != null) return _cmsContext;

                if(!HasSxcContext)
                    throw new Exception("value provider didn't have sxc provider - can't use module data source");

                var sxciProvider = ConfigurationProvider.Sources[LookUp.ConfigurationProvider.SxcInstanceKey];
                _cmsContext = (sxciProvider as LookUpCmsBlock)?
                              .CmsInstance 
                              ?? throw new Exception("value provider didn't have sxc provider - can't use module data source");

                return _cmsContext;
            }
        }

        [PrivateApi]
        internal bool HasSxcContext => ConfigurationProvider.Sources.ContainsKey(LookUp.ConfigurationProvider.SxcInstanceKey);

		private BlockConfiguration _blockConfiguration;
		private BlockConfiguration BlockConfiguration
		{
            get
            {
                if (_blockConfiguration != null) return _blockConfiguration;

                if (UseSxcInstanceContentGroup)
                {
                    Log.Add("need content-group, will use from sxc-context");
                    _blockConfiguration = CmsInstance.Block.Configuration;
                }
                else
                {
                    Log.Add("need content-group, will construct as cannot use context");
                    if (!InstanceId.HasValue)
                        throw new Exception("Looking up BlockConfiguration failed because ModuleId is null.");
                    var publish = Factory.Resolve<IEnvironmentFactory>().PagePublisher(Log);
                    var userMayEdit = HasSxcContext && CmsInstance.UserMayEdit;

                    var cms = new CmsRuntime(ZoneId, AppId, Log, HasSxcContext && userMayEdit, publish.IsEnabled(InstanceId.Value));
                    var cgm = cms.Blocks;

                    _blockConfiguration = cgm.GetInstanceContentGroup(InstanceId.Value, null);
                }
                return _blockConfiguration;
            }
        }

        public CmsBlock()
        {
            Out.Add(Eav.Constants.DefaultStreamName, new DataStream(this, Eav.Constants.DefaultStreamName, GetContent));
            Out.Add(ViewParts.ListContent, new DataStream(this, Eav.Constants.DefaultStreamName, GetListContent));

			Configuration.Add("ModuleId", $"[Settings:{Settings.InstanceId}||[Module:ModuleId]]");
        }

        #region Cached properties for Content, Presentation etc. --> not necessary, as each stream auto-caches
        private IEnumerable<IEntity> _content;

        private IEnumerable<IEntity> GetContent()
            => _content ?? (_content = GetStream(BlockConfiguration.Content, View.ContentItem,
                   BlockConfiguration.Presentation, View.PresentationItem, false));

        private IEnumerable<IEntity> _listContent;

        private IEnumerable<IEntity> GetListContent()
            => _listContent ?? (_listContent = GetStream(BlockConfiguration.Header, View.HeaderItem,
                   BlockConfiguration.HeaderPresentation, View.HeaderPresentationItem, true));

        #endregion

        private IView _view;
		private IView View => _view ?? (_view = OverrideView ?? BlockConfiguration.View);

	    private IEnumerable<IEntity> GetStream(List<IEntity> contentList, IEntity contentDemoEntity, List<IEntity> presentationList, IEntity presentationDemoEntity, bool isListHeader)
	    {
	        Log.Add($"get stream content⋮{contentList.Count}, demo#{contentDemoEntity?.EntityId}, present⋮{presentationList?.Count}, presDemo#{presentationDemoEntity?.EntityId}, header:{isListHeader}");
            try
            {
                var entitiesToDeliver = new List<IEntity>();
                // if no template is defined, return empty list
                if (BlockConfiguration.View == null && OverrideView == null)
                {
                    Log.Add("no template definition - will return empty list");
                    return entitiesToDeliver;
                }

                // Create copy of list (not in cache) because it will get modified
                var contentEntities = contentList.ToList(); 

                // If no Content Elements exist and type is content (means, presentationList is not null), add an empty entity (demo entry will be taken for this)
                if (contentList.Count == 0 && presentationList != null)
                {
                    Log.Add("empty list, will add a null-item");
                    contentEntities.Add(null);
                }

                IEnumerable<IEntity> originals = In[Eav.Constants.DefaultStreamName].List.ToList();
                int i = 0, entityId = 0, prevIdForErrorReporting = 0;
                try
                {
                    for (; i < contentEntities.Count; i++)
                    {
                        // new 2019-09-18 trying to mark demo-items for better detection in output #1792
                        var usingDemoItem = false;

                        // get the entity, if null: try to substitute with the demo item
                        var contentEntity = contentEntities[i];

                        // check if it "exists" in the in-stream. if not, then it's probably unpublished
                        // so try revert back to the demo-item (assuming it exists...)
                        if (contentEntity == null || !originals.Has(contentEntity.EntityId))
                        {
                            contentEntity = contentDemoEntity;
                            // new 2019-09-18 trying to mark demo-items for better detection in output #1792
                            usingDemoItem = true; 
                        }

                        // now check again...
                        // ...we can't deliver entities that are not delivered by base (original stream), so continue
                        if (contentEntity == null || !originals.Has(contentEntity.EntityId))
                            continue;

                        // use demo-entites where available
                        entityId = contentEntity.EntityId;

                        IEntity presentationEntity = null;
                        try
                        {
                            if (presentationList != null)
                            {
                                // Try to find presentationList entity
                                var presentationEntityId =
                                    presentationList.Count - 1 >= i && presentationList[i] != null &&
                                    originals.Has(presentationList[i].EntityId)
                                        ? presentationList[i].EntityId
                                        : new int?();

                                // If there is no presentationList entity, take default entity
                                if (!presentationEntityId.HasValue)
                                    presentationEntityId =
                                        presentationDemoEntity != null &&
                                        originals.Has(presentationDemoEntity.EntityId)
                                            ? presentationDemoEntity.EntityId
                                            : new int?();

                                presentationEntity =
                                    presentationEntityId.HasValue ? originals.One(presentationEntityId.Value) : null;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("trouble adding presentationList of " + entityId, ex);
                        }

                        try
                        {
                            var itm = originals.One(entityId);
                            entitiesToDeliver.Add(new EntityInContentGroup(itm)
                            {
                                SortOrder = isListHeader ? -1 : i,
                                ContentGroupItemModified = itm.Modified,
                                Presentation = presentationEntity,
                                GroupId = BlockConfiguration.ContentGroupGuid,
                                // new 2019-09-18 trying to mark demo-items for better detection in output #1792
                                IsDemoItem = usingDemoItem
                            });
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

                Log.Add($"stream:{(isListHeader ? "list" : "content")} - items⋮{entitiesToDeliver.Count}");
                return entitiesToDeliver;
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading items of a module - probably the module-id is incorrect - happens a lot with test-values on visual queries.", ex);
            }
        }




        internal bool UseSxcInstanceContentGroup = false;

        /// <summary>
        /// This allows external settings to override the view given by the configuration. This is used to temporarily use an alternate view.
        /// For example, when previewing a different template. 
        /// </summary>
        public IView OverrideView { get; set; }


        #region obsolete stuff
        [Obsolete("became obsolete in 2sxc 9.9, use InstanceId instead")]
        [PrivateApi]
        public int? ModuleId
        {
            get => InstanceId;
            set => InstanceId = value;
        }
        #endregion
    }
}