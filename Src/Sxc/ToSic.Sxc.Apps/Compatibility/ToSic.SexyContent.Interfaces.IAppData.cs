// 2025-05-14 2dm
// This was introduced to ensure compatibility with the old namespace
// but AFAIK all old code was never typed, so it would probably never have been using this interface anyhow!
//
// #TryToDropOldIAppData: 2025-05-14 2dm v20 try to remove this for now

//#if NETFRAMEWORK


//// ReSharper disable once CheckNamespace
//namespace ToSic.SexyContent.Interfaces
//{
//    // Note: we can't just remove this.
//    // Code which uses it, would work because it simulates the Eav IAppData
//    // So even if it looks unused, it could still be used
//    [Obsolete("please use the Eav.Apps.Interfaces.IAppData instead")]
//    [ShowApiWhenReleased(ShowApiMode.Never)]
//    public interface IAppData : Eav.Apps.IAppData { }
//}

//#endif