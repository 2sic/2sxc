using ToSic.Sxc.Adam.Sys.Manager;

namespace ToSic.Sxc.Adam.Sys.Storage;

[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract class AdamStorage(string? logName = default) : ServiceBase(logName ?? "Adm.Base")
{
    public void Init(AdamManager manager) => Manager = manager;

    protected AdamManager Manager { get; private set; } = null!;

    /// <summary>
    /// Root of this container
    /// </summary>
    public string Root => GeneratePath("");


    /// <summary>
    /// Figure out the path to a subfolder within this container
    /// </summary>
    /// <param name="subFolder"></param>
    /// <returns></returns>
    protected abstract string GeneratePath(string subFolder);

    /// <summary>
    /// Get the folder specified in App.Settings (BasePath) combined with the module's ID
    /// </summary>
    /// <remarks>
    /// Will create the folder if it does not exist
    /// </remarks>
    public IFolder? Folder(string subFolder, bool autoCreate)
    {
        var l = Log.Fn<IFolder?>($"{nameof(Folder)}(\"{subFolder}\", {autoCreate})");
        var fld = Manager.Folder(GeneratePath(subFolder), autoCreate);
        return l.ReturnAsOk(fld);
    }


    /// <summary>
    /// Get a (root) folder object for this container
    /// </summary>
    /// <returns></returns>
    public IFolder? RootFolder(bool autoCreate = false)
        => _folder.Get(() => Folder("", autoCreate));
    private readonly GetOnce<IFolder?> _folder = new();
}