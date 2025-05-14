using ToSic.Sxc.Adam;
using ToSic.Sxc.Adam.Internal;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Services;

[PrivateApi("hide implementation")]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class AdamService: IAdamService, INeedsCodeApiService
{
    #region Constructor etc.

    [PrivateApi]        
    public void ConnectToRoot(ICodeApiService codeRoot) => _codeRoot = codeRoot;
    private ICodeApiService _codeRoot;

    #endregion

    /// <inheritdoc />
    public IFile File(int id)
        => (_codeRoot as ICodeApiServiceInternal)?.Cdf?.File(id);

    /// <inheritdoc />
    public IFile File(string id)
    {
        var fileId = LinkParts.CheckIdStringForId(id);
        return fileId == null ? null : File(fileId.Value);
    }


    /// <inheritdoc />
    public IFile File(IField field)
    {
        var file = field?.Raw is string id ? File(id) : null;
        if (file != null) file.Field = field;
        return file;
    }

    /// <inheritdoc />
    public IFolder Folder(int id)
        => (_codeRoot as ICodeApiServiceInternal)?.Cdf?.Folder(id);

    /// <inheritdoc />
    public IFolder Folder(IField field) => _codeRoot?.AsAdam(field.Parent, field.Name);
}