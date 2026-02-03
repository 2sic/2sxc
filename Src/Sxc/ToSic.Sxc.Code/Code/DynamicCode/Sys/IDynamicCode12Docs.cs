using ToSic.Sxc.Data;
using ToSic.Sxc.Services;

// Disable warnings that properties should be marked as new
// Because we need them here as additional definition because of Razor problems with inherited interfaces
#pragma warning disable CS0108, CS0114

namespace ToSic.Sxc.Code.Sys;

/// <summary>
/// Just docs
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IDynamicCode12Docs
{

    #region Stuff added by DynamicCode12

    /// <summary>
    /// Convert one or many Entities and Dynamic entities into an <see cref="IDynamicStack"/>
    /// </summary>
    /// <param name="entities">one or more source object</param>
    /// <returns>a dynamic object for easier coding</returns>
    /// <remarks>
    /// New in 12.05
    /// </remarks>
    dynamic? AsDynamic(params object[] entities);


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
    dynamic? Resources { get; }

    /// <summary>
    /// Settings for this Scenario. This is a dynamic object based on the <see cref="IDynamicStack"/>.
    /// 
    /// It will combine both the Settings of the View and the App. The View-Settings will have priority. In future it may also include some global Settings. 
    /// 
    /// 🪒 Use in Razor: `@Settings.ItemsPerRow`
    /// </summary>
    /// <remarks>New in 12.03</remarks>
    dynamic? Settings { get; }

    #endregion


    #region DevTools

    [PrivateApi("Still WIP")]
    IDevTools DevTools { get; }

    #endregion

    #endregion
}