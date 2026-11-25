using ToSic.Eav.DataSource.Sys;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Eav.DataSources;

namespace ToSic.Sxc.Dnn.DataSources;

/// <summary>
/// Retrieves data from SQL, specifically using the DNN Connection String
/// </summary>
[PublicApi]
[VisualQuery(
    NiceName = "Dnn SQL",
    UiHint = "Data from the Dnn database",
    Icon = DataSourceIcons.DynamicForm,
    Type = DataSourceType.Source, 
    NameId = "b9df1b84-7b50-418c-a476-1ce49193cd77",
    NameIds =
    [
        "ToSic.Sxc.Dnn.DataSources.DnnSql, ToSic.Sxc.Dnn",
        "ToSic.SexyContent.DataSources.DnnSqlDataSource, ToSic.SexyContent",
        "ToSic.SexyContent.Environment.Dnn7.DataSources.DnnSqlDataSource, ToSic.SexyContent"
    ],
    HelpLink = "https://github.com/2sic/2sxc/wiki/DotNet-DataSource-DnnSqlDataSource",
    ConfigurationType = "|Config ToSic.SexyContent.DataSources.DnnSqlDataSource")]
public class DnnSql : Sql
{
    [PrivateApi]
    public DnnSql(Dependencies services) : base(services)
    {
        ConnectionStringName = DnnSqlPlatformInfo.SiteSqlServer;
    }
}