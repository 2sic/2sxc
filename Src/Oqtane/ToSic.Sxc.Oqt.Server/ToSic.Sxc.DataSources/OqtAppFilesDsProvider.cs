using ToSic.Sxc.DataSources;

namespace ToSic.Sxc.Oqt.Server.ToSic.Sxc.DataSources
{
    public class OqtAppFilesDsProvider : AppFilesDataSourceProvider
    {
        public OqtAppFilesDsProvider(MyServices services) : base(services, "Oqt.AppFiles") { }
    }
}
