using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks.Internal;

namespace ToSic.Sxc.Polymorphism.Internal;

/// <summary>
/// Mini service to read the polymorph config of the app
/// and then resolve the edition based on an <see cref="IPolymorphismResolver"/>
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class PolymorphConfigReader(ServiceSwitcher<IPolymorphismResolver> resolvers) : ServiceBase("Plm.Managr", connect: [resolvers ])
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

    public static string UseViewEditionOrGetLazy(IView view, Func<PolymorphConfigReader> lazyReader)
        // if Block-View comes with a preset edition, it's an ajax-preview which should be respected
        // else figure out edition using data provided by the function
        => view?.Edition.NullIfNoValue() ?? lazyReader().Edition();

    public string Edition()
    {
        var l = Log.Fn<string>();
        try
        {
            if (string.IsNullOrEmpty(Resolver)) 
                return l.ReturnNull("no resolver");

            var rInfo = resolvers.ByNameId(Resolver, true);
            if (rInfo == null)
                return l.ReturnNull("resolver not found");
            l.A($"resolver for {Resolver} found");
            var result = rInfo.Edition(Parameters, Log);

            return l.Return(result, "ok");
        }
        // We don't expect errors - but such a simple helper just shouldn't be able to throw errors
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.ReturnNull("error");
        }
    }

}