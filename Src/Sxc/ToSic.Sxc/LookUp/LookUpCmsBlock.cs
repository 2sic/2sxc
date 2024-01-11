using System.Collections.Generic;
using ToSic.Eav.DataSource.Internal.Query;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Internal;

namespace ToSic.Sxc.LookUp;

/// <inheritdoc />
/// <summary>
/// special "fake" value provider, which also transports the Sxc-dependency to underlying layers
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class LookUpCmsBlock : LookUpInDictionary
{
    /// <inheritdoc />
    /// <summary>
    /// The class constructor, can optionally take a dictionary to reference with, otherwise creates a new one
    /// </summary>
    public LookUpCmsBlock(string name, IBlock block): base(name, new Dictionary<string, string>
    {
        { QueryConstants.ParamsShowDraftsKey, block.Context.UserMayEdit.ToString() }
    })
    {
        Block = block;
    }

    public IBlock Block ;


}