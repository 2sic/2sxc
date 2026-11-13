using ToSic.Eav.Apps.ImportExport.Sys;
using ToSic.Eav.Apps.Inspect.Sys.Models;
using ToSic.Eav.Data.ContentTypes.Sys;
using ToSic.Eav.Data.Sys;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.Models;
using ToSic.Eav.Serialization.Sys.Options;
using ToSic.Sxc.Web.Sys.LightSpeed;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Backend.Views;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class WorkViewsGetDetailed(
    AppWorkChain<WorkViewsGet> workViews,
    LazySvc<IConvertToEavLight> convertToEavLight,
    Generator<ImpExpHelpers> impExpHelpers)
    : ServiceWithSetup<IAppWorkContext>("Bck.Views", connect: [convertToEavLight, impExpHelpers, workViews])
{
    public IEnumerable<ViewDetailsDto> GetAll(int appId)
    {
        var l = Log.Fn<IEnumerable<ViewDetailsDto>>($"get all a#{appId}");

        var appViews = workViews.New(MyOptions);
        var contentTypes = MyOptions.AppReader.ContentTypes.OfScope(ScopeConstants.Default).ToList();

        var viewList = appViews.AllViews.ToList();
        l.A($"fieldDef list count:{contentTypes.Count}, template count:{viewList.Count}");

        var views = viewList
            .Select(view =>
            {
                var lightspeed = view
                    .GetMetadataModel<LightSpeedDecorator>()
                    .NullOrGetWith(ls => new AppMetadataDto
                        {
                            Id = ls.Id,
                            Title = ls.Title,
                            IsEnabled = ls.IsEnabledNullable != false,
                        });

                return new ViewDetailsDto
                {
                    Id = view.Id,
                    Name = view.Name,
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
                    Used = view.Entity.Parents().Count(),
                    IsShared = view.IsShared,
                    EditInfo = new(view.Entity),
                    Metadata = convertToEavLight.Value.SubConverter.CreateListOfSubEntities(view.Metadata, SubEntitySerialization.NeverSerializeChildren()),
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
    private static ViewContentTypeDto TypeSpecs(IEnumerable<IContentType> allCTs, string staticName, IEntity? maybeEntity)
    {
        var found = allCTs.FirstOrDefault(ct => ct.NameId == staticName);
        return new()
        {
            StaticName = staticName, Id = found?.Id ?? 0, Name = found == null ? "no content type" : found.Name,
            DemoId = maybeEntity?.EntityId ?? 0,
            DemoTitle = maybeEntity?.GetBestTitle() ?? ""
        };
    }
}