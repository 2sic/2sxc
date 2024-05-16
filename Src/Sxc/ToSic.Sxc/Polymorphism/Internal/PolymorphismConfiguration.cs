namespace ToSic.Sxc.Polymorphism.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class PolymorphismConfiguration(IEntity entity) : EntityBasedType(entity)
{
    public const string StaticName = "3937fa17-ef2d-40a7-b089-64164eb10bab";
    public const string Name = "2sxcPolymorphismConfiguration";

    public string Mode => GetThis("");

    public string UsersWhoMaySwitchEditions => GetThis("");

    public List<int> UsersWhoMaySwitch => _usersWhoMaySwitch ??= new Func<List<int>>(() => UsersWhoMaySwitchEditions
        .Split(',')
        .Select(s => s.Trim())
        .Select(s => int.TryParse(s, out var result) ? result : -1)
        .Where(i => i > 0)
        .ToList()
    )();
    private List<int> _usersWhoMaySwitch;
}