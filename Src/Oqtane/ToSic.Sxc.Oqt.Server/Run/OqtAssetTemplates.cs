using ToSic.Eav.Documentation;
using ToSic.Sxc.Apps.Assets;

namespace ToSic.Sxc.Oqt.Server.Run
{
    [PrivateApi]
    public class OwtAssetTemplates : AssetTemplates
    {
        internal override string CustomsSearchCsCode => "Custom search code not implemented in Oqtane, as Oqtane doesn't have a search index.";
    }
}
