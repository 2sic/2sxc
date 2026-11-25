using ToSic.Sxc.Adam.Sys.Manager;

namespace ToSic.Sxc.Backend.Adam;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IAdamPrefetchHelper : /*IAdamWork,*/ IServiceWithSetup<AdamContext.Options>, IHasOptions<AdamContext.Options>
{
    /// <summary>
    /// Get a DTO list of items in a field
    /// </summary>
    /// <param name="subFolderName">Optional sub folder, when browsing a sub-folder</param>
    /// <param name="autoCreate">Auto-create the folder requested - default is true</param>
    /// <returns></returns>
    ICollection<AdamItemDto> GetAdamItemsForPrefetch(string subFolderName, bool autoCreate = true);
}