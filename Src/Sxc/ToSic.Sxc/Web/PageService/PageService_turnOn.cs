using ToSic.Lib.Logging;
using ToSic.Sxc.Web.PageFeatures;

namespace ToSic.Sxc.Web.PageService;

public partial class PageService
{
    /// <inheritdoc />
    public string TurnOn(object runOrSpecs,
        string noParamOrder = Eav.Parameters.Protector,
        object require = default,
        object data = default,
        bool condition = true,
        bool? noDuplicates = default)
    {
        var l = Log.Fn<string>($"{runOrSpecs}: {runOrSpecs}; {require}; {data}");

        // Check condition - default is true - so if it's false, this overload was called
        if (!condition)
            return l.ReturnNull("condition false");

        // first activate the page feature
        Activate(BuiltInFeatures.TurnOn.NameId);

        // then generate the turn-on and add to module state
        var tag = _turnOn.Value.Run(runOrSpecs, require: require, data: data);
        _moduleService.Value.AddToMore(tag, noDuplicates: noDuplicates == true);

        // Then return empty string
        return l.ReturnAsOk(null);
    }
}