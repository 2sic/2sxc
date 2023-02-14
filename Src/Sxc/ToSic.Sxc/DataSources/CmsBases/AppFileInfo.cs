using System;
using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Raw;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// Internal class to hold all the information about the App files,
    /// until it's converted to an IEntity in the <see cref="AppFiles"/> DataSource.
    ///
    /// Important: this is an internal object.
    /// We're just including in in the docs to better understand where the properties come from.
    /// We'll probably move it to another namespace some day.
    /// </summary>
    /// <remarks>
    /// Make sure the property names never change, as they are critical for the created Entity.
    /// </remarks>
    [InternalApi_DoNotUse_MayChangeWithoutNotice]
    public class AppFileInfo: IRawEntity
    {
        public const string TypeName = "File";

        public int Id { get; set; }

        public Guid Guid { get; set; }

        /// <summary>
        /// The file name with extension
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The file name without extension
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The file name extension
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// Starting in the App-Root
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Starting in the App-Root
        /// </summary>
        public string Folder { get; set; }

        public int ParentId { get; set; }

        public bool IsFolder { get; set; }

        public long Size { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }


        /// <summary>
        /// Data but without Id, Guid, Created, Modified
        /// </summary>
        [PrivateApi]
        public Dictionary<string, object> RawProperties => new Dictionary<string, object>
        {
            { Attributes.TitleNiceName, Title },
            { nameof(Name), Name },
            { nameof(Extension), Extension },
            //{ nameof(FullName), FullName },
            { nameof(Folder), Folder },
            { nameof(ParentId), ParentId },
            //{ nameof(IsFolder), IsFolder },
            { nameof(Size), Size }
        };
    }
}