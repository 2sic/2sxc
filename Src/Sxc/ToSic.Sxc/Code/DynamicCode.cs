using System.Collections.Generic;
using Custom.Hybrid;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp;
using ToSic.Lib.Coding;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code;

/// <summary>
/// This is a base class for dynamic code which is compiled at runtime.
///
/// > [!TIP]
/// > This is an old base class and works, but you should use a newer one such as <see cref="CodeTyped"/>
/// </summary>
[PublicApi]
public abstract class DynamicCode : CustomCodeBase, IHasCodeLog, IDynamicCode
{
    #region Constructor / Setup

    /// <summary>
    /// Main constructor, to enable easy inheriting in custom code.
    /// </summary>
    protected DynamicCode() : base("Sxc.DynCod") { }

    /// <inheritdoc cref="IHasCodeLog.Log" />
    public new ICodeLog Log => SysHlp.CodeLog;

    /// <inheritdoc cref="ToSic.Eav.Code.ICanGetService.GetService{TService}"/>
    public TService GetService<TService>() where TService : class => _DynCodeRoot.GetService<TService>();

    [PrivateApi] public override int CompatibilityLevel => Constants.CompatibilityLevel10;

    #endregion

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
    string IGetCodePath.CreateInstancePath { get; set; }

    /// <inheritdoc />
    public dynamic CreateInstance(string virtualPath, NoParamOrder noParamOrder = default, string name = null, string relativePath = null, bool throwOnError = true) =>
        SysHlp.CreateInstance(virtualPath, noParamOrder, name, relativePath, throwOnError);

    #endregion

    #region Context, Settings, Resources

    /// <inheritdoc cref="IDynamicCode.CmsContext" />
    public ICmsContext CmsContext => _DynCodeRoot?.CmsContext;

    #endregion CmsContext

    #region AsDynamic and AsEntity

    /// <inheritdoc />
    public dynamic AsDynamic(string json, string fallback = default) => _DynCodeRoot?.Cdf.Json2Jacket(json, fallback);

    /// <inheritdoc />
    public dynamic AsDynamic(IEntity entity) => _DynCodeRoot?.Cdf.CodeAsDyn(entity);

    /// <inheritdoc />
    public dynamic AsDynamic(object dynamicEntity) => _DynCodeRoot?.Cdf.AsDynamicFromObject(dynamicEntity);

    /// <inheritdoc cref="IDynamicCode12.AsDynamic(object[])" />
    public dynamic AsDynamic(params object[] entities) => _DynCodeRoot?.Cdf.MergeDynamic(entities);

    /// <inheritdoc />
    public IEntity AsEntity(object dynamicEntity) => _DynCodeRoot?.Cdf.AsEntity(dynamicEntity);

    #endregion

    #region AsList

    /// <inheritdoc />
    public IEnumerable<dynamic> AsList(object list) => _DynCodeRoot?.Cdf.CodeAsDynList(list);

    #endregion

    #region CreateSource

    /// <inheritdoc />
    public T CreateSource<T>(IDataStream source) where T : IDataSource
        => _DynCodeRoot.CreateSource<T>(source);

    /// <inheritdoc />
    public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = default) where T : IDataSource
        => _DynCodeRoot.CreateSource<T>(inSource, configurationProvider);


    #endregion

    #region AsAdam

    /// <inheritdoc />
    public IFolder AsAdam(ICanBeEntity item, string fieldName) => _DynCodeRoot?.AsAdam(item, fieldName);

    #endregion
}