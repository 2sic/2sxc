using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.Context;
using ToSic.Sxc.Cms.Users;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sys.Caching.Policies;

namespace ToSic.Sxc.Services.Cache.Sys;

/// <summary>
/// Converts cache configuration to cache-policy maker.
/// </summary>
internal record CacheContextTools
{
    #region Internal Bits to make it work

    //internal required CacheKeySpecs KeySpecs { get; init; }

    [field: AllowNull, MaybeNull]
    internal required IExecutionContext ExCtx
    {
        get => field ?? throw new NullReferenceException($"{nameof(CacheSpecs)}.{nameof(ExCtx)} should never be null at runtime, only during unit tests. Avoid test on aspects which need this.");
        init;
    }

    //internal required LazySvc<IAppReaderFactory> AppReaders { get; init; }

    internal required Generator<IAppPathsMicroSvc> AppPathsLazy { get; init; }

    // Note: actually internal...
    public required IPolicyMaker BasePolicyMaker { get; internal init; }

    internal IDictionary<string, object?>? Model { get; init; }

    internal required CacheKeySpecs KeySpecs { get; init; }

    #endregion

    [field: AllowNull, MaybeNull]
    public ICmsUser User => field ??= ExCtx.GetState<ICmsContext>().User;

    public UserElevation UserElevation => _userElevation ??= User?.GetElevation() ?? UserElevation.Unknown;
    private UserElevation? _userElevation;

    [field: AllowNull, MaybeNull]
    public ICmsModule Module => field ??= ExCtx.GetState<ICmsContext>().Module;

    [field: AllowNull, MaybeNull]
    public ICmsPage Page => field ??= ExCtx.GetState<ICmsContext>().Page;

    public ISite? Site => field ??= ExCtx.GetState<IContextOfBlock>()?.Site;

    [field: AllowNull, MaybeNull]
    internal IAppReader AppReader => field ??= ExCtx.GetState<IAppReader>();

}
