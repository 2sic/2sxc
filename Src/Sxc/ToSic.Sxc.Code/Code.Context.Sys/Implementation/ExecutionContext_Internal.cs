using ToSic.Eav.Apps.Sys.AppStack;
using ToSic.Eav.Data.Sys.PropertyLookup;
using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;

namespace ToSic.Sxc.Code.Context.Sys;

public partial class ExecutionContext
{
    [PrivateApi]
    protected void AttachApp(IAppForCode app)
    {
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

    public ContextData BlockData => (ContextData?)Block?.Data
                                    ?? throw new NullReferenceException($"{nameof(Block)} isn't available");

    [field: AllowNull, MaybeNull]
    private AppDataStackService AppDss => field ??= Services.DataStackService.Init(App!.AppReader);

    internal List<KeyValuePair<string, IPropertyLookup>> ResourcesSources => field
        ??= AppDss.GetStack(AppStackConstants.Resources, Block?.View?.Resources);


    internal List<KeyValuePair<string, IPropertyLookup>> SettingsSources => field
        ??= AppDss.GetStack(AppStackConstants.Settings, Block?.View?.Settings);

    public ITypedStack AllSettings => field ??= Cdf.AsTypedStack(AppStackConstants.RootNameSettings, SettingsSources);

    public ITypedStack AllResources => field ??= Cdf.AsTypedStack(AppStackConstants.RootNameResources, ResourcesSources);

}