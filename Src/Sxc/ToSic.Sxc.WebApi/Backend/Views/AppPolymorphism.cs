using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Eav.Data.Raw;
using ToSic.Eav.Models;
using ToSic.Sxc.Render.Polymorphism.Sys;

namespace ToSic.Sxc.Backend.Views;

[PrivateApi]
[VisualQuery(
    NiceName = "App Polymorphism Configuration",
    NameId = "a495b51f-44e7-4335-81db-b8a7e33120f0",
    NameIds = ["System.Polymorphism"], // Internal name for the system, used in some entity-pickers (to configure Copilot). Can change at any time.
    Type = DataSourceType.System,
    Audience = Audience.System,
    DataConfidentiality = DataConfidentiality.Confidential,
    UiHint = "Current Apps Polymorphism")]
// ReSharper disable once UnusedMember.Global
public class AppPolymorphism(CustomDataSource.Dependencies services, IAppReaderFactory appReaders)
    : CustomDataSource(services, logName: "Sxc.PolyMo", connect: [appReaders])
{
    /// <summary>
    /// Default data generation according to CustomDataSource - will return the polymorphism configuration for the current app.
    /// </summary>
    /// <returns></returns>
    protected override IEnumerable<IRawData> GetDefault()
    {
        var l = Log.Fn<IEnumerable<IRawData>>($"App: {AppId}");

        var poly = appReaders.Get(AppId).List
            .FirstModel<PolymorphismConfigurationModel>(options: new() { NullHandling = NullHandling.ReturnModel })!;

        var data = new AppPolymorphismRaw(poly.Id, poly.Resolver, PolymorphismConfigurationModel.ContentTypeName);

        return l.Return([data], $"{poly}");
    }

    /// <summary>
    /// The raw data structure for the AppPolymorphism DataSource.
    /// Reduced, so it doesn't contain all the properties of the PolymorphismConfigurationModel, but only the ones needed for the DataSource.
    /// </summary>
    private sealed record AppPolymorphismRaw(int Id, string? Resolver, string TypeName) : IRawEntityAutoConvert;
}