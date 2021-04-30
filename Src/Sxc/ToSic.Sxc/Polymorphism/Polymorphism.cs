using System;
using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Polymorphism
{
    public class Polymorphism: HasLog
    {
        public string Resolver;
        public string Parameters;
        public string Rule;
        public IEntity Entity;
        public Polymorphism(IEnumerable<IEntity> list, ILog parentLog) : base("Plm.Managr", parentLog)
        {
            Entity = list?.FirstOrDefaultOfType(PolymorphismConstants.Name);
            if (Entity == null) return;

            var rule = Entity.Value<string>(PolymorphismConstants.ModeField);

            SplitRule(rule);
        }

        /// <summary>
        /// Split the rule, which should have a "Resolver?parameters" syntax
        /// </summary>
        /// <param name="rule"></param>
        private void SplitRule(string rule)
        {
            Rule = rule;
            if (string.IsNullOrEmpty(rule)) return;
            var parts = rule.Split('?');
            Resolver = parts[0];
            if (parts.Length > 0) Parameters = parts[1];
        }

        public string Edition()
        {
            var wrapLog = Log.Call<string>();
            try
            {
                if (string.IsNullOrEmpty(Resolver)) return wrapLog("no resolver", null);

                if (!Resolvers.TryGetValue(Resolver, out var edResolver))
                    return wrapLog("resolver not found", null);
                Log.Add($"resolver for {Resolver} found");
                var result = edResolver.Edition(Parameters, Log);

                return wrapLog(null, result);
            }
            // We don't expect errors - but such a simple helper just shouldn't be able to throw errors
            catch
            {
                return wrapLog("error", null);
            }
        }

        // TODO: STATIC LIST OF ResolverTypes
        // - on first access will ask plumbing for all IResolvers
        // - then check the attributes to see what name they have
        // - add to list
        // 
        // Afterwards
        // When a resolver is needed, get it from the factory
        // Note: Almost identical setup exists for DataSources - DataSourceCatalog

        /// <summary>
        /// The global list of resolvers, used in checking what edition to return
        /// </summary>
        public static Dictionary<string, IResolver> Resolvers = new Dictionary<string, IResolver>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// Register a resolver
        /// </summary>
        /// <param name="resolver"></param>
        public static void Add(IResolver resolver) => Resolvers.Add(resolver.Name, resolver);
    }
}
