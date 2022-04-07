using System.Collections.Generic;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Web.PageFeatures
{
    public interface IPageFeaturesManager
    {
        IReadOnlyDictionary<string, IPageFeature> Features { get; }
        
        //void Register(params IPageFeature[] features);

        //List<IPageFeature> GetWithDependents(IPageService pageService, ILog log);

        List<IPageFeature> GetWithDependents(List<string> keys);
    }
}
