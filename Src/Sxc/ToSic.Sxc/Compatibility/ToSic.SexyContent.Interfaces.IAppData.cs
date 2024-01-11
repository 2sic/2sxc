#if NETFRAMEWORK


// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.Interfaces
{
    // Note: we can't just remove this.
    // Code which uses it, would work because it simulates the Eav IAppData
    // So even if it looks unused, it could still be used
    [Obsolete("please use the Eav.Apps.Interfaces.IAppData instead")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public interface IAppData: Eav.Apps.IAppData { }
}

#endif