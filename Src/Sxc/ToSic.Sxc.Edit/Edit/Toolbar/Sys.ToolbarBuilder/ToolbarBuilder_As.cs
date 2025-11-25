namespace ToSic.Sxc.Edit.Toolbar.Sys.ToolbarBuilder;

partial record ToolbarBuilder
{

    public IToolbarBuilder AsTag(object? target = null) =>
        With(mode: ToolbarHtmlModes.Standalone, target: target);

    public IToolbarBuilder AsAttributes(object? target = null) =>
        With(mode: ToolbarHtmlModes.OnTag, target: target);

    public IToolbarBuilder AsJson(object? target = null) =>
        With(mode: ToolbarHtmlModes.Json, target: target);

    /// <summary>
    /// Helper for the AsTag, AsAttributes, AsJson methods.
    /// </summary>
    /// <param name="noParamOrder"></param>
    /// <param name="mode"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    private IToolbarBuilder With(NoParamOrder npo = default, string? mode = default, object? target = default)
    {
        // Create clone before starting to log so it's in there too
        var clone = target == null
            ? this
            : (ToolbarBuilder)Parameters(target);   // Params will already copy/clone it

        return mode == null
            ? clone
            : clone with
            {
                Configuration = (Configuration ?? new()) with
                {
                    HtmlMode = mode
                }
            };
    }
}