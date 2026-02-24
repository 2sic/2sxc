using ToSic.Eav.Data;
using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Sys;
using ToSic.Eav.Models;
using ToSic.Sxc.Web.Sys.LightSpeed;

namespace ToSic.Sxc.WebLightSpeed;

public class LightSpeedTestData(DataAssembler dataAssembler, ContentTypeAssembler typeAssembler)
{
    public const int AppId = -1;
    internal const string DefTitle = "LightSpeed Configuration";

    internal LightSpeedDecorator Decorator(bool? isEnabled = default, bool? byUrlParameters = null, bool? caseSensitive = null, string? names = default, bool? othersDisableCache = default)
        => LightSpeedTestEntity(isEnabled: isEnabled, byUrlParameters: byUrlParameters, caseSensitive: caseSensitive, names: names, othersDisableCache: othersDisableCache)
            .ToModel<LightSpeedDecorator>(skipTypeCheck: true)!;

    /// <summary>
    /// Basic LightSpeed Content Type with Url Fields only for testing
    /// </summary>
    private IContentType LsCtUrlFields => typeAssembler.Type.CreateContentTypeTac(appId: AppId, name: LightSpeedDecorator.ContentTypeName, attributes:
        [
            typeAssembler.ContentTypeAttributeTac(AppId, nameof(LightSpeedDecorator.Title), DataTypes.Boolean, true),
            typeAssembler.ContentTypeAttributeTac(AppId, nameof(LightSpeedDecorator.IsEnabled), DataTypes.Boolean),
            typeAssembler.ContentTypeAttributeTac(AppId, nameof(LightSpeedDecorator.ByUrlParameters), DataTypes.Boolean),
            typeAssembler.ContentTypeAttributeTac(AppId, nameof(LightSpeedDecorator.UrlParametersCaseSensitive), DataTypes.Boolean),
            typeAssembler.ContentTypeAttributeTac(AppId, nameof(LightSpeedDecorator.UrlParameterNames), DataTypes.String),
            typeAssembler.ContentTypeAttributeTac(AppId, nameof(LightSpeedDecorator.UrlParametersOthersDisableCache), DataTypes.Boolean),
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