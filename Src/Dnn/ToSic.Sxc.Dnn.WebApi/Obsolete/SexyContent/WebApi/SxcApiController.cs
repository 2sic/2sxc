using ToSic.Eav.Data;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Lib.Coding;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Code.Sys.CodeApi;
using ToSic.Sxc.Code.Sys.CodeRunHelpers;
using ToSic.Sxc.Compatibility.Sxc;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Services;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Dnn.WebApi.Internal.HttpJson;
using ToSic.Sxc.Sys.ExecutionContext;
using IApp = ToSic.Sxc.Apps.IApp;
using IFolder = ToSic.Sxc.Adam.IFolder;

// ReSharper disable InheritdocInvalidUsage

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.WebApi;

/// <inheritdoc cref="DnnSxcCustomControllerBase" />
/// <summary>
/// This is the base class for API Controllers which need the full context
/// incl. the current App, DNN, Data, etc.
/// For others, please use the SxiApiControllerBase, which doesn't have all that, and is usually then
/// safer because it can't accidentally mix the App with a different appId in the params
/// </summary>
[DnnLogExceptions]
[Obsolete("This will continue to work, but you should use the Custom.Hybrid.Api14 or Custom.Dnn.Api12 instead.")]
[PrivateApi("This was the official base class a long time ago, Name & APIs must remain stable")]
[DefaultToNewtonsoftForHttpJson]
[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract partial class SxcApiController() :
    DnnSxcCustomControllerBase("OldApi"),
    IDynamicCode,
    IHasDnn,
    ICreateInstance,
    IDynamicWebApi,
    // #RemovedV20 #IAppAndDataHelpers
    //IDynamicCodeBeforeV10
    //#pragma warning disable 618
    //    IAppAndDataHelpers,
    //#pragma warning restore 618
    IHasCodeLog
{
    internal ICodeDynamicApiHelper CodeApi => field ??= ExCtx.GetDynamicApi();

    /// <inheritdoc cref="ICanGetService.GetService{TService}"/>
    public TService GetService<TService>() where TService : class => SysHlp.GetService<TService>();

    /// <inheritdoc cref="IHasDnn.Dnn"/>
    public IDnnContext Dnn => (ExCtx as IHasDnn)?.Dnn;

    [Obsolete]
    [PrivateApi]
    public SxcHelper Sxc => field
        ??= new(CodeApi?.Block?.Context?.Permissions.IsContentAdmin ?? false, SysHlp.GetService<IConvertToEavLight> ());

    /// <summary>
    /// Old API - probably never used, but we shouldn't remove it as we could break some existing code out there
    /// </summary>
    [PrivateApi]
    public IBlock Block => SysHlp.GetBlockAndContext(Request);

    [PrivateApi] public int CompatibilityLevel => CompatibilityLevels.CompatibilityLevel9Old;

    /// <inheritdoc cref="IDynamicCode.App" />
    public IApp App => CodeApi.App;

    /// <inheritdoc cref="IDynamicCode.Data" />
    public IDataSource Data => /*(IBlockDataSource)*/CodeApi.Data;

    // Explicit implementation of expected interface, but it should not work in the normal code
    // as the old code sometimes expects Data.Cache.GetContentType
    /// <inheritdoc />
    IDataSource IDynamicCode.Data => CodeApi.Data;


    #region AsDynamic implementations

    /// <inheritdoc />
    public dynamic AsDynamic(IEntity entity) => CodeApi.Cdf.CodeAsDyn(entity);

    /// <inheritdoc />
    public dynamic AsDynamic(object dynamicEntity) => CodeApi.Cdf.AsDynamicFromObject(dynamicEntity);

    /// <inheritdoc />
    [PublicApi("Careful - still Experimental in 12.02")]
    public dynamic AsDynamic(params object[] entities) => CodeApi.Cdf.MergeDynamic(entities);

    /// <inheritdoc />
    [PrivateApi("old api, only available in old API controller")]
    public dynamic AsDynamic(KeyValuePair<int, IEntity> entityKeyValuePair) => CodeApi.Cdf.CodeAsDyn(entityKeyValuePair.Value);

    /// <inheritdoc />
    public IEnumerable<dynamic> AsDynamic(IDataStream stream) => CodeApi.Cdf.CodeAsDynList(stream.List);

    /// <inheritdoc />
    public IEntity AsEntity(object dynamicEntity) =>  CodeApi.Cdf.AsEntity(dynamicEntity);

    /// <inheritdoc />
    public IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities) =>  CodeApi.Cdf.CodeAsDynList(entities);
    #endregion

    #region Compatibility with Eav.Interfaces.IEntity - introduced in 10.10 - Removed in v20

    //[PrivateApi]
    //[Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
    //public dynamic AsDynamic(Eav.Interfaces.IEntity entity) => CodeApi.Cdf.CodeAsDyn(entity as IEntity);


    //[PrivateApi]
    //[Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
    //public dynamic AsDynamic(KeyValuePair<int, Eav.Interfaces.IEntity> entityKeyValuePair) => CodeApi.Cdf.CodeAsDyn(entityKeyValuePair.Value as IEntity);

    //[PrivateApi]
    //[Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
    //public IEnumerable<dynamic> AsDynamic(IEnumerable<Eav.Interfaces.IEntity> entities) => CodeApi.Cdf.CodeAsDynList(entities.Cast<IEntity>());
    #endregion


    #region CreateSource implementations
    [Obsolete]
    [PrivateApi]
    public IDataSource CreateSource(string typeName = "", IDataSource inSource = null, ILookUpEngine configurationProvider = null)
        => new CodeApiServiceObsolete(ExCtx).CreateSource(typeName, inSource, configurationProvider);

    /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataSource, ILookUpEngine)" />
    public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = default) where T : IDataSource
        => CodeApi.CreateSource<T>(inSource, configurationProvider);

    /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataStream)" />
    public T CreateSource<T>(IDataStream source) where T : IDataSource 
        => CodeApi.CreateSource<T>(source);

    #endregion

    #region Content, Presentation & List
    /// <summary>
    /// content item of the current view
    /// </summary>
    public dynamic Content => CodeApi.Content;

    /// <summary>
    /// presentation item of the content-item. 
    /// </summary>
    [Obsolete("please use Content.Presentation instead")]
    public dynamic Presentation => CodeApi.Content?.Presentation;

    public dynamic Header => CodeApi.Header;

    [Obsolete("use Header instead")]
    public dynamic ListContent => CodeApi.Header;

    /// <summary>
    /// presentation item of the content-item. 
    /// </summary>
    [Obsolete("please use Header.Presentation instead")]
    public dynamic ListPresentation => CodeApi.Header?.Presentation;

    // #RemovedV20 #Element
    //[Obsolete("This is an old way used to loop things. Use Data[\"Default\"] instead. Will be removed in 2sxc v10")]
    //public List<Element> List => new CodeApiServiceObsolete(ExCtx).ElementList;

    #endregion


    #region Adam

    /// <inheritdoc cref="IDynamicCode.AsAdam" />
    public IFolder AsAdam(ICanBeEntity item, string fieldName) => CodeApi.AsAdam(item, fieldName);

    /// <inheritdoc cref="IDynamicWebApi.SaveInAdam"/>
    public IFile SaveInAdam(NoParamOrder noParamOrder = default, Stream stream = null, string fileName = null, string contentType = null,
        Guid? guid = null, string field = null, string subFolder = "")
        => DynHlp.SaveInAdam(noParamOrder, stream, fileName, contentType, guid, field, subFolder);

    #endregion

    #region CreateInstance

    string IGetCodePath.CreateInstancePath { get; set; }

    private CodeHelper CodeHlp => field ??= GetService<CodeHelper>().Init(this);

    /// <inheritdoc cref="ICreateInstance.CreateInstance"/>
    public dynamic CreateInstance(string virtualPath, NoParamOrder noParamOrder = default, string name = null, string relativePath = null, bool throwOnError = true)
        => CodeHlp.CreateInstance(virtualPath: virtualPath, name: name, throwOnError: throwOnError);

    #endregion

    #region Link & Edit - added in 2sxc 10.01
    /// <inheritdoc cref="IDynamicCode.Link" />
    public ILinkService Link => CodeApi?.Link!;
    /// <inheritdoc cref="IDynamicCode.Edit" />
    public IEditService Edit => CodeApi?.Edit!;

    #endregion

    #region CmsContext, Resources and Settings

    /// <inheritdoc cref="IDynamicCode.CmsContext" />
    public ICmsContext CmsContext => CodeApi.CmsContext;

    ///// <inheritdoc />
    //public dynamic Resources => _DynCodeRoot.Resources;

    ///// <inheritdoc />
    //public dynamic Settings => _DynCodeRoot.Settings;

    #endregion

    #region IHasLog

    /// <inheritdoc cref="IHasCodeLog.Log" />
    public new ICodeLog Log => SysHlp.CodeLog;

    #endregion
}