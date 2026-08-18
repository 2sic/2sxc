using ToSic.Eav.WebApi.Sys.Cms;
using ToSic.Sxc.Blocks.Sys.Work;

namespace ToSic.Sxc.Backend.Cms;

/// <summary>
/// Special List-Action to get information about a Content Block Header.
/// This is unusual, because if it's empty, we must retrieve the definition of the expected content-type from the view definition.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class ListActivityGetBlockHeader(Generator<WorkBlocks, IAppWorkCtxForDiWip> appBlocks)
    : ServiceWithSetup<IAppWorkCtxForDiWip>("Api.CntGrpRl", connect: [appBlocks])
{
    public List<EntityInListDto> ContentBlockHeader(Guid parent)
    {
        var l = Log.Fn<List<EntityInListDto>>($"header for:{parent}");
        //var appCtx = appBlocks.CtxSvc.ContextPlus(ctxService.BlockContextRequired().AppReaderRequired);
        var cg = appBlocks.New(MyOptions).GetBlockConfig(parent);

        // new in v11 - this call might be run on a non-content-block, in which case we return null
        var ent = (cg as ICanBeEntity)?.Entity;
        if (ent == null!)
            return l.Return([],"No entity found");
        if (ent.Type.Name != WorkBlocks.BlockTypeName)
            return l.Return([],"Entity type mismatch");

        var header = cg.Header.FirstOrDefault();

        return l.Return([
            new(header, 0)
            {
                Type = header?.Type.NameId ?? cg.View!.HeaderType
            }
        ]);
    }

}