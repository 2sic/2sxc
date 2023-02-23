using ToSic.Sxc.DataSources;

namespace ToSic.Sxc.Oqt.Server.ToSic.Sxc.DataSources
{
    public class OqtAdamDsProvider<TFolderId, TFileId> : AdamDataSourceProvider<TFolderId, TFileId>
    {
        public OqtAdamDsProvider(MyServices services) : base(services, "Oqt.Adam") { }
    }
}
