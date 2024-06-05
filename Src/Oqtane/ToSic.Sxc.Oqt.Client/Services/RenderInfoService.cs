using ToSic.Sxc.Oqt.Shared.Interfaces;

namespace ToSic.Sxc.Oqt.Client.Services
{
    /// <inheritdoc />
    public class RenderInfoService : IRenderInfoService
    {
        /// <inheritdoc />
        public bool IsStaticSsr(string renderMode) => false;

        /// <inheritdoc />
        public bool IsBlazorEnhancedNav(string renderMode) => false;

        /// <inheritdoc />
        public bool IsSsrFraming(string renderMode) => false;
    }
}
