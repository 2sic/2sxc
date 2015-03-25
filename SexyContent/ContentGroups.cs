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

		public ContentGroup GetContentGroup(int contentGroupId)
		{
			var dataSource = ContentGroupSource();
			dataSource = DataSource.GetDataSource<EntityIdFilter>(_zoneId, _appId, dataSource);
			((EntityIdFilter) dataSource).EntityIds = contentGroupId.ToString();
			return new ContentGroup(dataSource.List.FirstOrDefault().Value);
		}

	}
}