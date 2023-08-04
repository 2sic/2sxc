using System;
using System.Collections.Generic;
using System.Linq;
using Custom.Hybrid;
using DotNetNuke.Entities.Modules;
using ToSic.Eav.Data;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp;
using ToSic.Lib.Documentation;
using ToSic.SexyContent.Engines;
using ToSic.SexyContent.Search;
using ToSic.Sxc;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code;
using ToSic.Sxc.Compatibility;
using ToSic.Sxc.Compatibility.RazorPermissions;
using ToSic.Sxc.Compatibility.Sxc;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Search;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web;
using DynamicJacket = ToSic.Sxc.Data.DynamicJacket;
using IApp = ToSic.Sxc.Apps.IApp;
using IEntity = ToSic.Eav.Data.IEntity;

// ReSharper disable InheritdocInvalidUsage

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.Razor
{
    /// <summary>
    /// The core page type for delivering a 2sxc page
    /// Provides context infos like the Dnn object, helpers like Edit and much more. 
    /// </summary>
    public abstract class SexyContentWebPage : 
        RazorComponentBase,
        ICreateInstance,
        IHasDnn,
        IDnnRazorCustomize, 
        IDynamicCodeBeforeV10,
#pragma warning disable 618
        IAppAndDataHelpers
#pragma warning restore 618
    {
        #region Core Properties which should appear in docs

        /// <inheritdoc />
        public override ICodeLog Log => SysHlp.CodeLog;

        /// <inheritdoc />
        public override IHtmlHelper Html => SysHlp.Html;

        #endregion

        #region Helpers linked through AppAndData Helpers

        /// <inheritdoc cref="IDynamicCode.Link" />
        public ILinkService Link => _DynCodeRoot.Link;

        [PrivateApi]
        public dynamic DynamicModel => throw new NotSupportedException($"{nameof(DynamicModel)} not implemented on {nameof(SexyContentWebPage)}. {RazorComponent.NotImplementedUseCustomBase}");

        /// <inheritdoc cref="IDynamicCode.Edit" />
        public IEditService Edit => _DynCodeRoot.Edit;

        public IDnnContext Dnn => (_DynCodeRoot as IHasDnn)?.Dnn;

#pragma warning disable 612
        /// <inheritdoc />
        [PrivateApi("never public, shouldn't be in use elsewhere")]
        [Obsolete]
        public SxcHelper Sxc => _sxc ?? (_sxc = new SxcHelper(_DynCodeRoot.Block?.Context.UserMayEdit ?? false, GetService<IConvertToEavLight>()));
        [Obsolete]
        private SxcHelper _sxc;
#pragma warning restore 612

        /// <summary>
        /// Old API - probably never used, but we shouldn't remove it as we could break some existing code out there
        /// </summary>
        [PrivateApi] public IBlock Block => _DynCodeRoot.Block;

        /// <inheritdoc cref="ToSic.Eav.Code.ICanGetService.GetService{TService}"/>
        public TService GetService<TService>() where TService : class => _DynCodeRoot.GetService<TService>();

        [PrivateApi] public override int CompatibilityLevel => Constants.CompatibilityLevel9Old;

        /// <inheritdoc />
        public new IApp App => _DynCodeRoot.App;

        #region Data - with old interface #DataInAddWontWork

        /// <inheritdoc />
        public IBlockDataSourceOld Data => (IBlockDataSourceOld)_DynCodeRoot.Data;

        // This is explicitly implemented so the interfaces don't complain
        // but actually we're not showing this - in reality we're showing the Old (see above)
        IContextData IAppAndDataHelpers.Data => _DynCodeRoot.Data;
        
        #endregion

        /// <inheritdoc />
        IContextData IDynamicCode.Data => _DynCodeRoot.Data;

        public RazorPermissions Permissions => new RazorPermissions(_DynCodeRoot.Block?.Context.UserMayEdit ?? false);

        #region AsDynamic in many variations

        /// <inheritdoc />
        [Obsolete]
        public dynamic AsDynamic(IEntity entity) => _DynCodeRoot.AsC.CodeAsDyn(entity);


        /// <inheritdoc />
        public dynamic AsDynamic(object dynamicEntity) => _DynCodeRoot.AsC.AsDynamicFromObject(dynamicEntity);

        /// <inheritdoc />
        [PublicApi("Careful - still Experimental in 12.02")]
        public dynamic AsDynamic(params object[] entities) => _DynCodeRoot.AsC.MergeDynamic(entities);

        // todo: only in "old" controller, not in new one
        /// <inheritdoc />
        [Obsolete]
        public dynamic AsDynamic(KeyValuePair<int, IEntity> entityKeyValuePair) => _DynCodeRoot.AsC.CodeAsDyn(entityKeyValuePair.Value);



        /// <inheritdoc />
        [Obsolete]
        public IEnumerable<dynamic> AsDynamic(IDataStream stream) => _DynCodeRoot.AsC.CodeAsDynList(stream.List);

        /// <inheritdoc />
        public IEntity AsEntity(object dynamicEntity) => _DynCodeRoot.AsC.AsEntity(dynamicEntity);


        /// <inheritdoc />
        [Obsolete]
        public IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities) => _DynCodeRoot.AsC.CodeAsDynList(entities);

        #endregion

        #region Future features: AsList / Settings / Resources

        [PrivateApi]
        public IEnumerable<dynamic> AsList(object list)
            => throw new Exception("AsList is a newer feature in 2sxc. To use it, change your template type to " + nameof(Razor12) + " see https://go.2sxc.org/RazorComponent");

        [PrivateApi]
        public dynamic Resources
            => throw new Exception("Resources is a newer feature in 2sxc. To use it, change your template type to " + nameof(Razor12) + " see https://go.2sxc.org/RazorComponent");

        [PrivateApi]
        public dynamic Settings
            => throw new Exception("Settings is a newer feature in 2sxc. To use it, change your template type to " + nameof(Razor12) + " see https://go.2sxc.org/RazorComponent");

        #endregion

        #region Not supported, not captured new features (like Convert-Service) - because they would break old code
        //[PrivateApi] 
        //public IConvertService Convert => throw new NotSupportedException($"{nameof(Convert)} not implemented on {nameof(SexyContentWebPage)}. {RazorComponent.NotImplementedUseCustomBase}");
        #endregion


        #region Compatibility with Eav.Interfaces.IEntity - introduced in 10.10
        [PrivateApi]
        [Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        public dynamic AsDynamic(Eav.Interfaces.IEntity entity) => _DynCodeRoot.AsC.CodeAsDyn(entity as IEntity);


        [PrivateApi]
        [Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        public dynamic AsDynamic(KeyValuePair<int, Eav.Interfaces.IEntity> entityKeyValuePair) => _DynCodeRoot.AsC.CodeAsDyn(entityKeyValuePair.Value as IEntity);

        [PrivateApi]
        [Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
        public IEnumerable<dynamic> AsDynamic(IEnumerable<Eav.Interfaces.IEntity> entities) => _DynCodeRoot.AsC.CodeAsDynList(entities.Cast<IEntity>());
        #endregion


        #region Data Source Stuff
        /// <inheritdoc />
        [Obsolete]
        public IDataSource CreateSource(string typeName = "", IDataSource inSource = null, ILookUpEngine configurationProvider = null)
            => new DynamicCodeObsolete(_DynCodeRoot).CreateSource(typeName, inSource, configurationProvider);

        /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataSource, ILookUpEngine)" />
        [Obsolete("this is the old implementation with ILookUp Engine, don't think it was ever used publicly because people couldn't create these engines")]
        public T CreateSource<T>(IDataSource inSource = default, ILookUpEngine configurationProvider = default) where T : IDataSource
            => _DynCodeRoot.CreateSource<T>(inSource, configurationProvider);

        /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataStream)" />
        public T CreateSource<T>(IDataStream source) where T : IDataSource
            => _DynCodeRoot.CreateSource<T>(source);

        #endregion


        #region Content, Header, etc. and List
        /// <inheritdoc cref="IDynamicCode.Content" />
        public dynamic Content => _DynCodeRoot.Content;

        [Obsolete("use Content.Presentation instead")]
        [PrivateApi]
        public dynamic Presentation => _DynCodeRoot.Content?.Presentation;

        /// <summary>
        /// We are blocking this property on purpose, so that people will want to migrate to the new RazorComponent
        /// </summary>
        [PrivateApi]
        public dynamic Header => throw new Exception("The header property is a new feature in 2sxc 10.20. " +
                                                     "To use it, change your template type to inherit from " +
                                                     nameof(RazorComponent) + " see https://go.2sxc.org/RazorComponent");

#pragma warning disable 618
        [Obsolete("Use Header instead")]
        public dynamic ListContent => _DynCodeRoot.Header;

        [Obsolete("Use Header.Presentation instead")]
        public dynamic ListPresentation => _DynCodeRoot.Header?.Presentation;

        [Obsolete("This is an old way used to loop things - shouldn't be used any more - will be removed in a future version")]
        public List<Element> List => _list ?? (_list =new DynamicCodeObsolete(_DynCodeRoot).ElementList);
        [Obsolete("don't use any more")]
        private List<Element> _list;
#pragma warning restore 618

        /// <inheritdoc cref="IDynamicCode.AsDynamic(string, string)" />
        public dynamic AsDynamic(string json, string fallback = DynamicWrapperFactory.EmptyJson)
            => throw new Exception("The AsDynamic(string) is a new feature in 2sxc 10.20. To use it, change your template type to inherit from " 
                                   + nameof(RazorComponent) + " see https://go.2sxc.org/RazorComponent");

        #endregion

        #endregion

        #region Customize Data & Search

        /// <inheritdoc />
        public virtual void CustomizeData() {}

        /// <inheritdoc />
        public virtual void CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IModule moduleInfo, DateTime beginDate) { }

        [PrivateApi("this is the old signature, should still be supported")]
        public virtual void CustomizeSearch(Dictionary<string, List<ISearchInfo>> searchInfos, ModuleInfo moduleInfo, DateTime beginDate) { }

        public Purpose Purpose { get; internal set; }

        [Obsolete("left for compatibility, use Purpose instead")]
        public InstancePurposes InstancePurpose { get; internal set; }

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
            => SysHlp.CreateInstance(virtualPath, noParamOrder, name, throwOnError);

        #endregion

    }
}