using ToSic.Eav.Internal.Features;
using ToSic.Lib.Coding;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Adam.Work.Internal;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Services.Internal;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sxc.WebApi;
using ToSic.Sys.Capabilities.Features;
using static ToSic.Sys.Capabilities.Features.BuiltInFeatures;

namespace ToSic.Sxc.Backend.Adam;

/// <summary>
/// Adam Shared Code Across the APIs
/// See docs of official interface <see cref="IDynamicWebApi"/>
/// </summary>
[PrivateApi("Used by DynamicApiController and Hybrid.Api12_DynCode")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AdamCode(Generator<AdamWorkUpload, AdamWorkOptions> adamUploadGenerator, LazySvc<ISysFeaturesService> featuresLazy)
    : ServiceWithContext("AdamCode", connect: [adamUploadGenerator, featuresLazy])
{
    public IFile SaveInAdam(NoParamOrder noParamOrder = default,
        Stream stream = null,
        string fileName = null,
        string contentType = null,
        Guid? guid = null,
        string field = null,
        string subFolder = "")
    {
        var l = Log.Fn<IFile>();

        if (stream == null || fileName == null || contentType == null || guid == null || field == null)
            throw new ArgumentException($"all these arguments must be available: {nameof(stream)}, {nameof(field)}, {nameof(contentType)}, {nameof(guid)}, {nameof(field)}");

        var feats = new[] { SaveInAdamApi.Guid, PublicUploadFiles.Guid };

        if (!featuresLazy.Value.IsEnabled(feats, "can't save in ADAM", out var exp))
            throw l.Ex(exp);

        var appId = ExCtx?.GetState<IBlock>()?.AppId
                    ?? ExCtx?.GetApp()?.AppId
                    ?? throw l.Ex(new Exception("Error, SaveInAdam needs an App-Context to work, but the App is not known."));
        var adamUploader = adamUploadGenerator.New(new()
            {
                AppId = appId,
                ContentType = contentType,
                ItemGuid = guid.Value,
                Field = field,
                UsePortalRoot = false,
            });

        var upload = adamUploader.UploadOne(stream, fileName, subFolder, true);
        return upload;
    }
}