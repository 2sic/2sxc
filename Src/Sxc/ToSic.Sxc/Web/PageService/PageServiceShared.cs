using System;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using static ToSic.Eav.Configuration.ConfigurationStack;
using IFeaturesService = ToSic.Sxc.Services.IFeaturesService;

namespace ToSic.Sxc.Web.PageService
{
    /// <summary>
    /// This controller should collect what all the <see cref="ToSic.Sxc.Services.IPageService"/> objects do, for use on the final page
    /// It must be scoped, so that it's the same object across the entire page-lifecycle.
    /// </summary>
    public partial class PageServiceShared: IChangeQueue
    {

        public PageServiceShared(IPageFeatures features, IFeaturesService featuresService)
        {
            _featuresService = featuresService;
            Features = features;
        }

        private readonly IFeaturesService _featuresService;
        public IPageFeatures Features { get; }

        /// <summary>
        /// This must be called from any service which uses this with the dynamic data, so it can get settings / url parameters from the current page
        /// </summary>
        /// <param name="pageParameters"></param>
        /// <param name="pageSettings"></param>
        public void InitPageStuff(IParameters pageParameters, DynamicStack pageSettings)
        {
            _pageParameters = _pageParameters ?? pageParameters;
            _pageSettings = _pageSettings ?? pageSettings?.GetStack(PartSiteSystem, PartGlobalSystem, PartPresetSystem) as DynamicStack;
        }
        private IParameters _pageParameters;
        private DynamicStack _pageSettings;

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
