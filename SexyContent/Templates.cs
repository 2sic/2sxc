using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToSic.Eav;
using ToSic.Eav.DataSources;

namespace ToSic.SexyContent
{
	public class Templates
	{
		private const string TemplateTypeName = "2SexyContent-Template";

		private readonly int _zoneId;
		private readonly int _appId;

		public Templates(int zoneId, int appId)
		{
			_zoneId = zoneId;
			_appId = appId;
		}

		private IDataSource TemplateDataSource()
		{
			var dataSource = DataSource.GetInitialDataSource(_zoneId, _appId, false);
			dataSource = DataSource.GetDataSource<EntityTypeFilter>(_zoneId, _appId, dataSource);
			((EntityTypeFilter)dataSource).TypeName = TemplateTypeName;
			return dataSource;
		}

		public IEnumerable<Template> GetAllTemplates()
		{
			
            return TemplateDataSource().List.Select(p => new Template(p.Value));
        }

		public IEnumerable<Template> GetVisibleTemplates()
		{
			return GetAllTemplates().Where(t => !t.IsHidden);
		}

		public Template GetTemplate(int templateId)
		{
			var dataSource = TemplateDataSource();
			dataSource = DataSource.GetDataSource<EntityIdFilter>(_zoneId, _appId, dataSource);
			((EntityIdFilter)dataSource).EntityIds = templateId.ToString();
			return new Template(dataSource.List.FirstOrDefault().Value);
		}

	}
}