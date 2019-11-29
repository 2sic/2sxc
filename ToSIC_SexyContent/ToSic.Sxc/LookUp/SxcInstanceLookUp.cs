using System.Collections.Generic;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Blocks;


namespace ToSic.SexyContent.DataSources
{
    /// <inheritdoc />
    /// <summary>
    /// special "fake" value provider, which also transports the Sxc-dependency to underlying layers
    /// </summary>
    public class SxcInstanceLookUp : LookUpInDictionary
    {
        /// <inheritdoc />
        /// <summary>
        /// The class constructor, can optionally take a dictionary to reference with, otherwise creates a new one
        /// </summary>
        public SxcInstanceLookUp(string name, Dictionary<string, string> valueList, ICmsBlock cms): base(name, valueList)
        {
            CmsInstance = cms;
        }

        public ICmsBlock CmsInstance;


    }
}
