using ToSic.Sxc.Adam;
using ToSic.Sxc.Adam.Internal;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.Services;

[PrivateApi("hide implementation")]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class AdamService(): ServiceWithContext("Svc.AdamSv"), IAdamService
{
    private AdamManager AdamManagerWithContext => field
        ??= ExCtxOrNull?.GetServiceForData<AdamManager>();

    /// <inheritdoc />
    public IFile? File(int id)
        => AdamManagerWithContext?.AdamFs.GetFile(AdamAssetIdentifier.Create(id));

    /// <inheritdoc />
    public IFile? File(string id)
    {
        var fileId = LinkParts.CheckIdStringForId(id);
        return fileId == null
            ? null
            : File(fileId.Value);
    }


    /// <inheritdoc />
    public IFile? File(IField field)
    {
        if (field?.Raw is not string id)
            return null;
        // Get file - can be null if invalid id, empty string or not found
        var file = File(id);
        if (file == null)
            return null;
        file.Field = field;
        return file;
    }

    /// <inheritdoc />
    public IFolder? Folder(int id)
        => AdamManagerWithContext?.AdamFs.GetFolder(AdamAssetIdentifier.Create(id));

    /// <inheritdoc />
    public IFolder? Folder(IField field)
        => AdamManagerWithContext?.FolderOfField(field.Parent.Entity.EntityGuid, field.Name);

}