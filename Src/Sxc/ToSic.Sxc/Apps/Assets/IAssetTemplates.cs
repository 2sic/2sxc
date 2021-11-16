using System.Collections.Generic;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Apps.Assets
{
    [PrivateApi]
    public interface IAssetTemplates: IHasLog<IAssetTemplates>
    {
        string GetTemplate(string key);

        List<TemplateInfo> GetTemplates();

        TemplateInfo GetTemplateInfo(string key);
    }
}
