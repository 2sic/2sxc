using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.Metadata;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// Metadata on TypedItem Objects - like <see cref="IDynamicEntity"/> or <see cref="ToSic.Sxc.Adam.IAsset"/> (files/folders).
    /// 
    /// Behaves like a normal <see cref="ITypedItem"/>, but has additional commands to detect if specific Metadata exists
    /// </summary>
    /// <remarks>
    /// Added in v16.02
    /// </remarks>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("WIP v16.02")]
    public interface IMetadataTyped : ITypedItem, IHasMetadata
    {
        bool HasType(string typeName);

        IEnumerable<IEntity> OfType(string typeName);

    }
}
