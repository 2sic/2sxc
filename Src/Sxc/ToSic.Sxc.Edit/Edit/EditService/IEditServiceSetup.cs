using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Services;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Edit.Internal;

public interface IEditServiceSetup
{
    internal IEditService SetBlock(IExecutionContext codeRoot, IBlock block);
}