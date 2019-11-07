using System.Collections.Generic;
using ToSic.Eav.LookUp;


namespace ToSic.SexyContent.DataSources
{
    /// <inheritdoc />
    /// <summary>
    /// special "fake" value provider, which also transports the Sxc-depedency to underlying layers
    /// </summary>
    public class SxcInstanceLookUp : LookUpInDictionary
    {
        /// <inheritdoc />
        /// <summary>
        /// The class constructor, can optionally take a dictionary to reference with, otherwise creates a new one
        /// </summary>
        public SxcInstanceLookUp(string name, Dictionary<string, string> valueList, SxcInstance sxc): base(name, valueList)
        {
            SxcInstance = sxc;
        }

        public SxcInstance SxcInstance;


    }
}
