using System;

namespace ToSic.Sxc.Blocks
{
    /// <summary>
    /// Old name for the IRenderService, it's in use in some v12 App templates so we must keep it working.
    /// Will continue to work, but shouldn't be used. Please use <see cref="ToSic.Sxc.Services.IRenderService"/>  instead
    /// </summary>
    [Obsolete("Old name, used in 2-3 v12 apps released. Pls use ToSic.Sxc.Services.IRenderService instead.")]
    public interface IRenderService: ToSic.Sxc.Services.IRenderService
    {
    }
}
