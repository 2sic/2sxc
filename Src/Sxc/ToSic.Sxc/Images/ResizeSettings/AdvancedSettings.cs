using System;
using Newtonsoft.Json;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Images
{
    public class AdvancedSettings
    {
        [JsonConstructor]
        public AdvancedSettings(Recipe recipe = default)
        {
            Recipe = recipe;

            // Initialize the recipe and all sub-recipes
            Recipe?.InitAfterLoad();
        }

        /// <summary>
        /// Default Resize rules for everything which isn't specified more closely in the factors
        /// </summary>
        [JsonIgnore]
        internal Recipe Recipe { get; }

        [PrivateApi]
        public static AdvancedSettings Parse(object value) => InnerParse(value);

        private static AdvancedSettings InnerParse(object value)
        {
            if (value == null) return null;

            // It's already what's expected
            if (value is AdvancedSettings mrsValue) return mrsValue;

            // Parse any string which would be a typical MRS - convert to single rule
            if (value is string strValue && !string.IsNullOrWhiteSpace(strValue))
                value = new Recipe(variants: strValue);

            // Parse any single rule It's just one rule which should be used
            if (value is Recipe mrrValue)
                return new AdvancedSettings(mrrValue);

            return null;
        }

        [PrivateApi]
        public static AdvancedSettings FromJson(object value, ILog log = null)
        {
            var wrapLog = log.SafeCall<AdvancedSettings>();
            try
            {
                if (value is string advString && !string.IsNullOrWhiteSpace(advString))
                    return wrapLog("create", JsonConvert.DeserializeObject<AdvancedSettings>(advString));
            }
            catch (Exception ex)
            {
                log?.Add($"error converting json to AdvancedSettings. Json: {value}");
                log?.Exception(ex);
            }
            return wrapLog("new", new AdvancedSettings());
        }

    }
}
