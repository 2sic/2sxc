using ToSic.Eav.Data.ContentTypes.Sys;
using ToSic.Eav.Data.Sys;
using ToSic.Eav.Data.Sys.ValueConverter;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.Metadata.Sys;
using ToSic.Sxc.Apps.Sys.Ui;

namespace ToSic.Sxc.Apps.Sys.Work;

public class WorkViewsContentTypes(GenWorkPlus<WorkViews> workViews, IConvertToEavLight dataToFormatLight, LazySvc<IValueConverter> valConverterLazy, IAppWorkCtxForDiWip appWorkCtx)
    : ServiceBase("Viw.Cts", connect: [workViews, dataToFormatLight, valConverterLazy, appWorkCtx])
{
    // todo: check if this call could be replaced with the normal ContentTypeController.Get to prevent redundant code
    public IList<ContentTypeUiInfo> GetContentTypesWithStatus(string appPath, string appPathShared)
    {
        var templates = workViews.New(appWorkCtx).GetAll().ToList();
        var visible = templates.Where(t => !t.IsHidden).ToList();

        var valConverter = valConverterLazy.Value;

        var result = appWorkCtx.AppReader.ContentTypes
            .OfScope(ScopeConstants.Default)
            .Where(ct => templates.Any(t => t.ContentType == ct.NameId)) // must exist in at least 1 template
            .OrderBy(ct => ct.Name)
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