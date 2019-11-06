using System.Collections.Generic;
using ToSic.Eav.ValueProvider;
using ToSic.Eav.ValueProviders;

namespace ToSic.SexyContent.DataSources
{
    /// <inheritdoc />
    /// <summary>
    /// special "fake" value provider, which also transports the Sxc-depedency to underlying layers
    /// </summary>
    public class SxcInstanceValueProvider : StaticValueProvider
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
