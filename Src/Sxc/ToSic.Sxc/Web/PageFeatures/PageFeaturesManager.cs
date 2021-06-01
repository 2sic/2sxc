using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ToSic.Sxc.Web.PageFeatures
{
    public class PageFeaturesManager: IPageFeaturesManager
    {
        /// <summary>
        /// Constructor - ATM we'll just add our known services here. 
        /// </summary>
        /// <remarks>
        /// Important: if you want to add more services in a DI Startup, it must happen at Configure.
        /// If you do it earlier, the singleton retrieved then will not be the one at runtime.
        /// </remarks>
        public PageFeaturesManager()
        {
            Register(
                BuiltInFeatures.PageContext,
                BuiltInFeatures.Core,
                BuiltInFeatures.EditApi,
                BuiltInFeatures.EditUi,
                BuiltInFeatures.TurnOn
            );
        }
        
        private readonly ConcurrentDictionary<string, IPageFeature> _features = new ConcurrentDictionary<string, IPageFeature>(StringComparer.InvariantCultureIgnoreCase);

        /// <inheritdoc />
        public IReadOnlyDictionary<string, IPageFeature> Features { get; private set; }

        public void Register(params IPageFeature[] features)
        {
            // add all features if it doesn't yet exist, otherwise update
            foreach (var f in features)
                if (f != null)
                    _features.AddOrUpdate(f.Key, f, (key, existing) => f);
            
            // Reset the read-only dictionary
            Features = new ReadOnlyDictionary<string, IPageFeature>(_features);
        }

        public List<IPageFeature> GetWithDependents(List<string> keys)
        {
            if (keys == null || !keys.Any()) return new List<IPageFeature>();
            var collected = Get(keys);

            var added = collected;
            
            // Go max 5 levels deep
            for (var i = 0; i < 5; i++)
            {
                added = GetMissingDependencies(collected, added);
                if (!added.Any()) break;
                collected.AddRange(added);
            }
            return collected.Distinct().ToList();
        }

        private List<IPageFeature> GetMissingDependencies(List<IPageFeature> target, List<IPageFeature> toCheck)
        {
            // See what we still need
            var requiredKeys = toCheck.SelectMany(f => f.Requires).ToArray();
            
            // Skip those already in the target
            var missing = requiredKeys.Where(si =>
                !target.Any(c => c.Key.Equals(si, StringComparison.InvariantCultureIgnoreCase))).ToArray();
            return Get(missing);
        }

        private List<IPageFeature> Get(IEnumerable<string> keys) => 
            Features.Values.Where(f => keys.Contains(f.Key, StringComparer.InvariantCultureIgnoreCase)).ToList();
    }
}
