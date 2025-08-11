using ToSic.Eav.Apps;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Web.Sys.LightSpeed;
internal class LightSpeedConfigHelper(ILog? parentLog) : HelperBase(parentLog, "Sxc.LsCnRd")
{

    public LightSpeedDecorator GetLightSpeedConfigOfApp(IAppReader? appReader)
    {
        var l = Log.Fn<LightSpeedDecorator>();
        var decoFromPiggyBack = LightSpeedDecorator.GetFromAppStatePiggyBack(appReader/*, Log*/);
        return l.Return(decoFromPiggyBack, $"has decorator: {decoFromPiggyBack.Entity != null!}");
    }

    public LightSpeedDecorator? ViewConfigOrNull(IBlock? block) =>
        block?.ViewIsReady != true
            ? null
            : block.View.Metadata
                .OfType(LightSpeedDecorator.TypeNameId)
                .FirstOrDefault()
                .NullOrGetWith(viewLs => new LightSpeedDecorator(viewLs));

}
