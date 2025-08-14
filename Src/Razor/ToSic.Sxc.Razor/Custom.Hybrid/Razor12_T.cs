using System.Diagnostics.CodeAnalysis;
using Custom.Razor.Sys;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Services;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Code.Sys.CodeApi;
using ToSic.Sxc.Context;
using ToSic.Sxc.Engines.Sys;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

[PrivateApi("This will already be documented through the Dnn DLL so shouldn't appear again in the docs")]
[method: PrivateApi]
public abstract class Razor12<TModel>() : OqtRazorBase<TModel>(CompatibilityLevels.CompatibilityLevel12, "Oqt.Rzr12"),
    IHasCodeLog, IRazor, IRazor12, ISetDynamicModel
{
    internal ICodeDynamicApiHelper CodeApi => RzrHlp.ExCtxRoot.GetDynamicApi();

    #region Dynamic Model

    public dynamic DynamicModel => RzrHlp.DynamicModel;

    #endregion

    #region CreateInstance

    /// <inheritdoc cref="ICreateInstance.CreateInstancePath"/>
    [PrivateApi]
    [field: AllowNull, MaybeNull]
    // Note: The path for CreateInstance / GetCode - unsure if this is actually used anywhere on this object
    string IGetCodePath.CreateInstancePath
    {
        get => field ?? Path;
        set;
    }

    /// <inheritdoc cref="ICreateInstance.CreateInstance"/>
    public dynamic? CreateInstance(string virtualPath, NoParamOrder noParamOrder = default, string? name = null, string? relativePath = null, bool throwOnError = true)
        => RzrHlp.CreateInstance(virtualPath, noParamOrder, name, relativePath, throwOnError);

    #endregion

    #region Content, Header, etc.

    /// <inheritdoc cref="IDynamicCodeDocs.Content" />
    public dynamic? Content => CodeApi.Content;

    /// <inheritdoc cref="IDynamicCodeDocs.Header" />
    public dynamic? Header => CodeApi.Header;

    #endregion


    #region Link, Edit, App, Data

    /// <inheritdoc cref="IDynamicCodeDocs.Link" />
    public ILinkService Link => CodeApi.Link;

    /// <inheritdoc cref="IDynamicCodeDocs.Link" />
    public IEditService Edit => CodeApi.Edit;

    /// <inheritdoc cref="IDynamicCodeDocs.App" />
    public IApp App => CodeApi.App;

    /// <inheritdoc cref="IDynamicCodeDocs.Data" />
    public IDataSource Data => CodeApi.Data;

    #endregion

    #region AsDynamic in many variations + AsList

    /// <inheritdoc cref="IDynamicCodeDocs.AsDynamic(string, string)" />
    public dynamic? AsDynamic(string json, string? fallback = default) => CodeApi.Cdf.Json2Jacket(json, fallback);

    /// <inheritdoc cref="IDynamicCodeDocs.AsDynamic(object)" />
    public dynamic? AsDynamic(IEntity entity) => CodeApi.Cdf.CodeAsDyn(entity);

    /// <inheritdoc cref="IDynamicCodeDocs.AsDynamic(string, string)" />
    public dynamic? AsDynamic(object dynamicEntity) => CodeApi.Cdf.AsDynamicFromObject(dynamicEntity);

    /// <inheritdoc cref="IDynamicCode12Docs.AsDynamic(object[])" />
    public dynamic? AsDynamic(params object[] entities) => CodeApi.Cdf.MergeDynamic(entities);

    /// <inheritdoc cref="IDynamicCodeDocs.AsList" />
    public IEnumerable<dynamic> AsList(object list) => CodeApi.Cdf.CodeAsDynList(list);

    #endregion

    #region AsEntity

    /// <inheritdoc cref="IDynamicCodeDocs.AsEntity" />
    public IEntity AsEntity(object dynamicEntity) => CodeApi.Cdf.AsEntity(dynamicEntity);

    #endregion

    #region CreateSource

    /// <inheritdoc cref="IDynamicCodeDocs.CreateSource{T}(IDataStream)" />
    public T CreateSource<T>(IDataStream source) where T : IDataSource
        => CodeApi.CreateSource<T>(source);

    /// <inheritdoc cref="IDynamicCodeDocs.CreateSource{T}(IDataSource, ILookUpEngine)" />
    public T CreateSource<T>(IDataSource? inSource = null, ILookUpEngine? configurationProvider = default) where T : IDataSource
        => CodeApi.CreateSource<T>(inSource, configurationProvider);

    #endregion

    #region Convert-Service - V12 only!

    [PrivateApi] public IConvertService Convert => field ??= CodeApi.Convert;

    #endregion

    #region Adam

    /// <inheritdoc cref="IDynamicCodeDocs.AsAdam" />
    public IFolder AsAdam(ICanBeEntity item, string fieldName) => CodeApi.AsAdam(item, fieldName);

    #endregion

    #region CmsContext 

    /// <inheritdoc cref="IDynamicCodeDocs.CmsContext" />
    public ICmsContext CmsContext => CodeApi.CmsContext;

    /// <inheritdoc cref="IDynamicCode12Docs.Resources" />
    public dynamic Resources => CodeApi.Resources;

    /// <inheritdoc cref="IDynamicCode12Docs.Settings" />
    public dynamic Settings => CodeApi.Settings;

    #endregion

}