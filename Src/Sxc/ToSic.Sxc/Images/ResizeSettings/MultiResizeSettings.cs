using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Plumbing;

namespace ToSic.Sxc.Images
{
    public class MultiResizeSettings
    {
        /// <summary>
        /// Default Resize rules for everything which isn't specified more closely in the factors
        /// </summary>
        [JsonProperty("resize")]
        public MultiResizeRuleBundle Resize { get; set; }

        [PrivateApi("Hide, as this is only relevant for importing from JSON bu shouldn't be used otherwise")]
        [JsonProperty("factors")]
        public IDictionary<string, MultiResizeRuleBundle> FactorsImport { get; set; }


        [JsonIgnore] 
        public MultiResizeRuleBundle[] Factors { get; private set; }
    
        [PrivateApi]
        public MultiResizeSettings InitAfterLoad()
        {
            if (_alreadyInit) return this;
            _alreadyInit = true;
            Resize?.InitAfterLoad(1, 0, null);
            Factors = InitFactors();
            return this;
        }
        private bool _alreadyInit;

        private MultiResizeRuleBundle[] InitFactors()
        {
            if (FactorsImport == null || FactorsImport.Count == 0)
                return Array.Empty<MultiResizeRuleBundle>();

            var factorRules = FactorsImport.Select(pair =>
                {
                    var factor = ParseObject.DoubleOrNullWithCalculation(pair.Key);
                    return pair.Value.InitAfterLoad(factor ?? 0, pair.Value.Width, Resize);
                })
                .Where(fr => fr != default)
                .ToArray();
            return factorRules;
        }
    }
}
