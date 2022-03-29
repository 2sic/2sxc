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
    public abstract partial class DynamicCode : HasLog, IDynamicCode, IWrapper<IDynamicCode>, IHasDynamicCodeRoot, INeedsDynamicCodeRoot
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
        public virtual void ConnectToRoot(IDynamicCodeRoot codeRoot)
        {
            Log.LinkTo(codeRoot?.Log);
            Log.Call()(null);
            _DynCodeRoot = codeRoot;
        }

        /// <inheritdoc />
        /// <remarks>
        /// The parent of this object. It's not called Parent but uses an exotic name to ensure that your code won't accidentally create a property with the same name.
        /// </remarks>
        public IDynamicCode UnwrappedContents => _DynCodeRoot;

        public IDynamicCode GetContents() => _DynCodeRoot;

        [PrivateApi] public IDynamicCodeRoot _DynCodeRoot { get; private set; }

        #endregion

        /// <inheritdoc />
        public IApp App => _DynCodeRoot?.App;

        /// <inheritdoc />
        public IBlockDataSource Data => _DynCodeRoot?.Data;

        /// <inheritdoc />
        public TService GetService<TService>() => _DynCodeRoot.GetService<TService>();



        #region Content and Header

        /// <inheritdoc />
        public dynamic Content => _DynCodeRoot?.Content;
        /// <inheritdoc />
        public dynamic Header => _DynCodeRoot?.Header;

        #endregion


        #region Link and Edit
        /// <inheritdoc />
        public ILinkHelper Link => _DynCodeRoot?.Link;
        /// <inheritdoc />
        public IInPageEditingSystem Edit => _DynCodeRoot?.Edit;
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
            var instance = _DynCodeRoot?.CreateInstance(virtualPath, noParamOrder, name,
                relativePath ?? CreateInstancePath, throwOnError);
            return wrapLog((instance != null).ToString(), instance);
        }

        #endregion

        #region Context, Settings, Resources

        /// <inheritdoc />
        public ICmsContext CmsContext => _DynCodeRoot?.CmsContext;


        #endregion CmsContext  
    }
}
