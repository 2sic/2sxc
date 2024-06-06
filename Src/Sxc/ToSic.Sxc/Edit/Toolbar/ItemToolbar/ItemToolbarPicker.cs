using ToSic.Razor.Markup;

namespace ToSic.Sxc.Edit.Toolbar;

internal class ItemToolbarPicker
{
    internal static ItemToolbarBase ItemToolbar(IEntity entity, string actions = null, string newType = null,
        object prefill = null, object settings = null, object toolbar = null)
    {
        // Case v13+ Toolbar Builder
        if (toolbar is IToolbarBuilder toolbarBuilder)
            return new ItemToolbarV14(entity, toolbarBuilder);

        // Case 1 - use the simpler string format in V10.27
        var (isV10, _) = CheckIfParamsMeanV10(toolbar, settings, prefill);
        if (isV10)
        {
            // check conflicting prefill format
            if (prefill != null && prefill is not string)
                throw new("Tried to build toolbar in new V10 format, but prefill is not a string. In V10.27+ it expects a string in url format like field=value&field2=value2");

            return new ItemToolbarV10(entity, newType, (string)prefill, settings as string, toolbar);
        }

        // Case 2 - we have a classic V3 Toolbar object
        if (toolbar != null)
        {
            // check conflicting parameters
            if (actions != null || newType != null || prefill != null)
                throw new(
                    "trying to build toolbar but got both toolbar and actions/prefill/newType - this is conflicting, cannot continue");
        }

        return new ItemToolbar(entity, actions, newType, prefill, settings, toolbar);
    }

    internal static (bool IsV10, List<string> Rules) CheckIfParamsMeanV10(object toolbar = null, object settings = null, object prefill = null)
    {
        // Case 1 - use the simpler string format in V10.27
        var toolbarAsStringArray = ToolbarV10OrNull(toolbar);
        if (settings is string || toolbar is string || prefill is string || toolbarAsStringArray != null)
        {
            return (true, toolbarAsStringArray);
        }
        return (false, null);
    }

    /// <summary>
    /// Check if the configuration we got is a V10 Toolbar
    /// </summary>
    /// <param name="toolbar"></param>
    /// <returns></returns>
    internal static List<string> ToolbarV10OrNull(object toolbar)
    {
        // Fix 14.04 - I believe this case somehow got lost in history
        if (toolbar is string strToolbar)
            return [strToolbar];

        // Note: This is a bit complex because previously we checked for this:
        // return toolbar is IEnumerable<string> array && array.FirstOrDefault() != null;
        // But that failed, because sometimes razor made the new [] { "..." } be an object list instead
        // This is why it's more complex that you would intuitively do it - see https://github.com/2sic/2sxc/issues/2561

        if (toolbar is not IEnumerable<object> objEnum)
            return null;

        var asArray = objEnum.ToArray();
        return !asArray.All(o => o is string or IRawHtmlString)
            ? null
            : asArray
                .Select(o => o.ToString())
                .Where(s => s != "")
                .ToList();
    }

}