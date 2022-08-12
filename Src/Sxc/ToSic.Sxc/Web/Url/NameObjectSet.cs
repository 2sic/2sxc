namespace ToSic.Sxc.Web.Url
{
    public class NameObjectSet
    {

        public NameObjectSet(string name, object value, string prefix = null)
        {
            Name = name;
            Value = value;
            Prefix = prefix;
        }

        public NameObjectSet(NameObjectSet original, string name = null, object value = null, bool? keep = null)
        {
            Prefix = original?.Prefix;
            Name = name ?? original?.Name;
            Value = value ?? original?.Value;
            Keep = keep ?? original?.Keep ?? true;
        }

        public string Prefix { get; }

        public string Name { get; }
        public object Value { get; }
        public bool Keep { get; }

        public string FullName => Prefix + Name;

    }
}
