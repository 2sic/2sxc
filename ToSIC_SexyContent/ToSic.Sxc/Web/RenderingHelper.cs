using System;
using System.Web;
using Newtonsoft.Json;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Web.JsContext;

namespace ToSic.Sxc.Web
{
    public abstract class RenderingHelper: HasLog, IRenderingHelper
    {
        #region Constructors and DI

        protected RenderingHelper(IHttp http, string logName) : base(logName)
        {
            Http = http;
        }

        protected readonly IHttp Http;

        public IRenderingHelper Init(IBlockBuilder blockBuilder, ILog parentLog)
        {
            this.LinkLog(parentLog);
            var appRoot = Http.ToAbsolute("~/"); //  VirtualPathUtility.ToAbsolute("~/");
            BlockBuilder = blockBuilder;
            Context = blockBuilder?.Context;
            AppRootPath = appRoot;

            return this;
        }

        #endregion


        protected IInstanceContext Context;
        protected IBlockBuilder BlockBuilder;
        protected string AppRootPath;


        public string WrapInContext(string content,
            string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter,
            int instanceId = 0,
            int contentBlockId = 0,
            bool editContext = false,
            string tag = Constants.DefaultContextTag,
            bool autoToolbar = false,
            bool addLineBreaks = true)
        {
            Eav.Constants.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, "ContextAttributes", $"{nameof(instanceId)},{nameof(contentBlockId)},{nameof(editContext)},{nameof(tag)},{nameof(autoToolbar)},{nameof(addLineBreaks)}");

            var contextAttribs = ContextAttributes(instanceId, contentBlockId, editContext);

            var lineBreaks = addLineBreaks ? "\n" : "";

            return $"<{tag} class='{Constants.ClassToMarkContentBlock}' {contextAttribs}>{lineBreaks}" +
                   $"{content}" +
                   $"{lineBreaks}</{tag}>";
        }

        public string ContextAttributes(int instanceId, int contentBlockId, bool includeEditInfos)
        {
            var contextAttribs = "";
            if (instanceId != 0) contextAttribs += $" data-cb-instance='{instanceId}'";

            if (contentBlockId != 0) contextAttribs += $" data-cb-id='{contentBlockId}'";

            // optionally add editing infos
            if (includeEditInfos) contextAttribs += Build.Attribute("data-edit-context", UiContextInfos());
            return contextAttribs;
        }

        public string DesignErrorMessage(Exception ex, bool addToEventLog, string visitorAlternateError, bool addMinimalWrapper, bool encodeMessage)
        {
            var intro = "Error";
            var msg = intro + ": " + ex;
            if (addToEventLog)
                LogToEnvironmentExceptions(ex);

            if (!Context.User.IsSuperUser)
                msg = visitorAlternateError ?? "error showing content";

            if (encodeMessage)
                msg = HttpUtility.HtmlEncode(msg);

            // add dnn-error-div-wrapper
            msg = "<div class='dnnFormMessage dnnFormWarning'>" + msg + "</div>";

            // add another, minimal id-wrapper for those cases where the rendering-wrapper is missing
            if (addMinimalWrapper)
                msg = WrapInContext(msg, instanceId: Context.Container.Id, contentBlockId: Context.Container.Id);

            return msg;
        }

        public string UiContextInfos()
            => JsonConvert.SerializeObject(new JsContextAll(AppRootPath, Context, BlockBuilder, Log));


        protected abstract void LogToEnvironmentExceptions(Exception ex);
    }
}
