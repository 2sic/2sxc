using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Web.MvcPipeline.ModuleControl;
using DotNetNuke.Web.MvcPipeline.ModuleControl.Page;
using DotNetNuke.Web.MvcPipeline.ModuleControl.Razor;
using System.Web.Http;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.Services;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Dnn.Mvc;
public class SxcMvcModuleControl : RazorModuleControlBase, IActionable, IPageContributor
{
    private const string DefaultViewVirtualPath = "~/DesktopModules/ToSic.Sxc/Views/View.cshtml";

    protected override string DefaultViewName => DefaultViewVirtualPath;

    public override IRazorModuleResult Invoke()
        => _moduleResult ??= BuildModuleResult();

    private IRazorModuleResult BuildModuleResult()
    {
        Runtime.BeginRequest(ModuleConfiguration?.ModuleTitle ?? "");

        try
        {
            Runtime.Prepare();
            _renderState = Runtime.Render(renderNaked: false);
            return View(DefaultViewVirtualPath, new SxcMvcViewModel(_renderState.RenderResult.Html, IsError: false));
        }
        catch (Exception ex)
        {
            Exceptions.LogException(ex);
            return View(DefaultViewVirtualPath, new SxcMvcViewModel(BuildFriendlyErrorHtml(ex), IsError: true));
        }
    }

    public void ConfigurePage(PageConfigurationContext context)
    {
        _ = Invoke();
        if (_renderState?.RenderResult == null)
            return;

        try
        {
            Runtime.GetService<DnnPageChanges>(Runtime.Log).ApplyMvc(context, _renderState.RenderResult);
        }
        catch
        {
            /* ignore */
        }

        try
        {
            Runtime.GetService<DnnClientResources>(Runtime.Log).AddEverythingMvc(context, _renderState.RenderResult.Features);
        }
        catch
        {
            /* ignore */
        }
    }

    public ModuleActionCollection ModuleActions => Actions ??= GetModuleActions();

    private ModuleActionCollection GetModuleActions()
    {
        var l = Runtime.Log.Fn<ModuleActionCollection>();
        try
        {
            var result = new SxcModuleActionsBuilder(Runtime.GetService<IUser>(Runtime.Log))
                .Build(ModuleConfiguration, Runtime.Block, ModuleId, GetNextActionID,
                    key => Localization.GetString(key, LocalResourceFile) ?? key);
            return l.Return(result);
        }
        catch (Exception e)
        {
            Exceptions.LogException(e);
            return l.ReturnAsError([]);
        }
    }

    private string BuildFriendlyErrorHtml(Exception ex)
    {
        try
        {
            return Runtime.BuildFriendlyErrorHtml(ex);
        }
        catch
        {
            return "Something went really wrong in MVC module rendering - check error logs";
        }
    }

    private string HtmlLog()
        => Runtime.Log.Dump(" - ", "<!-- 2sxc insights for " + ModuleId + "\n", "-->");

    private string GetOptionalDetailedLogToAttach()
    {
        try
        {
            if (Request.QueryString["debug"] == "true")
                if (UserInfo?.IsSuperUser == true
                    || DnnLogging.EnableLogging(GlobalConfiguration.Configuration.Properties))
                    return HtmlLog();
        }
        catch
        {
            /* ignore */
        }

        return "";
    }

    private ModuleViewRuntime Runtime => field ??= new(ModuleConfiguration, ModuleId, TabId, DnnStaticDi.CreateModuleScopedServiceProvider(), GetOptionalDetailedLogToAttach);

    private ModuleViewRenderState _renderState;
    private IRazorModuleResult _moduleResult;
}
