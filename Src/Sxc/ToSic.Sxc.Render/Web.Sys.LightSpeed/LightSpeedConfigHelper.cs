using ToSic.Eav.Apps;
using ToSic.Sxc.Blocks.Sys;

namespace ToSic.Sxc.Web.Sys.LightSpeed;
internal class LightSpeedConfigHelper(ILog? parentLog) : HelperBase(parentLog, "Sxc.LsCnRd")
{

    public LightSpeedDecorator GetLightSpeedConfigOfApp(IAppReader? appReader)
    {
        var l = Log.Fn<LightSpeedDecorator>();
        var decoFromPiggyBack = LightSpeedDecorator.GetFromAppStatePiggyBack(appReader);
        return l.Return(decoFromPiggyBack, $"has decorator: {(decoFromPiggyBack as ICanBeEntity)?.Entity != null!}");
    }

    public LightSpeedDecorator? ViewConfigOrNull(IBlock? block)
    {
        var l = Log.Fn<LightSpeedDecorator?>();
        if (block?.ViewIsReady != true)
            return l.ReturnNull("view not ready");
            
        var md = block.View.Metadata.First<LightSpeedDecorator>();

        return md == null
            ? l.ReturnNull($"no view metadata for LightSpeedDecorator; view: {block.View.Id}")
            : l.Return(md, $"entity: {md.Id}; view: {block.View.Id}");
    }
}
