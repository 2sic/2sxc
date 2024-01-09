using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Polymorphism.Internal;

/// <summary>
/// Mini service to read the polymorph config of the app
/// and then resolve the edition based on an <see cref="IResolver"/>
/// </summary>
/// <param name="serviceProvider"></param>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class PolymorphConfigReader(IServiceProvider serviceProvider) : ServiceBase("Plm.Managr")
{
    public string Resolver;
    public string Parameters;
    public string Rule;
    public IEntity Entity;

    public PolymorphConfigReader Init(IEnumerable<IEntity> list)
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

            var rInfo = ResolversCache.FirstOrDefault(r => r.Name.EqualsInsensitive(Resolver));
            if (rInfo == null)
                return (null, "resolver not found");
            Log.A($"resolver for {Resolver} found");
            var editionResolver = (IResolver)serviceProvider.GetService(rInfo.Type);
            var result = editionResolver.Edition(Parameters, Log);

            return (result, "ok");
        }
        // We don't expect errors - but such a simple helper just shouldn't be able to throw errors
        catch
        {
            return (null, "error");
        }
    });

    private static List<ResolverInfo> ResolversCache { get; } = AssemblyHandling
        .FindInherited(typeof(IResolver))
        .Select(t => new ResolverInfo(t))
        .ToList();
}