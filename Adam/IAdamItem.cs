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

