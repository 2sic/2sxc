using ToSic.Eav.Data;
using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Sys;
using ToSic.Eav.Models;
using ToSic.Sxc.Web.Sys.LightSpeed;

namespace ToSic.Sxc.WebLightSpeed;

public class LightSpeedTestData(DataAssembler dataAssembler, ContentTypeAssemblyKit ctAssemblyKit)
{
    public const int AppId = -1;
    internal const string DefTitle = "LightSpeed Configuration";

    internal LightSpeedDecorator Decorator(bool? isEnabled = default, bool? byUrlParameters = null, bool? caseSensitive = null, string? names = default, bool? othersDisableCache = default)
        => LightSpeedTestEntity(isEnabled: isEnabled, byUrlParameters: byUrlParameters, caseSensitive: caseSensitive, names: names, othersDisableCache: othersDisableCache)
            .ToModel<LightSpeedDecorator>(options: new() { TypeName = ToModelOptions.TypeNameAny })!;

    /// <summary>
    /// Basic LightSpeed Content Type with Url Fields only for testing
    /// </summary>
    private IContentType LsCtUrlFields => ctAssemblyKit.Type.CreateContentTypeTac(appId: AppId, name: LightSpeedDecorator.ContentTypeName, attributes:
        [
            ctAssemblyKit.ContentTypeFieldTac(AppId, nameof(LightSpeedDecorator.Title), DataTypes.Boolean, true),
            ctAssemblyKit.ContentTypeFieldTac(AppId, nameof(LightSpeedDecorator.IsEnabled), DataTypes.Boolean),
            ctAssemblyKit.ContentTypeFieldTac(AppId, nameof(LightSpeedDecorator.ByUrlParameters), DataTypes.Boolean),
            ctAssemblyKit.ContentTypeFieldTac(AppId, nameof(LightSpeedDecorator.UrlParametersCaseSensitive), DataTypes.Boolean),
            ctAssemblyKit.ContentTypeFieldTac(AppId, nameof(LightSpeedDecorator.UrlParameterNames), DataTypes.String),
            ctAssemblyKit.ContentTypeFieldTac(AppId, nameof(LightSpeedDecorator.UrlParametersOthersDisableCache), DataTypes.Boolean),
        ]
    );

    private IEntity LightSpeedTestEntity(bool? isEnabled = default, bool? byUrlParameters = default, bool? caseSensitive = default, string? names = default, bool? othersDisableCache = default)
    {
        var values = new Dictionary<string, object>
        {
            {nameof(LightSpeedDecorator.Title), DefTitle},
            {nameof(LightSpeedDecorator.IsEnabled), isEnabled},
            {nameof(LightSpeedDecorator.ByUrlParameters), byUrlParameters},
            {nameof(LightSpeedDecorator.UrlParametersCaseSensitive), caseSensitive},
            {nameof(LightSpeedDecorator.UrlParameterNames), names},
            {nameof(LightSpeedDecorator.UrlParametersOthersDisableCache), othersDisableCache }
        };
        var ent = dataAssembler.CreateEntityTac(appId: AppId, entityId: 1, contentType: LsCtUrlFields, values: values, titleField: nameof(LightSpeedDecorator.Title));
        return ent;
    }

}