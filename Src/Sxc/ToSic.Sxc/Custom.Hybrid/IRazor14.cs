using ToSic.Lib.Documentation;
using ToSic.Sxc.Code;
using ToSic.Sxc.Services;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

[PrivateApi("not sure yet if this will stay in Hybrid or go to Web.Razor or something, so keep it private for now")]
public interface IRazor14<out TModel, out TServiceKit>: IRazor, IDynamicCode14<TModel, TServiceKit>
    where TModel : class
    where TServiceKit : ServiceKit
{
    /// <summary>
    /// Dynamic object containing parameters. So in Dnn it contains the PageData, in Oqtane it contains the Model
    /// </summary>
    /// <remarks>
    /// New in v12
    /// </remarks>
    [PrivateApi]
    dynamic DynamicModel { get; }

    ///// <summary>
    ///// The path to this Razor WebControl.
    ///// This is for consistency, because asp.net Framework has a property "VirtualPath" whereas .net core uses "Path"
    ///// From now on it should always be Path for cross-platform code
    ///// </summary>
    //[PublicApi("This is a polyfill to ensure the old Razor has the same property as .net Core Razor")]
    //string Path { get; }

}