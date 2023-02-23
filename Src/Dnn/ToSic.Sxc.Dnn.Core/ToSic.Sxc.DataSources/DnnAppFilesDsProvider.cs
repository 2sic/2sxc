using ToSic.Lib.Documentation;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources
{
    [PrivateApi]
    public class DnnAppFilesDsProvider: AppFilesDataSourceProvider
    {
        public DnnAppFilesDsProvider(MyServices services) : base(services, "Dnn.AppFiles") { }
    };
}
