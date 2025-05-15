using ToSic.Sxc.Adam;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal;
using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.Services;

[PrivateApi("hide implementation")]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class AdamService(): ServiceForDynamicCode("Svc.AdamSv"), IAdamService
{
    /// <inheritdoc />
    public IFile File(int id)
        => _CodeApiSvc?.Cdf?.File(id);

    /// <inheritdoc />
    public IFile File(string id)
    {
        var fileId = LinkParts.CheckIdStringForId(id);
        return fileId == null ? null : File(fileId.Value);
    }


    /// <inheritdoc />
    public IFile File(IField field)
    {
        var file = field?.Raw is string id
            ? File(id)
            : null;
        if (file != null)
            file.Field = field;
        return file;
    }

    /// <inheritdoc />
    public IFolder Folder(int id)
        => _CodeApiSvc?.Cdf?.Folder(id);

    /// <inheritdoc />
    public IFolder Folder(IField field)
        => _CodeApiSvc?.Cdf.Folder(field.Parent, field.Name, field);
}