﻿using System;
using System.Collections.Generic;
using ToSic.Eav.Data.New;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.DataSources
{

    [InternalApi_DoNotUse_MayChangeWithoutNotice]
    public class AdamItemDataNew: INewEntity
    {
        public const string TypeName = "AdamItem";

        public int Id { get; set; }
        public Guid Guid { get; set; }

        /// <summary>
        /// The file name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// This contains the code like "file:2742"
        /// </summary>
        public string ReferenceId { get; set; }

        /// <summary>
        /// Normal url to access the resource
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The Adam type, such as "folder", "image" etc.
        /// </summary>
        public string Type { get; set; }

        public bool IsFolder { get; set; }

        public int Size { get; set; }
        public string Path { get; set; }
        

        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }


        /// <summary>
        /// Data but without Id, Guid, Created, Modified
        /// </summary>
        [PrivateApi]
        public virtual Dictionary<string, object> GetProperties(CreateFromNewOptions options) => new Dictionary<string, object>
        {
            { nameof(Name), Name },
            { nameof(ReferenceId), ReferenceId },
            { nameof(Url), Url },
            { nameof(Type), Type },
            { nameof(IsFolder), IsFolder },
            { nameof(Size), Size },
            { nameof(Path), Path }
        };
    }
}