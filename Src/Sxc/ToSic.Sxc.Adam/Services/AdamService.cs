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
    public IFile File(int id)
        => AdamManagerWithContext?.File(id);

    /// <inheritdoc />
    public IFile File(string id)
    {
        var fileId = LinkParts.CheckIdStringForId(id);
        return fileId == null ? null : File(fileId.Value);
    }


    /// <inheritdoc />
    public IFile File(IField field)
    {
        if (field?.Raw is not string id)
            return null;
        var file = File(id);
        file.Field = field;
        return file;
    }

    /// <inheritdoc />
    public IFolder Folder(int id)
        => AdamManagerWithContext?.Folder(id);

    /// <inheritdoc />
    public IFolder Folder(IField field)
        => AdamManagerWithContext?.Folder(field.Parent.Entity.EntityGuid, field.Name);

    //private ICodeDataFactory CdfOrNull => _cdf.Get(() => _CodeApiSvc?.Cdf);
    //private readonly GetOnce<ICodeDataFactory> _cdf = new();
    ///// <inheritdoc />
    //public IFile File(int id)
    //    => CdfOrNull?.File(id);

    ///// <inheritdoc />
    //public IFile File(string id)
    //{
    //    var fileId = LinkParts.CheckIdStringForId(id);
    //    return fileId == null
    //        ? null
    //        : File(fileId.Value);
    //}


    ///// <inheritdoc />
    //public IFile File(IField field)
    //{
    //    var file = field?.Raw is string id
    //        ? File(id)
    //        : null;
    //    if (file != null)
    //        file.Field = field;
    //    return file;
    //}

    ///// <inheritdoc />
    //public IFolder Folder(int id)
    //    => CdfOrNull?.Folder(id);

    ///// <inheritdoc />
    //public IFolder Folder(IField field)
    //    => CdfOrNull.Folder(field.Parent, field.Name, field);
}