using System.Collections.Generic;

namespace ToSic.Sxc.Compatibility
{
    public class InputTypes
    {
        /// <summary>
        /// This lists old, obsolete or never-used input types and what the new name should be
        /// It's used because historically and for future-features, some input-types have been defined
        /// like string-wysiwyg-tinymce, but they are actually the same as string-wysiwyg-default and
        /// to keep things streamlined, we don't want to clutter the system with additional type definitions
        /// even though they are the same.
        /// </summary>
        public static Dictionary<string, string> InputTypeMap = new Dictionary<string, string>
        {
            // This one would be used once multiple wysiwyg implementations would need
            // to explicitly say TinyMCE. As of now, the default is the same
            { "string-wysiwyg-tinymce", Eav.Data.InputTypes.InputTypeWysiwyg },

            // This one used to say "use the wysiwyg of DNN" 
            // this is currently not supported in 2sxc 10
            { "string-wysiwyg-dnn", Eav.Data.InputTypes.InputTypeWysiwyg },

            // This one was thought to be a configurable wysiwyg
            // ATM this feature doesn't exist yet, so just fall back to default
            { "string-wysiwyg-adv", Eav.Data.InputTypes.InputTypeWysiwyg },
        };

        public static string MapInputTypeV10(string original) =>
            InputTypeMap.TryGetValue(original, out var result)
                ? result
                : original;
    }
}
