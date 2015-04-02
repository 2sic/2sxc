using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using ToSic.Eav;

namespace ToSic.SexyContent
{
	public class ContentGroup
	{

		private IEntity _contentGroupEntity;
		private readonly int _zoneId;
		private readonly int _appId;


	    public ContentGroup(IEntity contentGroupEntity, int zoneId, int appId)
	    {
			if (contentGroupEntity == null)
				throw new Exception("ContentGroup entity is null");

		    _contentGroupEntity = contentGroupEntity;
		    _zoneId = zoneId;
		    _appId = appId;
	    }

		public Template Template
		{
			get
			{
				var templateEntity = ((Eav.Data.EntityRelationship) _contentGroupEntity.Attributes["Template"][0]).FirstOrDefault();
				if (templateEntity == null)
					return null;
				return new Template(templateEntity);
			}
		}

		public int ContentGroupId { get { return _contentGroupEntity.EntityId; } }
		public Guid ContentGroupGuid { get { return _contentGroupEntity.EntityGuid; } }

		public List<IEntity> Content
		{
			get
			{
				var list = ((Eav.Data.EntityRelationship) _contentGroupEntity.GetBestValue("Content")).ToList();
				if(list.Count == 0)
					return new List<IEntity>() { null };
				return list;
			}
		}

		public List<IEntity> Presentation { get { return ((Eav.Data.EntityRelationship)_contentGroupEntity.GetBestValue("Presentation")).ToList(); } }
		public List<IEntity> ListContent { get { return ((Eav.Data.EntityRelationship)_contentGroupEntity.GetBestValue("ListContent")).ToList(); } }
		public List<IEntity> ListPresentation { get { return ((Eav.Data.EntityRelationship)_contentGroupEntity.GetBestValue("ListPresentation")).ToList(); } }

		public List<IEntity> this[string type]
		{
			get
			{
				switch (type)
				{
					case "Content":
						return this.Content;
					case "Presentation":
						return this.Presentation;
					case "ListContent":
						return this.ListContent;
					case "ListPresentation":
						return this.ListPresentation;
					default:
						throw new Exception("Type " + type + " not allowed");
				}
			}
		}

		public void Update(int? templateId)
		{
			var values = new Dictionary<string, object>
			{
				{ "Template", templateId.HasValue ? new[] { templateId.Value } : new int[]{} }
			};

			var context = EavContext.Instance(_zoneId, _appId);
			context.UpdateEntity(_contentGroupEntity.EntityGuid, values);
		}

		public void UpdateEntity(string type, int sortOrder, int? entityId)
		{
			if (sortOrder == -1)
				sortOrder = 0;

			var entityIds = this[type].Select(p => p == null ? new int?() : p.EntityId).ToList();

			if (entityIds.Count < sortOrder + 1)
				entityIds.AddRange(Enumerable.Repeat(new int?(), (sortOrder + 1) - entityIds.Count));

			entityIds[sortOrder] = entityId;
			UpdateEntities(type, entityIds);
		}

		private void UpdateEntities(string type, IEnumerable<int?> entityIds)
		{
			if (type == "Presentation" && entityIds.Count() > Content.Count)
				throw new Exception("Presentation may not contain more items than Content.");

			var values = new Dictionary<string, object>
			{
				{ type, entityIds.ToArray() }
			};

			var context = EavContext.Instance(_zoneId, _appId);
			context.UpdateEntity(_contentGroupEntity.EntityGuid, values);

			// Refresh content group entity (ensures contentgroup is up to date)
			this._contentGroupEntity = new ContentGroups(_zoneId, _appId).GetContentGroup(_contentGroupEntity.EntityGuid)._contentGroupEntity;
		}

		/// <summary>
		/// Removes entities from a group. This will also remove the corresponding presentation entities.
		/// </summary>
		/// <param name="contentGroupGuid"></param>
		/// <param name="type"></param>
		/// <param name="sortOrder"></param>
		public void RemoveContentAndPresentationEntities(int sortOrder)
		{
			var type = "Content";
			if (sortOrder == -1)
			{
				type = "ListContent";
				sortOrder = 0;
			}

			RemoveEntity(type, sortOrder);
			RemoveEntity(type.Replace("Content", "Presentation"), sortOrder);
		}

		public void RemovePresentationEntity(int sortOrder)
		{
			UpdateEntity(sortOrder == -1 ? "ListPresentation" : "Presentation", sortOrder, null);
		}

		private void RemoveEntity(string type, int sortOrder)
		{
			var entityIds = this[type].Select(p => p == null ? new int?() : p.EntityId).ToList();
			entityIds.RemoveAt(sortOrder);
			UpdateEntities(type, entityIds);
		}

		/// <summary>
		/// If SortOrder is not specified, adds at the end
		/// </summary>
		/// <param name="sortOrder"></param>
		public void AddContentAndPresentationEntity(int? sortOrder)
		{
			// ToDo: Fix duplicate code (see above)
			var type = "Content";
			if (sortOrder == -1)
			{
				type = "ListContent";
				sortOrder = 0;
			}

			if (!sortOrder.HasValue)
				sortOrder = Content.Count;

			SyncContentAndPresentationEntitiesCount();
			AddEntity(type, sortOrder.Value);
			AddEntity(type.Replace("Content", "Presentation"), sortOrder.Value);
		}

		public void AddEntity(string type, int sortOrder)
		{
			var entityIds = this[type].Select(p => p == null ? new int?() : p.EntityId).ToList();
			entityIds.Insert(sortOrder, new int?());
			UpdateEntities(type, entityIds);
		}

		private void SyncContentAndPresentationEntitiesCount()
		{
			var difference = Content.Count - Presentation.Count;

			if (difference < 0)
				throw new Exception("There are more Presentation elements than Content elements.");

			var entityIds = Presentation.Select(p => p == null ? new int?() : p.EntityId).ToList();
			entityIds.AddRange(Enumerable.Repeat(new int?(), difference));
			UpdateEntities("Presentation", entityIds);
		}

		public void ReorderEntities(int sortOrder, int destinationSortOrder)
		{
			var contentIds = Content.Select(p => p == null ? new int?() : p.EntityId).ToList();
			var presentationIds = Presentation.Select(p => p == null ? new int?() : p.EntityId).ToList();

			var contentId = contentIds[sortOrder];
			var presentationId = presentationIds[sortOrder];

			contentIds.RemoveAt(sortOrder);
			presentationIds.RemoveAt(sortOrder);

			contentIds.Insert(destinationSortOrder, contentId);
			presentationIds.Insert(destinationSortOrder, presentationId);

			UpdateEntities("Content", contentIds);
			UpdateEntities("Presentation", presentationIds);
		}
	}
}