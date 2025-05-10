namespace ToSic.Sxc.Backend.Cms;

/// <summary>
/// Load Settings Provider which supply additional content-types which the UI needs for editing.
/// Eg when the UI needs to know which content-types are available for a picker.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface ILoadSettingsContentTypesProvider: IHasLog
{
    List<IContentType> GetContentTypes(LoadSettingsProviderParameters parameters);
}