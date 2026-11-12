using ToSic.Eav.Data.Raw;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Sys.Catalog;
using ToSic.Eav.DataSource.VisualQuery;

namespace ToSic.Sxc.Backend.Admin.Query;

[PrivateApi]
[VisualQuery(
    NiceName = "Data Sources",
    NameId = "2de41d89-3cb9-480a-ac3c-40f77fd3af4e",
    NameIds = ["System.DataSources"],
    Type = DataSourceType.System,
    Audience = Audience.System,
    DataConfidentiality = DataConfidentiality.Internal,
    UiHint = "Data sources available to visual query")]
public class DataSources(CustomDataSource.Dependencies services, LazySvc<DataSourceCatalog> dataSourceCatalogLazy)
    : CustomDataSource(services, "Sxc.DataSources", connect: [dataSourceCatalogLazy])
{
    ///// <summary>
    ///// The configuration for which zone to look in.
    ///// Cannot be named `ZoneId` as that property already exists.
    ///// </summary>
    //[Configuration(Field = "ZoneId")]
    //public int OfZoneId => Configuration.GetThis(ZoneId);

    /// <summary>
    /// Get installed DataSources from .NET Runtime but only those with [PipelineDesigner Attribute]
    /// </summary>
    protected override IEnumerable<IRawData> GetDefault()
    {
        var appIdentity = new AppIdentity(ZoneId, AppId);
        var l = Log.Fn<IEnumerable<DataSourceDto>>($"a#{appIdentity.AppId}");
        var dsCat = dataSourceCatalogLazy.Value;
        var installedDataSources = dataSourceCatalogLazy.Value.GetAll(true, appIdentity.AppId);

        var result = installedDataSources
            .Select(ds =>
            {
                var outNames = ds.VisualQuery?.DynamicOut == true
                    ? null
                    : dsCat.GetOutStreamNames(ds);
                return new DataSourceDto(ds, outNames);
            })
            .OrderBy(ds => ds.TypeNameForUi) // sort for better debugging in F12
            .ToListOpt();

        return l.Return(result, result.Count.ToString());
    }
}