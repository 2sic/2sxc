using System.IO;
using ToSic.Eav.Internal.Features;
using ToSic.Lib.Coding;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Services.Internal;
using ToSic.Sxc.WebApi;
using static ToSic.Eav.Internal.Features.BuiltInFeatures;

namespace ToSic.Sxc.Backend.Adam;

/// <summary>
/// Adam Shared Code Across the APIs
/// See docs of official interface <see cref="IDynamicWebApi"/>
/// </summary>
[PrivateApi("Used by DynamicApiController and Hybrid.Api12_DynCode")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AdamCode(
    Generator<AdamTransUpload<int, int>> adamUploadGenerator,
    LazySvc<IEavFeaturesService> featuresLazy)
    : ServiceForDynamicCode("AdamCode", connect: [adamUploadGenerator, featuresLazy])
{
    public IFile SaveInAdam(NoParamOrder noParamOrder = default,
        Stream stream = null,
        string fileName = null,
        string contentType = null,
        Guid? guid = null,
        string field = null,
        string subFolder = "")
    {
        if (stream == null || fileName == null || contentType == null || guid == null || field == null)
            throw new();

        var feats = new[] { SaveInAdamApi.Guid, PublicUploadFiles.Guid };

        if (!featuresLazy.Value.IsEnabled(feats, "can't save in ADAM", out var exp))
            throw exp;

        var appId = ((ICodeApiServiceInternal)_CodeApiSvc)?._Block?.AppId ?? _CodeApiSvc?.App?.AppId ?? throw new("Error, SaveInAdam needs an App-Context to work, but the App is not known.");
        return adamUploadGenerator.New()
            .Init(appId, contentType, guid.Value, field, false)
            .UploadOne(stream, fileName, subFolder, true);
    }
}