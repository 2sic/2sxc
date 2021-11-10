using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Apps.Assets
{
    [PrivateApi]
    public interface IAssetTemplates: IHasLog<IAssetTemplates>
    {
        string GetTemplate(Type type);
    }
}
