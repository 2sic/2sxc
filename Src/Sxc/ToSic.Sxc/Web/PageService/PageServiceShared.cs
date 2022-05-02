using System;
using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Code;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Web.ContentSecurityPolicy;
using IFeaturesService = ToSic.Sxc.Services.IFeaturesService;

namespace ToSic.Sxc.Web.PageService
{
    /// <summary>
    /// This controller should collect what all the <see cref="ToSic.Sxc.Services.IPageService"/> objects do, for use on the final page
    /// It must be scoped, so that it's the same object across the entire page-lifecycle.
    /// </summary>
    public partial class PageServiceShared: IChangeQueue // , INeedsDynamicCodeRoot
    {

        public PageServiceShared(IPageFeatures pageFeatures, IFeaturesService featuresService, ModuleLevelCsp csp)
        {
            FeaturesService = featuresService;
            PageFeatures = pageFeatures;
            Csp = csp;
        }

        internal readonly IFeaturesService FeaturesService;
        public IPageFeatures PageFeatures { get; }
        public ModuleLevelCsp Csp { get; }

        ///// <summary>
        ///// Connect to code root, so page-parameters and settings will be available later on.
        ///// Important: page-parameters etc. are not available at this time, so don't try to get them until needed
        ///// </summary>
        ///// <param name="codeRoot"></param>
        //public void ConnectToRoot(IDynamicCodeRoot codeRoot) => _codeRoot = codeRoot;
        //private IDynamicCodeRoot _codeRoot;

        //internal IParameters PageParameters => _pageParameters.Get(() => _codeRoot?.CmsContext?.Page?.Parameters);
        //private readonly ValueGetOnce<IParameters> _pageParameters = new ValueGetOnce<IParameters>();
        //internal DynamicStack PageSettings => _pageSettings.Get(() => _codeRoot?.Settings as DynamicStack);
        //private readonly ValueGetOnce<DynamicStack> _pageSettings = new ValueGetOnce<DynamicStack>();

        /// <summary>
        /// How the changes given to this object should be processed.
        /// </summary>
        [PrivateApi("not final yet, will probably change")]
        public PageChangeModes ChangeMode { get; set; } = PageChangeModes.Auto;

        [PrivateApi("not final yet")]
        protected PageChangeModes GetMode(PageChangeModes modeForAuto)
        {
            switch (ChangeMode)
            {
                case PageChangeModes.Default:
                case PageChangeModes.Auto:
                    return modeForAuto;
                case PageChangeModes.Replace:
                case PageChangeModes.Append:
                case PageChangeModes.Prepend:
                    return ChangeMode;
                default:
                    throw new ArgumentOutOfRangeException(nameof(ChangeMode), ChangeMode, null);
            }
        }

    }
}
