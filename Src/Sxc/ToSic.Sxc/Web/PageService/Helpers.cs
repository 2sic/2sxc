using System;


namespace ToSic.Sxc.Web.PageService
{
    public class Helpers
    {
        public static string UpdateProperty(string original, PagePropertyChange change)
        {
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
    }
}
