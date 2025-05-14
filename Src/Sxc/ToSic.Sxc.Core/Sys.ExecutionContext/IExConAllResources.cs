using ToSic.Sxc.Data;

namespace ToSic.Sxc.Sys.ExecutionContext;

public interface IExConAllResources
{
    ITypedStack AllResources { get; }
}