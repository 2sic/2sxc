using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Web.PageFeatures
{
    [PrivateApi("Hide implementation")]
    public class PageFeatures: IPageFeatures
    {
        private readonly IPageFeaturesManager _pfm;
        //public IPageService Parent { get; private set; }
        
        public PageFeatures(IPageFeaturesManager pfm)
        {
            _pfm = pfm;
        }
        
        //public IPageFeatures Init(IPageService parent)
        //{
        //    Parent = parent;
        //    return this;
        //}
       
        /// <inheritdoc />
        public void Activate(params string[] keys)
        {
            var realKeys = keys.Where(k => !string.IsNullOrWhiteSpace(k));
            ActiveKeys.AddRange(realKeys);
            
        }

        public List<string> ActiveKeys { get; } = new List<string>();
        
        
        public List<string> GetKeysAndFlush()
        {
            var keys = ActiveKeys.ToArray().ToList();
            ActiveKeys.Clear();
            return keys;
        }

        public List<IPageFeature> GetWithDependentsAndFlush(ILog log)
        {
            // if (_features != null) return _features;
            var wrapLog = log.Call<List<IPageFeature>>();
            log.Add("Try to get new specs from IPageService");
            var features = GetKeysAndFlush();
            log.Add($"Got {features.Count} items");
            var unfolded = _pfm.GetWithDependents(features);
            log.Add($"Got unfolded features {unfolded.Count}");
            // _features = unfolded;
            return wrapLog("ok", unfolded);
        }

    }
}
