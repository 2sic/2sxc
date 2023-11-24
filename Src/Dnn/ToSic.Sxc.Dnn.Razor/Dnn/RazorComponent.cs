using System;
using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp;
using ToSic.Lib.Documentation;
using ToSic.Eav.Run;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Search;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web;
using ToSic.Sxc.DataSources;

namespace ToSic.Sxc.Dnn
{
    /// <summary>
    /// The base class for Razor-Components in 2sxc 10+ to 2sxc 11 - deprecated now<br/>
    /// Provides context infos like the Dnn object, helpers like Edit and much more. <br/>
    /// </summary>
    [PublicApi("...but deprecated! use Razor14, RazorTyped or newer")]
    public abstract partial class RazorComponent : RazorComponentBase, 
#pragma warning disable CS0618
        IDnnRazorCustomize, 
#pragma warning restore CS0618
        IDnnRazorCompatibility, IDnnRazor11, ICreateInstance
    {
        /// <inheritdoc />
        public IDnnContext Dnn => (_DynCodeRoot as IHasDnn)?.Dnn;

        public const string NotImplementedUseCustomBase = "Use a newer base class like Custom.Hybrid.Razor12 or Custom.Dnn.Razor12 to leverage this.";

        #region Core Properties which should appear in docs

        /// <inheritdoc />
        public override ICodeLog Log => SysHlp.CodeLog;

        /// <inheritdoc />
        public override IHtmlHelper Html => SysHlp.Html;

        #endregion


        #region CustomizeSearch corrections

        /// <inheritdoc />
        [Obsolete("Shouldn't be used any more, but will continue to work for indefinitely for old base classes, not in v12. There are now better ways of doing this")]
        public virtual void CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IModule moduleInfo,
            DateTime beginDate)
        {
            // in 2sxc 11.11 the signature changed. 
            // so the engine will call this function
            // but the override will be the other one - so I must call that
            // unless of course this method was overridden by the final inheriting RazorComponent
#pragma warning disable 618 // disable warning about IContainer being obsolete
            CustomizeSearch(searchInfos, moduleInfo as IContainer, beginDate);
#pragma warning restore 618
        }

        [PrivateApi("shouldn't be used any more, but was still in v12 when released. v13+ must completely remove this")]
#pragma warning disable 618 // disable warning about IContainer being obsolete
        [Obsolete("Shouldn't be used any more, but will continue to work for indefinitely for old base classes, not in v12. There are now better ways of doing this")]
        public virtual void CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IContainer moduleInfo,
#pragma warning restore 618
            DateTime beginDate)
        {
            // new in 2sxc 11, if it has not been overridden, then try to check if code has something for us.
            var code = CodeManager.CodeOrNull;
            if (code == null) return;
            if (code is RazorComponentCode codeAsRazor) codeAsRazor.CustomizeSearch(searchInfos, moduleInfo, beginDate);
        }

        [Obsolete("Shouldn't be used any more, but will continue to work for indefinitely for old base classes, not in v12. There are now better ways of doing this")]
        public virtual void CustomizeData()
        {
            // new in 2sxc 11, if it has not been overridden, then try to check if code has something for us.
            var code = CodeManager.CodeOrNull;
            if (code == null) return;
            if (code is RazorComponentCode codeAsRazor) codeAsRazor.CustomizeData();
        }

        /// <inheritdoc />
        [Obsolete("Shouldn't be used any more, but will continue to work for indefinitely for old base classes, not in v12. There are now better ways of doing this")]
        public Purpose Purpose { get; internal set; }


        #endregion

        #region Link, Edit, Dnn, App, Data

        /// <inheritdoc cref="IDynamicCode.Link" />
        public ILinkService Link => _DynCodeRoot.Link;

        /// <inheritdoc cref="IDynamicCode.Edit" />
        public IEditService Edit => _DynCodeRoot.Edit;

        /// <inheritdoc cref="ToSic.Eav.Code.ICanGetService.GetService{TService}"/>
        public TService GetService<TService>() where TService : class => _DynCodeRoot.GetService<TService>();

        [PrivateApi] public override int CompatibilityLevel => Constants.CompatibilityLevel10;

        /// <inheritdoc />
        public new IApp App => _DynCodeRoot.App;

        #endregion

        #region Data - with old interface #DataInAddWontWork

        [PrivateApi]
        public IBlockDataSource Data => (IBlockDataSource)_DynCodeRoot.Data;

        // This is explicitly implemented so the interfaces don't complain
        // but actually we're not showing this - in reality we're showing the Old (see above)
        IContextData IDynamicCode.Data => _DynCodeRoot.Data;

        #endregion


        #region AsDynamic in many variations

        /// <inheritdoc cref="IDynamicCode.AsDynamic(string, string)" />
        public dynamic AsDynamic(string json, string fallback = default) => _DynCodeRoot.Cdf.Json2Jacket(json, fallback);

        /// <inheritdoc cref="IDynamicCode.AsDynamic(IEntity)" />
        public dynamic AsDynamic(IEntity entity) => _DynCodeRoot.Cdf.CodeAsDyn(entity);

        /// <inheritdoc cref="IDynamicCode.AsDynamic(string, string)" />
        public dynamic AsDynamic(object dynamicEntity) => _DynCodeRoot.Cdf.AsDynamicFromObject(dynamicEntity);

        #endregion

        #region AsEntity

        /// <inheritdoc cref="IDynamicCode.AsEntity" />
        public IEntity AsEntity(object dynamicEntity) => _DynCodeRoot.Cdf.AsEntity(dynamicEntity);

        #endregion

        #region AsList

        /// <inheritdoc cref="IDynamicCode.AsList" />
        public IEnumerable<dynamic> AsList(object list) => _DynCodeRoot.Cdf.CodeAsDynList(list);

        #endregion


        #region Data Source Stuff

        /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataSource, ILookUpEngine)" />
        public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = default) where T : IDataSource
            => _DynCodeRoot.CreateSource<T>(inSource, configurationProvider);

        /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataStream)" />
        public T CreateSource<T>(IDataStream source) where T : IDataSource
            => _DynCodeRoot.CreateSource<T>(source);

        #endregion


        #region Content, Header, etc. and List
        /// <inheritdoc cref="IDynamicCode.Content" />
        public dynamic Content => _DynCodeRoot.Content;

        /// <inheritdoc cref="IDynamicCode.Header" />
        public dynamic Header => _DynCodeRoot.Header;

        #endregion


        #region Adam 

        /// <inheritdoc cref="IDynamicCode.AsAdam" />
        public IFolder AsAdam(ICanBeEntity item, string fieldName) => _DynCodeRoot.AsAdam(item, fieldName);

        #endregion

        #region CmsContext

        /// <inheritdoc cref="IDynamicCode.CmsContext" />
        public ICmsContext CmsContext => _DynCodeRoot.CmsContext;

        #endregion

        #region CreateInstance

        [PrivateApi] string IGetCodePath.CreateInstancePath { get; set; }

        /// <inheritdoc />
        public virtual dynamic CreateInstance(string virtualPath, string noParamOrder = ToSic.Eav.Parameters.Protector, string name = null, string relativePath = null, bool throwOnError = true)
            => SysHlp.CreateInstance(virtualPath, noParamOrder, name, throwOnError: throwOnError);

        #endregion

    }
}
