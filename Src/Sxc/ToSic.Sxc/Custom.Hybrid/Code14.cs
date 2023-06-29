using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.DevTools;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;
using static ToSic.Eav.Parameters;


// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    /// <summary>
    /// Base class for v14 Dynamic Code files.
    /// 
    /// Will provide the <see cref="ServiceKit14"/> on property `Kit`.
    /// This contains all the popular services used in v14, so that your code can be lighter. 
    /// </summary>
    /// <remarks>
    /// Important: The property `Convert` which exited on Razor12 was removed. use `Kit.Convert` instead.
    /// </remarks>
    [PublicApi]
    public abstract class Code14 : DynamicCodeBase, IHasCodeLog, IDynamicCode, IDynamicCode14<object, ServiceKit14>
    {

        #region Constructor / Setup

        /// <summary>
        /// Main constructor. May never have parameters, otherwise inheriting code will run into problems. 
        /// </summary>
        protected Code14() : base("Sxc.Code14") { }

        // <inheritdoc />
        public new ICodeLog Log => SysHlp.CodeLog;

        /// <inheritdoc cref="IDynamicCode12.GetService{TService}" />
        public TService GetService<TService>() => _DynCodeRoot.GetService<TService>();

        #endregion

        public ServiceKit14 Kit => _kit.Get(() => _DynCodeRoot.GetKit<ServiceKit14>());
        private readonly GetOnce<ServiceKit14> _kit = new GetOnce<ServiceKit14>();

        #region Stuff added by Code12

        ///// <inheritdoc cref="IDynamicCode12.Convert" />
        //public IConvertService Convert => _DynCodeRoot.Convert;

        /// <inheritdoc cref="IDynamicCode12.Resources" />
        public dynamic Resources => _DynCodeRoot?.Resources;

        /// <inheritdoc cref="IDynamicCode12.Settings" />
        public dynamic Settings => _DynCodeRoot?.Settings;

        [PrivateApi("Not yet ready")]
        public IDevTools DevTools => _DynCodeRoot.DevTools;

        #endregion




        // Stuff "inherited" from DynamicCode (old base class)

        #region App / Data / Content / Header

        /// <inheritdoc cref="IDynamicCode.App" />
        public IApp App => _DynCodeRoot?.App;

        /// <inheritdoc cref="IDynamicCode.Data" />
        public IContextData Data => _DynCodeRoot?.Data;

        /// <inheritdoc cref="IDynamicCode.Content" />
        public dynamic Content => _DynCodeRoot?.Content;
        /// <inheritdoc cref="IDynamicCode.Header" />
        public dynamic Header => _DynCodeRoot?.Header;

        #endregion



        #region Link and Edit
        /// <inheritdoc cref="IDynamicCode.Link" />
        public ILinkService Link => _DynCodeRoot?.Link;
        /// <inheritdoc cref="IDynamicCode.Edit" />
        public IEditService Edit => _DynCodeRoot?.Edit;

        #endregion

        #region SharedCode - must also map previous path to use here

        /// <inheritdoc />
        [PrivateApi]
        public string CreateInstancePath { get; set; }

        /// <inheritdoc cref="IDynamicCode.CreateInstance" />
        public dynamic CreateInstance(string virtualPath, string noParamOrder = Protector, string name = null, string relativePath = null, bool throwOnError = true) =>
            SysHlp.CreateInstance(virtualPath, noParamOrder, name, relativePath, throwOnError);

        #endregion

        #region Context, Settings, Resources

        /// <inheritdoc cref="IDynamicCode.CmsContext" />
        public ICmsContext CmsContext => _DynCodeRoot?.CmsContext;

        #endregion CmsContext

        #region AsDynamic and AsEntity

        /// <inheritdoc cref="IDynamicCode.AsDynamic(string, string)" />
        public dynamic AsDynamic(string json, string fallback = default) => _DynCodeRoot?.AsC.AsDynamicFromJson(json, fallback);

        /// <inheritdoc cref="IDynamicCode.AsDynamic(IEntity)" />
        public dynamic AsDynamic(IEntity entity) => _DynCodeRoot?.AsC.AsDynamic(entity);

        /// <inheritdoc cref="IDynamicCode.AsDynamic(object)" />
        public dynamic AsDynamic(object dynamicEntity) => _DynCodeRoot?.AsC.AsDynamicInternal(dynamicEntity);

        /// <inheritdoc cref="IDynamicCode12.AsDynamic(object[])" />
        public dynamic AsDynamic(params object[] entities) => _DynCodeRoot?.AsC.MergeDynamic(entities);

        /// <inheritdoc cref="IDynamicCode.AsEntity" />
        public IEntity AsEntity(object dynamicEntity) => _DynCodeRoot?.AsC.AsEntity(dynamicEntity);

        #endregion

        #region AsList

        /// <inheritdoc cref="IDynamicCode.AsList" />
        public IEnumerable<dynamic> AsList(object list) => _DynCodeRoot?.AsC.AsDynamicList(list);

        #endregion

        #region CreateSource

        /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataStream)" />
        public T CreateSource<T>(IDataStream source) where T : IDataSource
            => _DynCodeRoot.CreateSource<T>(source);

        /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataSource, ILookUpEngine)" />
        public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = default) where T : IDataSource
            => _DynCodeRoot.CreateSource<T>(inSource, configurationProvider);


        #endregion

        /// <inheritdoc cref="IDynamicCode.AsAdam" />
        public IFolder AsAdam(ICanBeEntity item, string fieldName) => _DynCodeRoot?.AsAdam(item, fieldName);


        /// <inheritdoc />
        public ITypedItem AsTyped(object original, string noParamOrder = Protector, bool? required = default) => _DynCodeRoot.AsC.AsItem(original);

        /// <inheritdoc />
        public IEnumerable<ITypedItem> AsTypedList(object list,
            string noParamOrder = Protector,
            bool? required = default,
            IEnumerable<ITypedItem> fallback = default)
            => _DynCodeRoot.AsC.AsItems(list, required: required, fallback: fallback);

    }
}
