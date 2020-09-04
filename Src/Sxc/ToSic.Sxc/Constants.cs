namespace ToSic.Sxc
{
    internal class Constants : Eav.Constants // inherit from EAV constants to make coding easier
    {
        // additional json-node for metadata in serialized entities, if user has edit rights
        public const string JsonEntityEditNodeName = "_2sxcEditInformation";
        public const string JsonModifiedNodeName = "Modified";
        public const string JsonEntityIdNodeName = "EntityId";

        public const string EavLogKey = "EavLog";
        public const string DnnContextKey = "DnnContext";
        public const string AdvancedLoggingEnabledKey = "2sxc-enable-extended-logging";
        public const string AdvancedLoggingTillKey = "2sxc-extended-logging-expires";


        public const string DefaultContextTag = "div";
        public const string ClassToMarkContentBlock = "sc-content-block";
    }
}