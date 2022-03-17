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
        internal MultiResizeRule Default => _resize ?? (_resize = FactorMapHelper.FindRuleForTarget(Rules, MultiResizeRule.RuleForDefault));
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
            //rules
            //    .Where(r => r.Type != MultiResizeRule.RuleForDefault)
            //    .ToList()
            //    .ForEach(r =>
            //    {
            //        var factor = ParseObject.DoubleOrNullWithCalculation(r.Factor);
            //        r.InitAfterLoad(factor ?? 0, r.Width, Default);
            //    });
            //var factorRules = Rules
            //    .Where(r => r.Type != MultiResizeRule.RuleForDefault)
            //    .Select(r =>
            //    {
            //        var factor = ParseObject.DoubleOrNullWithCalculation(r.Factor);
            //        return r.InitAfterLoad(factor ?? 0, r.Width, Default);
            //    })
            //    //.Where(fr => fr != default)
            //    .ToArray();
            //return factorRules;
        }

        public static MultiResizeSettings Parse(object value)
        {
            // It's already what's expected
            if (value is MultiResizeSettings mrsValue) return mrsValue;

            // It's just one rule which should be used
            if (value is MultiResizeRule mrrValue)
                return new MultiResizeSettings() { Rules = new[] { mrrValue } };
            
            return null;
        }
    }
}
