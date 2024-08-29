using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Serialization;
using ToSic.Sxc.Apps.Internal.Work;
using ToSic.Sxc.Backend.ImportExport;
using ToSic.Sxc.Web.Internal.LightSpeed;

namespace ToSic.Sxc.Backend.Views;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ViewsBackend(
    GenWorkBasic<WorkViewsMod> workViewsMod,
    GenWorkPlus<WorkViews> workViews,
    IContextOfSite context,
    LazySvc<IConvertToEavLight> convertToEavLight,
    Generator<ImpExpHelpers> impExpHelpers)
    : ServiceBase("Bck.Views", connect: [workViewsMod, convertToEavLight, impExpHelpers, workViews, context])
{
    public IEnumerable<ViewDetailsDto> GetAll(int appId)
    {
        var l = Log.Fn<IEnumerable<ViewDetailsDto>>($"get all a#{appId}");

        var appViews = workViews.New(appId);
        var contentTypes = appViews.AppWorkCtx.AppReader.ContentTypes.OfScope(Scopes.Default).ToList();

        var viewList = appViews.GetAll().ToList();
        Log.A($"attribute list count:{contentTypes.Count}, template count:{viewList.Count}");
        var ser = convertToEavLight.Value as ConvertToEavLight;

        var views = viewList
            .Select(view =>
            {
                var lightspeed = view.Metadata
                    .FirstOrDefaultOfType(LightSpeedDecorator.TypeNameId)
                    .NullOrGetWith(lsEntity => new LightSpeedDecorator(lsEntity)
                        .Map(lightSpeedDeco => new AppMetadataDto
                        {
                            Id = lightSpeedDeco.Id,
                            Title = lightSpeedDeco.Title,
                            IsEnabled = /*lightSpeedDeco.IsEnabled*/lightSpeedDeco.IsEnabledNullable != false,
                        })
                    );

                return new ViewDetailsDto
                {
                    Id = view.Id, Name = view.Name,
                    ContentType = TypeSpecs(contentTypes, view.ContentType, view.ContentItem),
                    PresentationType = TypeSpecs(contentTypes, view.PresentationType, view.PresentationItem),
                    ListContentType = TypeSpecs(contentTypes, view.HeaderType, view.HeaderItem),
                    ListPresentationType = TypeSpecs(contentTypes, view.HeaderPresentationType, view.HeaderPresentationItem),
                    TemplatePath = view.Path,
                    IsHidden = view.IsHidden,
                    ViewNameInUrl = view.UrlIdentifier,
                    Guid = view.Guid,
                    List = view.UseForList,
                    HasQuery = view.QueryRaw != null,
                    Used = view.Entity.Parents().Count,
                    IsShared = view.IsShared,
                    EditInfo = new(view.Entity),
                    Metadata = ser?.CreateListOfSubEntities(view.Metadata, SubEntitySerialization.NeverSerializeChildren()),
                    Permissions = new() { Count = view.Entity.Metadata.Permissions.Count() },
                    Lightspeed = lightspeed,
                };
            })
            .ToList();
        return l.Return(views, $"{views.Count}");
    }


    /// <summary>
    /// Helper to prepare a quick-info about 1 content type
    /// </summary>
    /// <param name="allCTs"></param>
    /// <param name="staticName"></param>
    /// <param name="maybeEntity"></param>
    /// <returns></returns>
    private static ViewContentTypeDto TypeSpecs(IEnumerable<IContentType> allCTs, string staticName, IEntity maybeEntity)
    {
        var found = allCTs.FirstOrDefault(ct => ct.NameId == staticName);
        return new()
        {
            StaticName = staticName, Id = found?.Id ?? 0, Name = found == null ? "no content type" : found.Name,
            DemoId = maybeEntity?.EntityId ?? 0,
            DemoTitle = maybeEntity?.GetBestTitle() ?? ""
        };
    }

    public bool Delete(int appId, int id)
    {
        // todo: extra security to only allow zone change if host user
        Log.A($"delete a{appId}, t:{id}");
        var app = impExpHelpers.New().GetAppAndCheckZoneSwitchPermissions(context.Site.ZoneId, appId, context.User, context.Site.ZoneId);
        workViewsMod.New(app).DeleteView(id);
        return true;
    }
}