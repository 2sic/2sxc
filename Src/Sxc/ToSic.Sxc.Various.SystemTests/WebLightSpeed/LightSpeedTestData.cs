using ToSic.Eav.Data;
using ToSic.Eav.Data.Build;
using ToSic.Sxc.Web.Internal.LightSpeed;

namespace ToSic.Sxc.WebLightSpeed;

[Startup(typeof(StartupSxcCoreOnly))]
public class LightSpeedTestData(DataBuilder builder) //: TestBaseEavCore
{
    public const int AppId = -1;
    internal const string DefTitle = "LightSpeed Configuration";

    /// <summary>
    /// Basic LightSpeed Content Type with Url Fields only for testing
    /// </summary>
    private IContentType LsCtUrlFields => builder.ContentType.CreateContentTypeTac(appId: AppId, name: LightSpeedDecorator.NiceName, attributes:
        [
            builder.ContentTypeAttributeTac(AppId, nameof(LightSpeedDecorator.Title), DataTypes.Boolean, true),
            builder.ContentTypeAttributeTac(AppId, nameof(LightSpeedDecorator.IsEnabled), DataTypes.Boolean),
            builder.ContentTypeAttributeTac(AppId, nameof(LightSpeedDecorator.ByUrlParameters), DataTypes.Boolean),
            builder.ContentTypeAttributeTac(AppId, nameof(LightSpeedDecorator.UrlParametersCaseSensitive), DataTypes.Boolean),
            builder.ContentTypeAttributeTac(AppId, nameof(LightSpeedDecorator.UrlParameterNames), DataTypes.String),
            builder.ContentTypeAttributeTac(AppId, nameof(LightSpeedDecorator.UrlParametersOthersDisableCache), DataTypes.Boolean),
        ]
    );

    public IEntity LightSpeedTestEntity(bool? isEnabled = default, bool? byUrlParameters = default, bool? caseSensitive = default, string names = default, bool? othersDisableCache = default)
    {
        var valDaniel = new Dictionary<string, object>
        {
            {nameof(LightSpeedDecorator.Title), DefTitle},
            {nameof(LightSpeedDecorator.IsEnabled), isEnabled},
            {nameof(LightSpeedDecorator.ByUrlParameters), byUrlParameters},
            {nameof(LightSpeedDecorator.UrlParametersCaseSensitive), caseSensitive},
            {nameof(LightSpeedDecorator.UrlParameterNames), names},
            {nameof(LightSpeedDecorator.UrlParametersOthersDisableCache), othersDisableCache }
        };
        var ent = builder.CreateEntityTac(appId: AppId, entityId: 1, contentType: LsCtUrlFields, values: valDaniel, titleField: nameof(LightSpeedDecorator.Title));
        return ent;
    }

    internal LightSpeedDecorator Decorator(bool? isEnabled = default, bool? byUrlParameters = null, bool? caseSensitive = null, string names = default, bool? othersDisableCache = default)
        => new(LightSpeedTestEntity(isEnabled: isEnabled, byUrlParameters: byUrlParameters, caseSensitive: caseSensitive, names: names, othersDisableCache: othersDisableCache));
}