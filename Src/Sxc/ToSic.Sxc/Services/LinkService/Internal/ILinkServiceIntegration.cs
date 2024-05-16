namespace ToSic.Sxc.Services.Internal;

/// <summary>
/// Not yet used - idea is to make the LinkService more composition instead of inheritance.
/// 2024-05-14 it's difficult though, because the inner workings need the _CodeApiService, which is not available in the interface.
/// </summary>
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface ILinkServiceIntegration
{
    string ToApi(string api, string parameters = default);

    string ToPage(int? pageId, string parameters = default, string language = default);
}