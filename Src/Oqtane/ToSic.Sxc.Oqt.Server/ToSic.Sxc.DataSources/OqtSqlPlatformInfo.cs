using Oqtane.Infrastructure;
using Oqtane.Shared;
using ToSic.Eav.DataSources.Internal;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;

namespace ToSic.Sxc.Oqt.Server.ToSic.Sxc.DataSources;

internal class OqtSqlPlatformInfo(LazySvc<IConfigManager> configManager) : SqlPlatformInfo
{
    public override string DefaultConnectionStringName => SettingKeys.ConnectionStringKey;

    public override string FindConnectionString(string name)
    {
        if (name.EqualsInsensitive(DefaultConnectionStringName))
            return configManager.Value.GetSetting("ConnectionStrings:" + SettingKeys.ConnectionStringKey, "");

        // TODO
        // Where are all the connection strings stored, I think base... doesn't work
        // Where would the site connection string be?
        return base.FindConnectionString(name);
    }

}