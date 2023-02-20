using System;
using System.Collections.Generic;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Raw;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// Internal class to hold all the information about the role.
    /// until it's converted to an IEntity in the <see cref="Roles"/> DataSource.
    ///
    /// For detailed documentation, check the docs of the underlying objects:
    ///
    /// * TODO:
    /// * TODO:
    /// Important: this is an internal object.
    /// We're just including in in the docs to better understand where the properties come from.
    /// We'll probably move it to another namespace some day.
    /// </summary>
    /// <remarks>
    /// Make sure the property names never change, as they are critical for the created Entity.
    /// </remarks>
    [InternalApi_DoNotUse_MayChangeWithoutNotice]
    public class RoleDataRaw: RawEntityBase, IRawEntity, IRole
    {
        internal static string TypeName = "Role";
        internal static string TitleFieldName = nameof(Name);

        public string Name { get; set; }

        /// <summary>
        /// Data but without Id, Guid, Created, Modified
        /// </summary>
        [PrivateApi]
        public override Dictionary<string, object> GetProperties(CreateRawOptions options) => new Dictionary<string, object>
        {
            { nameof(Name), Name },
        };
    }
}