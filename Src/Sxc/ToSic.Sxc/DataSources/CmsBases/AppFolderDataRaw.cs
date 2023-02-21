using System.Collections.Generic;
using System.Drawing;
using ToSic.Eav.Data.Raw;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// Internal class to hold all the information about the App folders,
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
    public class AppFolderDataRaw: AppFileDataRawBase
    {
        public const string TypeName = "Folder";

        //public string FolderPath => Path;

        ///// <summary>
        ///// Data but without Id, Guid, Created, Modified
        ///// </summary>
        //[PrivateApi]
        //public override Dictionary<string, object> GetProperties(CreateRawOptions options) => new Dictionary<string, object>(base.GetProperties(options))
        //{
        //    { "FilePath", "*.*-not-a-file-path" },
        //    { nameof(FolderPath), FolderPath }
        //};
    }
}