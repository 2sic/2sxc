namespace ToSic.Sxc.Adam.Work.Internal;

/// <summary>
/// Just an empty interface to mark all the AdamWork classes to support setup
/// </summary>
public interface IAdamWork
{
    internal void SetupInternal(int appId, string contentType, Guid itemGuid, string field, bool usePortalRoot);
}
