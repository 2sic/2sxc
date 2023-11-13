using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Lib.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Polymorphism
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class Polymorphism: ServiceBase
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

        public Polymorphism Init(IEnumerable<IEntity> list)
        {
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

        public string Edition() => Log.Func(() =>
        {
            try
            {
                if (string.IsNullOrEmpty(Resolver)) return (null, "no resolver");

                var rInfo = Cache.FirstOrDefault(r =>
                    r.Name.Equals(Resolver, StringComparison.InvariantCultureIgnoreCase));
                if (rInfo == null)
                    return (null, "resolver not found");
                Log.A($"resolver for {Resolver} found");
                var editionResolver = (IResolver)_serviceProvider.GetService(rInfo.Type);
                var result = editionResolver.Edition(Parameters, Log);

                return (result, "ok");
            }
            // We don't expect errors - but such a simple helper just shouldn't be able to throw errors
            catch
            {
                return (null, "error");
            }
        });

        private static List<ResolverInfo> Cache { get; } = AssemblyHandling
            .FindInherited(typeof(IResolver))
            .Select(t => new ResolverInfo(t))
            .ToList();
    }
}
