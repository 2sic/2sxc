using System.Collections.Generic;
using ToSic.Eav.ValueProvider;

namespace ToSic.SexyContent.DataSources
{
    /// <inheritdoc />
    /// <summary>
    /// Property Accessor to test a Pipeline with Static Values
    /// </summary>
    public class SxcInstanceValueProvider : StaticValueProvider// IValueProvider
    {
        /// <inheritdoc />
        /// <summary>
        /// The class constructor, can optionally take a dictionary to reference with, otherwise creates a new one
        /// </summary>
        public SxcInstanceValueProvider(string name, Dictionary<string, string> valueList, SxcInstance sxc): base(name, valueList)
        {
            SxcInstance = sxc;
        }

        public SxcInstance SxcInstance;


    }
}
