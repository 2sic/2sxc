using System.Collections.Generic;
using ToSic.Eav.Data.Build;
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
    public class AppFileDataRaw: AppFileDataRawBase
    {
        public const string TypeName = "File";

        public static DataFactoryOptions Options = new DataFactoryOptions(typeName: TypeName, titleField: nameof(Name));

        /// <summary>
        /// The file name extension
        /// </summary>
        public string Extension { get; set; }

        public long Size { get; set; }

        /// <summary>
        /// Data but without Id, Guid, Created, Modified
        /// </summary>
        [PrivateApi]
        public override Dictionary<string, object> Attributes(RawConvertOptions options) => new Dictionary<string, object>(base.Attributes(options))
        {
            { nameof(Extension), Extension },
            { nameof(Size), Size },
        };

        public override IEnumerable<object> RelationshipKeys(RawConvertOptions options) => new List<object> { $"FileIn:{ParentFolderInternal}" };

    }
}