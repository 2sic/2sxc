using ToSic.Sxc.Web.PageFeatures;

namespace ToSic.Sxc.Web.PageService
{
    public partial class PageService
    {
        /// <inheritdoc />
        public string TurnOn(object runOrSpecs,
            string noParamOrder = Eav.Parameters.Protector,
            object require = default,
            object data = default)
        {
            // first activate the page feature
            Activate(BuiltInFeatures.TurnOn.NameId);

            // then generate the turn-on and add to module state
            var tag = _turnOn.Ready.Run(runOrSpecs, require: require, data: data);
            _moduleService.Ready.AddToMore(tag);

            // Then return empty string
            return "";
        }
    }
}
