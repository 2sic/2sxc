using System.Collections.Generic;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.LookUp
{
    /// <inheritdoc />
    /// <summary>
    /// special "fake" value provider, which also transports the Sxc-dependency to underlying layers
    /// </summary>
    public class LookUpCmsBlock : LookUpInDictionary
    {
        /// <inheritdoc />
        /// <summary>
        /// The class constructor, can optionally take a dictionary to reference with, otherwise creates a new one
        /// </summary>
        public LookUpCmsBlock(string name, Dictionary<string, string> valueList, IBlockBuilder cms): base(name, valueList)
        {
            BlockBuilder = cms;
        }

        public IBlockBuilder BlockBuilder;


    }
}
