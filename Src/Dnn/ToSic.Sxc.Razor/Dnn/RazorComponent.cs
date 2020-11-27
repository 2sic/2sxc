using ToSic.Eav.Documentation;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.Web;


namespace ToSic.Sxc.Dnn
{
    /// <summary>
    /// The base class for Razor-Components in 2sxc 10+ <br/>
    /// Provides context infos like the Dnn object, helpers like Edit and much more. <br/>
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public abstract partial class RazorComponent : Hybrid.Razor.RazorComponent, IRazorComponent
    {

        #region Link, Edit, Dnn, App, Data

        ///// <inheritdoc />
        //public ILinkHelper Link => DynCode.Link;

        ///// <inheritdoc />
        //public IInPageEditingSystem Edit => DynCode.Edit;

        /// <inheritdoc />
        public IDnnContext Dnn => DynCode.Dnn;

        //[PrivateApi] public IBlock Block => throw new NotSupportedException("don't use this");

        //[PrivateApi] public int CompatibilityLevel => DynCode.CompatibilityLevel;

        ///// <inheritdoc />
        //public new IApp App => DynCode.App;

        ///// <inheritdoc />
        //public IBlockDataSource Data => DynCode.Data;

        #endregion
    }
}
