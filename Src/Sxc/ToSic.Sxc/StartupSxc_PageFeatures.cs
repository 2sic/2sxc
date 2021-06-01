using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Web.PageFeatures;

namespace ToSic.Sxc
{
    public static partial class StartupSxc
    {
        ///// <summary>
        ///// WIP 12.02 - registered features to enable on the page. Implementation far from done, but we must get turnOn to activate like this.
        ///// </summary>
        ///// <param name="services"></param>
        //public static void RegisterPageFeatures(IServiceCollection services)
        //{
        //    var sp = services.BuildServiceProvider();
        //    var featureManager = sp.Build<IPageFeaturesManager>();

        //    featureManager.Register(
        //        BuiltInFeatures.PageContext,
        //        BuiltInFeatures.Core,
        //        BuiltInFeatures.EditApi,
        //        BuiltInFeatures.EditUi,
        //        BuiltInFeatures.TurnOn
        //    );
        //}
    }
}