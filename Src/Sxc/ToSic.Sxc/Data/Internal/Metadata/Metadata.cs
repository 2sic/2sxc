using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Data.Internal.Convert;
using ToSic.Sxc.Data.Internal.Dynamic;

namespace ToSic.Sxc.Data.Internal.Metadata;


[PrivateApi("Hide implementation")]
internal partial class Metadata: DynamicEntity, IMetadata, IHasPropLookup, IHasJsonSource
{
    internal Metadata(IMetadataOf metadata, Internal.CodeDataFactory cdf)
        : base(metadata, null, "Metadata(virtual-field)", Eav.Constants.TransientAppId, propsRequired: false, cdf)
    {
        _metadata = metadata;
    }
    IPropertyLookup IHasPropLookup.PropertyLookup => _propLookup ??= new(this, () => Debug);
    private PropLookupMetadata _propLookup;

    [PrivateApi]
    private CodeItemHelper ItemHelper => _itemHelper ??= new(GetHelper, this);
    private CodeItemHelper _itemHelper;

    [PrivateApi("Hide this")]
    private readonly IMetadataOf _metadata;


    IMetadataOf IHasMetadata.Metadata => _metadata;



    public override bool HasType(string type) => _metadata.HasType(type);

    public override IEnumerable<IEntity> OfType(string type) => _metadata.OfType(type);

    #region Properties from the interfaces which are not really supported

    public override bool IsDemoItem => false;

    public new ITypedItem Presentation => throw new NotSupportedException();

    #endregion

    #region Other interfaces: JsonSource, IEquatable<ITypedItem>

    object IHasJsonSource.JsonSource() => throw new NotImplementedException();

    bool IEquatable<ITypedItem>.Equals(ITypedItem other) => Equals(other);

    #endregion

}