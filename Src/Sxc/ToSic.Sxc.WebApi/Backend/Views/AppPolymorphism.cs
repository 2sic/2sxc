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
public class AppPolymorphism : CustomDataSource
{
    public AppPolymorphism(Dependencies services, IAppReaderFactory appReaders)
        : base(services, logName: "Sxc.PolyMo", connect: [appReaders])
    {
        ProvideOutRaw(() => AppConfig(appReaders));
    }


    private IEnumerable<AppPolymorphismRaw> AppConfig(IAppReaderFactory appReaders)
    {
        var l = Log.Fn<IEnumerable<AppPolymorphismRaw>>($"App: {AppId}");

        var poly = appReaders.Get(AppId).List
            .FirstModel<PolymorphismConfigurationModel>(options: new() { NullHandling = NullHandling.ReturnModel })!;

        var data = new AppPolymorphismRaw(
            Id: poly.Id,
            Resolver: poly.Resolver,
            TypeName: PolymorphismConfigurationModel.ContentTypeName);

        return l.Return([data], $"{poly}");
    }

    private sealed record AppPolymorphismRaw(int Id, string? Resolver, string TypeName) : IRawEntityAutoConvert;
}