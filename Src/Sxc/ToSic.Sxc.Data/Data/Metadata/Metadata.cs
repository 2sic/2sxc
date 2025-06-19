using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Data.Sys;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Data.Internal.Convert;
using ToSic.Sxc.Data.Internal.Dynamic;

namespace ToSic.Sxc.Data.Internal.Metadata;


[PrivateApi("Hide implementation")]
internal partial class Metadata(IMetadataOf metadata, ICodeDataFactory cdf)
    : DynamicEntity(metadata, null, "Metadata(virtual-field)", KnownAppsConstants.TransientAppId, propsRequired: false, cdf),
        IMetadata, IHasPropLookup, IHasJsonSource
{
    IPropertyLookup IHasPropLookup.PropertyLookup => _propLookup ??= new(this, () => Debug);
    private PropLookupMetadata? _propLookup;

    [PrivateApi]
    [field: AllowNull, MaybeNull]
    private CodeItemHelper ItemHelper => field ??= new(GetHelper, this);

    [PrivateApi("Hide this")]
    private readonly IMetadataOf _metadata = metadata;


    IMetadataOf IHasMetadata.Metadata => _metadata;



    public override bool HasType(string type) => _metadata.HasType(type);

    public override IEnumerable<IEntity> OfType(string type) => _metadata.OfType(type);

    #region Properties from the interfaces which are not really supported

    public override bool IsDemoItem => false;

    public new ITypedItem Presentation => throw new NotSupportedException();

    #endregion

    #region Other interfaces: JsonSource, IEquatable<ITypedItem>

    object IHasJsonSource.JsonSource() => throw new NotImplementedException();

    bool IEquatable<ITypedItem>.Equals(ITypedItem? other) => Equals(other);

    #endregion

}