using System.Linq;
using Newtonsoft.Json;
using ToSic.Eav.Data.Shared;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Edit.ClientContextInfo;
using ToSic.Sxc.Web.PageFeatures;

namespace ToSic.Sxc.Web.JsContext
{
    public class JsContextAll : HasLog
    {
        public JsContextEnvironment Environment;
        public JsContextUser User;
        public JsContextLanguage Language;
        
        [JsonProperty("contentBlockReference")]
        public ContentBlockReferenceDto ContentBlockReference; // todo: still not sure if these should be separate...
        
        [JsonProperty("contentBlock")]
        public ContentBlockDto ContentBlock;
        // ReSharper disable once InconsistentNaming
        public ErrorDto error;

        public UiDto Ui;

        public JsContextAll(JsContextLanguage jsLangCtx) : base("Sxc.CliInf") => _jsLangCtx = jsLangCtx;
        private readonly JsContextLanguage _jsLangCtx;

        public JsContextAll Init(string systemRootUrl, IBlock block, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            var ctx = block.Context;

            Environment = new JsContextEnvironment(systemRootUrl, ctx);
            Language = _jsLangCtx.Init(ctx.Site, block.ZoneId);

            // New in v13 - if the view is from remote, don't allow design
            var view = block.View; // can be null
            var blockCanDesign = (view?.Entity.HasAncestor() ?? false) ? (bool?)false : null;

            User = new JsContextUser(ctx.User, blockCanDesign);

            ContentBlockReference = new ContentBlockReferenceDto(block, ctx.Publishing.Mode);
            ContentBlock = new ContentBlockDto(block);
            var autoToolbar = ctx.UserMayEdit;

            // If auto toolbar is false / not certain, and we have features activated...
            // find out if the Toolbars-Auto is enabled, in which case we should activate them
            if (!autoToolbar && block.BlockFeatureKeys.Any())
            {
                var features = block.Context.PageServiceShared.PageFeatures.GetWithDependents(block.BlockFeatureKeys, Log);
                autoToolbar = features.Contains(BuiltInFeatures.ToolbarsAutoInternal);
            }

            Ui = new UiDto(autoToolbar);

            error = new ErrorDto(block);
            return this;
        }
    }
}
