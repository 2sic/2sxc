using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Edit.EditService;

public interface IEditServiceSetup
{
    internal IEditService SetBlock(ICodeApiService codeRoot, IBlock block);
}