using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Query;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Dnn.DataSources
{
    /// <summary>
    /// Retrieves data from SQL, specifically using the DNN Connection String
    /// </summary>
    [PublicApi]
	[VisualQuery(
        GlobalName = "ToSic.Sxc.Dnn.DataSources.DnnSql, ToSic.SexyContent",
        Type = DataSourceType.Source, 
        DynamicOut = false,
        Icon = "database",
        PreviousNames = new []
        {
            "ToSic.SexyContent.DataSources.DnnSqlDataSource, ToSic.SexyContent",
            "ToSic.SexyContent.Environment.Dnn7.DataSources.DnnSqlDataSource, ToSic.SexyContent"
        },
	    HelpLink = "https://github.com/2sic/2sxc/wiki/DotNet-DataSource-DnnSqlDataSource",
	    ExpectsDataOfType = "|Config ToSic.SexyContent.DataSources.DnnSqlDataSource")]
	public class DnnSql : SqlDataSource
	{
		public DnnSql()
		{
			Configuration[ConnectionStringNameKey] = "SiteSqlServer";	// String "SiteSqlServer" isn't available in any constant in DNN
		}
	}

}