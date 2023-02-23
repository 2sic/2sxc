using ToSic.Sxc.DataSources;

namespace ToSic.Sxc.ToSic.Sxc.DataSources
{
    public class DnnAdamDsProvider<TFolderId, TFileId> : AdamDataSourceProvider<TFolderId, TFileId>
    {
        public DnnAdamDsProvider(MyServices services) : base(services, "Dnn.Adam") { }
    }
}
