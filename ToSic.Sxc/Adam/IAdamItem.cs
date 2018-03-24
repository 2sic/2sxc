using ToSic.SexyContent;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Adam
{
    internal interface IAdamItem
    {
        #region Metadata
        bool HasMetadata { get; }
        DynamicEntity Metadata { get; }
        #endregion


        string Url { get; }

        string Type { get; }

        string Name { get; }
    };
}

