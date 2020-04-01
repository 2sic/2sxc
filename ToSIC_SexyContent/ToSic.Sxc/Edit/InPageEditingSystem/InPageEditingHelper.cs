using System.Web;
using Newtonsoft.Json;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Edit.InPageEditingSystem
{
    public partial class InPageEditingHelper : HasLog, IInPageEditingSystem
    {
        internal InPageEditingHelper(IBlockBuilder blockBuilder, ILog parentLog) : base("Edt", parentLog)
        {
            Enabled = blockBuilder.UserMayEdit;
            BlockBuilder = blockBuilder;
        }

        [PrivateApi]
        protected  IBlockBuilder BlockBuilder;

        #region Attribute-helper

        /// <inheritdoc/>
        public HtmlString Attribute(string name, string value)
            => !Enabled ? null : SexyContent.Html.Build.Attribute(name, value);

        /// <inheritdoc/>
        public HtmlString Attribute(string name, object value)
            => !Enabled ? null : SexyContent.Html.Build.Attribute(name, JsonConvert.SerializeObject(value));

        #endregion Attribute Helper

    }
}