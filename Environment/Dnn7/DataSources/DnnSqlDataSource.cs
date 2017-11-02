using ToSic.Eav.DataSources;
using ToSic.Eav.Types.Attributes;

namespace ToSic.SexyContent.Environment.Dnn7.DataSources
{
	[PipelineDesigner]
	[ExpectsDataOfType("|Config ToSic.SexyContent.DataSources.DnnSqlDataSource")]
	public class DnnSqlDataSource : SqlDataSource
	{
		public DnnSqlDataSource()
		{
			Configuration[ConnectionStringNameKey] = "SiteSqlServer";	// String "SiteSqlServer" isn't available in any constant in DNN
		}
	}
}