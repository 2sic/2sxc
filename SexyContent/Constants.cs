using System.Configuration;

namespace ToSic.SexyContent
{
    internal class Constants : Eav.Constants // inherit from EAV constants to make coding easier
    {
        public static string[] ExcludeFolders =
        {
            ".git",
            "node_modules",
            "bower_components",
            ".vs",
            ".data"
        };

        // additional json-node for metadata in serialized entities, if user has edit rights
        public const string JsonEntityEditNodeName = "_2sxcEditInformation";

        // Special use cases of entities
        public const string ContentKey = "Content";
        public const string ContentKeyLower = "content";

        public const string PresentationKey = "Presentation";
        public const string PresentationKeyLower = "presentation";


        // special uses of Apps
        public const string ContentAppName = "Content";
    }
}