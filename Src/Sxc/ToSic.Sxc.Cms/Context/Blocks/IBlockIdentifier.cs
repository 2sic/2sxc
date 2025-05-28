using ToSic.Eav.Apps;

namespace ToSic.Eav.Cms.Internal;

/// <summary>
/// Identifies a content-block with all the parameters necessary to find it in the system
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IBlockIdentifier: IAppIdentity
{
    /// <summary>
    /// The App NameId - only used in scenarios where the app isn't found, and we need the NameId to show an error
    /// </summary>
    string AppNameId { get; }

    /// <summary>
    /// The block identifier
    /// </summary>
    Guid Guid { get; }

    /// <summary>
    /// The preview-view identifies a view in an App
    /// It's only used when there is no content-block yet (so Guid is empty)
    /// Once an app has been fully added to the page, the PreviewView isn't used any more
    /// </summary>
    Guid PreviewView { get; }
}