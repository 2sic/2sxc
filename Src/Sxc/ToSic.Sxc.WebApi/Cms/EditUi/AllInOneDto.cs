using System.Collections.Generic;
using Newtonsoft.Json;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Configuration;
using ToSic.Eav.ImportExport.Json.V1;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Formats;

namespace ToSic.Sxc.WebApi.Cms
{
    public class AllInOneDto
    {
        /// <summary>
        /// The items in the load/save package.
        /// </summary>
        public List<BundleWithHeader<JsonEntity>> Items;

        /// <summary>
        /// Content-Type information for the form
        /// </summary>
        public List<JsonContentType> ContentTypes;

        /// <summary>
        /// Relevant input-types and their default labels & configuration
        /// </summary>
        public List<InputTypeInfo> InputTypes;

        /// <summary>
        /// If this set is to be published - default true
        /// </summary>
        public bool IsPublished = true;

        /// <summary>
        /// If this set is to not-published, whether it should branch, leaving the original published
        /// </summary>
        public bool DraftShouldBranch = false;

        /// <summary>
        /// List of system features the UI should know about
        /// </summary>
        public List<Feature> Features;

        /// <summary>
        /// Experimental additional data for configuration
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<JsonEntity> ContentTypeItems;

        public ContextDto Context;
    }

}
