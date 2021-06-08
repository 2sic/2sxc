using System;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Web.PageService
{
    public partial class Page: IPageService, IChangeQueue
    {
        public Page(IPageFeatures features)
        {
            Features = features.Init(this);
        }

        /// <summary>
        /// How the changes given to this object should be processed.
        /// </summary>
        [WorkInProgressApi("not final yet")]
        public PageChangeModes ChangeMode { get; set; } = PageChangeModes.Auto;

        [PrivateApi]
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
