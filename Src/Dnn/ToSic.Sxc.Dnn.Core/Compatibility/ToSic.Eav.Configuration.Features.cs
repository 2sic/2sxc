using System;
using System.Collections.Generic;
using ToSic.Eav.Documentation;

// ReSharper disable once CheckNamespace
namespace ToSic.Eav.Configuration
{
    /// <summary>
    /// This in an old API for determining if a system feature is enabled. Some will continue to work, but you should not use them.
    ///
    /// Prefer to use the IFeatureService instead
    /// </summary>
    [Obsolete("Obsolete in 2sxc 12 - please use the IFeaturesService instead")]
    [PrivateApi("this is just fyi, hidden since 2022-01-04")]
    public static class Features
    {

        /// <summary>
        /// Informs you if the enabled features are valid or not - meaning if they have been countersigned by the 2sxc features system.
        /// As of now, it's not enforced, but in future it will be. 
        /// </summary>
        /// <returns>true if the features were signed correctly</returns>
        [PrivateApi]
        [Obsolete("Deprecated in 2sxc 12 - use IFeatures.Valid")]
        public static bool Valid => FeaturesService.ValidInternal;

        public static IFeaturesInternal FeaturesFromDi = null;

        // 2022-01-24 2dm disabled, remove ca. April 2022 if all is fine
        //[Obsolete("Was private before, now deprecated in 12.05, will remove in v13 as it should never have been used outside")]
        //[PrivateApi] public static IEnumerable<FeatureState> All => FeaturesFromDi.All;

        //[Obsolete("Was private before, now deprecated in 12.05, will remove in v13 as it should never have been used outside")]
        //[PrivateApi] public static IEnumerable<FeatureState> Ui => FeaturesFromDi.Ui;

        /// <summary>
        /// Checks if a feature is enabled
        /// </summary>
        /// <param name="guid">The feature Guid</param>
        /// <returns>true if the feature is enabled</returns>
        [Obsolete("Do not use anymore, get the IFeaturesService for this. Will not remove for a long time, because in use in public Apps like Mobius")]
        public static bool Enabled(Guid guid) => FeaturesFromDi.Enabled(guid);

        /// <summary>
        /// Checks if a list of features are enabled, in case you need many features to be activated.
        /// </summary>
        /// <param name="guids">list/array of Guids</param>
        /// <returns>true if all features are enabled, false if any one of them is not</returns>
        [Obsolete("Do not use anymore, get the IFeaturesService for this. Will not remove for a long time, because in use in public Apps like Mobius")]
        public static bool Enabled(IEnumerable<Guid> guids) => FeaturesFromDi.Enabled(guids);

    }
}
