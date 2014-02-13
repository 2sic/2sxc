using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToSic.SexyContent
{
    public class Element
    {
        /// <summary>
        /// The ContentGroupID of this Element
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The DynamicContent object, as dynamic
        /// </summary>
        public dynamic Content { get; set; }

        /// <summary>
        /// The Presentation object, as dynamic
        /// </summary>
        public dynamic Presentation { get; set; }
        
        /// <summary>
        /// The ViewConfig object, as dynamic / DynamicContent
        /// </summary>
        public dynamic ViewConfig { get; set; }

        /// <summary>
        /// The EntityID of the ContentGroupItem
        /// </summary>
        public int? EntityID { get; set; }

        /// <summary>
        /// The TemplateID of the ContentGroupItem
        /// </summary>
        public int? TemplateID { get; set; }

        /// <summary>
        /// The SortOrder of the ContentGroupItem
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// The ContentGroupID of the ContentGroupItem
        /// </summary>
        public int GroupID { get; set; }
    }
}