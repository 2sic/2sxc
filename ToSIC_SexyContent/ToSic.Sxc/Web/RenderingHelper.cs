using System;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Web
{
    public abstract class RenderingHelper: HasLog, IRenderingHelpers
    {
        protected RenderingHelper(string logName) : base(logName) { }

        public abstract IRenderingHelpers Init(IBlockBuilder blockBuilder, ILog parentLog);

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

        public abstract string ContextAttributes(int instanceId, int contentBlockId, bool includeEditInfos);

        public abstract string DesignErrorMessage(Exception ex, bool addToEventLog, string visitorAlternateError, bool addMinimalWrapper,
            bool encodeMessage);


        protected abstract void LogToEnvironmentExceptions(Exception ex);
    }
}
