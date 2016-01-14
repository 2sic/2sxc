using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToSic.SexyContent.Adam
{
    interface IAdamItem
    {
        bool HasMetadata { get; }
        DynamicEntity Metadata { get; }

        string Url { get; }

        string Type { get; }
    };
    }

