using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Attributes;

namespace ToSic.SexyContent.Environment.Dnn7.DataSources
{
	[PipelineDesigner]
	[DataSourceProperties(Type = DataSourceType.Source, DynamicOut = true,
	    ExpectsDataOfType = "|Config ToSic.SexyContent.DataSources.DnnSqlDataSource")]
	public class DnnSqlDataSource : SqlDataSource
	{
		public DnnSqlDataSource()
		{
			Configuration[ConnectionStringNameKey] = "SiteSqlServer";	// String "SiteSqlServer" isn't available in any constant in DNN
		}
	}
}