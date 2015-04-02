using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Services.Journal;
using ToSic.Eav;
using ToSic.Eav.DataSources;

namespace ToSic.SexyContent
{
	public class ContentGroups
	{
		private const string ContentGroupTypeName = "2SexyContent-ContentGroup";

		private readonly int _zoneId;
		private readonly int _appId;

		public ContentGroups(int zoneId, int appId)
		{
			_zoneId = zoneId;
			_appId = appId;
		}

		private IDataSource ContentGroupSource()
		{
			var dataSource = DataSource.GetInitialDataSource(_zoneId, _appId, false);
			dataSource = DataSource.GetDataSource<EntityTypeFilter>(_zoneId, _appId, dataSource);
			((EntityTypeFilter)dataSource).TypeName = ContentGroupTypeName;
			return dataSource;
		}

		public IEnumerable<ContentGroup> GetContentGroups()
		{
			return ContentGroupSource().List.Select(p => new ContentGroup(p.Value, _zoneId, _appId));
		}

		public ContentGroup GetContentGroup(Guid contentGroupGuid)
		{
			var dataSource = ContentGroupSource();
			return new ContentGroup(dataSource.List.FirstOrDefault(e => e.Value.EntityGuid == contentGroupGuid).Value, _zoneId, _appId);
		}

		public bool IsConfigurationInUse(int templateId, string type)
		{
			var contentGroups = GetContentGroups().Where(p => p.Template != null && p.Template.TemplateId == templateId);
			return contentGroups.Any(p => p[type].Any(c => c != null));
		}

		public Guid CreateContentGroup()
		{
			var context = EavContext.Instance(_zoneId, _appId);
			var contentType = DataSource.GetCache(_zoneId, _appId).GetContentType(ContentGroupTypeName);

			var values = new Dictionary<string, object>()
			{
				{"Template", new int[] {}},
				{"Content", new int[] {}},
				{"Presentation", new int[] {}},
				{"ListContent", new int[] {}},
				{"ListPresentation", new int[] {}}
			};

			var entity = context.AddEntity(contentType.AttributeSetId, values, null, null);
			return entity.EntityGUID;
		}

	}
}