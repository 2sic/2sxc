using System.Collections.Generic;
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

        public override Dictionary<string, IList<string>> Relationships => new Dictionary<string, IList<string>>
        {
            { "Parent", new List<string> { $"Folder:{ParentFolderInternal}" } },
            { "Folders", new List<string> { $"FolderIn:{FullName}" } },
            { "Files", new List<string> { $"FileIn:{FullName}" } }
        };

        public override List<string> RelationshipKeys => new List<string>
        {
            // For Relationships looking for this folder
            $"Folder:{FullName}",
            // For Relationships looking for folders having a specific parent
            $"FolderIn:{ParentFolderInternal}",
        };

    }
}