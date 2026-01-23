using ToSic.Eav.Apps;
using ToSic.Eav.DataSource;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys.Factory;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Sys.ExecutionContext;

public partial class ExecutionContext
    : IWrapper<ICodeDataFactory>,
        IExCtxAttachApp
{
    [PrivateApi]
    public void AttachApp(IApp app)
    {
        if (app is App appClassic)
            appClassic.SetupCodeDataFactory(Cdf);

        App = app;

        _editionForHotBuild = Block == null
            ? null
            : Services.Polymorphism.UseViewEditionOrGet(Block.View, ((IAppWithInternal)App).AppReader);
    }

    private string? _editionForHotBuild;

    //[PrivateApi]
    //[Obsolete("Warning - avoid using this on the DynamicCode Root - always use the one on the AsC")]
    //public int CompatibilityLevel => Cdf.CompatibilityLevel;

    /// <summary>
    /// The current block - can be null if the context only knows about the App, but not about the block.
    /// </summary>
    [PrivateApi] public IBlock? Block { get; private set; } = null!;

    [PrivateApi]
    [field: AllowNull, MaybeNull]
    internal IAppTyped AppTyped => field ??= GetService<IAppTyped>(reuse: true);


    public TState GetState<TState>() where TState : class
    {
        if (typeof(TState) == typeof(ICmsContext))
            return (TState)CmsContext;

        if (typeof(TState) == typeof(IApp))
            return (TState)App;

        if (typeof(TState) == typeof(IAppReader))
            return (TState)((IAppWithInternal)App)?.AppReader!;

        if (typeof(TState) == typeof(IDataSource))
            return (TState)(Block?.Data ?? throw new NullReferenceException($"{nameof(Block)} isn't available"));

        if (typeof(TState) == typeof(IBlock))
            return (TState)Block!;

        if (typeof(TState) == typeof(IContextOfBlock))
            return (TState)Block?.Context!;

        if (typeof(TState) == typeof(IAppTyped))
            return (TState)AppTyped;

        throw new InvalidOperationException(
            $"Can't get state of type {typeof(TState).Name} - only {nameof(IApp)}, {nameof(IDataSource)}, {nameof(IBlock)} and {nameof(IAppTyped)} are supported");
    }

    public TState GetDataStack<TState>(string name) where TState : class
    {
        if (typeof(TState) == typeof(IDynamicStack) && name == ExecutionContextStateNames.Settings)
            return (TState)Settings;

        if (typeof(TState) == typeof(ITypedStack) && name == ExecutionContextStateNames.AllSettings)
            return (TState)AllSettings;

        if (typeof(TState) == typeof(ITypedStack) && name == ExecutionContextStateNames.AllResources)
            return (TState)AllResources;

        throw new InvalidOperationException("Can only retrieve 'Settings', 'AllSettings', 'AllResources'");
    }

    ICodeDataFactory IWrapper<ICodeDataFactory>.GetContents()
        => Cdf;
}