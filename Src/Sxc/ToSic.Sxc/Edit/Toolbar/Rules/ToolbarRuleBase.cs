using ToSic.Sxc.Web;

namespace ToSic.Sxc.Edit.Toolbar
{
    public class ToolbarRuleBase: HybridHtmlString
    {
        #region Key Constants

        //protected const string KeyEntityId = "entityId";
        //protected const string KeyContentType = "contentType";

        #endregion

        protected ToolbarRuleBase(): base(string.Empty) {}

        protected ToolbarRuleBase(string rule): base(rule) { }


        public ToolbarContext Context { get; protected set; }

    }
}
