using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Internal;

// ReSharper disable once CheckNamespace
namespace Custom.Dnn;

/// <summary>
/// This is the base class for custom code (.cs) files in your Apps.
/// By inheriting from this base class, you will automatically have the context like the App object etc. available. 
/// </summary>
[PublicApi]
[ShowApiWhenReleased(ShowApiMode.Never)]   // #DocsButNotForIntellisense
public abstract class Code12 : DynamicCode12, IHasDnn
{
    /// <inheritdoc />
    public IDnnContext Dnn => (ExCtxOrNull as IHasDnn)?.Dnn;

    [PrivateApi] public override int CompatibilityLevel => CompatibilityLevels.CompatibilityLevel12;

}