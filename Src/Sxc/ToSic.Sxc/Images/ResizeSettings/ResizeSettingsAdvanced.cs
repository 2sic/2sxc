using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ToSic.Sxc.Plumbing;

namespace ToSic.Sxc.Images
{
    public class ResizeSettingsAdvanced
    {
        /// <summary>
        /// Default Resize rules for everything which isn't specified more closely in the factors
        /// </summary>
        [JsonProperty("resize")]
        public ResizeSettingsBundle Resize { get; set; }

        [JsonProperty("factors")]
        public IDictionary<string, ResizeSettingsBundle> FactorsImport { get; set; }


        [JsonIgnore] 
        public ResizeSettingsBundle[] Factors { get; private set; }
    

        public ResizeSettingsAdvanced InitAfterLoad()
        {
            if (_alreadyInit) return this;
            _alreadyInit = true;
            Resize?.InitAfterLoad(1, 0, null); // = (Resize ?? new ResizeSettingsBundle()).InitAfterLoad();
            Factors = InitFactors();
            return this;
        }
        private bool _alreadyInit;

        private ResizeSettingsBundle[] InitFactors()
        {
            if (FactorsImport == null || FactorsImport.Count == 0)
                return Array.Empty<ResizeSettingsBundle>();

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
