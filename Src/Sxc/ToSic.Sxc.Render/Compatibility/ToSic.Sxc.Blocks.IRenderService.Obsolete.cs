// #RemoveBlocksIRenderService

//#if NETFRAMEWORK
//using ToSic.Razor.Markup;
//using ToSic.Sxc.Data;

//namespace ToSic.Sxc.Blocks;

///// <summary>
///// Old name for the IRenderService, it's in use in some v12 App templates so we must keep it working.
///// Will continue to work, but shouldn't be used. Please use <see cref="ToSic.Sxc.Services.IRenderService"/>  instead
///// </summary>
//[Obsolete("Old name, used in 2-3 v12 apps released. Pls use ToSic.Sxc.Services.IRenderService instead.")]
//[ShowApiWhenReleased(ShowApiMode.Never)]
//public interface IRenderService
//{
//    [PrivateApi]
//    [Obsolete("Old interface. Pls use ToSic.Sxc.Services.IRenderService instead.")]
//    IRawHtmlString One(ICanBeItem parent, NoParamOrder noParamOrder = default,
//        ICanBeEntity? item = null, object? data = null, string? field = null, Guid? newGuid = null);

//    [PrivateApi]
//    [Obsolete("Old interface. Pls use ToSic.Sxc.Services.IRenderService instead.")]
//    IRawHtmlString All(ICanBeItem parent, NoParamOrder noParamOrder = default,
//        string? field = null, string? apps = null, int max = 100, string? merge = null);
//}
//#endif