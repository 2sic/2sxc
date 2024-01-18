using ToSic.Eav.Data.Build;

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.DataSources;

[Obsolete("This class was moved / to the ToSic.Sxc.Dnn.DataSources namespace, use that instead.")]
public class DnnSqlDataSource : Sxc.Dnn.DataSources.DnnSql
{
    // Todo: leave this helper class/message in till 2sxc 09.00, then either extract into an own DLL
    // - we might also write some SQL to update existing pipelines, but it's not likely to have been used much...
    // - and otherwise im might be in razor-code, which we couldn't auto-update

    public DnnSqlDataSource(MyServices services, IDataFactory dataFactory) : base(services, dataFactory)
    {
    }
}