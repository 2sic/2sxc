using ToSic.Sxc.Dnn;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Dnn.Run;

// ReSharper disable once CheckNamespace
namespace Custom.Dnn;

/// <summary>
/// The base class for Razor-Components in 2sxc 12+ <br/>
/// Provides context infos like the Dnn object, helpers like Edit and much more. <br/>
/// </summary>
[PublicApi]
public abstract class Razor12 : Hybrid.Razor12, IHasDnn, IRazor12, IDnnRazorCompatibility
{
    /// <inheritdoc />
    public IDnnContext Dnn => (_CodeApiSvc as IHasDnn)?.Dnn;

}