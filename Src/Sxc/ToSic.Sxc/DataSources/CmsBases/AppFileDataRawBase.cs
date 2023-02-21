using System;
using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Raw;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.DataSources
{

    [InternalApi_DoNotUse_MayChangeWithoutNotice]
    public abstract class AppFileDataRawBase: IRawEntity
    {

        public int Id { get; set; }

        public Guid Guid { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        /// <summary>
        /// Starting in the App-Root
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Starting in the App-Root
        /// </summary>
        public string Path { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }


        /// <summary>
        /// Data but without Id, Guid, Created, Modified
        /// </summary>
        [PrivateApi]
        public virtual Dictionary<string, object> GetProperties(CreateRawOptions options) => new Dictionary<string, object>
        {
            { nameof(Name), Name },
            { nameof(FullName), FullName },
            { nameof(Path), Path }
        };
    }
}