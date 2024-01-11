using ToSic.Sxc.Internal.Plumbing;

namespace ToSic.Sxc.Web.Internal.PageService;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class Helpers
{
    public static string UpdateProperty(string original, PagePropertyChange change)
    {
        change = InjectOriginalInValue(change, original);
        if (string.IsNullOrEmpty(original)) return change.Value ?? original;

        if (!string.IsNullOrEmpty(change.ReplacementIdentifier))
        {
            var pos = original.IndexOf(change.ReplacementIdentifier, StringComparison.InvariantCultureIgnoreCase);
            if (pos >= 0)
            {
                var suffixPos = pos + change.ReplacementIdentifier.Length;
                var suffix = (suffixPos < original.Length ? original.Substring(suffixPos) : "");
                return original.Substring(0, pos) + change.Value + suffix;
            }

            if (change.ChangeMode == PageChangeModes.ReplaceOrSkip) return original;
        }

        switch (change.ChangeMode)
        {
            case PageChangeModes.Default:
            case PageChangeModes.Auto:
            case PageChangeModes.Replace:
                return change.Value ?? original;
            case PageChangeModes.Append:
                return original + change.Value;
            case PageChangeModes.Prepend:
                return change.Value + original;
            case PageChangeModes.ReplaceOrSkip:
                return original;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// If new value has placeholder token [original], token will be replaced
    /// with old value, effectively injecting old value in new value
    /// </summary>
    /// <param name="original"></param>
    /// <param name="originalValue">old value</param>
    public static PagePropertyChange InjectOriginalInValue(PagePropertyChange original, string originalValue)
    {
        if (string.IsNullOrEmpty(original.Value) || original.Value.IndexOf(OriginalToken, StringComparison.OrdinalIgnoreCase) == -1)
            return original;

        var clone = new PagePropertyChange(original);

        clone.Value = clone.Value.ReplaceIgnoreCase(OriginalToken, originalValue);
        clone.ChangeMode = PageChangeModes.Replace;

        return original;
    }

    private const string OriginalToken = "[original]"; // new value can have [original] placeholder to inject old value in that position
}