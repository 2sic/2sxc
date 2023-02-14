using ToSic.Lib.Documentation;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources
{
    [PrivateApi]
    public class DnnAppFilesDsProvider: AppFilesDataSourceProvider
    {
        public DnnAppFilesDsProvider(Dependencies dependencies) : base(dependencies, "Dnn.AppFiles")
        { }
    };
}
