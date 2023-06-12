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

namespace ToSic.Sxc.Dnn
{
    /// <summary>
    /// The base class for Razor-Components in 2sxc 10+ to 2sxc 11 - deprecated now<br/>
    /// Provides context infos like the Dnn object, helpers like Edit and much more. <br/>
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public abstract partial class RazorComponent : RazorComponentBase, IDnnRazorCustomize, IDnnRazor, IDnnRazor11
    {
        /// <inheritdoc />
        public IDnnContext Dnn => (_DynCodeRoot as IDnnDynamicCode)?.Dnn;

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

        /// <inheritdoc />
        public ILinkService Link => _DynCodeRoot.Link;

        /// <inheritdoc />
        public IEditService Edit => _DynCodeRoot.Edit;

        /// <inheritdoc />
        public TService GetService<TService>() => _DynCodeRoot.GetService<TService>();

        [PrivateApi] public int CompatibilityLevel => _DynCodeRoot.CompatibilityLevel;

        /// <inheritdoc />
        public new IApp App => _DynCodeRoot.App;

        /// <inheritdoc />
        public IContextData Data => _DynCodeRoot.Data;

        #endregion

        #region AsDynamic in many variations

        /// <inheritdoc/>
        public dynamic AsDynamic(string json, string fallback = default) => _DynCodeRoot.AsC.AsDynamicFromJson(json, fallback);

        /// <inheritdoc/>
        public dynamic AsDynamic(IEntity entity) => _DynCodeRoot.AsDynamic(entity);

        /// <inheritdoc/>
        public dynamic AsDynamic(object dynamicEntity) => _DynCodeRoot.AsDynamic(dynamicEntity);

        ///// <inheritdoc/>
        //[PublicApi("Careful - still Experimental in 12.02")]
        //public dynamic AsDynamic(params object[] entities) => _DynCodeRoot.AsDynamic(entities);

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
        public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = default) where T : IDataSource
            => _DynCodeRoot.CreateSource<T>(inSource, configurationProvider);

        /// <inheritdoc/>
        public T CreateSource<T>(IDataStream source) where T : IDataSource
            => _DynCodeRoot.CreateSource<T>(source);

        #endregion


        #region Content, Header, etc. and List
        /// <inheritdoc/>
        public dynamic Content => _DynCodeRoot.Content;

        /// <inheritdoc />
        public dynamic Header => _DynCodeRoot.Header;

        #endregion


        #region Adam 

        /// <inheritdoc />
        public IFolder AsAdam(ICanBeEntity item, string fieldName) => _DynCodeRoot.AsAdam(item, fieldName);

        #endregion

        #region CmsContext

        /// <inheritdoc />
        public ICmsContext CmsContext => _DynCodeRoot.CmsContext;

        #endregion
    }
}
