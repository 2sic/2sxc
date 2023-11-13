#if NETFRAMEWORK
// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.Interfaces
{
    /// <summary>
    /// This is the old interface with the "wrong" namespace
    /// We'll probably need to keep it alive so old code doesn't break
    /// But this interface shouldn't be enhanced or documented publicly
    /// </summary>

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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