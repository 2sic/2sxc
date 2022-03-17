using System;
using System.Linq;
using Newtonsoft.Json;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Plumbing;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Images
{
    public class MultiResizeSettings
    {
        /// <summary>
        /// Default Resize rules for everything which isn't specified more closely in the factors
        /// </summary>
        [JsonIgnore]
        internal MultiResizeRule Default => _resize ?? (_resize = ResizeSettingsHelper.FindRuleForTarget(Rules, MultiResizeRule.RuleForDefault));
        private MultiResizeRule _resize;

        [JsonProperty("rules")]
        public MultiResizeRule[] Rules { get; set; }
    
        [PrivateApi]
        public MultiResizeSettings InitAfterLoad()
        {
            if (_alreadyInit) return this;
            _alreadyInit = true;

            // Ensure Factors not null
            Rules = Rules ?? Array.Empty<MultiResizeRule>();

            // Init Default first
            Default?.InitAfterLoad(1, 0, null);

            Rules = InitFactors();
            return this;
        }
        private bool _alreadyInit;

        private MultiResizeRule[] InitFactors()
        {
            // Drop null-rules
            var rules = Rules.Where(fr => fr != default).ToArray();

            // Only init non-defaults, as that should only exist once and was already initialized
            foreach (var r in rules.Where(r => r != Default))
            {
                var factor = ParseObject.DoubleOrNullWithCalculation(r.Factor);
                r.InitAfterLoad(factor ?? 0, r.Width, Default);
            }
            return rules;
        }

        public static MultiResizeSettings Parse(object value) => InnerParse(value)?.InitAfterLoad();

        private static MultiResizeSettings InnerParse(object value)
        {
            if (value == null) return null;

            // It's already what's expected
            if (value is MultiResizeSettings mrsValue) return mrsValue;

            // Parse any string which would be a typical MRS - convert to single rule
            if (value is string strValue && !string.IsNullOrWhiteSpace(strValue))
                value = new MultiResizeRule { SrcSet = strValue };

            // Parse any single rule It's just one rule which should be used
            if (value is MultiResizeRule mrrValue)
                return new MultiResizeSettings { Rules = new[] { mrrValue } };

            return null;
        }
    }
}
