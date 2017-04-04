using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.AppEngine;
using ToSic.Eav.DataSources;
using ToSic.SexyContent.EAVExtensions;

namespace ToSic.SexyContent.DataSources
{
	[PipelineDesigner]
    public sealed class ModuleDataSource : BaseDataSource
    {
        private SxcInstance _sxcContext;

        internal SxcInstance SxcContext
        {
            get
            {
                if (_sxcContext == null)
                {
                    var provider = ConfigurationProvider as SxcValueCollectionProvider;
                    _sxcContext = provider?.SxcInstance;
                    if (provider == null)
                        throw new Exception("SxcContent is still null - can't access the SxcContent before initializing it");
                }
                return _sxcContext;
            }
        }

		private ContentGroup _contentGroup;
		private ContentGroup ContentGroup
		{
            get
            {
                if (_contentGroup == null)
                {
                    if (UseSxcInstanceContentGroup)
                        _contentGroup = SxcContext.ContentGroup;
                    else
                    {
                        if (!ModuleId.HasValue)
                            throw new Exception("Looking up ContentGroup failed because ModuleId is null.");
                        _contentGroup = new ContentGroupManager(ZoneId, AppId).GetContentGroupForModule(ModuleId.Value);
                    }
                }
                return _contentGroup;
            }
        }

        public ModuleDataSource()
        {
            Out.Add("Default", new DataStream(this, "Default", GetContent));
            Out.Add("ListContent", new DataStream(this, "Default", GetListContent));

			Configuration.Add("ModuleId", "[Module:ModuleID||[Module:ModuleId]]");	// Look for ModuleID and ModuleId
        }

        #region Cached properties for Content, Presentation etc. --> not necessary, as each stream auto-caches
        private IDictionary<int, IEntity> _content;
        private IDictionary<int, IEntity> GetContent()
        {
            return _content ??
                   (_content =
                       GetStream(ContentGroup.Content, Template.ContentDemoEntity, ContentGroup.Presentation,
                           Template.PresentationDemoEntity));
        }

	    private IDictionary<int, IEntity> _listContent;
        private IDictionary<int, IEntity> GetListContent()
        {
            return _listContent ??
                   (_listContent =
                       GetStream(ContentGroup.ListContent, Template.ListContentDemoEntity, ContentGroup.ListPresentation,
                           Template.ListPresentationDemoEntity, true));
        }

	    #endregion

        private Template _template;
		private Template Template => _template ?? (_template = OverrideTemplate ?? ContentGroup.Template);

	    private IDictionary<int, IEntity> GetStream(List<IEntity> content, IEntity contentDemoEntity, List<IEntity> presentation, IEntity presentationDemoEntity, bool isListHeader = false)
        {
			var entitiesToDeliver = new Dictionary<int, IEntity>();
            // if no template is defined, return empty list
			if (ContentGroup.Template == null && OverrideTemplate == null /*!OverrideTemplateId.HasValue*/) return entitiesToDeliver;

			var contentEntities = content.ToList(); // Create copy of list (not in cache) because it will get modified

            // If no Content Elements exist and type is content (means, presentation is not null), add an empty entity (demo entry will be taken for this)
            if (content.Count == 0 && presentation != null)
                contentEntities.Add(null);

            var originals = In["Default"].List;

            for (var i = 0; i < contentEntities.Count; i++)
            {
                // get the entity, if null: try to substitute with the demo item
                var contentEntity = contentEntities[i];

                // check if it "exists" in the in-stream. if not, then it's probably unpublished
                // so try revert back to the demo-item (assuming it exists...)
                if (contentEntity == null || !originals.ContainsKey(contentEntity.EntityId))
                    contentEntity = contentDemoEntity;

                // now check again...
                // ...we can't deliver entities that are not delivered by base (original stream), so continue
                if (contentEntity == null || !originals.ContainsKey(contentEntity.EntityId))
                    continue;

                // use demo-entites where available
                var entityId = contentEntity.EntityId;

                IEntity presentationEntity = null;

	            if (presentation != null)
	            {
		            // Try to find presentation entity
		            var presentationEntityId = (presentation.Count - 1 >= i) && presentation[i] != null && originals.ContainsKey(presentation[i].EntityId) ? presentation[i].EntityId : new int?();

		            // If there is no presentation entity, take default entity
		            if (!presentationEntityId.HasValue)
			            presentationEntityId = presentationDemoEntity != null && originals.ContainsKey(presentationDemoEntity.EntityId) ? presentationDemoEntity.EntityId : new int?();

		            presentationEntity = presentationEntityId.HasValue ? originals[presentationEntityId.Value] : null;
	            }


	            var key = entityId;

                // This ensures that if an entity is added more than once, the dictionary doesn't complain because of duplicate keys
                while (entitiesToDeliver.ContainsKey(key))
                    key += 1000000000;

				entitiesToDeliver.Add(key, new EntityInContentGroup(originals[entityId])
				{
				    SortOrder = isListHeader ? -1 : i, 
                    ContentGroupItemModified = originals[entityId].Modified, 
                    Presentation = presentationEntity, 
                    GroupId = ContentGroup.ContentGroupGuid
				});
            }

            return entitiesToDeliver;
        }

        public int? ModuleId
        {
            get
            {
                EnsureConfigurationIsLoaded();
                var listIdString = Configuration["ModuleId"];
                int listId;
                return int.TryParse(listIdString, out listId) ? listId : new int?();
            }
            set { Configuration["ModuleId"] = value.ToString(); }
        }

	    public bool UseSxcInstanceContentGroup = false;

        public Template OverrideTemplate { get; set; }
    }
}