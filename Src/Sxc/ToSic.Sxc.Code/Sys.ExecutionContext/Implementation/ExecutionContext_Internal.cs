using ToSic.Eav.Apps;
using ToSic.Eav.DataSource;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys.Factory;

namespace ToSic.Sxc.Sys.ExecutionContext;

public partial class ExecutionContext
    : IWrapper<ICodeDataFactory>,
        IExCtxAttachApp
{
    [PrivateApi]
    public void AttachApp(IAppForCode app)
    {
        if (app is App appClassic)
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
    [PrivateApi] public IBlock? Block { get; private set; } = null!;

    public TState GetState<TState>() where TState : class
    {
        if (typeof(TState) == typeof(ICmsContext))
            return (TState)CmsContext;

        if (typeof(TState) == typeof(IAppForCode))
            return (TState)App!;

        if (typeof(TState) == typeof(IAppReader))
            return (TState)App?.AppReader!;

        if (typeof(TState) == typeof(IDataSource))
            return (TState)(Block?.Data ?? throw new NullReferenceException($"{nameof(Block)} isn't available"));

        if (typeof(TState) == typeof(IBlock))
            return (TState)Block!;

        if (typeof(TState) == typeof(IContextOfBlock))
            return (TState)Block?.Context!;

        if (typeof(TState) == typeof(IAppTyped))
            return (TState)TypedApi.AppTyped;

        throw new InvalidOperationException(
            $"Can't get state of type {typeof(TState).Name} - only {nameof(IAppForCode)}, {nameof(IDataSource)}, {nameof(IBlock)} and {nameof(IAppTyped)} are supported");
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