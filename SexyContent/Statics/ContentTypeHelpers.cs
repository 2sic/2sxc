using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToSic.Eav;

namespace ToSic.SexyContent.Statics
{
    public class ContentTypeHelpers
    {
        /// <summary>
        /// Returns the Default AssignmentObjectTypeID (no assignment / default)
        /// </summary>
        public static int AssignmentObjectTypeIDDefault
        {
            get
            {
                return Eav.Configuration.AssignmentObjectTypeIdDefault;
            }
        }

        /// <summary>
        /// Returns the AssignmentObjectTypeID for 2sxc Templates
        /// </summary>        
        [Obsolete("Do not use this anymore")]
        public static int AssignmentObjectTypeIDSexyContentTemplate
        {
            get
            {
                return DataSource.GetCache(Constants.DefaultZoneId, Constants.MetaDataAppId).GetAssignmentObjectTypeId("2SexyContent-Template");
            }
        }

        /// <summary>
        /// Returns the AssignmentObjectTypeID for 2sxc Apps
        /// </summary>        
        public static int AssignmentObjectTypeIDSexyContentApp
        {
            get
            {
                return DataSource.GetCache(Constants.DefaultZoneId, Constants.MetaDataAppId).GetAssignmentObjectTypeId("App");
            }
        }
    }
}