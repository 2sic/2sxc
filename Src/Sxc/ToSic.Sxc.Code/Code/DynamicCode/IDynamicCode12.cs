using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;
using ToSic.Sxc.Sys.ExecutionContext;
using IFolder = ToSic.Sxc.Adam.IFolder;
// Disable warnings that properties should be marked as new
// Because we need them here as additional definition because of Razor problems with inherited interfaces
#pragma warning disable CS0108, CS0114

namespace ToSic.Sxc.Code;

/// <summary>
/// Interface for Dynamic Code with enhancements after v12. It extends <see cref="IDynamicCode"/>
/// 
/// Dynamic Code is the API for files like Razor or WebApis.
/// Supports many properties like App, etc. to ensure that the dynamic code has everything you need. <br />
/// Also provides many Conversions between <see cref="IEntity"/> and <see cref="IDynamicEntity"/>.
/// Important for dynamic code files like Razor or WebApi. Note that there are many overloads to ensure that AsDynamic and AsEntity "just work" even if you give them the original data.
/// </summary>
/// <remarks>
/// This interface is only used by developers who use the <see cref="IDynamicCodeService"/> to retrieve the object which helps them access things such as the App.
/// </remarks>
[PublicApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IDynamicCode12 : IDynamicCode, ICreateInstance, ICompatibilityLevel, IHasLog
{
    #region IDynamicCode Repeats - keep this in sync
    // **************************************************
    // WARNING
    // **************************************************
    // Razor has a small problem with interfaces inheriting interfaces. 
    // If an object is of an interface which inherits another interface
    // then Razor will not find methods of the root interface and give errors like
    // Microsoft.CSharp.RuntimeBinder.RuntimeBinderException: 'ToSic.Sxc.Code.IDynamicCode12' does not contain a definition for 'AsList' at CallSite.Target
    //
    // Because of this, we repeat the ENTIRE definition for IDynamicCode here
    // Make sure they remain in-sync
    // **************************************************


    /// <inheritdoc cref="ICanGetService.GetService{TService}"/>
    TService GetService<TService>() where TService : class;

    /// <inheritdoc cref="IDynamicCodeDocs.App" />
    IApp App { get; }

    /// <inheritdoc cref="IDynamicCodeDocs.Data" />
    IDataSource Data { get; }

    #region Content and Header

    /// <inheritdoc cref="IDynamicCodeDocs.Content" />
    dynamic? Content { get; }

    /// <inheritdoc cref="IDynamicCodeDocs.Header" />
    dynamic? Header { get; }

    #endregion

    #region AsAdam, Linking, Edit

    /// <inheritdoc cref="IDynamicCodeDocs.AsAdam" />
    IFolder AsAdam(ICanBeEntity item, string fieldName);

    /// <inheritdoc cref="IDynamicCodeDocs.Link" />
    ILinkService Link { get; }


    /// <inheritdoc cref="IDynamicCodeDocs.Edit" />
    IEditService Edit { get; }

    #endregion

    #region AsDynamic for Strings

    /// <inheritdoc cref="IDynamicCodeDocs.AsDynamic(string, string)" />

    dynamic? AsDynamic(string json, string? fallback = default);

    #endregion 

    #region AsDynamic for Entities

    /// <inheritdoc cref="IDynamicCodeDocs.AsDynamic(IEntity)" />
    dynamic? AsDynamic(IEntity entity);


    /// <inheritdoc cref="IDynamicCodeDocs.AsDynamic(object)" />
    dynamic? AsDynamic(object dynamicEntity);


    #endregion

    #region AsEntity

    /// <inheritdoc cref="IDynamicCodeDocs.AsEntity" />
    IEntity AsEntity(object dynamicEntity);

    #endregion

    #region AsList

    /// <inheritdoc cref="IDynamicCodeDocs.AsList" />
    IEnumerable<dynamic>? AsList(object list);

    #endregion


    #region Create Data Sources

    /// <inheritdoc cref="IDynamicCodeDocs.CreateSource{T}(IDataStream)" />
    T CreateSource<T>(IDataStream source) where T : IDataSource;


    /// <inheritdoc cref="IDynamicCodeDocs.CreateSource{T}(IDataSource, ILookUpEngine)" />
    T CreateSource<T>(IDataSource? inSource = null, ILookUpEngine? configurationProvider = default) where T : IDataSource;

    #endregion

    #region Context

    /// <inheritdoc cref="IDynamicCodeDocs.CmsContext" />
    ICmsContext CmsContext { get; }

    #endregion



    #endregion



    #region Stuff added by DynamicCode12

    /// <inheritdoc cref="IDynamicCode12Docs.AsDynamic(object[])" />
    dynamic? AsDynamic(params object[] entities);


    #region Convert-Service

    /// <inheritdoc cref="IDynamicCode12Docs.Convert" />
    IConvertService Convert { get; }

    #endregion

    #region Resources and Settings

    /// <inheritdoc cref="IDynamicCode12Docs.Resources" />
    dynamic? Resources { get; }

    /// <inheritdoc cref="IDynamicCode12Docs.Settings" />
    dynamic? Settings { get; }

    #endregion


    #region DevTools

    [PrivateApi("Still WIP")]
    IDevTools DevTools { get; }

    #endregion

    #endregion
}