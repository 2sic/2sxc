using Newtonsoft.Json;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Web;
#if NET472
using HtmlString = System.Web.HtmlString;
#else
using HtmlString = Microsoft.AspNetCore.Html.HtmlString;
#endif

namespace ToSic.Sxc.Edit.InPageEditingSystem
{
    public partial class InPageEditingHelper : HasLog, IInPageEditingSystem
    {
        internal InPageEditingHelper(IBlock block, ILog parentLog = null) : base("Edt", parentLog ?? block?.Log)
        {
            Block = block;
            Enabled = Block?.Context.UserMayEdit ?? false;
        }

        protected IBlock Block;

        #region Attribute-helper

        /// <inheritdoc/>
        public HtmlString Attribute(string name, string value)
            => !Enabled ? null : Build.Attribute(name, value);

        /// <inheritdoc/>
        public HtmlString Attribute(string name, object value)
            => !Enabled ? null : Build.Attribute(name, JsonConvert.SerializeObject(value));

        #endregion Attribute Helper

    }
}
