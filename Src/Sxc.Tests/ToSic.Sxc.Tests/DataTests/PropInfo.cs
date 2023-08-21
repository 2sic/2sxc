namespace ToSic.Sxc.Tests.DataTests
{
    /// <summary>
    /// Helper structure to create data describing what a property is like
    /// for tests which then use this
    /// </summary>
    public struct PropInfo
    {
        public PropInfo(string name, bool exists, bool hasData = false, object value = default, string note = default)
        {
            Note = note;
            Name = name;
            Exists = exists;
            HasData = hasData;
            Value = value;
        }
        public string Name;
        public bool Exists;
        public bool HasData;
        public object Value;
        public string Note;
        public override string ToString() => $"{Name} {Note}";

        //public static IEnumerable<object[]> ToTestEnum(IEnumerable<PropertyTestInfo> source)
        //    => source.Select(bk => new object[] { bk }).ToList();
    }
}