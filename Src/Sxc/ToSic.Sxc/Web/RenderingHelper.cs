﻿using System;
using System.Web;
using Newtonsoft.Json;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Run;

using ToSic.Sxc.Web.JsContext;

namespace ToSic.Sxc.Web
{
    public class RenderingHelper: HasLog, IRenderingHelper
    {
        #region Constructors and DI

        public RenderingHelper(ILinkPaths linkPaths, IEnvironmentLogger errorLogger) : base("Sxc.RndHlp")
        {
            _linkPaths = linkPaths;
            _errorLogger = errorLogger;
        }

        private readonly ILinkPaths _linkPaths;
        private readonly IEnvironmentLogger _errorLogger;

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


        protected IContextOfBlock Context;
        protected IBlockBuilder BlockBuilder;
        protected IBlock Block;
        protected string AppRootPath;


        public string WrapInContext(string content,
            string dontRelyOnParameterOrder = Eav.Parameters.Protector,
            int instanceId = 0,
            int contentBlockId = 0,
            bool editContext = false,
            string tag = Constants.DefaultContextTag,
            bool autoToolbar = false,
            bool addLineBreaks = true)
        {
            Eav.Parameters.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, "ContextAttributes", $"{nameof(instanceId)},{nameof(contentBlockId)},{nameof(editContext)},{nameof(tag)},{nameof(autoToolbar)},{nameof(addLineBreaks)}");

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
            if (addToEventLog) _errorLogger?.LogException(ex);

            if (!Context.User.IsSuperUser)
                msg = visitorAlternateError ?? "error showing content";

            if (encodeMessage)
                msg = HttpUtility.HtmlEncode(msg);

            // add dnn-error-div-wrapper
            msg = "<div class='dnnFormMessage dnnFormWarning'>" + msg + "</div>";

            // add another, minimal id-wrapper for those cases where the rendering-wrapper is missing
            if (addMinimalWrapper)
                msg = WrapInContext(msg, instanceId: Context.Module.Id, contentBlockId: Context.Module.Id);

            return msg;
        }

        public string UiContextInfos() => JsonConvert.SerializeObject(Context.ServiceProvider.Build<JsContextAll>().Init(AppRootPath, Block, Log));

    }
}
