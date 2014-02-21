using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToSic.SexyContent
{
    public class Element
    {
        [Obsolete("Deprecated: Use Id instead of ID")]
        public int ID { get { return this.Id; } set { this.Id = value; } }

        /// <summary>
        /// The ContentGroupID of this Element
        /// </summary>
        public int Id { get; set; }

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
        public int? EntityId { get; set; }

        /// <summary>
        /// The TemplateID of the ContentGroupItem
        /// </summary>
        public int? TemplateId { get; set; }

        /// <summary>
        /// The SortOrder of the ContentGroupItem
        /// </summary>
        public int SortOrder { get; set; }

        [Obsolete("Deprecated: Use GroupId instead of GroupID")]
        public int GroupID
        {
            get { return this.GroupId; }
            set { this.GroupId = value; }
        }

        /// <summary>
        /// The ContentGroupID of the ContentGroupItem
        /// </summary>
        public int GroupId { get; set; }
    }
}