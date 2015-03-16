using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.DataSources;

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

        private List<ContentGroupItem> _contentGroupItems;
        private IEnumerable<ContentGroupItem> ContentGroupItems {
            get
            {
                if (!ListId.HasValue)
                    return new List<ContentGroupItem>();

                if (_contentGroupItems == null)
                    _contentGroupItems = Sexy.TemplateContext.GetContentGroupItems(ListId.Value).ToList();
                return _contentGroupItems;
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
                _content = GetStream(ContentGroupItemType.Content, ContentGroupItemType.Presentation);
            }
            return _content;
        }

        private IDictionary<int, IEntity> _presentation;
        private IDictionary<int, IEntity> GetPresentation()
        {
            if (_presentation == null)
            {
                _presentation = GetStream(ContentGroupItemType.Presentation, null);
            }
            return _presentation;
        }

        private IDictionary<int, IEntity> _listContent;
        private IDictionary<int, IEntity> GetListContent()
        {
            if (_listContent == null)
            {
                _listContent = GetStream(ContentGroupItemType.ListContent, ContentGroupItemType.ListPresentation);
            }
            return _listContent;
        }

        private IDictionary<int, IEntity> _listPresentation;
        private IDictionary<int, IEntity> GetListPresentation()
        {
            if (_listPresentation == null)
            {
                _listPresentation = GetStream(ContentGroupItemType.ListPresentation, null);
            }
            return _listPresentation;
        }

		private IDictionary<int, IEntity> GetStream(ContentGroupItemType itemType, ContentGroupItemType? presentationItemType)
        {
			var entitiesToDeliver = new Dictionary<int, IEntity>();
			if (!ContentGroupItems.Any(c => c.TemplateID.HasValue) && !OverrideTemplateId.HasValue) return entitiesToDeliver;

            var items = ContentGroupItems.ToList(); // Create copy of list (not in cache) because it will get modified
			var templateId = OverrideTemplateId.HasValue ? OverrideTemplateId.Value : items.First().TemplateID.Value;
			var templateDefaults = Sexy.GetTemplateDefaults(templateId);
			var templateDefault = templateDefaults.FirstOrDefault(t => t.ItemType == itemType);

            // If no Content Elements exist and type is List, add a ContentGroupItem to List (not to DB)
            if ((itemType == ContentGroupItemType.Content || itemType == ContentGroupItemType.ListContent) && !items.Any(p => p.ItemType == itemType))
            {
				if (!ListId.HasValue)
					throw new Exception("GetStream() failed because ListId is null. ModuleId is " + (ModuleId.HasValue ? ModuleId.ToString() : "null"));

                items.Add(new ContentGroupItem
                {
                    ContentGroupID = ListId.Value,
                    ContentGroupItemID = -1,
                    EntityID = new int?(),
                    SortOrder = (itemType == ContentGroupItemType.ListContent ? -1 : 0),
                    SysCreated = DateTime.Now,
                    SysCreatedBy = -1,
                    TemplateID = templateId,
                    Type = itemType.ToString("F")
                });
            }

            var originals = In["Default"].List;

            foreach (var i in items.Where(p => p.ItemType == itemType))
            {
                // use demo-entites where available
                var entityId = i.EntityID.HasValue
                    ? i.EntityID.Value
                    : (templateDefault != null && templateDefault.DemoEntityID.HasValue
                        ? templateDefault.DemoEntityID.Value
                        : new int?());

                // We can't deliver entities that are not delivered by base (original stream), so continue
                if (!entityId.HasValue || !originals.ContainsKey(entityId.Value))
                    continue;

	            IEntity presentation = null;


	            if (presentationItemType.HasValue)
	            {
		            // Try to find presentation entity
		            var presentationEntityId = items.Where(p =>
			            p.SortOrder == i.SortOrder && p.ItemType == presentationItemType && p.EntityID.HasValue &&
			            originals.ContainsKey(p.EntityID.Value)).Select(p => p.EntityID).FirstOrDefault();

		            // If there is no presentation entity, take default entity
		            if (!presentationEntityId.HasValue)
			            presentationEntityId =
				            templateDefaults.Where(
					            d =>
						            d.ItemType == presentationItemType && d.DemoEntityID.HasValue &&
						            originals.ContainsKey(d.DemoEntityID.Value))
					            .Select(p => p.DemoEntityID).FirstOrDefault();

		            presentation = presentationEntityId.HasValue ? originals[presentationEntityId.Value] : null;
	            }


	            var key = entityId.Value;

                // This ensures that if an entity is added more than once, the dictionary doesn't complain because of duplicate keys
                while (entitiesToDeliver.ContainsKey(key))
                    key += 1000000000;

				entitiesToDeliver.Add(key, new EAVExtensions.EntityInContentGroup(originals[entityId.Value]) { SortOrder = i.SortOrder, ContentGroupItemModified = i.SysModified, Presentation = presentation, GroupId = ListId.Value });
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

		private int? _listId;
        private int? ListId
        {
			get
			{
				if (!_listId.HasValue)
					_listId = ModuleId.HasValue ? Sexy.GetContentGroupIdFromModule(ModuleId.Value) : new int?();
				return _listId;
			}
        }

    }
}