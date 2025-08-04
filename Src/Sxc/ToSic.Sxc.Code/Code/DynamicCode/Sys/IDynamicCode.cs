using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;
using ToSic.Sxc.Sys.ExecutionContext;
using IFolder = ToSic.Sxc.Adam.IFolder;

namespace ToSic.Sxc.Code.Sys;

/// <summary>
/// Dynamic code files like Razor or WebApis.
/// Supports many properties like App, etc. to ensure that the dynamic code has everything you need. <br />
/// Also provides many Conversions between <see cref="IEntity"/> and <see cref="IDynamicEntity"/>.
/// Important for dynamic code files like Razor or WebApi. Note that there are many overloads to ensure that AsDynamic and AsEntity "just work" even if you give them the original data. 
/// </summary>
[PrivateApi("Was public till v17")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IDynamicCode: ICreateInstance, ICompatibilityLevel, IHasLog // inherit from old namespace to ensure compatibility
{
    /// <inheritdoc cref="ICanGetService.GetService{TService}"/>
    TService GetService<TService>() where TService : class;

    /// <inheritdoc cref="IDynamicCodeDocs.App"/>
    IApp App { get; }

    /// <inheritdoc cref="IDynamicCodeDocs.Data"/>
    IDataSource Data { get; }

    #region Content and Header

    /// <inheritdoc cref="IDynamicCodeDocs.Content"/>
    dynamic? Content { get; }

    /// <inheritdoc cref="IDynamicCodeDocs.Header"/>
    dynamic? Header { get; }

    #endregion

    #region AsAdam

    /// <inheritdoc cref="IDynamicCodeDocs.AsAdam"/>
    IFolder AsAdam(ICanBeEntity item, string fieldName);

    #endregion

    #region Linking

    /// <inheritdoc cref="IDynamicCodeDocs.Link"/>
    ILinkService Link { get; }

    #endregion

    #region Edit

    /// <inheritdoc cref="IDynamicCodeDocs.Edit"/>
    IEditService Edit { get; }
    #endregion

    #region AsDynamic for Strings

    /// <inheritdoc cref="IDynamicCodeDocs.AsDynamic(string, string)"/>
    dynamic? AsDynamic(string json, string? fallback = default);

    #endregion 

    #region AsDynamic for Entities

    /// <inheritdoc cref="IDynamicCodeDocs.AsDynamic(IEntity)"/>
    dynamic? AsDynamic(IEntity entity);


    /// <inheritdoc cref="IDynamicCodeDocs.AsDynamic(object)"/>
    dynamic? AsDynamic(object dynamicEntity);


    #endregion

    #region AsEntity

    /// <inheritdoc cref="IDynamicCodeDocs.AsEntity(object)"/>
    IEntity? AsEntity(object dynamicEntity);

    #endregion

    #region AsList

    /// <inheritdoc cref="IDynamicCodeDocs.AsList(object)"/>
    IEnumerable<dynamic>? AsList(object list);

    #endregion


    #region Create Data Sources

    /// <inheritdoc cref="IDynamicCodeDocs.CreateSource{T}(IDataStream)"/>
    T CreateSource<T>(IDataStream source) where T: IDataSource;


    /// <inheritdoc cref="IDynamicCodeDocs.CreateSource{T}(IDataSource, ILookUpEngine)"/>
    T CreateSource<T>(IDataSource? inSource = null, ILookUpEngine? configurationProvider = default) where T : IDataSource;

    #endregion


    #region Context

    /// <inheritdoc cref="IDynamicCodeDocs.CmsContext"/>
    ICmsContext CmsContext { get; }

    #endregion

}