namespace ToSic.Sxc.Data.Sys.Typed;

internal interface IValueOverrider
{
    /// <summary>
    /// Replace the raw value before any parsing/conversion happens.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="originalValue"></param>
    /// <returns></returns>
    string? String(string name, string? originalValue);
}