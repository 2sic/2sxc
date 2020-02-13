using System;
using System.Web;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using Newtonsoft.Json;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Run;
using ToSic.Sxc.Dnn.Web.ClientInfos;
using ToSic.Sxc.Interfaces;

namespace ToSic.Sxc.Dnn.Web
{
    public class DnnRenderingHelpers : IHasLog, IRenderingHelpers
    {
        protected Blocks.IBlockBuilder BlockBuilder;
        private PortalSettings _portalSettings;
        private UserInfo _userInfo;
        private string _applicationRoot;
        private IContainer _moduleInfo;

        public ILog Log { get; } = new Log("Dnn.Render");

        // Blank constructor for IoC
        public DnnRenderingHelpers() { }

        public DnnRenderingHelpers(Blocks.IBlockBuilder blockBuilder, ILog parentLog) => Init(blockBuilder, parentLog);

        public IRenderingHelpers Init(Blocks.IBlockBuilder blockBuilder, ILog parentLog)
        {
            this.LinkLog(parentLog);
            var appRoot = VirtualPathUtility.ToAbsolute("~/");
            _moduleInfo = blockBuilder?.Container;
            BlockBuilder = blockBuilder;
            _portalSettings = PortalSettings.Current;

            _userInfo = PortalSettings.Current.UserInfo;
            _applicationRoot = appRoot;

            return this;
        }



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

            var contextAttribs = ContextAttributes(instanceId, contentBlockId, editContext, autoToolbar);

            var lineBreaks = addLineBreaks ? "\n" : "";

            return $"<{tag} class='{Constants.ClassToMarkContentBlock}' {contextAttribs}>{lineBreaks}"  +
                   $"{content}" +
                   $"{lineBreaks}</{tag}>";
        }


        public string ContextAttributes(int instanceId, int contentBlockId, bool includeEditInfos, bool autoToolbar)
        {
            var contextAttribs = "";
            if (instanceId != 0) contextAttribs += $" data-cb-instance='{instanceId}'";

            if (contentBlockId != 0) contextAttribs += $" data-cb-id='{contentBlockId}'";

            // optionally add editing infos
            if (includeEditInfos) contextAttribs += SexyContent.Html.Build.Attribute("data-edit-context", UiContextInfos(autoToolbar));
            return contextAttribs;
        }




        // new
        public string UiContextInfos(bool autoToolbars)
            => JsonConvert.SerializeObject(new ClientInfosAll(_applicationRoot, _portalSettings, _moduleInfo, BlockBuilder, _userInfo,
                BlockBuilder.Block.ZoneId // 2019-11-09, Id not nullable any more // ?? 0
                , BlockBuilder.Block.ContentGroupExists, autoToolbars, Log));



        public string DesignErrorMessage(Exception ex, bool addToEventLog, string visitorAlternateError, bool addMinimalWrapper, bool encodeMessage)
        {
            var intro = "Error";
            var msg = intro + ": " + ex;
            if (addToEventLog)
                Exceptions.LogException(ex);

            if (!_userInfo.IsSuperUser)
                msg = visitorAlternateError ?? "error showing content";

            if (encodeMessage)
                msg = HttpUtility.HtmlEncode(msg);

            // add dnn-error-div-wrapper
            msg = "<div class='dnnFormMessage dnnFormWarning'>" + msg + "</div>";

            // add another, minimal id-wrapper for those cases where the rendering-wrapper is missing
            if (addMinimalWrapper)
                msg = WrapInContext(msg, instanceId: _moduleInfo.Id, contentBlockId: _moduleInfo.Id);

            return msg;
        }

    }

}