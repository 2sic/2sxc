namespace ToSic.Sxc.Polymorphism.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class PolymorphismConfiguration(IEntity entity) : EntityBasedType(entity)
{
    public PolymorphismConfiguration(IEnumerable<IEntity> list): this(list?.FirstOrDefaultOfType(Name))
    {
    }

    public const string StaticName = "3937fa17-ef2d-40a7-b089-64164eb10bab";
    public const string Name = "2sxcPolymorphismConfiguration";

    public string Mode => GetThisIfEntity("");

    public string UsersWhoMaySwitchEditions => GetThisIfEntity("");

    public List<int> UsersWhoMaySwitch => _usersWhoMaySwitch ??= new Func<List<int>>(() => UsersWhoMaySwitchEditions
        .Split(',')
        .Select(s => s.Trim())
        .Select(s => int.TryParse(s, out var result) ? result : -1)
        .Where(i => i > 0)
        .ToList()
    )();
    private List<int> _usersWhoMaySwitch;

    public string Resolver => SplitMode().Resolver;

    public string Parameters => SplitMode().Parameters;

    private (string Resolver, string Parameters) SplitMode()
    {
        if (_resolverAndParameters != default)
            return _resolverAndParameters;

        var rule = Mode;
        if (string.IsNullOrEmpty(Mode)) return (null, null);
        var parts = rule.Split('?');
        var resolver = parts[0];
        var parameters = parts.Length > 0 ? parts[1] : null;
        return _resolverAndParameters = (resolver, parameters);
    }
    private (string Resolver, string Parameters) _resolverAndParameters;
}