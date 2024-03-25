namespace ToSic.Sxc.Compatibility.Internal;

internal class InputTypes
{
    public static string InputTypeWysiwyg = "string-wysiwyg";

    /// <summary>
    /// This lists old, obsolete or never-used input types and what the new name should be
    /// It's used because historically and for future-features, some input-types have been defined
    /// like string-wysiwyg-tinymce, but they are actually the same as string-wysiwyg-default and
    /// to keep things streamlined, we don't want to clutter the system with additional type definitions
    /// even though they are the same.
    /// </summary>
    private static readonly Dictionary<string, string> InputTypeMap = new()
    {
        // This one would be used once multiple wysiwyg implementations would need
        // to explicitly say TinyMCE. As of now, the default is the same
        { "string-wysiwyg-tinymce", InputTypeWysiwyg },

        // This one used to say "use the wysiwyg of DNN" 
        // this is currently not supported in 2sxc 10
        { "string-wysiwyg-dnn", InputTypeWysiwyg },

        // This one was thought to be a configurable wysiwyg
        // ATM this feature doesn't exist yet, so just fall back to default
        { "string-wysiwyg-adv", InputTypeWysiwyg },
    };

    internal static string MapInputTypeV10(string original) =>
        InputTypeMap.TryGetValue(original, out var result)
            ? result
            : original;
}