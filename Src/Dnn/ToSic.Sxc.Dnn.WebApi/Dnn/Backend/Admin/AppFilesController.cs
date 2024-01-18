using ToSic.Eav.WebApi.Assets;
using ToSic.Sxc.Apps.Internal.Assets;
using ToSic.Sxc.Backend.Admin.AppFiles;
using RealController = ToSic.Sxc.Backend.Admin.AppFiles.AppFilesControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Admin;

/// <summary>
/// This one supplies portal-wide (or cross-portal) settings / configuration
/// </summary>
[SupportedModules(DnnSupportedModuleNames)]
[DnnLogExceptions]
[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
[ValidateAntiForgeryToken]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AppFilesController() : DnnSxcControllerBase(RealController.LogSuffix), IAppFilesController
{
    private RealController Real => SysHlp.GetService<RealController>();

    [HttpGet]
    public List<string> All(int appId, bool global, string path = null, string mask = "*.*", bool withSubfolders = false, bool returnFolders = false) 
        => Real.All(appId, global, path, mask, withSubfolders, returnFolders);

    [HttpGet]
    public AssetEditInfo Asset(int appId, 
        int templateId = 0, string path = null, // identifier is always one of these two
        bool global = false)
        => Real.Asset(appId, templateId, path, global);

    [HttpPost]
    public bool Create(
        [FromUri] int appId,
        [FromUri] string path,
        [FromUri] bool global,
        [FromUri] string templateKey)
        => Real.Create(appId, path, global, templateKey);

    [HttpPost]
    public bool Asset(
        [FromUri] int appId, 
        [FromBody] AssetEditInfo template,
        [FromUri] int templateId = 0, 
        [FromUri] string path = null, // identifier is either template Id or path
        // todo w/SPM - global never seems to be used - must check why and if we remove or add to UI
        // TODO: NEW PARAM TEMPLATEKey SHOULD BE USED TO CREATE THE FILE
        [FromUri] bool global = false) 
        => Real.Asset(appId: appId, template: template, templateId: templateId, path: path, global: global);

    [HttpGet]
    public TemplatesDto GetTemplates(string purpose = null, string type = null) => Real.GetTemplates(purpose, type);

    [HttpGet]
    public TemplatePreviewDto Preview(int appId, string path, string templateKey, bool global = false)
        => Real.Preview(appId, path, templateKey, global);

    [HttpGet]
    public AllFilesDto AppFiles(int appId, string path = null, string mask = null) => Real.AppFiles(appId, path, mask);
}