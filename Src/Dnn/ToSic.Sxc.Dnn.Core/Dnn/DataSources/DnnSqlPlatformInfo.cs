using ToSic.Eav.DataSources.Internal;

namespace ToSic.Sxc.Dnn.DataSources;

public class DnnSqlPlatformInfo: SqlPlatformInfo
{
    // String "SiteSqlServer" isn't available in any constant in DNN
    internal const string SiteSqlServer = "SiteSqlServer";

    public override string DefaultConnectionStringName => SiteSqlServer;
}