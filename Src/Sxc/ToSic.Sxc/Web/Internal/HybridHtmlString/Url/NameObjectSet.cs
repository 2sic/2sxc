namespace ToSic.Sxc.Web.Internal.Url;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class NameObjectSet
{

    public NameObjectSet(string name, object value, string prefix = default)
    {
        Name = name;
        Value = value;
        Prefix = prefix;
    }

    public NameObjectSet(NameObjectSet original, string name = default, object value = default, bool? keep = default, string prefix = default)
    {
        Prefix = original?.Prefix;
        Name = name ?? original?.Name;
        Value = value ?? original?.Value;
        Keep = keep ?? original?.Keep ?? Keep;
        Prefix = prefix ?? original?.Prefix;
    }

    public string Prefix { get; }

    public string Name { get; }
    public object Value { get; }
    public bool Keep { get; } = true;

    public string FullName => Prefix + Name;

}