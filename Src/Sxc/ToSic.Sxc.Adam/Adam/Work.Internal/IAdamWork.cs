using ToSic.Lib.Services;
using ToSic.Sxc.Adam.Manager.Internal;

namespace ToSic.Sxc.Adam.Work.Internal;

/// <summary>
/// Just an empty interface to mark all the AdamWork classes to support setup
/// </summary>
public interface IAdamWork: IServiceWithSetup<AdamWorkOptions>
{
    AdamContext AdamContext { get; }
}
