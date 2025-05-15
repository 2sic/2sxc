using ToSic.Eav.DataSource;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Code.Internal;

/// <summary>
/// Experimental: Make sure that ICodeApiService can provide everything that IDynamicCode12 needs,
/// without implementing that specific interface, which has a much larger scope.
/// WIP 2025-05-11 2dm
///
/// Note that our goal is to get rid of this again, or shrink as much as possible, once we've moved the dependencies
/// </summary>
public interface ICodeApiServiceForDynamicCode12Wip: IHasLog
{
    /// <inheritdoc cref="IDynamicCode.App" />
    IApp App { get; }

    /// <inheritdoc cref="IDynamicCode.Data" />
    IDataSource Data { get; }

    #region Convert-Service

    ///// <summary>
    ///// Conversion helper for common data conversions in Razor and WebAPIs
    ///// </summary>
    ///// <remarks>
    ///// Added in 2sxc 12.05
    ///// </remarks>
    //IConvertService Convert { get; }

    #endregion


    /// <inheritdoc cref="IDynamicCode.CmsContext" />
    ICmsContext CmsContext { get; }

}