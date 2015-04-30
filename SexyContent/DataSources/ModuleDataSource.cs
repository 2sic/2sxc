using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.DataSources;
using ToSic.SexyContent.EAVExtensions;

namespace ToSic.SexyContent.DataSources
{
	[PipelineDesigner]
    public class ModuleDataSource : BaseDataSource
    {
        private SexyContent _sexy;

        internal SexyContent Sexy
        {
            get
            {
                if(_sexy == null)
                    _sexy = new SexyContent(In["Default"].Source.ZoneId, In["Default"].Source.AppId);
                return _sexy;
            }
            set { _sexy = value; }
        }

		private ContentGroup _contentGroup;
		private ContentGroup ContentGroup
		{
			get
			{
				if (!ModuleId.HasValue)
					throw new Exception("Looking up ContentGroup failed because ModuleId is null.");
				if (_contentGroup == null)
					_contentGroup = Sexy.ContentGroups.GetContentGroupForModule(ModuleId.Value);
				return _contentGroup;
			}
		}

        public ModuleDataSource()
        {
            Out.Add("Default", new DataStream(this, "Default", GetContent));
            Out.Add("Presentation", new DataStream(this, "Default", GetPresentation));
            Out.Add("ListContent", new DataStream(this, "Default", GetListContent));
            Out.Add("ListPresentation", new DataStream(this, "Default", GetListPresentation));

			Configuration.Add("ModuleId", "[Module:ModuleID||[Module:ModuleId]]");	// Look for ModuleID and ModuleId
        }

        private IDictionary<int, IEntity> _content;
        private IDictionary<int, IEntity> GetContent()
        {
            if (_content == null)
            {
                _content = GetStream(ContentGroup.Content, Template.ContentDemoEntity, ContentGroup.Presentation, Template.PresentationDemoEntity);
            }
            return _content;
        }

        private IDictionary<int, IEntity> _presentation;
        private IDictionary<int, IEntity> GetPresentation()
        {
            if (_presentation == null)
            {
                _presentation = GetStream(ContentGroup.Presentation, Template.PresentationDemoEntity, null, null);
            }
            return _presentation;
        }

        private IDictionary<int, IEntity> _listContent;
        private IDictionary<int, IEntity> GetListContent()
        {
            if (_listContent == null)
            {
                _listContent = GetStream(ContentGroup.ListContent, Template.ListContentDemoEntity, ContentGroup.ListPresentation, Template.ListPresentationDemoEntity);
            }
            return _listContent;
        }

        private IDictionary<int, IEntity> _listPresentation;
        private IDictionary<int, IEntity> GetListPresentation()
        {
            if (_listPresentation == null)
            {
                _listPresentation = GetStream(ContentGroup.ListPresentation, Template.ListPresentationDemoEntity, null, null);
            }
            return _listPresentation;
        }

		private Template _template;
		private Template Template
		{
			get
			{
				if (_template == null)
					_template = OverrideTemplateId.HasValue
						? Sexy.Templates.GetTemplate(OverrideTemplateId.Value)
						: ContentGroup.Template;
				return _template;
			}
		}

		private IDictionary<int, IEntity> GetStream(List<IEntity> content, IEntity contentDemoEntity, List<IEntity> presentation, IEntity presentationDemoEntity)
        {
			var entitiesToDeliver = new Dictionary<int, IEntity>();
			if (ContentGroup.Template == null && !OverrideTemplateId.HasValue) return entitiesToDeliver;

			var contentEntities = content.ToList(); // Create copy of list (not in cache) because it will get modified
			
            // If no Content Elements exist and type is content (means, presentation is not null), add an empty entity (demo entry will be taken for this)
            if (content.Count == 0 && presentation != null)
                contentEntities.Add(null);

            var originals = In["Default"].List;

            for (var i = 0; i < contentEntities.Count; i++)
            {
	            var contentEntity = contentEntities[i];

                // use demo-entites where available
				var entityId = contentEntity != null
					? contentEntity.EntityId :
					(contentDemoEntity != null ? contentDemoEntity.EntityId : new int?());

                // We can't deliver entities that are not delivered by base (original stream), so continue
                if (!entityId.HasValue || !originals.ContainsKey(entityId.Value))
                    continue;

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


	            var key = entityId.Value;

                // This ensures that if an entity is added more than once, the dictionary doesn't complain because of duplicate keys
                while (entitiesToDeliver.ContainsKey(key))
                    key += 1000000000;

				entitiesToDeliver.Add(key, new EntityInContentGroup(originals[entityId.Value])
				{
				    SortOrder = i, 
                    ContentGroupItemModified = originals[entityId.Value].Modified, 
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

        public int? OverrideTemplateId { get; set; }

    }
}