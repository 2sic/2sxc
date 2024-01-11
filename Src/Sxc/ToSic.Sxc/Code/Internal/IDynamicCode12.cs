using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Services;
using IEntity = ToSic.Eav.Data.IEntity;
using IFolder = ToSic.Sxc.Adam.IFolder;
// Disable warnings that properties should be marked as new
// Because we need them here as additional definition because of Razor problems with inherited interfaces
#pragma warning disable CS0108, CS0114

namespace ToSic.Sxc.Code.Internal;

/// <summary>
/// Interface for Dynamic Code with enhancements after v12. It extends <see cref="IDynamicCode"/>
/// 
/// Dynamic Code is the API for files like Razor or WebApis.
/// Supports many properties like App, etc. to ensure that the dynamic code has everything you need. <br />
/// Also provides many Conversions between <see cref="IEntity"/> and <see cref="IDynamicEntity"/>.
/// Important for dynamic code files like Razor or WebApi. Note that there are many overloads to ensure that AsDynamic and AsEntity "just work" even if you give them the original data.
/// </summary>
[PrivateApi("Was public an on ToSic.Sxc.Code before, but now all docs should be on the object so it doesn't need to have the interface in the docs")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IDynamicCode12 : IDynamicCode
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


    /// <inheritdoc cref="ToSic.Eav.Code.ICanGetService.GetService{TService}"/>
    TService GetService<TService>() where TService : class;

    /// <inheritdoc cref="IDynamicCode.App" />
    IApp App { get; }

    /// <inheritdoc cref="IDynamicCode.Data" />
    IBlockInstance Data { get; }

    #region Content and Header

    /// <inheritdoc cref="IDynamicCode.Content" />
    dynamic Content { get; }

    /// <inheritdoc cref="IDynamicCode.Header" />
    dynamic Header { get; }

    #endregion

    #region AsAdam, Linking, Edit

    /// <inheritdoc cref="IDynamicCode.AsAdam" />
    IFolder AsAdam(ICanBeEntity item, string fieldName);

    /// <inheritdoc cref="IDynamicCode.Link" />
    ILinkService Link { get; }


    /// <inheritdoc cref="IDynamicCode.Edit" />
    IEditService Edit { get; }

    #endregion

    #region AsDynamic for Strings

    /// <inheritdoc cref="IDynamicCode.AsDynamic(string, string)" />

    dynamic AsDynamic(string json, string fallback = default);

    #endregion 

    #region AsDynamic for Entities

    /// <inheritdoc cref="IDynamicCode.AsDynamic(IEntity)" />
    dynamic AsDynamic(IEntity entity);


    /// <inheritdoc cref="IDynamicCode.AsDynamic(object)" />
    dynamic AsDynamic(object dynamicEntity);


    #endregion

    #region AsEntity

    /// <inheritdoc cref="IDynamicCode.AsEntity" />
    IEntity AsEntity(object dynamicEntity);

    #endregion

    #region AsList

    /// <inheritdoc cref="IDynamicCode.AsList" />
    IEnumerable<dynamic> AsList(object list);

    #endregion


    #region Create Data Sources

    /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataStream)" />
    T CreateSource<T>(IDataStream source) where T : IDataSource;


    /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataSource, ILookUpEngine)" />
    T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = default) where T : IDataSource;

    #endregion

    #region Context

    /// <inheritdoc cref="IDynamicCode.CmsContext" />
    ICmsContext CmsContext { get; }

    #endregion



    #endregion



    #region Stuff added by DynamicCode12

    /// <summary>
    /// Convert one or many Entities and Dynamic entities into an <see cref="IDynamicStack"/>
    /// </summary>
    /// <param name="entities">one or more source object</param>
    /// <returns>a dynamic object for easier coding</returns>
    /// <remarks>
    /// New in 12.05
    /// </remarks>
    dynamic AsDynamic(params object[] entities);


    #region Convert-Service

    /// <summary>
    /// Conversion helper for common data conversions in Razor and WebAPIs
    /// </summary>
    /// <remarks>
    /// Added in 2sxc 12.05
    /// </remarks>
    IConvertService Convert { get; }

    #endregion

    #region Resources and Settings

    /// <summary>
    /// Resources for this Scenario. This is a dynamic object based on the <see cref="IDynamicStack"/>.
    ///
    /// It will combine both the Resources of the View and the App. The View-Resources will have priority. In future it may also include some global Resources. 
    /// 
    /// 🪒 Use in Razor: `@Resources.CtaButtonLabel`
    /// </summary>
    /// <remarks>New in 12.03</remarks>
    dynamic Resources { get; }

    /// <summary>
    /// Settings for this Scenario. This is a dynamic object based on the <see cref="IDynamicStack"/>.
    /// 
    /// It will combine both the Settings of the View and the App. The View-Settings will have priority. In future it may also include some global Settings. 
    /// 
    /// 🪒 Use in Razor: `@Settings.ItemsPerRow`
    /// </summary>
    /// <remarks>New in 12.03</remarks>
    dynamic Settings { get; }

    #endregion


    #region DevTools

    [PrivateApi("Still WIP")]
    IDevTools DevTools { get; }

    #endregion

    #endregion
}