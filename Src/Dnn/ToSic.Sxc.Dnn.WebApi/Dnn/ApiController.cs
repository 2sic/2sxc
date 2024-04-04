using System.IO;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp;
using ToSic.Lib.Coding;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Code.Internal.CodeRunHelpers;
using ToSic.Sxc.Context;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.WebApi;
using ToSic.Sxc.Dnn.WebApi.Internal.HttpJson;
using ToSic.Sxc.Internal;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Dnn;

/// <summary>
/// This is the base class for all custom API Controllers. <br/>
/// With this, your code receives the full context  incl. the current App, DNN, Data, etc.
/// </summary>
[PublicApi("This was the official base class before v12. Try to move away from it, go to the latest base class on Custom.Dnn.Api12")]
[DnnLogExceptions]
[Obsolete("This will continue to work, but you should use the Custom.Hybrid.Api14 or Custom.Dnn.Api12 instead.")]
[DefaultToNewtonsoftForHttpJson]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class ApiController : DnnSxcCustomControllerBase, 
    IDnnDynamicWebApi,
    ICreateInstance,
    IDynamicCode, 
    IDynamicWebApi, 
    IHasCodeApiService,
    IHasCodeLog
{
    internal const string ErrRecommendedNamespaces = "To use it, use the new base class from Custom.Hybrid.Api14 or Custom.Dnn.Api12 instead.";

    /// <remarks>
    /// Probably obsolete, but a bit risky to just remove
    /// We will only add it to ApiController but not to Api12, because no new code should ever use that.
    /// </remarks>
    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public IBlock Block => SysHlp.GetBlockAndContext(Request);

    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public int CompatibilityLevel => CompatibilityLevels.CompatibilityLevel9Old;

    /// <inheritdoc cref="IDynamicCode.App" />
    public IApp App => _CodeApiSvc.App;

    /// <inheritdoc cref="IDynamicCode.Data" />
    public IBlockInstance Data => _CodeApiSvc.Data;

    /// <inheritdoc cref="ToSic.Eav.Code.ICanGetService.GetService{TService}"/>
    public TService GetService<TService>() where TService : class => SysHlp.GetService<TService>();

    /// <inheritdoc cref="IHasDnn.Dnn"/>
    public IDnnContext Dnn => (_CodeApiSvc as IHasDnn)?.Dnn;

    #region AsDynamic implementations
    /// <inheritdoc cref="IDynamicCode.AsDynamic(string, string)" />
    public dynamic AsDynamic(string json, string fallback = default) => _CodeApiSvc.Cdf.Json2Jacket(json, fallback);

    /// <inheritdoc cref="IDynamicCode.AsDynamic(IEntity)" />
    public dynamic AsDynamic(IEntity entity) => _CodeApiSvc.Cdf.CodeAsDyn(entity);

    /// <inheritdoc cref="IDynamicCode.AsDynamic(object)" />
    public dynamic AsDynamic(object dynamicEntity) => _CodeApiSvc.Cdf.AsDynamicFromObject(dynamicEntity);

    /// <inheritdoc cref="IDynamicCode12.AsDynamic(object[])" />
    public IEntity AsEntity(object dynamicEntity) => _CodeApiSvc.Cdf.AsEntity(dynamicEntity);

    #endregion


    #region AsList

    /// <inheritdoc />
    public IEnumerable<dynamic> AsList(object list) => _CodeApiSvc?.Cdf.CodeAsDynList(list);

    #endregion

    #region CreateSource implementations

    /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataSource, ILookUpEngine)" />
    public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = default) where T : IDataSource
        => _CodeApiSvc.CreateSource<T>(inSource, configurationProvider);

    /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataStream)" />
    public T CreateSource<T>(IDataStream source) where T : IDataSource
        => _CodeApiSvc.CreateSource<T>(source);

    #endregion

    #region Content, Presentation & List

    /// <inheritdoc cref="IDynamicCode.Content" />
    public dynamic Content => _CodeApiSvc.Content;

    /// <inheritdoc cref="IDynamicCode.Header" />
    public dynamic Header => _CodeApiSvc.Header;


    #endregion

    #region Adam

    /// <inheritdoc cref="IDynamicCode.AsAdam" />
    public IFolder AsAdam(ICanBeEntity item, string fieldName) => _CodeApiSvc.AsAdam(item, fieldName);

    public dynamic File(NoParamOrder noParamOrder = default, bool? download = null, string virtualPath = null,
        string contentType = null, string fileDownloadName = null, object contents = null) =>
        throw new NotSupportedException("Not implemented. " + ErrRecommendedNamespaces);

    /// <inheritdoc cref="IDynamicWebApi.SaveInAdam"/>
    public IFile SaveInAdam(NoParamOrder noParamOrder = default, Stream stream = null, string fileName = null, string contentType = null,
        Guid? guid = null, string field = null, string subFolder = "")
        => DynHlp.SaveInAdam(noParamOrder, stream, fileName, contentType, guid, field, subFolder);

    #endregion

    #region Link & Edit - added to API in 2sxc 10.01

    /// <inheritdoc cref="IDynamicCode.Link" />
    public ILinkService Link => _CodeApiSvc?.Link;

    /// <inheritdoc cref="IDynamicCode.Edit" />
    public IEditService Edit => _CodeApiSvc?.Edit;

    #endregion

    #region CmsContext

    /// <inheritdoc cref="IDynamicCode.CmsContext" />
    public ICmsContext CmsContext => _CodeApiSvc?.CmsContext;
    #endregion

    #region CreateInstance

    string IGetCodePath.CreateInstancePath { get; set; }

    private CodeHelper CodeHlp => _codeHlp ??= GetService<CodeHelper>().Init(this);
    private CodeHelper _codeHlp;



    /// <inheritdoc cref="ICreateInstance.CreateInstance"/>
    public dynamic CreateInstance(string virtualPath, NoParamOrder noParamOrder = default, string name = null, string relativePath = null, bool throwOnError = true)
        => CodeHlp.CreateInstance(virtualPath: virtualPath, name: name, throwOnError: throwOnError);

    #endregion

    #region IHasLog

    /// <inheritdoc cref="IHasCodeLog.Log" />
    public new ICodeLog Log => SysHlp.CodeLog;

    #endregion
}