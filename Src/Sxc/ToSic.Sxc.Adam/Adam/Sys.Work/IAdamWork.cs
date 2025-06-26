using ToSic.Lib.Services;
using ToSic.Sxc.Adam.Sys.Manager;

namespace ToSic.Sxc.Adam.Sys.Work;

/// <summary>
/// Just an empty interface to mark all the AdamWork classes to support setup
/// </summary>
public interface IAdamWork: IServiceWithSetup<AdamWorkOptions>
{
    AdamContext AdamContext { get; }
}
