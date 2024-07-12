using System.Xml.Linq;
using ToSic.Eav.Apps.Internal.Work;
using ToSic.Eav.ImportExport;
using ToSic.Eav.ImportExport.Internal;
using ToSic.Eav.ImportExport.Internal.Xml;
using ToSic.Eav.Repositories;
using ToSic.Sxc.Apps.Internal.Work;

namespace ToSic.Sxc.Apps.Internal.ImportExport;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public partial class XmlImportFull: XmlImportWithFiles
{
    private readonly GenWorkBasic<WorkViewsMod> _workViewsMod;
    private readonly IRepositoryLoader _repositoryLoader;

    public XmlImportFull(
        MyServices services,
        GenWorkBasic<WorkViewsMod> workViewsMod,
        IRepositoryLoader repositoryLoader
    ) : base(services, "Sxc.XmlImp")
    {
        ConnectLogs([
            _workViewsMod = workViewsMod,
            _repositoryLoader = repositoryLoader
        ]);
    }

    // ReSharper disable once UnusedMember.Global
    // The system says it's never used, but it's provided through DI as the base class
    public new bool ImportXml(int zoneId, int appId, XDocument doc, bool leaveExistingValuesUntouched = true)
    {
        var l = Log.Fn<bool>($"{zoneId}, {appId}, ..., {leaveExistingValuesUntouched}");
        var ok = base.ImportXml(zoneId, appId, parentAppId: null /* not sure if we never have a parent here */, doc, leaveExistingValuesUntouched);
        if (!ok)
            return l.ReturnFalse("error");

        l.A("Now import templates - if found");

        var xmlSource = doc.Element(XmlConstants.RootNode)
                        ?? throw new("error import - xmlSource should always exist");

        if (xmlSource.Elements(XmlConstants.Templates).Any())
        {
            l.A("found some templates");
            ImportXmlTemplates(xmlSource);
        }
        else
            l.A("No templates found");

        return l.ReturnTrue("ok");
    }

}