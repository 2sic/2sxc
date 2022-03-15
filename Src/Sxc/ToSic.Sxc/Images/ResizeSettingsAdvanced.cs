using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ToSic.Sxc.Plumbing;

namespace ToSic.Sxc.Images
{
    public class ResizeSettingsAdvanced
    {
        [JsonProperty(PropertyName = "factors")]
        public IDictionary<string, FactorRule> FactorsImport { get; set; }


        [JsonIgnore]
        public FactorRule[] Factors {
            get
            {
                if (_factorRules != null) return _factorRules;
                if (FactorsImport == null || FactorsImport.Count == 0) 
                    return _factorRules = Array.Empty<FactorRule>();

                _factorRules = FactorsImport.Select(pair =>
                    {
                        var factor = ParseObject.DoubleOrNullWithCalculation(pair.Key);
                        if (factor == null) return default;
                        pair.Value.Factor = factor.Value;
                        return pair.Value;
                    })
                    .Where(fr => fr != default)
                    .ToArray();
                return _factorRules;
            }
        }
        private FactorRule[] _factorRules;


    }
}
