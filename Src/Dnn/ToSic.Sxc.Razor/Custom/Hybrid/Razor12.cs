using System.Collections.Generic;
using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Web;
using DynamicJacket = ToSic.Sxc.Data.DynamicJacket;
using IApp = ToSic.Sxc.Apps.IApp;
using IEntity = ToSic.Eav.Data.IEntity;


// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    /// <summary>
    /// The base class for Hybrid Razor-Components in 2sxc 12 <br/>
    /// Provides context objects like CmsContext, helpers like Edit and much more. <br/>
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("As of now just fyi, should be finalized for 2sxc 12")]
    public abstract partial class Razor12 : RazorComponentBase, IRazor12
    {
        [PrivateApi("Hide this, no need to publish; would only confuse users")]
        protected Razor12()
        {
            // Set the error message to ensure that this will not work in Hybrid razor
            _ErrorWhenUsingCreateInstanceCshtml = "CreateInstance(*.cshtml) is not supported in Hybrid Razor. Use .cs files instead.";
            _ErrorWhenUsingRenderPage = "RenderPage(...) is not supported in Hybrid Razor. Use Html.Partial(...) instead.";
        }

        #region Link, Edit, Dnn, App, Data

        /// <inheritdoc />
        public ILinkHelper Link => _DynCodeRoot.Link;

        /// <inheritdoc />
        public IInPageEditingSystem Edit => _DynCodeRoot.Edit;

        /// <inheritdoc />
        public TService GetService<TService>() => _DynCodeRoot.GetService<TService>();

        [PrivateApi] public int CompatibilityLevel => _DynCodeRoot.CompatibilityLevel;

        /// <inheritdoc />
        public new IApp App => _DynCodeRoot.App;

        /// <inheritdoc />
        public IBlockDataSource Data => _DynCodeRoot.Data;

        #endregion

        #region AsDynamic in many variations

        /// <inheritdoc/>
        public dynamic AsDynamic(string json, string fallback = DynamicJacket.EmptyJson) => _DynCodeRoot.AsDynamic(json, fallback);

        /// <inheritdoc/>
        public dynamic AsDynamic(IEntity entity) => _DynCodeRoot.AsDynamic(entity);

        /// <inheritdoc/>
        public dynamic AsDynamic(object dynamicEntity) => _DynCodeRoot.AsDynamic(dynamicEntity);


        #endregion

        #region AsEntity
        /// <inheritdoc/>
        public IEntity AsEntity(object dynamicEntity) => _DynCodeRoot.AsEntity(dynamicEntity);
        #endregion

        #region AsList

        /// <inheritdoc />
        public IEnumerable<dynamic> AsList(object list) => _DynCodeRoot.AsList(list);

        #endregion


        #region Data Source Stuff

        /// <inheritdoc/>
        public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = null)
            where T : IDataSource
            => _DynCodeRoot.CreateSource<T>(inSource, configurationProvider);

        /// <inheritdoc/>
        public T CreateSource<T>(IDataStream inStream) where T : IDataSource
            => _DynCodeRoot.CreateSource<T>(inStream);

        #endregion



        #region Content, Header, etc. and List
        /// <inheritdoc/>
        public dynamic Content => _DynCodeRoot.Content;

        /// <inheritdoc />
        public dynamic Header => _DynCodeRoot.Header;

        #endregion





        #region Adam 

        /// <inheritdoc />
        public IFolder AsAdam(IDynamicEntity entity, string fieldName) => _DynCodeRoot.AsAdam(entity, fieldName);


        /// <inheritdoc />
        public IFolder AsAdam(IEntity entity, string fieldName) => _DynCodeRoot.AsAdam(entity, fieldName);

        #endregion

        #region CmsContext

        public ICmsContext CmsContext => _DynCodeRoot.CmsContext;

        [PrivateApi("WIP 12.02")]
        public dynamic Resources => _DynCodeRoot.Resources;

        [PrivateApi("WIP 12.02")]
        public dynamic Settings => _DynCodeRoot.Settings;

        #endregion
    }
}
