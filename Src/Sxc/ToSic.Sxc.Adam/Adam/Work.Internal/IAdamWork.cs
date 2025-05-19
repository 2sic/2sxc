using ToSic.Sxc.Adam.Internal;
using ToSic.Sys.Services;

namespace ToSic.Sxc.Adam.Work.Internal;

/// <summary>
/// Just an empty interface to mark all the AdamWork classes to support setup
/// </summary>
public interface IAdamWork: IServiceWithOptionsToSetup<AdamWorkOptions>
{
    //public void SetupInternal(int appId, string contentType, Guid itemGuid, string field, bool usePortalRoot);

    //public void SetupInternal(AdamWorkOptions options);
    AdamContext AdamContext { get; }
}
