using System;
using System.Text.Json;
using System.Web;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.DI;
using ToSic.Eav.Logging;
using ToSic.Eav.Serialization;
using ToSic.Sxc.Context;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.JsContext;

namespace ToSic.Sxc.Blocks.Output
{
    public class RenderingHelper: HasLog, IRenderingHelper
    {
        #region Constructors and DI

        public RenderingHelper(ILinkPaths linkPaths, IEnvironmentLogger errorLogger, Generator<JsContextAll> jsContextAllGen) : base("Sxc.RndHlp")
        {
            _linkPaths = linkPaths;
            _errorLogger = errorLogger;
            _jsContextAllGen = jsContextAllGen;
        }

        private readonly ILinkPaths _linkPaths;
        private readonly IEnvironmentLogger _errorLogger;
        private readonly Generator<JsContextAll> _jsContextAllGen;

        public IRenderingHelper Init(IBlock block, ILog parentLog)
        {
            this.LinkLog(parentLog);
            //var appRoot = _linkPaths.ToAbsolute("~/");
            Block = block;
            Context = block.Context;
            AppRootPath = _linkPaths.AsSeenFromTheDomainRoot("~/");

            return this;
        }

        #endregion

        public const string DefaultVisitorError = "Error Showing Content - please login as admin for details.";

        protected IContextOfBlock Context;
        protected IBlockBuilder BlockBuilder;
        protected IBlock Block;
        protected string AppRootPath;


        public string WrapInContext(string content,
            string noParamOrder = Eav.Parameters.Protector,
            int instanceId = 0,
            int contentBlockId = 0,
            bool editContext = false,
            string tag = Constants.DefaultContextTag,
            // bool autoToolbar = false,
            bool addLineBreaks = true)
        {
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, "ContextAttributes", $"{nameof(instanceId)},{nameof(contentBlockId)},{nameof(editContext)},{nameof(tag)},{nameof(addLineBreaks)}");

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

        public string DesignErrorMessage(Exception ex, bool addToEventLog, string visitorAlternateError = null,
            string additionalInfo = null, bool addContextWrapper = false, bool encodeMessage = true)
        {
            const string prefix = "Error: ";
            var msg = prefix + ex + additionalInfo;
            if (addToEventLog) _errorLogger?.LogException(ex);

            if (!Context.User.IsSystemAdmin)
                msg = visitorAlternateError ?? DefaultVisitorError;

            return DesignMessage(msg, addContextWrapper, encodeMessage);
        }
        public string DesignError(string msgSuperUser, string msgVisitors = null, bool addContextWrapper = false, bool encodeMessage = true)
        {
            msgSuperUser = "Error: " + msgSuperUser;

            if (!Context.User.IsSystemAdmin)
                msgSuperUser = msgVisitors ?? DefaultVisitorError;

            return DesignMessage(msgSuperUser, addContextWrapper, encodeMessage);
        }

        private string DesignMessage(string msg, bool addContextWrapper, bool encodeMessage)
        {
            if (encodeMessage)
                msg = HttpUtility.HtmlEncode(msg);

            // add dnn-error-div-wrapper
            msg = $"<div class='dnnFormMessage dnnFormWarning'>{msg}</div>";

            // add another, minimal id-wrapper for those cases where the rendering-wrapper is missing
            if (addContextWrapper)
                msg = WrapInContext(msg, instanceId: Context.Module.Id, contentBlockId: Context.Module.Id);

            return msg;
        }

        public string DesignWarningForSuperUserOnly(string warning, bool addContextWrapper = false, bool encodeMessage = true)
        {
            if (!Context.User.IsSystemAdmin) return null;
            return DesignMessage($"Warning: {warning}", addContextWrapper, encodeMessage);
        }

        public string UiContextInfos() => JsonSerializer.Serialize(_jsContextAllGen.New.Init(AppRootPath, Block, Log), JsonOptions.SafeJsonForHtmlAttributes);

    }
}
