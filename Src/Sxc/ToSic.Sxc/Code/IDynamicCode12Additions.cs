//using ToSic.Eav.Documentation;
//using ToSic.Sxc.Code.DevTools;
//using ToSic.Sxc.Data;
//using ToSic.Sxc.Services;

//namespace ToSic.Sxc.Code
//{
//    /// <summary>
//    /// Interfaces added to Dynamic Code in v12
//    /// </summary>
//    public interface IDynamicCode12Additions
//    {

//        /// <summary>
//        /// Convert one or many Entities and Dynamic entities into an <see cref="IDynamicStack"/>
//        /// </summary>
//        /// <param name="entities">one or more source object</param>
//        /// <returns>a dynamic object for easier coding</returns>
//        /// <remarks>
//        /// New in 12.05
//        /// </remarks>
//        [PublicApi]
//        dynamic AsDynamic(params object[] entities);


//        #region Convert-Service

//        /// <summary>
//        /// Conversion helper for common data conversions in Razor and WebAPIs
//        /// </summary>
//        /// <remarks>
//        /// Added in 2sxc 12.05
//        /// </remarks>
//        IConvertService Convert { get; }

//        #endregion

//        #region Resources and Settings WIP 12.02

//        /// <summary>
//        /// Resources for this Scenario. This is a dynamic object based on the <see cref="IDynamicStack"/>.
//        ///
//        /// It will combine both the Resources of the View and the App. The View-Resources will have priority. In future it may also include some global Resources. 
//        /// 
//        /// 🪒 Use in Razor: `@Resources.CtaButtonLabel`
//        /// </summary>
//        /// <remarks>New in 12.03</remarks>
//        [PublicApi]
//        dynamic Resources { get; }

//        /// <summary>
//        /// Settings for this Scenario. This is a dynamic object based on the <see cref="IDynamicStack"/>.
//        /// 
//        /// It will combine both the Settings of the View and the App. The View-Settings will have priority. In future it may also include some global Settings. 
//        /// 
//        /// 🪒 Use in Razor: `@Settings.ItemsPerRow`
//        /// </summary>
//        /// <remarks>New in 12.03</remarks>
//        [PublicApi]
//        dynamic Settings { get; }

//        #endregion


//        #region DevTools

//        [InternalApi_DoNotUse_MayChangeWithoutNotice]
//        IDevTools DevTools { get; }

//        #endregion

//    }
//}
