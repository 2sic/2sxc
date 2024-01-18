namespace ToSic.Sxc.Edit.Toolbar;

public partial interface IToolbarBuilder
{
    /// <summary>
    /// Get a toolbar builder which would render to HTML as a standalone tag.
    ///
    /// **Important:** Toolbars created using this will behave differently from previous standalone toolbars.
    /// These standalone toolbars will _not_ float automatically as previous ones did.
    /// You can still get them to float by adjusting the Settings as you need them.
    /// 
    /// This is because many years ago, standalone toolbars were configured floated automatically.
    /// As the APIs got better, this wasn't a great default any more, but we couldn't introduce a breaking change.
    /// Anything created now with this new API will be new, so this will behave more in line with expectations.
    /// See also [issue](https://github.com/2sic/2sxc/issues/2838)
    /// </summary>
    /// <param name="target">_optional_ entity-like target, see [target guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Target)</param>
    /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
    IToolbarBuilder AsTag(
        object target = null
    );

    /// <summary>
    /// Get a toolbar builder which would render to HTML as attributes on an existing tag.
    /// Note that this is the default, so you will usually not need this. 
    /// </summary>
    /// <param name="target">_optional_ entity-like target, see [target guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Target)</param>
    /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
    IToolbarBuilder AsAttributes(
        object target = null
    );


    /// <summary>
    /// Test code
    /// </summary>
    /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
    /// <param name="target">_optional_ entity-like target, see [target guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Target)</param>
    [PrivateApi("wip")]
    [System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    IToolbarBuilder AsJson(
        object target = null
    );
        
}