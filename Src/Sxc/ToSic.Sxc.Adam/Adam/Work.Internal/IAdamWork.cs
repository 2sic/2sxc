using ToSic.Lib.Services;
using ToSic.Sxc.Adam.Internal;

namespace ToSic.Sxc.Adam.Work.Internal;

/// <summary>
/// Just an empty interface to mark all the AdamWork classes to support setup
/// </summary>
public interface IAdamWork: IServiceWithSetup<AdamWorkOptions>
{
    //public void SetupInternal(int appId, string contentType, Guid itemGuid, string field, bool usePortalRoot);

    //public void SetupInternal(AdamWorkOptions options);
    AdamContext AdamContext { get; }
}
