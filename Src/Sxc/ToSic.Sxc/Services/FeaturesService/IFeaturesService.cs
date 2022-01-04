using System;
using System.Collections.Generic;
using ToSic.Eav.Documentation;

// Important
// This is just the public API for this
// It basically matches the Eav.Services implementation
// So make sure to keep these in sync for the docs
namespace ToSic.Sxc.Services
{
    /// <summary>
    /// Features lets your code find out what system features are currently enabled/disabled in the environment.
    /// It's important to detect if the admin must activate certain features to let your code do it's work.
    /// </summary>
    /// <remarks>
    /// This replaces the older static Features accessor - please only use this from now on
    /// </remarks>
    [PublicApi]
    public interface IFeaturesService
    {
        /// <summary>
        /// Checks if a feature is enabled
        /// </summary>
        /// <param name="guid">The feature Guid</param>
        /// <returns>true if the feature is enabled</returns>
        bool Enabled(Guid guid);

        /// <summary>
        /// Checks if a feature is enabled
        /// </summary>
        /// <param name="nameId">The feature name ID</param>
        /// <returns>true if the feature is enabled</returns>
        /// <remarks>
        /// Added in v13.01
        /// </remarks>
        bool Enabled(string nameId);

        /// <summary>
        /// Checks if a list of features are enabled, in case you need many features to be activated.
        /// </summary>
        /// <param name="guids">list/array of Guids</param>
        /// <returns>true if all features are enabled, false if any one of them is not</returns>
        bool Enabled(IEnumerable<Guid> guids);

        /// <summary>
        /// Checks if a list of features are enabled, in case you need many features to be activated.
        /// </summary>
        /// <param name="nameIds">list/array of name IDs</param>
        /// <returns>true if all features are enabled, false if any one of them is not</returns>
        /// <remarks>
        /// Added in v13.01
        /// </remarks>
        bool Enabled(IEnumerable<string> nameIds);

        /// <summary>
        /// Informs you if the enabled features are valid or not - meaning if they have been countersigned by the 2sxc features system.
        /// As of now, it's not enforced, but in future it will be. 
        /// </summary>
        /// <returns>true if the features were signed correctly</returns>
        bool Valid { get; }

    }
}
