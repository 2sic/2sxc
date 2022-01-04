﻿using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// Metadata on Dynamic Objects - like <see cref="IDynamicEntity"/> or <see cref="ToSic.Sxc.Adam.IAsset"/> (files/folders).
    /// 
    /// Behaves like a normal DynamicEntity, but has additional commands to detect if specific Metadata exists
    /// </summary>
    /// <remarks>
    /// Added in v13
    /// </remarks>
    [PublicApi]
    public interface IDynamicMetadata: IDynamicEntity, IHasMetadata
    {
        bool HasType(string typeName);

        IEnumerable<IEntity> OfType(string typeName);

    }
}