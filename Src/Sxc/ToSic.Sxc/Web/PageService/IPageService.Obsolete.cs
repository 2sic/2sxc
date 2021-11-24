using System;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Web
{
    /// <summary>
    /// Old name for the IPageService, it's in use in some v12 App templates so we must keep it working.
    /// Will continue to work, but shouldn't be used. Please use <see cref="ToSic.Sxc.Services.IPageService"/>  instead
    /// </summary>
    [Obsolete("Use ToSic.Sxc.Services.IPageService instead")]
    public interface IPageService: ToSic.Sxc.Services.IPageService
    {
    }
}
