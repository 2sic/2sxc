using System;

namespace ToSic.SexyContent
{
    internal class Constants : Eav.Constants // inherit from EAV constants to make coding easier
    {
        // additional json-node for metadata in serialized entities, if user has edit rights
        public const string JsonEntityEditNodeName = "_2sxcEditInformation";
        public const string JsonModifiedNodeName = "Modified";
        public const string JsonEntityIdNodeName = "EntityId";

        // Special constant to protect functions which should use named parameters
        internal const string RandomProtectionParameter = "random-y023n";

        public const string EavLogKey = "EavLog";
        public const string DnnContextKey = "DnnContext";
        public const string AdvancedLoggingEnabledKey = "2sxc-enable-extended-logging";
        public const string AdvancedLoggingTillKey = "2sxc-extended-logging-expires";

        // ReSharper disable once UnusedParameter.Local
        internal static void ProtectAgainstMissingParameterNames(string criticalParameter, string protectedMethod, string paramNames)
        {
            if (criticalParameter == null || criticalParameter != RandomProtectionParameter)
                throw new Exception($"when using '{protectedMethod}' you must use named parameters " +
                                    "- otherwise you are relying on the parameter order staying the same. " +
                                    $"this command experts params like {paramNames}");
        }


        public const string DefaultContextTag = "div";
        public const string ClassToMarkContentBlock = "sc-content-block";
    }
}