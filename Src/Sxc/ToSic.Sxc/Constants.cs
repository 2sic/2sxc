namespace ToSic.Sxc
{
    internal class Constants : Eav.Constants // inherit from EAV constants to make coding easier
    {
        public const string SxcLogName = "Sxc";

        /// <summary>
        /// Additional json-node for metadata in serialized entities, if user has edit rights
        /// </summary>
        public const string JsonEntityEditNodeName = "_2sxcEditInformation";
        //public const string JsonModifiedNodeName = "Modified";
        
        /// <summary>
        /// Additional JSON node with the real EntityId - in case the "Id" property is already taken
        /// </summary>
        public const string JsonEntityIdNodeName = "EntityId";
        
        /// <summary>
        /// Wrapper tag which contains the context information.
        /// Usually just used in edit mode, but in rare cases also at runtime
        /// </summary>
        public const string DefaultContextTag = "div";
        
        /// <summary>
        /// Decorator crass to mark a content-block in the HTML
        /// </summary>
        public const string ClassToMarkContentBlock = "sc-content-block";
    }
}