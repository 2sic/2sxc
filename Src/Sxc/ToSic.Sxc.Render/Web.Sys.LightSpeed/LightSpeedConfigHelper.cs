using ToSic.Eav.Apps;
using ToSic.Sxc.Blocks.Sys;

namespace ToSic.Sxc.Web.Sys.LightSpeed;
internal class LightSpeedConfigHelper(ILog? parentLog) : HelperBase(parentLog, "Sxc.LsCnRd")
{

    public LightSpeedDecorator GetLightSpeedConfigOfApp(IAppReader? appReader)
    {
        var l = Log.Fn<LightSpeedDecorator>();
        var decoFromPiggyBack = LightSpeedDecorator.GetFromAppStatePiggyBack(appReader/*, Log*/);
        return l.Return(decoFromPiggyBack, $"has decorator: {decoFromPiggyBack.Entity != null!}");
    }

    public LightSpeedDecorator? ViewConfigOrNull(IBlock? block)
    {
        var l = Log.Fn<LightSpeedDecorator?>();
        if (block?.ViewIsReady != true)
            return l.ReturnNull("view not ready");
            
        var md = block.View.Metadata
                .OfType(LightSpeedDecorator.TypeNameId)
                .FirstOrDefault();
        
        return md == null
            ? l.ReturnNull($"no view metadata for LightSpeedDecorator; view: {block.View.Id}")
            : l.Return(new(md), $"entity: {md.EntityId}; view: {block.View.Id}");
    }
}
