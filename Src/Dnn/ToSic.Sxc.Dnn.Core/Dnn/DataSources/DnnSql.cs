using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Queries;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Dnn.DataSources
{
    /// <summary>
    /// Retrieves data from SQL, specifically using the DNN Connection String
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
	[VisualQuery(
        NiceName = "Dnn SQL",
        UiHint = "Data from the Dnn database",
        Icon = Icons.DynamicForm,
        Type = DataSourceType.Source, 
        GlobalName = "ToSic.Sxc.Dnn.DataSources.DnnSql, ToSic.Sxc.Dnn",
        DynamicOut = false,
        PreviousNames = new []
        {
            "ToSic.SexyContent.DataSources.DnnSqlDataSource, ToSic.SexyContent",
            "ToSic.SexyContent.Environment.Dnn7.DataSources.DnnSqlDataSource, ToSic.SexyContent"
        },
	    HelpLink = "https://github.com/2sic/2sxc/wiki/DotNet-DataSource-DnnSqlDataSource",
	    ExpectsDataOfType = "|Config ToSic.SexyContent.DataSources.DnnSqlDataSource")]
	public class DnnSql : Sql
	{
		public DnnSql()
		{
			Configuration[ConnectionStringNameKey] = "SiteSqlServer";	// String "SiteSqlServer" isn't available in any constant in DNN
		}
	}

}