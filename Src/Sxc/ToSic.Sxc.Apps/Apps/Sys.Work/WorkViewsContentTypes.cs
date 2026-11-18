using ToSic.Eav.Apps.Inspect.Sys.UiDto;
using ToSic.Eav.Data.ContentTypes.Sys;
using ToSic.Eav.Data.Sys;
using ToSic.Eav.Data.Sys.ValueConverter;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.Metadata.Sys;

namespace ToSic.Sxc.Apps.Sys.Work;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class WorkViewsContentTypes(AppDataChain<WorkViewsGet> workViews, IConvertToEavLight dataToFormatLight, LazySvc<IValueConverter> valConverterLazy)
    : ServiceWithSetup<IAppDataContext>("Viw.Cts", connect: [workViews, dataToFormatLight, valConverterLazy])
{
    public IList<ContentTypeUiInfo> GetContentTypesWithStatus(string appPath, string appPathShared)
    {
        var templates = workViews.New(MyOptions).AllViews.ToList();
        var visible = templates.Where(t => !t.IsHidden).ToList();

        var valConverter = valConverterLazy.Value;

        var result = MyOptions.AppReader.ContentTypes
            .OfScope(ScopeConstants.Default)
            .Where(ct => templates.Any(t => t.ContentType == ct.NameId)) // must exist in at least 1 template
            .OrderBy(ct => ct.Name, StringComparer.OrdinalIgnoreCase)
            .Select(ct =>
            {
                var details = ct.DetailsOrNull();
                var thumbnail = valConverter.ToValue(details?.Icon);
                if (AppIconHelpers.HasAppPathToken(thumbnail))
                    thumbnail = AppIconHelpers.AppPathTokenReplace(thumbnail, appPath, appPathShared);
                return new ContentTypeUiInfo
                {
                    StaticName = ct.NameId,
                    Name = ct.Name,
                    IsHidden = visible.All(t => t.ContentType != ct.NameId),   // must check if *any* template is visible, otherwise tell the UI that it's hidden
                    Thumbnail = thumbnail,
                    Properties = ((details as ICanBeEntity)?.Entity).NullOrGetWith(dataToFormatLight.Convert),
                    IsDefault = ct.Metadata.HasType(KnownDecorators.IsDefaultDecorator),
                };
            })
            .ToList();
        return result;
    }
}