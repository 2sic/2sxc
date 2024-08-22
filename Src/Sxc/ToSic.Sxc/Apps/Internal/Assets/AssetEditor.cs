using System.IO;
using System.Text.RegularExpressions;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Integration;
using ToSic.Eav.Apps.Internal.Specs;
using ToSic.Eav.Apps.Internal.Work;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps.Internal.Work;
using ToSic.Sxc.Blocks.Internal;

namespace ToSic.Sxc.Apps.Internal.Assets;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AssetEditor(
    GenWorkPlus<WorkViews> workViews,
    IUser user,
    LazySvc<AppFolderInitializer> appFolderInitializer,
    ISite site,
    IAppPathsMicroSvc appPaths)
    : ServiceBase("Sxc.AstEdt", connect: [user, appFolderInitializer, workViews, site, appPaths])
{

    #region Constructor / DI

    private IAppSpecs _appSpecs;

    private AssetEditInfo EditInfo { get; set; }

    private IAppPaths _appPaths;


    public AssetEditor Init(IAppReader appReader, string path, bool global, int viewId)
    {
        _appSpecs = appReader.Specs;
        _appPaths = appPaths.Get(appReader, site);
        EditInfo = new(_appSpecs.AppId, _appSpecs.Name, path, global);
        if (viewId == 0) return this;

        var view = workViews.New(appReader).Get(viewId);
        AddViewDetailsAndTypes(EditInfo, view);
        return this;
    }

    #endregion

    public AssetEditInfo EditInfoWithSource
    {
        get
        {
            EditInfo.Code = Source; // do this later, because it relies on the edit-info to exist
            return EditInfo;
        }
    }

    /// <summary>
    /// Check permissions and if not successful, give detailed explanation
    /// </summary>
    public void EnsureUserMayEditAssetOrThrow(string fullPath = null)
    {
        // check super user permissions - then all is allowed
        if (user.IsSystemAdmin) return;

        // ensure current user is admin - this is the minimum of not super-user
        if (!user.IsSiteAdmin) throw new AccessViolationException("current user may not edit templates, requires admin rights");

        // if not super user, check if razor (not allowed; super user only)
        if (!EditInfo.IsSafe)
            throw new AccessViolationException("current user may not edit razor templates - requires super user");

        // if not super user, check if cross-portal storage (not allowed; super user only)
        if (EditInfo.IsShared)
            throw new AccessViolationException(
                "current user may not edit templates in central storage - requires super user");

        // optionally check if the file is really in the portal
        if (fullPath == null) return;

        var path = new FileInfo(fullPath);
        if (path.Directory == null)
            throw new AccessViolationException("path is null");

        if (path.Directory.FullName.IndexOf(_appPaths.PhysicalPath, StringComparison.InvariantCultureIgnoreCase) != 0)
            throw new AccessViolationException("current user may not edit files outside of the app-scope");
    }

    private static AssetEditInfo AddViewDetailsAndTypes(AssetEditInfo template, IView view)
    {
        // Template specific properties, not really available in other files
        template.Type = view.Type;
        template.Name = view.Name;
        template.HasList = view.UseForList;
        template.TypeContent = view.ContentType;
        template.TypeContentPresentation = view.PresentationType;
        template.TypeList = view.HeaderType;
        template.TypeListPresentation = view.HeaderPresentationType;
        return template;
    }

    public string InternalPath => _internalPath ??= NormalizePath(Path.Combine(_appPaths.PhysicalPathSwitch(EditInfo.IsShared), EditInfo.FileName));
    private string _internalPath;

    private static string NormalizePath(string path) => Path.GetFullPath(new Uri(path).LocalPath);

    /// <summary>
    /// Read / Write the source code of the template file
    /// </summary>
    public string Source
    {
        get
        {
            EnsureUserMayEditAssetOrThrow(InternalPath);
            if (File.Exists(InternalPath))
                return File.ReadAllText(InternalPath);

            throw new FileNotFoundException("could not find file"
                                            + (user.IsSystemAdmin ? $" for superuser - file tried '{InternalPath}'" : ""));
        }
        set
        {
            EnsureUserMayEditAssetOrThrow(InternalPath);
            if (File.Exists(InternalPath))
                File.WriteAllText(InternalPath, value);
            else
                throw new FileNotFoundException("could not find file"
                                                + (user.IsSystemAdmin ? $" for superuser - file tried '{InternalPath}'" : ""));
        }
    }

    public bool Create(string contents)
    {
        // don't create if it already exits
        if (SanitizeFileNameAndCheckIfAssetAlreadyExists()) return false;

        // ensure the web.config exists (usually missing in the global area)
        appFolderInitializer.Value.EnsureTemplateFolderExists(_appSpecs.Folder, EditInfo.IsShared);

        var absolutePath = InternalPath;

        EnsureFolders(absolutePath);

        // now create the file
        CreateAsset(absolutePath, contents);

        return true;
    }

    private void SanitizeFileName()
    {
        // todo: maybe add some security for special dangerous file names like .cs, etc.?
        EditInfo.FileName = Regex.Replace(EditInfo.FileName, @"[?:\/*""<>|]", "");
    }

    // check if the folder already exists, or create it...
    private static void EnsureFolders(string absolutePath)
    {
        var foundFolder = absolutePath.LastIndexOf("\\", StringComparison.InvariantCulture);
        if (foundFolder > -1)
        {
            var folderPath = absolutePath.Substring(0, foundFolder);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
        }
    }

    private static void CreateAsset(string absolutePath, string contents)
    {
        using var stream = new StreamWriter(File.Create(absolutePath));
        stream.Write(contents);
        stream.Flush();
        stream.Close();
    }

    public bool SanitizeFileNameAndCheckIfAssetAlreadyExists()
    {
        SanitizeFileName();
        return File.Exists(InternalPath);
    }
}