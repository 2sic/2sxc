using ToSic.Lib.Data;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code
{
    /// <summary>
    /// This is a base class for dynamic code which is compiled at runtime. <br/>
    /// It delegates all properties like App and methods like AsDynamic() to the parent item which initially caused it to be compiled.
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public abstract partial class DynamicCode : ServiceForDynamicCode, IHasCodeLog, IDynamicCode, IWrapper<IDynamicCode>, IHasDynamicCodeRoot, INeedsDynamicCodeRoot
    {

        #region Constructor - NOT for DI

        /// <summary>
        /// Main constructor, may never have parameters, otherwise inheriting code will run into problems. 
        /// </summary>
        protected DynamicCode() : base("Sxc.DynCod") { }

        #endregion

        #region IHasLog

        // <inheritdoc />
        public new ICodeLog Log => _codeLog.Get(() => new CodeLog(base.Log));
        private readonly GetOnce<ICodeLog> _codeLog = new GetOnce<ICodeLog>();

        #endregion

        #region Dynamic Code Coupling

        [PrivateApi]
        public override void ConnectToRoot(IDynamicCodeRoot codeRoot) => base.Log.Do(() =>
        {
            base.ConnectToRoot(codeRoot);
            _codeLog.Reset(); // reset in case it was already used before
        });

        [PrivateApi]
        public IDynamicCode GetContents() => _DynCodeRoot;

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
        public ILinkService Link => _DynCodeRoot?.Link;
        /// <inheritdoc />
        public IEditService Edit => _DynCodeRoot?.Edit;
        #endregion

        #region SharedCode - must also map previous path to use here

        /// <inheritdoc />
        public string CreateInstancePath { get; set; }

        /// <inheritdoc />
        public dynamic CreateInstance(string virtualPath,
            string noParamOrder = Eav.Parameters.Protector,
            string name = null,
            string relativePath = null,
            bool throwOnError = true) => base.Log.Func(() =>
        {
            // usually we don't have a relative path, so we use the preset path from when this class was instantiated
            relativePath = relativePath ?? CreateInstancePath;
            var instance = _DynCodeRoot?.CreateInstance(virtualPath, noParamOrder, name,
                relativePath ?? CreateInstancePath, throwOnError);
            return (object)instance;
        });

        #endregion

        #region Context, Settings, Resources

        /// <inheritdoc />
        public ICmsContext CmsContext => _DynCodeRoot?.CmsContext;


        #endregion CmsContext

    }
}
