using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Services.FileSystem;
using ToSic.Eav;

namespace ToSic.SexyContent.Adam
{
    public class AdamFile : FileInfo
    {
        public App App;

        /// <summary>
        /// Metadata for this file
        /// This is usually an entity which has additional information related to this file
        /// </summary>
        public IEntity Metadata {
            get
            {
                return App.Data.Metadata.GetAssignedEntities(Constants.AssignmentObjectTypeCmsObject, "file:" + FileId)
                    .FirstOrDefault();
            }
        }
    }
}