using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.CodeApi.Internal;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Code.Internal.CodeRunHelpers;
using ToSic.Sxc.Context;
using ToSic.Sxc.Internal;
using ToSic.Sxc.Services;


// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

/// <summary>
/// Base class for v14 Dynamic Code files.
/// 
/// Will provide the <see cref="ServiceKit14"/> on property `Kit`.
/// This contains all the popular services used in v14, so that your code can be lighter. 
/// </summary>
/// <remarks>
/// Important: The property `Convert` which exited on Razor12 was removed. use `Kit.Convert` instead.
/// The constructor cannot have parameters, otherwise inheriting code will run into problems.
/// </remarks>
[PublicApi]
[ShowApiWhenReleased(ShowApiMode.Never)]   // #DocsButNotForIntellisense
public abstract class Code14()
    : CustomCodeBase("Sxc.Code14"), IHasCodeLog, IDynamicCode, IDynamicCode14<object, ServiceKit14>
{

    #region Constructor / Setup

    internal ICodeDynamicApiHelper CodeApi => field ??= ExCtx.GetDynamicApi();

    /// <inheritdoc cref="IHasCodeLog.Log" />
    public new ICodeLog Log => CodeHlp.CodeLog;

    /// <inheritdoc cref="ToSic.Eav.Code.ICanGetService.GetService{TService}"/>
    public TService GetService<TService>() where TService : class => CodeApi.GetService<TService>();

    [PrivateApi] public override int CompatibilityLevel => CompatibilityLevels.CompatibilityLevel12;

    private CodeHelperV14 CodeHelper => field ??= new(new(ExCtx, false, "c# code file"));

    #endregion

    public ServiceKit14 Kit => field ??= CodeApi.ServiceKit14;

    #region Stuff added by Code12

    ///// <inheritdoc cref="IDynamicCode12.Convert" />
    //public IConvertService Convert => _DynCodeRoot.Convert;

    /// <inheritdoc cref="IDynamicCode12.Resources" />
    public dynamic Resources => CodeApi?.Resources;

    /// <inheritdoc cref="IDynamicCode12.Settings" />
    public dynamic Settings => CodeApi?.Settings;

    [PrivateApi("Not yet ready")]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public IDevTools DevTools => CodeHelper.DevTools;

    #endregion




    // Stuff "inherited" from DynamicCode (old base class)

    #region App / Data / Content / Header

    /// <inheritdoc cref="IDynamicCode.App" />
    public IApp App => CodeApi?.App;

    /// <inheritdoc cref="IDynamicCode.Data" />
    public IDataSource Data => CodeApi?.Data;

    /// <inheritdoc cref="IDynamicCode.Content" />
    public dynamic Content => CodeApi?.Content;
    /// <inheritdoc cref="IDynamicCode.Header" />
    public dynamic Header => CodeApi?.Header;

    #endregion



    #region Link and Edit

    /// <inheritdoc cref="IDynamicCode.Link" />
    public ILinkService Link => CodeApi?.Link;

    /// <inheritdoc cref="IDynamicCode.Edit" />
    public IEditService Edit => CodeApi?.Edit;

    #endregion

    #region SharedCode - must also map previous path to use here

    /// <inheritdoc />
    [PrivateApi]
    string IGetCodePath.CreateInstancePath { get; set; }

    /// <inheritdoc cref="IDynamicCode.CreateInstance" />
    public dynamic CreateInstance(string virtualPath, NoParamOrder noParamOrder = default, string name = null, string relativePath = null, bool throwOnError = true) =>
        CodeHlp.CreateInstance(virtualPath: virtualPath, name: name, relativePath: relativePath, throwOnError: throwOnError);

    /// <inheritdoc cref="IDynamicCode16.GetCode"/>
    [PrivateApi("added in 16.05, but not sure if it should be public")]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public dynamic GetCode(string path, NoParamOrder noParamOrder = default, string className = default)
        => CodeHlp.GetCode(path: path, className: className);

    #endregion

    #region Context, Settings, Resources

    /// <inheritdoc cref="IDynamicCode.CmsContext" />
    public ICmsContext CmsContext => CodeApi?.CmsContext;

    #endregion CmsContext

    #region AsDynamic and AsEntity

    /// <inheritdoc cref="IDynamicCode.AsDynamic(string, string)" />
    public dynamic AsDynamic(string json, string fallback = default) => CodeApi?.Cdf.Json2Jacket(json, fallback);

    /// <inheritdoc cref="IDynamicCode.AsDynamic(IEntity)" />
    public dynamic AsDynamic(IEntity entity) => CodeApi?.Cdf.CodeAsDyn(entity);

    /// <inheritdoc cref="IDynamicCode.AsDynamic(object)" />
    public dynamic AsDynamic(object dynamicEntity) => CodeApi?.Cdf.AsDynamicFromObject(dynamicEntity);

    /// <inheritdoc cref="IDynamicCode12.AsDynamic(object[])" />
    public dynamic AsDynamic(params object[] entities) => CodeApi?.Cdf.MergeDynamic(entities);

    /// <inheritdoc cref="IDynamicCode.AsEntity" />
    public IEntity AsEntity(object dynamicEntity) => CodeApi?.Cdf.AsEntity(dynamicEntity);

    #endregion

    #region AsList

    /// <inheritdoc cref="IDynamicCode.AsList" />
    public IEnumerable<dynamic> AsList(object list) => CodeApi?.Cdf.CodeAsDynList(list);

    #endregion

    #region CreateSource

    /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataStream)" />
    public T CreateSource<T>(IDataStream source) where T : IDataSource
        => CodeApi.CreateSource<T>(source);

    /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataSource, ILookUpEngine)" />
    public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = default) where T : IDataSource
        => CodeApi.CreateSource<T>(inSource, configurationProvider);


    #endregion

    /// <inheritdoc cref="IDynamicCode.AsAdam" />
    public IFolder AsAdam(ICanBeEntity item, string fieldName) => CodeApi?.AsAdam(item, fieldName);

}