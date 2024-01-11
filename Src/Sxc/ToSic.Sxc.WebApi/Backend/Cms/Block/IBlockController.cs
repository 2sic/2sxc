using ToSic.Eav.Apps.Internal.Ui;
using ToSic.Sxc.Backend.InPage;

namespace ToSic.Sxc.Backend.Cms;

public interface IBlockController
{
    /// <summary>
    /// used to be GET Module/GenerateContentBlock
    /// </summary>
    string Block(int parentId, string field, int index, string app = "", Guid? guid = null);

    /// <summary>
    /// used to be GET Module/AddItem
    /// </summary>
    void Item(int? index = null);

    /// <summary>
    /// used to be GET Module/SetAppId
    /// </summary>
    /// <param name="appId"></param>
    /*new*/
    void App(int? appId);

    /// <summary>
    /// used to be GET Module/GetSelectableApps
    /// </summary>
    /// <param name="apps"></param>
    /// <returns></returns>
    IEnumerable<AppUiInfo> Apps(string apps = null);

    /// <summary>
    /// used to be GET Module/GetSelectableContentTypes
    /// </summary>
    /// <returns></returns>
    IEnumerable<ContentTypeUiInfo> ContentTypes();

    /// <summary>
    /// used to be GET Module/GetSelectableTemplates
    /// </summary>
    /// <returns></returns>
    IEnumerable<TemplateUiInfo> Templates();

    /// <summary>
    /// Used in InPage.js
    /// used to be GET Module/SaveTemplateId
    /// </summary>
    /// <param name="templateId"></param>
    /// <param name="forceCreateContentGroup"></param>
    /// <returns></returns>
    Guid? Template(int templateId, bool forceCreateContentGroup);

    /// <summary>
    /// used to be GET Module/RenderTemplate
    /// js changed
    /// </summary>
    /// <summary>
    /// Used in InPage.js
    /// </summary>
    AjaxRenderDto Render(int templateId, string lang, string edition);

    /// <summary>
    /// Used to be GET Module/Publish
    /// </summary>
    bool Publish(string part, int index);
}