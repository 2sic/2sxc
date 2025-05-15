using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code.Internal;

/// <summary>
/// Experimental: Make sure that ICodeApiService can provide everything that IDynamicCode12 needs,
/// without implementing that specific interface, which has a much larger scope.
/// WIP 2025-05-11 2dm
///
/// Note that our goal is to get rid of this again, or shrink as much as possible, once we've moved the dependencies
/// </summary>
public interface ICodeApiServiceForDynamicCode12Wip: IHasLog, ICodeDynamicApiService, ICodeTypedApiService
{
    /// <inheritdoc cref="IDynamicCode.App" />
    IApp App { get; }

    /// <inheritdoc cref="IDynamicCode.AsAdam" />
    IFolder AsAdam(ICanBeEntity item, string fieldName);

    /// <inheritdoc cref="IDynamicCode.Data" />
    IDataSource Data { get; }

    //#region DevTools

    //[PrivateApi("Still WIP")]
    //IDevTools DevTools { get; }

    //#endregion

    #region Convert-Service

    ///// <summary>
    ///// Conversion helper for common data conversions in Razor and WebAPIs
    ///// </summary>
    ///// <remarks>
    ///// Added in 2sxc 12.05
    ///// </remarks>
    //IConvertService Convert { get; }

    #endregion

    //#region Create Data Sources

    ///// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataStream)" />
    //T CreateSource<T>(IDataStream source) where T : IDataSource;


    ///// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataSource, ILookUpEngine)" />
    //T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = default) where T : IDataSource;

    //#endregion


    /// <inheritdoc cref="IDynamicCode.CmsContext" />
    ICmsContext CmsContext { get; }

    /// <inheritdoc cref="IDynamicCode.Edit" />
    IEditService Edit { get; }


    /// <inheritdoc cref="IDynamicCode.Link" />
    ILinkService Link { get; }

}