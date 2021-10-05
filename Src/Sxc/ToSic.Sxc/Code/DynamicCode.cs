using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Code
{
    /// <summary>
    /// This is a base class for dynamic code which is compiled at runtime. <br/>
    /// It delegates all properties like App and methods like AsDynamic() to the parent item which initially caused it to be compiled.
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public abstract partial class DynamicCode : HasLog, IDynamicCode, IWrapper<IDynamicCode>, ICoupledDynamicCode
    {

        #region Constructor - NOT for DI

        /// <summary>
        /// Main constructor, may never have parameters, otherwise inheriting code will run into problems. 
        /// </summary>
        protected DynamicCode() : base("Sxc.DynCod")
        {

        }
        
        #endregion

        #region Dynamic Code Coupling

        [PrivateApi]
        public virtual void DynamicCodeCoupling(IDynamicCodeRoot parent)
        {
            Log.LinkTo(parent?.Log);
            Log.Call()(null);
            _contents = parent;
        }

        /// <inheritdoc />
        /// <remarks>
        /// The parent of this object. It's not called Parent but uses an exotic name to ensure that your code won't accidentally create a property with the same name.
        /// </remarks>
        public IDynamicCode UnwrappedContents => _contents;// { get; private set; }

        public IDynamicCode GetContents() => _contents;
        protected IDynamicCodeRoot _contents;

        [PrivateApi] public IDynamicCodeRoot _DynCodeRoot => _contents;// (UnwrappedContents as IHasDynamicCodeRoot)?._DynCodeRoot;

        #endregion

        /// <inheritdoc />
        public IApp App => UnwrappedContents?.App;

        /// <inheritdoc />
        public IBlockDataSource Data => UnwrappedContents?.Data;

        /// <inheritdoc />
        public TService GetService<TService>() => UnwrappedContents.GetService<TService>();



        #region Content and Header

        /// <inheritdoc />
        public dynamic Content => UnwrappedContents?.Content;
        /// <inheritdoc />
        public dynamic Header => UnwrappedContents?.Header;

        #endregion


        #region Link and Edit
        /// <inheritdoc />
        public ILinkHelper Link => UnwrappedContents?.Link;
        /// <inheritdoc />
        public IInPageEditingSystem Edit => UnwrappedContents?.Edit;
        #endregion

        #region SharedCode - must also map previous path to use here

        /// <inheritdoc />
        public string CreateInstancePath { get; set; }

        /// <inheritdoc />
        public dynamic CreateInstance(string virtualPath, 
            string noParamOrder = Eav.Parameters.Protector,
            string name = null,
            string relativePath = null,
            bool throwOnError = true)
        {
            var wrapLog = Log.Call<dynamic>();
            // usually we don't have a relative path, so we use the preset path from when this class was instantiated
            relativePath = relativePath ?? CreateInstancePath;
            var instance = UnwrappedContents?.CreateInstance(virtualPath, noParamOrder, name,
                relativePath ?? CreateInstancePath, throwOnError);
            return wrapLog((instance != null).ToString(), instance);
        }

        #endregion

        #region Context, Settings, Resources

        /// <inheritdoc />
        public ICmsContext CmsContext => UnwrappedContents?.CmsContext;


        #endregion CmsContext  
    }
}
