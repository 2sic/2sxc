using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.DataSources;
using ToSic.Sys.Caching.PiggyBack;

namespace ToSic.Sxc.Code.Context.Sys;

/// <summary>
/// Internal context information about the current execution.
/// </summary>
/// <remarks>
/// It contains:
/// 1. state
/// 2. services
/// 3. ...and more
///
/// The initial interface has none or few properties, so that we can pass it around without all projects being tied to each other.
/// </remarks>
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface ICodeContext: ICodeContextGetService, ICodeContextSettings, IHasPiggyBack, IHasLog
{
    IAppForCode? App { get; }

    ITypedStack AllResources { get; }

    public IBlock? Block { get; }
    ContextData BlockData { get; }

    ICmsContext CmsContext { get; }

    ICodeDataFactory Cdf { get; }
}
