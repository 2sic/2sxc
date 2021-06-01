using System.Collections.Generic;
using ToSic.Eav.Documentation;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Web
{
    /// <summary>
    /// Part of the <see cref="IPageService"/> to activate features on the page.
    /// </summary>
    [PrivateApi("not public ATM")]
    public interface IPageFeatures
    {
        void Activate(params string[] keys);
        
        List<string> ActiveKeys { get; }

        List<string> GetKeysAndFlush();
    }
}
