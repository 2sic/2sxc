using System;
using System.Collections.Generic;
using ToSic.Lib.Documentation;

namespace ToSic.Eav.Configuration
{
    /// <summary>
    /// Features lets your code find out what system features are currently enabled/disabled in the environment.
    /// It's important to detect if the admin must activate certain features to let your code do it's work.
    /// </summary>
    /// <remarks>
    /// This replaces the older static Features accessor - please only use this from now on
    /// </remarks>
    [PrivateApi("was published in previous versions of 2sxc, so we must keep this available, but don't plan on providing it any more")]
    public interface IFeaturesService
    {
        /// <summary>
        /// Checks if a feature is enabled
        /// </summary>
        /// <param name="guid">The feature Guid</param>
        /// <returns>true if the feature is enabled</returns>
        bool Enabled(Guid guid);

        /// <summary>
        /// Checks if a list of features are enabled, in case you need many features to be activated.
        /// </summary>
        /// <param name="guids">list/array of Guids</param>
        /// <returns>true if all features are enabled, false if any one of them is not</returns>
        bool Enabled(IEnumerable<Guid> guids);


        /// <summary>
        /// Informs you if the enabled features are valid or not - meaning if they have been countersigned by the 2sxc features system.
        /// As of now, it's not enforced, but in future it will be. 
        /// </summary>
        /// <returns>true if the features were signed correctly</returns>
        bool Valid { get; }

    }
}
