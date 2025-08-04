using ToSic.Sxc.Data.Sys.Factory;

namespace ToSic.Sxc.Data.Sys.Fields;

/// <summary>
/// Older dynamic code used the Field.Metadata.Description etc. which we don't support in the new Typed system.
/// </summary>
/// <param name="parent"></param>
/// <param name="name"></param>
/// <param name="cdf"></param>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
// Note: must be public, otherwise get RuntimeBinderException: 'ToSic.Sxc.Data.Sys.Fields.Field' does not contain a definition for 'Metadata'
// since it would only inspect the public classes!
public class FieldForDynamic(ITypedItem parent, string name, ICodeDataFactory cdf) : Field(parent, name, cdf)
{
    private readonly ICodeDataFactory _cdf = cdf;

    /// <summary>
    /// 2025-06 2dm: It appears we reactivated it, because various older apps had code like this:
    /// `var altText = Text.First(post.Field("Image").Metadata.Description, post.Title);`
    /// Apps include Blog v5.3.1, News 5.2.3, ImageHotspot 3.2.2
    /// </summary>
    /// <remarks>
    /// test system 2dm https://2sxc-dnn961.dnndev.me/x1504a - old content app uses this
    /// </remarks>
    /// <remarks>
    /// Return is an object, because it should not work with typed code.
    /// Only dynamic code will actually inspect it and successfully use it.
    /// </remarks>
    public object Metadata => _dynMeta.Get(() => _cdf.MetadataDynamic(MetadataOfValue!))!;
    private readonly GetOnce<object> _dynMeta = new();
}
