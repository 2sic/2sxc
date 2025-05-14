// 2025-05-14 2dm
// This was introduced to ensure compatibility with the old namespace
// but AFAIK all old code was never typed, so it would probably never have been using this interface anyhow!
//
// #TryToDropOldIApp: 2025-05-14 2dm v20 try to remove this for now


#if NETFRAMEWORK
// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.Interfaces
{
    /// <summary>
    /// This is the old interface with the "wrong" namespace
    /// We'll probably need to keep it alive so old code doesn't break
    /// But this interface shouldn't be enhanced or documented publicly
    /// </summary>

    [ShowApiWhenReleased(ShowApiMode.Never)]
    public interface IApp: Eav.Apps.IApp
    {
        dynamic Configuration { get;  }

        dynamic Settings { get;  }

        dynamic Resources { get;  }

        string Path { get; }

        string PhysicalPath { get; }

        string Thumbnail { get; }
    }
}
#endif