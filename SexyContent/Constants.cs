using System;
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
        public const string JsonModifiedNodeName = "Modified";
        public const string JsonEntityIdNodeName = "EntityId";

        // Special use cases of entities
        public const string ContentKey = "Content";
        public const string ContentKeyLower = "content";

        public const string PresentationKey = "Presentation";
        public const string PresentationKeyLower = "presentation";


        // special uses of Apps
        public const string ContentAppName = "Content";

        // Special constant to protect functions which should use named parameters
        internal const string RandomProtectionParameter = "random-y023n";
        // ReSharper disable once UnusedParameter.Local
        internal static void ProtectAgainstMissingParameterNames(string criticalParameter, string protectedMethod)
        {
            if (criticalParameter == null || criticalParameter != RandomProtectionParameter)
                throw new Exception("when using the command " + protectedMethod + ", please use named parameters - otherwise you are relying on the parameter order staying the same.");
        }

    }
}