using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
			return ContentGroupSource().List.Select(p => new ContentGroup(p.Value));
		}

		public ContentGroup GetContentGroup(Guid contentGroupId)
		{
			var dataSource = ContentGroupSource();
			// ToDo: Need a EntityGuidFilter to filter this
			return new ContentGroup(dataSource.List.FirstOrDefault(e => e.Value.EntityGuid == contentGroupId).Value);
		}

		public void UpdateContentGroup(Guid contentGroupId, int? templateId)
		{
			// ToDo: Should add ContentGroup if it does not exist?!
			throw new Exception("Not implemented yet (code for changing the contentgroup)");
		}

	}
}