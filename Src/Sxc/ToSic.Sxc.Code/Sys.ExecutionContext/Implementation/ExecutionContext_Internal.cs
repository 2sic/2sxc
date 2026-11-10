using ToSic.Eav.Apps.Sys.AppStack;
using ToSic.Eav.Data.Sys.PropertyLookup;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Sys.ExecutionContext;

public partial class ExecutionContext
    : IExCtxAttachApp
{
    [PrivateApi]
    public void AttachApp(IAppForCode app)
    {
        if (app is AppDynamic appClassic)
            appClassic.SetupCodeDataFactory(Cdf);

        App = app;

        EditionForHotBuild = Block == null
            ? null
            : Services.EditionSvc.Edition(Block.View, App.AppReader);
    }

    internal string? EditionForHotBuild;

    /// <summary>
    /// The current block - can be null if the context only knows about the App, but not about the block.
    /// </summary>
    [PrivateApi] public IBlock? Block { get; private set; }

    [field: AllowNull, MaybeNull]
    private AppDataStackService AppDss => field ??= Services.DataStackService.Init(App!.AppReader);

    internal List<KeyValuePair<string, IPropertyLookup>> ResourcesSources => field
        ??= AppDss.GetStack(AppStackConstants.Resources, Block?.View?.Resources);


    internal List<KeyValuePair<string, IPropertyLookup>> SettingsSources => field
        ??= AppDss.GetStack(AppStackConstants.Settings, Block?.View?.Settings);

    public ITypedStack AllSettings => field ??= Cdf.AsTypedStack(AppStackConstants.RootNameSettings, SettingsSources);

    public ITypedStack AllResources => field ??= Cdf.AsTypedStack(AppStackConstants.RootNameResources, ResourcesSources);

}