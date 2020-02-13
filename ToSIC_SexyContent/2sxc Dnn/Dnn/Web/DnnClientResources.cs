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

namespace ToSic.Sxc.Dnn.Web
{
    public class DnnClientResources: HasLog
    {
        protected BlockBuilder BlockBuilder;
        protected Page Page;
        protected DnnJsApiHeader Header;

        public DnnClientResources(Page page, BlockBuilder blockBuilder, ILog parentLog) : base("Dnn.JsCss", parentLog)
        {
            Page = page;
            BlockBuilder = blockBuilder;
            Header = new DnnJsApiHeader(Log);
        }

        public bool AddEverything()
        {
            var wrapLog = Log.Call<bool>();

            // new in 10.25 - by default jQuery isn't loaded!
            // 2020-01-13 disabled this, as the parent must call it in the right moment, not here
            //EnsurePre1025Behavior();

            // normal scripts
            var editJs = BlockBuilder?.UiAddEditApi ?? false;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            var readJs = BlockBuilder?.UiAddJsApi ?? editJs;
            var editCss = BlockBuilder?.UiAddEditUi ?? false;

            if (!readJs && !editJs && !editCss)
                return wrapLog("nothing added", true);

            Log.Add("user is editor, or template requested js/css, will add client material");

            // register scripts and css
            RegisterClientDependencies(Page, readJs, editJs, editCss);

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
            Log.Add(nameof(EnsurePre1025Behavior) + ": Activate Anti-Forgery for compatibility with old behavior");
            ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
            Header.AddHeaders();
        }



        public void RegisterClientDependencies(Page page, bool readJs, bool editJs, bool editCss)
        {
            var wrapLog = Log.Call($"-, {nameof(readJs)}:{readJs}, {nameof(editJs)}:{editJs}, {nameof(editCss)}:{editCss}");
            var root = "~/desktopmodules/tosic_sexycontent/";
            root = page.ResolveUrl(root);
            var ext = IsDebugUrl(page.Request) ? ".min.js" : ".js";
            var ver = Settings.Version.ToString();
            var priority = (int) FileOrder.Js.DefaultPriority - 2;

            // add edit-mode CSS
            if (editCss) RegisterCss(page, root + "dist/inpage/inpage.min.css");

            // add read-js
            if (readJs || editJs)
            {
                Log.Add("add $2sxc api and headers");
                RegisterJs(page, ver, root + "js/2sxc.api" + ext, true, priority);
                Header.AddHeaders();
            }

            // add edit-js (commands, manage, etc.)
            if (editJs)
            {
                Log.Add("add 2sxc edit api; also request jQuery and anti-forgery");
                // note: the inpage only works if it's not in the head, so we're adding it below
                RegisterJs(page, ver, root + "dist/inpage/inpage.min.js", false, priority + 1);
                // request full $services and jQuery etc.
                JavaScript.RequestRegistration(CommonJs.jQuery);
                ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
            }

            wrapLog("ok");
        }


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
