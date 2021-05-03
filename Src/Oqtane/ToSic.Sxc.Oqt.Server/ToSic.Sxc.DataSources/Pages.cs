using System.Collections.Generic;
using ToSic.Eav.DataSources.Queries;
using ToSic.Eav.Documentation;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// Deliver a list of pages from the current platform (Dnn or Oqtane)
    /// </summary>
    [PublicApi]
    [VisualQuery(
        ExpectsDataOfType = VqExpectsDataOfType,
        GlobalName = VqGlobalName,
        HelpLink = VqHelpLink,
        Icon = VqIcon,
        NiceName = VqNiceName,
        Type = VqType,
        UiHint = VqUiHint)]
    public class Pages : CmsBases.PagesBase
    {
        protected override List<TempPageInfo> GetPagesInternal()
        {
            throw new System.NotImplementedException();
        }
    }
}
