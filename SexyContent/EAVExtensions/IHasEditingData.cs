using System;

namespace ToSic.SexyContent.EAVExtensions
{
    interface IHasEditingData
    {
        int SortOrder { get; set; }
        DateTime ContentGroupItemModified { get; set; }
    }
}
