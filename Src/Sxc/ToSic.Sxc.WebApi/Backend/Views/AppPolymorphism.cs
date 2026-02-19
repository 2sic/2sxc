using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Sxc.Polymorphism.Sys;

namespace ToSic.Sxc.Backend.Views;

[PrivateApi]
[VisualQuery(
    NiceName = "App Polymorphism Configuration",
    NameId = "a495b51f-44e7-4335-81db-b8a7e33120f0",
    NameIds = ["System.Polymorphism"], // Internal name for the system, used in some entity-pickers. Can change at any time.
    Type = DataSourceType.System,
    Audience = Audience.System,
    DataConfidentiality = DataConfidentiality.Confidential,
    UiHint = "Current Apps Polymorphism")]
// ReSharper disable once UnusedMember.Global
public class AppPolymorphism : CustomDataSource
{
    public AppPolymorphism(Dependencies services, IAppReaderFactory appReaders)
        : base(services, logName: "Sxc.PolyMo", connect: [appReaders])
    {
        ProvideOut(() => AppConfig(appReaders));
    }


    private IEnumerable<IEntity> AppConfig(IAppReaderFactory appReaders)
    {
        var l = Log.Fn<IEnumerable<IEntity>>($"App: {AppId}");

        var poly = appReaders.Get(AppId).List
            .First<PolymorphismConfiguration>(nullHandling: ModelNullHandling.PreferModelForce)!;

        var data = DataFactory
            .SpawnNew(new() { AutoId = false })
            .Create(new Dictionary<string, object?>
            {
                { nameof(poly.Resolver), poly.Resolver },
                { "TypeName", PolymorphismConfiguration.ContentTypeName },
            }, id: poly.Id);

        return l.Return([data], $"{poly}");
    }
}