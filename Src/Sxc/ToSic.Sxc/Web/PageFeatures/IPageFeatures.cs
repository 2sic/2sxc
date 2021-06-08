using System.Collections.Generic;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Sxc.Web.PageFeatures;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Web
{
    /// <summary>
    /// Part of the <see cref="IPageService"/> to activate features on the page.
    /// </summary>
    [PrivateApi("not public ATM")]
    public interface IPageFeatures
    {
        IPageFeatures Init(IPageService pageService);
        
        void Activate(params string[] keys);
        
        List<string> ActiveKeys { get; }

        List<string> GetKeysAndFlush();

        List<IPageFeature> GetWithDependentsAndFlush(ILog log);

    }
}
