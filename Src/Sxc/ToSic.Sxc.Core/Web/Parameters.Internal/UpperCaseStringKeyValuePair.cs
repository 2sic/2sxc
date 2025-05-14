namespace ToSic.Sxc.Web.Parameters;

/// <summary>
/// Workaround for deserializing KeyValuePair - it requires lowercase properties (case sensitive), 
/// which seems to be a issue in some Newtonsoft.Json versions: http://stackoverflow.com/questions/11266695/json-net-case-insensitive-property-deserialization
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Local
internal class UpperCaseStringKeyValuePair
{
    // ReSharper disable UnusedAutoPropertyAccessor.Local
    public string Key { get; set; }
    public string Value { get; set; }
    // ReSharper restore UnusedAutoPropertyAccessor.Local

}