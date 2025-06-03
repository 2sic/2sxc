namespace ToSic.Eav.WebApi.PublicApi;

/// <summary>
/// Contract for WebApi controllers supporting ADAM calls
/// </summary>
public interface IAdamController<in TId>
{
    /// <summary>
    /// POST or PUT Upload a file to ADAM
    /// </summary>
    /// <param name="appId">App ID</param>
    /// <param name="contentType">Content type of the field we're adding something to</param>
    /// <param name="guid">Entity Guid we're adding a file to</param>
    /// <param name="field">Field where the file is added to</param>
    /// <param name="subfolder">Folder information within that field</param>
    /// <param name="usePortalRoot">If we should add something to the portal root instead of the field</param>
    /// <returns></returns>

    // Note: #AdamItemDto - as of now, we must use object because System.Io.Text.Json will otherwise not convert the object correctly :(
    // Wip #2902 - ATM must return object, otherwise the result isn't perfectly JSON serialized
    /*AdamItemDto*/ object Upload(int appId, string contentType, Guid guid, string field, string subfolder = "", bool usePortalRoot = false);

    /// <summary>
    /// GET all the ADAM items for an entity, within that folder etc.
    /// </summary>
    /// <param name="appId">App ID</param>
    /// <param name="contentType">Content type of the field we're adding something to</param>
    /// <param name="guid">Entity Guid we're adding a file to</param>
    /// <param name="field">Field where the file is added to</param>
    /// <param name="subfolder">Folder information within that field</param>
    /// <param name="usePortalRoot">If we should add something to the portal root instead of the field</param>
    /// <returns></returns>
    IEnumerable</*AdamItemDto*/object> Items(int appId, string contentType, Guid guid, string field, string subfolder, bool usePortalRoot = false);

    /// <summary>
    /// POST create a folder.
    /// </summary>
    /// <param name="appId">App ID</param>
    /// <param name="contentType">Content type of the field we're adding something to</param>
    /// <param name="guid">Entity Guid we're adding a file to</param>
    /// <param name="field">Field where the file is added to</param>
    /// <param name="subfolder">Folder information within that field</param>
    /// <param name="newFolder">name of the new folder</param>
    /// <param name="usePortalRoot">If we should add something to the portal root instead of the field</param>
    /// <returns></returns>
    IEnumerable</*AdamItemDto*/object> Folder(int appId, string contentType, Guid guid, string field, string subfolder,
        string newFolder, bool usePortalRoot);

    /// <summary>
    /// DELETE an item (folder, file) in ADAM
    /// </summary>
    /// <param name="appId">App ID</param>
    /// <param name="contentType">Content type of the field we're adding something to</param>
    /// <param name="guid">Entity Guid we're adding a file to</param>
    /// <param name="field">Field where the file is added to</param>
    /// <param name="subfolder">Folder information within that field</param>
    /// <param name="isFolder">true/false if we're deleting a folder</param>
    /// <param name="id">ID of the item to delete</param>
    /// <param name="usePortalRoot">If we should add something to the portal root instead of the field</param>
    /// <returns></returns>
    bool Delete(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder,
        TId id, bool usePortalRoot);

    /// <summary>
    /// GET rename an item. 
    /// </summary>
    /// <param name="appId">App ID</param>
    /// <param name="contentType">Content type of the field we're adding something to</param>
    /// <param name="guid">Entity Guid we're adding a file to</param>
    /// <param name="field">Field where the file is added to</param>
    /// <param name="subfolder">Folder information within that field</param>
    /// <param name="isFolder">true/false if we're deleting a folder</param>
    /// <param name="id">ID of the item to delete</param>
    /// <param name="newName">New name for the item</param>
    /// <param name="usePortalRoot">If we should add something to the portal root instead of the field</param>
    /// <returns></returns>
    bool Rename(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder,
        TId id, string newName, bool usePortalRoot);
}