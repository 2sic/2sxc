using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.BLL;
using EntityRelationship = ToSic.Eav.Data.EntityRelationship;

namespace ToSic.SexyContent
{
	public class ContentGroup
	{

		private IEntity _contentGroupEntity;
		private readonly int _zoneId;
		private readonly int _appId;

		private readonly Guid? _previewTemplateId;


	    public ContentGroup(IEntity contentGroupEntity, int zoneId, int appId)
	    {
			if (contentGroupEntity == null)
				throw new Exception("ContentGroup entity is null");

		    _contentGroupEntity = contentGroupEntity;
		    _zoneId = zoneId;
		    _appId = appId;
	    }

		/// <summary>
		/// Instanciate a "temporary" ContentGroup with the specified templateId and no Content items
		/// </summary>
		public ContentGroup(Guid? previewTemplateId, int zoneId, int appId)
		{
			_previewTemplateId = previewTemplateId;
			_zoneId = zoneId;
			_appId = appId;
		}

		/// <summary>
		/// Returns true if a content group entity for this group really exists
		/// Means for example, that the app can't be changed anymore
		/// </summary>
		public bool Exists
		{
			get { return _contentGroupEntity != null; }
		}

		private Template _template;
		public Template Template
		{
			get
			{
				if (_template == null)
				{

					IEntity templateEntity = null;

					if (_previewTemplateId.HasValue)
					{
						var dataSource = DataSource.GetInitialDataSource(_zoneId, _appId);
						// ToDo: Should use an indexed Guid filter
						templateEntity = dataSource.List.FirstOrDefault(e => e.Value.EntityGuid == _previewTemplateId).Value;
					}
					else if (_contentGroupEntity != null)
						templateEntity = ((EntityRelationship) _contentGroupEntity.Attributes["Template"][0]).FirstOrDefault();

					_template = templateEntity == null ? null : new Template(templateEntity);
				}

				return _template;
			}
		}

		public int ContentGroupId { get { return _contentGroupEntity.EntityId; } }
		public Guid ContentGroupGuid { get { return _contentGroupEntity == null ? Guid.Empty : _contentGroupEntity.EntityGuid; } }

		public List<IEntity> Content
		{
			get
			{
				if (_contentGroupEntity != null)
				{
					var list = ((EntityRelationship) _contentGroupEntity.GetBestValue("Content")).ToList();
					if (list.Count > 0)
						return list;
				}

				return new List<IEntity> { null };
			}
		}

		public List<IEntity> Presentation
		{
			get
			{
				if(_contentGroupEntity == null)
					return new List<IEntity>();
				return ((EntityRelationship) _contentGroupEntity.GetBestValue("Presentation")).ToList();
			}
		}

		public List<IEntity> ListContent
		{
			get
			{
				if (_contentGroupEntity == null)
					return new List<IEntity>();
				return ((EntityRelationship)_contentGroupEntity.GetBestValue("ListContent")).ToList();
			}
		}
		public List<IEntity> ListPresentation
		{
			get
			{
				if (_contentGroupEntity == null)
					return new List<IEntity>(); 
				return((EntityRelationship)_contentGroupEntity.GetBestValue("ListPresentation")).ToList();
			}
		}

		public List<IEntity> this[string type]
		{
			get
			{
				switch (type)
				{
					case "Content":
						return Content;
					case "Presentation":
						return Presentation;
					case "ListContent":
						return ListContent;
					case "ListPresentation":
						return ListPresentation;
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

		    var context = EavDataController.Instance(_zoneId, _appId).Entities;// EavContext.Instance(_zoneId, _appId);
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

		    var context = EavDataController.Instance(_zoneId, _appId).Entities;// EavContext.Instance(_zoneId, _appId);
			context.UpdateEntity(_contentGroupEntity.EntityGuid, values);

			// Refresh content group entity (ensures contentgroup is up to date)
			_contentGroupEntity = new ContentGroups(_zoneId, _appId).GetContentGroup(_contentGroupEntity.EntityGuid)._contentGroupEntity;
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

			if (difference != 0)
			{
				var entityIds = Presentation.Select(p => p == null ? new int?() : p.EntityId).ToList();
				entityIds.AddRange(Enumerable.Repeat(new int?(), difference));
				UpdateEntities("Presentation", entityIds);
			}
		}

		public void ReorderEntities(int sortOrder, int destinationSortOrder)
		{
			SyncContentAndPresentationEntitiesCount();

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