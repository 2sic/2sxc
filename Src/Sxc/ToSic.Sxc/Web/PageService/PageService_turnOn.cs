using ToSic.Lib.Logging;
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
            var l = Log.Fn<string>($"{runOrSpecs}: {runOrSpecs}; {require}; {data}");
            // first activate the page feature
            Activate(BuiltInFeatures.TurnOn.NameId);

            // then generate the turn-on and add to module state
            var tag = _turnOn.Value.Run(runOrSpecs, require: require, data: data);
            _moduleService.Value.AddToMore(tag);

            // Then return empty string
            return l.ReturnAsOk("");
        }
    }
}
