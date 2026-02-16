using ToSic.Eav.Data.Raw.Sys;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;

namespace ToSic.Sxc.Edit.Toolbar.Sys;

[PrivateApi]
[VisualQuery(
    NiceName = "ToolbarButtonActions",
    NameId = "529752a2-11ea-473b-a81f-5634f935e57f",
    NameIds = ["System.ToolbarButtonActions"], // Internal name for the system, used in some entity-pickers. Can change at any time.
    Type = DataSourceType.System,
    Audience = Audience.System,
    DataConfidentiality = DataConfidentiality.Public,
    UiHint = "Buttons")]
// ReSharper disable once UnusedMember.Global
public class ToolbarButtonActions : CustomDataSource
{
    public ToolbarButtonActions(Dependencies services)
        : base(services, logName: "CDS.TlbAct")
    {
        ProvideOutRaw(
            Generators,
            options: () => new()
            {
                AutoId = false,
                TitleField = "Name",
                TypeName = nameof(ToolbarButtonActions),
            });

    }

    private IEnumerable<IRawEntity> Generators()
    {
        var l = Log.Fn<IEnumerable<IRawEntity>>();
        var list = new[]
            {
                "app",
                "app-import",
                "app-settings",
                "app-resources",
                "apps",
                "system",
                "insights",

                "add",
                "add-existing",
                "list",
                "movedown",
                "moveup",
                "remove",
                "replace",

                "delete",
                "edit",
                "new",
                "copy",
                "data",
                "publish",
                "metadata",

                "layout",
                "code",
                "fields",
                "template",
                "query",
                "view",
                "edition",


                "more",

                "info",
            }
            .OrderBy(a => a)
            .Select(action => new RawEntity(new()
            {
                { "Name", action },
            }))
            .ToList();

        return l.Return(list, $"{list.Count}");
    }


}