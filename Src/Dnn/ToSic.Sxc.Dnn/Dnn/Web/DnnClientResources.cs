using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using DotNetNuke.Application;
using DotNetNuke.Framework;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Web.Client;
using DotNetNuke.Web.Client.ClientResourceManagement;
using DotNetNuke.Web.Client.Providers;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Edit;

namespace ToSic.Sxc.Dnn.Web
{
    public class DnnClientResources: HasLog
    {
        protected BlockBuilder BlockBuilder;
        protected Page Page;
        protected DnnJsApiHeader Header;

        public DnnClientResources(Page page, IBlockBuilder blockBuilder, ILog parentLog) : base("Dnn.JsCss", parentLog)
        {
            Page = page;
            BlockBuilder = blockBuilder as BlockBuilder;
            Header = new DnnJsApiHeader(Log);
        }

        public bool AddEverything()
        {
            var wrapLog = Log.Call<bool>();

            // normal scripts
            var editJs = BlockBuilder?.UiAddEditApi ?? false;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            var readJs = BlockBuilder?.UiAddJsApi ?? editJs;
            var editCss = BlockBuilder?.UiAddEditUi ?? false;

            if (!readJs && !editJs && !editCss)
                return wrapLog("nothing added", true);

            Log.Add("user is editor, or template requested js/css, will add client material");

            // register scripts and css
            RegisterClientDependencies(Page, readJs, editJs, editCss, BlockBuilder?.NamedScriptsWIP);

            // New in 11.11.02 - DNN has a strange behavior where the current language isn't known till PreRender
            // so we have to move adding the header to here.
            // MustAddHeaders may have been set earlier by the engine, or now by the various js added
            Log.Add($"{nameof(MustAddHeaders)}={MustAddHeaders}");
            if (MustAddHeaders) Header.AddHeaders();

            return wrapLog("ok", true);
        }


        public void EnsurePre1025Behavior()
        {
            // new in 10.25 - by default jQuery isn't loaded!
            // but any old behaviour, incl. no-view defined, etc. should activate compatibility
            var addAntiForgeryToken = BlockBuilder
                                          ?.GetEngine(Purpose.WebView)
                                          ?.CompatibilityAutoLoadJQueryAndRVT
                                      ?? true;
            if (!addAntiForgeryToken) return;

            // If we got this far, we want the old behavior which always enables headers etc.
            Log.Add(nameof(EnsurePre1025Behavior) + ": Activate Anti-Forgery for compatibility with old behavior");
            ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
            MustAddHeaders = true;
        }



        public void RegisterClientDependencies(Page page, bool readJs, bool editJs, bool editCss, List<string> namedScripts = null)
        {
            var wrapLog = Log.Call($"-, {nameof(readJs)}:{readJs}, {nameof(editJs)}:{editJs}, {nameof(editCss)}:{editCss}");
            var root = DnnConstants.SysFolderRootVirtual;
            root = page.ResolveUrl(root);
            var ver = Settings.Version.ToString();
            var priority = (int) FileOrder.Js.DefaultPriority - 2;

            // add edit-mode CSS
            if (editCss) RegisterCss(page, root + InpageCms.EditCss);

            // add read-js
            if (readJs || editJs)
            {
                Log.Add("add $2sxc api and headers");
                RegisterJs(page, ver, root + InpageCms.CoreJs, true, priority);
                MustAddHeaders = true;
            }

            // add edit-js (commands, manage, etc.)
            if (editJs)
            {
                Log.Add("add 2sxc edit api; also request jQuery and anti-forgery");
                // note: the inpage only works if it's not in the head, so we're adding it below
                RegisterJs(page, ver, root + InpageCms.EditJs, false, priority + 1);
                // request full $services and jQuery etc.
                JavaScript.RequestRegistration(CommonJs.jQuery);
                ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
            }

            if (namedScripts?.Contains(BlockBuilder.JsTurnOn) ?? false)
                RegisterJs(page, ver, root + InpageCms.TurnOnJs, true, priority + 10);

            wrapLog("ok");
        }


        #region DNN Bug with Current Culture

        // We must add the _js header but we must wait beyond the initial page-load until Pre-Render
        // Because for reasons unknown DNN (at least in V7.4+ but I think also in 9) doesn't have 
        // the right PortalAlias and language set until then. 
        // before that it assumes the PortalAlias is a the default alias, even if the url clearly shows another language

        private bool MustAddHeaders { get; set; }

        #endregion


        /// <summary>
        /// Return true if the URL is a debug URL
        /// </summary>
        private static bool IsDebugUrl(HttpRequest request) => string.IsNullOrEmpty(request.QueryString["debug"]);


        #region add scripts / css with bypassing the official ClientResourceManager

        private static void RegisterJs(Page page, string version, string path, bool toHead, int priority)
        {
            var url = $"{path}{(path.IndexOf('?') > 0 ? '&' : '?')}v={version}";
            if (toHead)
            {
                // don't add version in DNN 7 and probably 8, because it breaks the client-dependency - but only in the head
                if (DotNetNukeContext.Current.Application.Version.Major < 9) url = path;
                ClientResourceManager.RegisterScript(page, url, priority, DnnPageHeaderProvider.DefaultName);
            }
            else
                page.ClientScript.RegisterClientScriptInclude(typeof(Page), path, url);
        }

        private static void RegisterCss(Page page, string path)
            => ClientResourceManager.RegisterStyleSheet(page, path);

        #endregion



    }
}
