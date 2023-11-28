using System;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Oqt.Shared.Models;

/// <summary>
/// Used to transfer what / how page properties should change based on the Razor file
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public struct OqtPagePropertyChanges
{
    public OqtPagePropertyOperation Change { get; set; }
    public OqtPageProperties Property { get; set; }
    public string Value { get; set; }
    public string Placeholder { get; set; }



    /// <summary>
    /// If new value has placeholder token [original], token will be replaced
    /// with old value, effectively injecting old value in new value
    /// </summary>
    /// <param name="original">old value</param>
    public OqtPagePropertyChanges InjectOriginalInValue(string original)
    {
        if (string.IsNullOrEmpty(Value) || !Value.Contains(OriginalToken, StringComparison.OrdinalIgnoreCase))
            return this;

        Value = Value?.Replace(OriginalToken, original ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        Change = OqtPagePropertyOperation.Replace;

        return this;
    }

    public const string OriginalToken = "[original]"; // new value can have [original] placeholder to inject old value in that position
}

public enum OqtPageProperties
{
    Title,
    Keywords,
    Description,
    Base
}

// equivalent to ToSic.Sxc.Web.PageChangeModes
// GetOp convert from PageChangeModes to OqtPagePropertyOperation
[PrivateApi("not final yet, probably will not be implemented like this")]

public enum OqtPagePropertyOperation
{
    /// <summary>
    /// Replace the original implementation.
    /// </summary>
    Replace,

    /// <summary>
    /// Attempt to replace, otherwise don't apply the change.
    /// </summary>
    ReplaceOrSkip,

    /// <summary>
    /// Suffix the change.
    /// </summary>
    Suffix,

    /// <summary>
    /// Prefix the change. 
    /// </summary>
    Prefix
}