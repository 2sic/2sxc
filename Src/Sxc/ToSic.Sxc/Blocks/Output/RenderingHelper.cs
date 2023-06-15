using System;
using System.Text.Json;
using System.Web;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Serialization;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Context;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.JsContext;
using static ToSic.Sxc.Blocks.BlockBuildingConstants;

namespace ToSic.Sxc.Blocks.Output
{
    public class RenderingHelper: ServiceBase, IRenderingHelper
    {
        #region Constructors and DI

        public RenderingHelper(
            ILinkPaths linkPaths,
            LazySvc<IEnvironmentLogger> errorLogger,
            Generator<JsContextAll> jsContextAllGen) : base("Sxc.RndHlp")
        {
            ConnectServices(_linkPaths = linkPaths,
                _errorLogger = errorLogger,
                _jsContextAllGen = jsContextAllGen
            );
        }

        private readonly ILinkPaths _linkPaths;
        private readonly LazySvc<IEnvironmentLogger> _errorLogger;
        private readonly Generator<JsContextAll> _jsContextAllGen;

        public IRenderingHelper Init(IBlock block)
        {
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
            bool addLineBreaks = true,
            string errorCode = default,
            Exception exOrNull = default
        )
        {
            Eav.Parameters.Protect(noParamOrder, $"{nameof(instanceId)},{nameof(contentBlockId)},{nameof(editContext)},{nameof(tag)},{nameof(addLineBreaks)}");

            var contextAttribs = ContextAttributes(instanceId, contentBlockId, editContext, errorCode, exOrNull);

            var lineBreaks = addLineBreaks ? "\n" : "";

            return $"<{tag} class='{Constants.ClassToMarkContentBlock}' {contextAttribs}>{lineBreaks}" +
                   $"{content}" +
                   $"{lineBreaks}</{tag}>";
        }

        private string ContextAttributes(int instanceId, int contentBlockId, bool includeEditInfos, string errorCode, Exception exOrNull)
        {
            var contextAttribs = "";
            if (instanceId != 0) contextAttribs += $" data-cb-instance='{instanceId}'";

            if (contentBlockId != 0) contextAttribs += $" data-cb-id='{contentBlockId}'";

            // optionally add editing infos
            var context = _jsContextAllGen.New().GetJsContext(AppRootPath, Block, errorCode, exOrNull);
            var contextInfos = JsonSerializer.Serialize(context, JsonOptions.SafeJsonForHtmlAttributes);
            if (includeEditInfos) contextAttribs += Build.Attribute("data-edit-context", contextInfos);
            return contextAttribs;
        }

        private const string ErrPrefix = "Error:";
        private const string WarnPrefix = "Warning:";

        public string DesignErrorMessage(Exception ex, bool addToEventLog, string msgVisitors = null,
            string additionalInfo = null, bool addContextWrapper = false, bool encodeMessage = true)
        {
            if (addToEventLog) _errorLogger.Value.LogException(ex);
            return DesignError($"{ex}{additionalInfo}", msgVisitors, addContextWrapper, encodeMessage);

            //var msg = Context.User.IsSystemAdmin
            //    ? $"{ErrPrefix} {ex}{additionalInfo}"
            //    : msgVisitors ?? DefaultVisitorError;

            //return DesignMessage(msg, addContextWrapper, encodeMessage, ErrorGeneral);
        }
        public string DesignError(string msgSuperUser, string msgVisitors = null, bool addContextWrapper = false, bool encodeMessage = true)
        {
            var msg = Context.User.IsSystemAdmin
                ? $"{ErrPrefix} {msgSuperUser}"
                : msgVisitors ?? DefaultVisitorError;
            return DesignMessage(msg, addContextWrapper, encodeMessage, ErrorGeneral);
        }

        private string DesignMessage(string msg, bool addContextWrapper, bool encodeMessage, string errorCode)
        {
            if (encodeMessage)
                msg = HttpUtility.HtmlEncode(msg);

            // add dnn-error-div-wrapper together with a special HTML marker so errors can handled better
            msg = $"<div class='dnnFormMessage dnnFormWarning'>{ErrorHtmlMarker}{msg}</div>";

            // add another, minimal id-wrapper for those cases where the rendering-wrapper is missing
            if (addContextWrapper)
                msg = WrapInContext(msg, instanceId: Context.Module.Id, contentBlockId: Context.Module.Id, errorCode: errorCode);

            return msg;
        }

        public string DesignWarningForSuperUserOnly(string warning, bool addContextWrapper = false, bool encodeMessage = true) =>
            Context.User.IsSystemAdmin 
                ? DesignMessage($"{WarnPrefix} {warning}", addContextWrapper, encodeMessage, null) 
                : null;

        //public string UiContextInfos(string errorCode)
        //{
        //    var context = _jsContextAllGen.New().GetJsContext(AppRootPath, Block, errorCode);
        //    return JsonSerializer.Serialize(context, JsonOptions.SafeJsonForHtmlAttributes);
        //}
    }
}
