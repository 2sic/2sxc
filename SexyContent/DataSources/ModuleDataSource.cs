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

        private List<TemplateDefault> _templateDefaults;
        private IEnumerable<TemplateDefault> TemplateDefaults
        {
            get
            {
                if (!ListId.HasValue || (!OverrideTemplateId.HasValue && (!ContentGroupItems.Any() || !ContentGroupItems.First().TemplateID.HasValue)))
                    return new List<TemplateDefault>();

                if (_templateDefaults == null)
                    _templateDefaults = Sexy.GetTemplateDefaults(OverrideTemplateId.HasValue ? OverrideTemplateId.Value : ContentGroupItems.First().TemplateID.Value);
                return _templateDefaults;
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
                _content = GetStream(ContentGroupItemType.Content);
            }
            return _content;
        }

        private IDictionary<int, IEntity> _presentation;
        private IDictionary<int, IEntity> GetPresentation()
        {
            if (_presentation == null)
            {
                _presentation = GetStream(ContentGroupItemType.Presentation);
            }
            return _presentation;
        }

        private IDictionary<int, IEntity> _listContent;
        private IDictionary<int, IEntity> GetListContent()
        {
            if (_listContent == null)
            {
                _listContent = GetStream(ContentGroupItemType.ListContent);
            }
            return _listContent;
        }

        private IDictionary<int, IEntity> _listPresentation;
        private IDictionary<int, IEntity> GetListPresentation()
        {
            if (_listPresentation == null)
            {
                _listPresentation = GetStream(ContentGroupItemType.ListPresentation);
            }
            return _listPresentation;
        }

        private IDictionary<int, IEntity> GetStream(ContentGroupItemType itemType)
        {
            var templateDefault = TemplateDefaults.FirstOrDefault(t => t.ItemType == itemType);
            var items = ContentGroupItems.Where(p => p.ItemType == itemType).ToList(); // Create copy of list (not in cache)

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
                    SortOrder = itemType == ContentGroupItemType.ListContent ? -1 : 0,
                    SysCreated = DateTime.Now,
                    SysCreatedBy = -1,
                    TemplateID = OverrideTemplateId.HasValue ? OverrideTemplateId.Value : ContentGroupItems.First().TemplateID.Value,
                    Type = itemType.ToString("F")
                });
            }

            var originals = In["Default"].List;
            var entitiesToDeliver = new Dictionary<int, IEntity>();

            foreach (var i in items)
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

                var key = entityId.Value;

                // This ensures that if an entity is added again, the dictionary doesn't complain because of duplicate keys
                while (entitiesToDeliver.ContainsKey(key))
                    key += 1000000000;

                entitiesToDeliver.Add(key, new EAVExtensions.EntityInContentGroup(originals[entityId.Value]) { SortOrder = i.SortOrder, ContentGroupItemModified = i.SysModified, Presentation = null });
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

        private int? ListId
        {
            get { return ModuleId.HasValue ? Sexy.GetContentGroupIdFromModule(ModuleId.Value) : new int?(); }
        }

        private IEnumerable<Element> GetElements(ContentGroupItemType contentItemType, ContentGroupItemType presentationItemType)
        {
            
            var elements = new List<Element>();
            if (!ContentGroupItems.Any(c => c.TemplateID.HasValue) && !OverrideTemplateId.HasValue) return elements;

            // Create a clone of the list (because it will get modified)
            var items = ContentGroupItems.ToList();
            var templateId = OverrideTemplateId.HasValue ? OverrideTemplateId.Value : items.First().TemplateID.Value;
            var defaults = Sexy.GetTemplateDefaults(templateId);
            var dimensionIds = new[] { System.Threading.Thread.CurrentThread.CurrentCulture.Name };

            var contentEntities = GetStream(contentItemType);
            var allEntities = In["Default"].List;

			// If no Content Elements exist, add a ContentGroupItem to List (not to DB)
			if (!items.Any(p => p.ItemType == contentItemType))
			{
				items.Add(new ContentGroupItem()
				{
                    ContentGroupID = ListId.Value,
                    ContentGroupItemID = -1,
                    EntityID = new int?(),
                    SortOrder = contentItemType == ContentGroupItemType.ListContent ? -1 : 0,
                    SysCreated = DateTime.Now,
                    SysCreatedBy = -1,
                    TemplateID = templateId,
                    Type = contentItemType.ToString("F")
                });
            }

            // Transform to list of Elements
            elements = (from c in items
                           where c.ItemType == contentItemType
                               // don't show items whose Content entity is not in source
                                 && (!c.EntityID.HasValue || contentEntities.ContainsKey(c.EntityID.Value))
                           select new Element
                           {
                               ID = c.ContentGroupItemID,
                               EntityId = c.EntityID,
                               TemplateId = c.TemplateID,
                               Content = c.EntityID.HasValue ? new DynamicEntity(contentEntities[c.EntityID.Value], dimensionIds, Sexy) :
                                   defaults.Where(d => d.ItemType == contentItemType && d.DemoEntityID.HasValue && contentEntities.ContainsKey(d.DemoEntityID.Value))
                                       .Select(d => new DynamicEntity(contentEntities[d.DemoEntityID.Value], dimensionIds, Sexy)).FirstOrDefault(),
                               // Get Presentation object - Take Default if it does not exist
                               Presentation = (from p in items
                                               where p.SortOrder == c.SortOrder && p.ItemType == presentationItemType && p.EntityID.HasValue && allEntities.ContainsKey(p.EntityID.Value)
                                               select new DynamicEntity(allEntities[p.EntityID.Value], dimensionIds, Sexy)).FirstOrDefault() ??
                                              (from d in defaults
                                               where d.ItemType == presentationItemType && d.DemoEntityID.HasValue && allEntities.ContainsKey(d.DemoEntityID.Value)
                                               select new DynamicEntity(allEntities[d.DemoEntityID.Value], dimensionIds, Sexy)).FirstOrDefault()
                               ,
                               GroupId = ListId.Value,
                               SortOrder = c.SortOrder
                           }).ToList();

            return elements;
        }

		//private List<Element> _contentElements;
		//public List<Element> ContentElements
		//{
		//	get
		//	{
		//		if (_contentElements == null)
		//			_contentElements = GetElements(ContentGroupItemType.Content, ContentGroupItemType.Presentation).ToList();
		//		return _contentElements;
		//	}
		//}

		//private Element _listElement;
		//public Element ListElement
		//{
		//	get
		//	{
		//		if(_listElement == null)
		//			_listElement = GetElements(ContentGroupItemType.ListContent, ContentGroupItemType.ListPresentation).FirstOrDefault();
		//		return _listElement;
		//	}
		//}

    }
}