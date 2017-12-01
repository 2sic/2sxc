using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.VisualQuery;

namespace ToSic.SexyContent.Environment.Dnn7.DataSources
{

	[VisualQuery(Type = DataSourceType.Source, DynamicOut = false,
        Icon = "database",
	    HelpLink = "https://github.com/2sic/2sxc/wiki/DotNet-DataSource-DnnSqlDataSource",
	    ExpectsDataOfType = "|Config ToSic.SexyContent.DataSources.DnnSqlDataSource")]
	public class DnnSqlDataSource : SqlDataSource
	{
		public DnnSqlDataSource()
		{
			Configuration[ConnectionStringNameKey] = "SiteSqlServer";	// String "SiteSqlServer" isn't available in any constant in DNN
		}
	}
}