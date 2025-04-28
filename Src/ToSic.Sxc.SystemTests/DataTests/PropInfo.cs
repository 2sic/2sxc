using Xunit.Abstractions;

namespace ToSic.Sxc.DataTests;

/// <summary>
/// Helper structure to create data describing what a property is like
/// for tests which then use this
/// </summary>
public struct PropInfo(string name, bool exists, bool hasData = false, object value = default, string note = default): IXunitSerializable
{
    public string Name = name;
    public bool Exists = exists;
    public bool HasData = hasData;
    public object Value = value;
    public string Note = note;
    public override string ToString() => $"{Name} {Note}";

    //public static IEnumerable<object[]> ToTestEnum(IEnumerable<PropertyTestInfo> source)
    //    => source.Select(bk => new object[] { bk }).ToList();
    public void Deserialize(IXunitSerializationInfo info)
    {
        //throw new NotImplementedException();
    }

    public void Serialize(IXunitSerializationInfo info)
    {
        //throw new NotImplementedException();
    }
}