using ToSic.Lib.Documentation;

namespace ToSic.Sxc
{
    [PrivateApi]
    public class Constants : Eav.Constants // inherit from EAV constants to make coding easier
    {
        public const string SxcLogName = "Sxc";
        public static string SxcLogAppCodeLoader = "global-app-code-compiler";

        /// <summary>
        /// Additional json-node for metadata in serialized entities, if user has edit rights
        /// </summary>
        public const string JsonEntityEditNodeName = "_2sxcEditInformation";

        /// <summary>
        /// Wrapper tag which contains the context information.
        /// Usually just used in edit mode, but in rare cases also at runtime
        /// </summary>
        public const string DefaultContextTag = "div";
        
        /// <summary>
        /// Decorator crass to mark a content-block in the HTML
        /// </summary>
        public const string ClassToMarkContentBlock = "sc-content-block";

        internal const int CompatibilityLevel9Old = 9;

        /// <summary>
        /// This enforces certain features to go away or appear, like
        /// - Off: DynamiEntity.Render
        /// </summary>
        public const int CompatibilityLevel10 = 10;

        public const int CompatibilityLevel12 = 12;

        public const int CompatibilityLevel16 = 16;

        public const int MaxLevelForAutoJQuery = CompatibilityLevel9Old;
        public const int MaxLevelForEntityDotToolbar = CompatibilityLevel9Old;
        public const int MaxLevelForEntityDotRender = CompatibilityLevel9Old;

        public const int MaxLevelForStaticRender = CompatibilityLevel10;

        public const string Anonymous = "anonymous";

        /// <summary>
        /// Name of the web.config file which is copied to the 2sxc folder.
        /// Probably only used in DNN
        /// </summary>
        public static readonly string WebConfigFileName = "web.config";

        /// <summary>
        /// Name of the template web.config file which is copied to each 2sxc-folder
        /// </summary>
        public static readonly string WebConfigTemplateFile = "WebConfigTemplate.config";
    }
}