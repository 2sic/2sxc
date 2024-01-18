using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Dnn.Run;

namespace ToSic.Sxc.Dnn;

// ReSharper disable once UnusedMember.Global
/// <summary>
/// This is a base class for custom code files with context. <br/>
/// If you create a class file for dynamic use and inherit from this, then the compiler will automatically add objects like Link, Dnn, etc.
/// The class then also has AsDynamic(...) and AsList(...) commands like a normal razor page.
/// </summary>
[PublicApi]
public abstract class DynamicCode : Sxc.Code.DynamicCode, Sxc.Code.IDynamicCode, IDnnDynamicCode
{
    /// <inheritdoc />
    public IDnnContext Dnn => (_CodeApiSvc as IHasDnn)?.Dnn;
}