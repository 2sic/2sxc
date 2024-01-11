namespace ToSic.Sxc.Internal.Plumbing;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal static class StringExtensions
{
    /// <summary>
    /// Returns a new string in which all occurrences of a specified string in the current instance are replaced with another specified string.
    /// This is to be used in .NET Framework or .netstandard 2.0 because .NET 5+ already has this string.Replace() method
    /// https://stackoverflow.com/a/36317315
    /// </summary>
    /// <param name="value">The string performing the replace method.</param>
    /// <param name="oldValue">The string to be replaced.</param>
    /// <param name="newValue">The string replace all occurrences of oldValue.</param>
    /// <param name="comparisonType">Type of the comparison.</param>
    /// <returns></returns>
    public static string ReplaceIgnoreCase(this string value, string oldValue, string newValue, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
    {
        oldValue ??= string.Empty;
        newValue ??= string.Empty;
        if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(oldValue) || oldValue.Equals(newValue, comparisonType))
            return value;

        int foundAt;
        while ((foundAt = value.IndexOf(oldValue, 0, comparisonType)) != -1)
            value = value.Remove(foundAt, oldValue.Length).Insert(foundAt, newValue);

        return value;
    }


}