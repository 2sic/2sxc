using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Services;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Edit.Internal;

public interface IEditServiceSetup
{
    internal IEditService SetBlock(IExecutionContext? exCtxOrNull, IBlock block);
}