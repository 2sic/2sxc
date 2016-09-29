using ToSic.Eav.DataSources;

namespace ToSic.SexyContent.Environment.Dnn7.DataSources
{
	[PipelineDesigner]
	public class DnnSqlDataSource : SqlDataSource
	{
		public DnnSqlDataSource()
		{
			Configuration[ConnectionStringNameKey] = "SiteSqlServer";	// String "SiteSqlServer" isn't available in any constant in DNN
		}
	}
}