using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Polymorphism
{
    public class Polymorphism: HasLog
    {
        private readonly IServiceProvider _serviceProvider;
        public string Resolver;
        public string Parameters;
        public string Rule;
        public IEntity Entity;
       
        public Polymorphism(IServiceProvider serviceProvider) : base("Plm.Managr")
        {
            _serviceProvider = serviceProvider;
        }

        public Polymorphism Init(IEnumerable<IEntity> list, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            Entity = list?.FirstOrDefaultOfType(PolymorphismConstants.Name);
            if (Entity == null) return this;

            var rule = Entity.Value<string>(PolymorphismConstants.ModeField);

            SplitRule(rule);
            return this;
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

                var rInfo = Cache.FirstOrDefault(r => r.Name.Equals(Resolver, StringComparison.InvariantCultureIgnoreCase));
                if (rInfo == null)
                    return wrapLog("resolver not found", null);
                Log.A($"resolver for {Resolver} found");
                var editionResolver = (IResolver)_serviceProvider.GetService(rInfo.Type);
                var result = editionResolver.Edition(Parameters, Log);

                return wrapLog(null, result);
            }
            // We don't expect errors - but such a simple helper just shouldn't be able to throw errors
            catch
            {
                return wrapLog("error", null);
            }
        }

        private static List<ResolverInfo> Cache { get; } = AssemblyHandling
            .FindInherited(typeof(IResolver))
            .Select(t => new ResolverInfo(t))
            .ToList();
    }
}
