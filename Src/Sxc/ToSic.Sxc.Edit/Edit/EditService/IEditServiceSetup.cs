using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Services;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Edit.EditService;

public interface IEditServiceSetup
{
    internal IEditService SetBlock(IExecutionContext? exCtxOrNull, IBlock block);
}