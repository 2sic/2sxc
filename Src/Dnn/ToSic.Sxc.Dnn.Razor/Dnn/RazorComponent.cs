using ToSic.Eav.LookUp;
using ToSic.Eav.Run;
using ToSic.Lib.LookUp.Engines;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code.CodeApi.Internal;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Search;
using ToSic.Sxc.DataSources.Internal.Compatibility;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Dnn;

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
    internal ICodeDynamicApiHelper CodeApi => field ??= ExCtx.GetDynamicApi();

    /// <inheritdoc />
    public IDnnContext Dnn => (ExCtx as IHasDnn)?.Dnn;

    public const string NotImplementedUseCustomBase = "Use a newer base class like Custom.Hybrid.Razor12 or Custom.Dnn.Razor12 to leverage this.";

    #region Core Properties which should appear in docs

    /// <inheritdoc />
    public override ICodeLog Log => RzrHlp.CodeLog;

    /// <inheritdoc />
    public override IHtmlHelper Html => RzrHlp.Html;

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
    public ILinkService Link => CodeApi.Link;

    /// <inheritdoc cref="IDynamicCode.Edit" />
    public IEditService Edit => CodeApi.Edit;

    /// <inheritdoc cref="ICanGetService.GetService{TService}"/>
    public TService GetService<TService>() where TService : class => CodeApi.GetService<TService>();

    [PrivateApi] public override int CompatibilityLevel => CompatibilityLevels.CompatibilityLevel10;

    /// <inheritdoc />
    public new IApp App => CodeApi.App;

    #endregion

    #region Data - with old interface #DataInAddWontWork

    [PrivateApi]
    public IBlockDataSource Data => (IBlockDataSource)CodeApi.Data;

    // This is explicitly implemented so the interfaces don't complain
    // but actually we're not showing this - in reality we're showing the Old (see above)
    IDataSource IDynamicCode.Data => CodeApi.Data;

    #endregion


    #region AsDynamic in many variations

    /// <inheritdoc cref="IDynamicCode.AsDynamic(string, string)" />
    public dynamic AsDynamic(string json, string fallback = default) => CodeApi.Cdf.Json2Jacket(json, fallback);

    /// <inheritdoc cref="IDynamicCode.AsDynamic(IEntity)" />
    public dynamic AsDynamic(IEntity entity) => CodeApi.Cdf.CodeAsDyn(entity);

    /// <inheritdoc cref="IDynamicCode.AsDynamic(string, string)" />
    public dynamic AsDynamic(object dynamicEntity) => CodeApi.Cdf.AsDynamicFromObject(dynamicEntity);

    #endregion

    #region AsEntity

    /// <inheritdoc cref="IDynamicCode.AsEntity" />
    public IEntity AsEntity(object dynamicEntity) => CodeApi.Cdf.AsEntity(dynamicEntity);

    #endregion

    #region AsList

    /// <inheritdoc cref="IDynamicCode.AsList" />
    public IEnumerable<dynamic> AsList(object list) => CodeApi.Cdf.CodeAsDynList(list);

    #endregion


    #region Data Source Stuff

    /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataSource, ILookUpEngine)" />
    public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = default) where T : IDataSource
        => CodeApi.CreateSource<T>(inSource, configurationProvider);

    /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataStream)" />
    public T CreateSource<T>(IDataStream source) where T : IDataSource
        => CodeApi.CreateSource<T>(source);

    #endregion


    #region Content, Header, etc. and List
    /// <inheritdoc cref="IDynamicCode.Content" />
    public dynamic Content => CodeApi.Content;

    /// <inheritdoc cref="IDynamicCode.Header" />
    public dynamic Header => CodeApi.Header;

    #endregion


    #region Adam 

    /// <inheritdoc cref="IDynamicCode.AsAdam" />
    public IFolder AsAdam(ICanBeEntity item, string fieldName) => CodeApi.AsAdam(item, fieldName);

    #endregion

    #region CmsContext

    /// <inheritdoc cref="IDynamicCode.CmsContext" />
    public ICmsContext CmsContext => CodeApi.CmsContext;

    #endregion

    #region CreateInstance

    [PrivateApi] string IGetCodePath.CreateInstancePath { get; set; }

    /// <inheritdoc />
    public virtual dynamic CreateInstance(string virtualPath, NoParamOrder noParamOrder = default, string name = null, string relativePath = null, bool throwOnError = true)
        => RzrHlp.CreateInstance(virtualPath, noParamOrder, name, throwOnError: throwOnError);

    #endregion

}