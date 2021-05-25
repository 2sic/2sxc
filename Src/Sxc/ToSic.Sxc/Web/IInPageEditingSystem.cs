using System;
using System.Web;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Data;
#if NET451
using HtmlString = System.Web.HtmlString;
#else
using HtmlString = Microsoft.AspNetCore.Html.HtmlString;
#endif

// ReSharper disable UnusedMember.Global

namespace ToSic.Sxc.Web
{
    /// <summary>
    /// Contains status and commands to configure the in-page editing system. 
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public interface IInPageEditingSystem
    {
        /// <summary>
        /// If editing is enabled or not
        /// </summary>
        /// <returns>True if enabled, false if not.</returns>
        bool Enabled
        {
            get;
            [PrivateApi("hide the set - it's only used for demo code")]
            set;
        }

        /// <summary>
        /// Generate a toolbar tag - must be used in normal html, not as an attribute. <br/>
        /// See also @HowTo.Razor.Edit.Toolbar
        /// </summary>
        /// <param name="target">
        /// The content-item this toolbar is for, can be null. <br/>
        /// Usually a @NetCode.DynamicCode.DynamicEntity?text=DynamicEntity or a @NetCode.DynamicCode.Entity?text=Entity
        /// </param>
        /// <param name="noParameterOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="actions">
        /// List of actions on this toolbar. If null, will use default actions for this item.
        /// If provided, must be comma-separated action-names - see [](xref:JsCode.Commands.Index).
        /// </param>
        /// <param name="contentType">Content-type of this toolbar, used when it has `new` or `add` buttons.
        /// This allows you to create a button for a new "Category" and another button for a new "BlogPost" etc.
        /// </param>
        /// <param name="prefill">
        /// Allows a `new` dialog to receive values as a prefill.
        /// For example to already specify a date, title, category, etc. <br/>
        /// It's a dynamic object, see also the JS documentation on the prefill.
        /// </param>
        /// <param name="toolbar">
        /// Full manual toolbar configuration. Setting this will cause `actions` to be ignored. <br/>
        /// See [](xref:Basics.Browser.EditUx.Toolbars.Index)
        /// </param>
        /// <param name="settings">
        /// Toolbar settings controlling hover etc. <br/>
        /// See [](xref:JsCode.Toolbars.Settings)
        /// </param>
        /// <returns>If the user is an editor, it returns HTML UL tag containing all the toolbar configuration.</returns>
        /// <remarks>
        /// **History**
        /// 1. Added in 2sxc 8.04
        /// </remarks>
        HtmlString Toolbar(object target = null,
            string noParameterOrder = Eav.Parameters.Protector,
            string actions = null,
            string contentType = null,
            object prefill = null,
            object toolbar = null,
            object settings = null);

        /// <summary>
        /// Generate a toolbar attribute inside an html-tag <br/>
        /// See also @HowTo.Razor.Edit.Toolbar
        /// </summary>
        /// <param name="target">
        /// The content-item this toolbar is for, can be null. <br/>
        /// Usually a @NetCode.DynamicCode.DynamicEntity?text=DynamicEntity or a @HowTo.DynamicCode.Entity?text=Entity
        /// </param>
        /// <param name="noParameterOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="actions">
        /// List of actions on this toolbar. If null, will use default actions for this item.
        /// If provided, must be comma-separated action-names - see [](xref:JsCode.Commands.Index).
        /// </param>
        /// <param name="contentType">Content-type of this toolbar, used when it has `new` or `add` buttons.
        /// This allows you to create a button for a new "Category" and another button for a new "BlogPost" etc.
        /// </param>
        /// <param name="prefill">
        /// Allows a `new` dialog to receive values as a prefill.
        /// For example to already specify a date, title, category, etc. <br/>
        /// It's a dynamic object, see also the JS documentation on the prefill.
        /// </param>
        /// <param name="toolbar">
        /// Full manual toolbar configuration. Setting this will cause `actions` to be ignored. <br/>
        /// See [](xref:Basics.Browser.EditUx.Toolbars.Index)
        /// </param>
        /// <param name="settings">
        /// Toolbar settings controlling hover etc. <br/>
        /// See [](xref:JsCode.Toolbars.Settings)
        /// </param>
        /// <returns>If the user is an editor, it returns the attribute containing all the toolbar configuration.</returns>
        /// <remarks>
        /// **History**
        /// 1. Added in 2sxc 9.40
        /// </remarks>
        HtmlString TagToolbar(object target = null,
            string noParameterOrder = Eav.Parameters.Protector,
            string actions = null,
            string contentType = null,
            object prefill = null,
            object toolbar = null,
            object settings = null);

        /// <summary>
        /// Get html-attributes to mark the current context
        /// these will be added to a wrapper tag (usually a div)
        /// so that in-page editing knows what the context is <br/>
        /// Please read more about [](xref:Basics.Cms.InnerContent.Index)
        /// </summary>
        /// <param name="target">The content-item for which the new context should be.
        /// This item usually has a field which has [](xref:Basics.Cms.InnerContent.Index)</param>
        /// <param name="noParameterOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="field">the field of this content-item, which contains the inner-content-items</param>
        /// <param name="contentType">type name used for 'new' items in a toolbar - usually for inner-content and list-contexts</param>
        /// <param name="newGuid">the guid of a new item - use null for auto-generate</param>
        /// <param name="apps">Beta / WIP</param>
        /// <param name="max">Beta / WIP</param>
        /// <returns>An <see cref="HtmlString"/> object containing an html-attribute to add to the wrapper of the inner content</returns>
        /// <remarks>
        /// **History** <br/>
        /// 1. Introduced in 2sxc 8.4
        /// 1. Enhanced with apps in 10.27
        /// </remarks>
        HtmlString ContextAttributes(IDynamicEntity target, 
            string noParameterOrder = Eav.Parameters.Protector, 
            string field = null, 
            string contentType = null,
            Guid? newGuid = null,
            string apps = null, 
            int max = 100);

        /// <summary>
        /// Wrap something in a context wrapper-tag
        /// This is mainly meant for internal use
        /// </summary>
        /// <param name="content">the string / tags to wrap</param>
        /// <param name="noParameterOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="tag">optional tag to use for the wrapper, default is div</param>
        /// <param name="full">include full context (default is partial context only)</param>
        /// <param name="enableEdit">include information needed for editing</param>
        /// <param name="instanceId">id to include in context - important for API calls</param>
        /// <param name="contentBlockId">content block this is for - important for API calls</param>
        /// <returns></returns>
        /// <remarks>
        /// **History** <br/>
        /// 1. Introduced in 2sxc 8.4
        /// </remarks>
        [PrivateApi]
        HtmlString WrapInContext(object content,
            string noParameterOrder = Eav.Parameters.Protector,
            string tag = Constants.DefaultContextTag,
            bool full = false,
            bool? enableEdit = null,
            int instanceId = 0,
            int contentBlockId = 0
        );

        /// <summary>
        /// Ensure that the UI will load the correct assets to enable editing. See @NetCode.Razor.Edit.Enable?text=How+to+use+Edit.Enable
        /// </summary>
        /// <param name="noParameterOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="js">optional, default false. automatically true if `api` or `forms` is true<br/>
        /// Will add the basic JS APIs ($2sxc) usually for enabling WebApi calls from your JS code. <br/>
        /// _added in v10.20_
        /// </param>
        /// <param name="api">optional, default false. automatically true, if `forms` is true<br/>
        ///     If JS etc. should be included to enable editing API - ensures javascripts are loaded enabling commands to run</param>
        /// <param name="forms">optional, default false. <br/>
        ///     Ensures javascripts are loaded enabling forms to open</param>
        /// <param name="turnOn">activate turnOn script helpers - this is BETA / WIP</param>
        /// <param name="context">optional, default false. <br/>
        ///     If context ([](xref:Basics.Browser.EditUx.EditContext)) should be added, to ensure in-instance data editing</param>
        /// <param name="autoToolbar">optional, default false. <br/>
        ///     Disables the automatic generation of a toolbar (this is important, as there usually won't be a toolbar in public pages, which would then trigger the fallback-toolbar to be generated)</param>
        /// <param name="styles">optional, default false. <br/>
        ///     Ensures styles to be loaded, which would be necessary for the standard toolbars to look right</param>
        /// <returns>null - but we wanted to make sure it returns something, so you can use it in razor like @Edit.Enable(...)</returns>
        /// <remarks>
        /// **History** <br/>
        /// 1. Introduced in 2sxc 9.30
        /// 2. Enhanced with parameter jsApi in 10.20
        /// </remarks>
        string Enable(string noParameterOrder = Eav.Parameters.Protector,
            bool? js = null,
            bool? api = null, 
            bool? turnOn = null,
            bool? forms = null,
            bool? context = null,
            bool? autoToolbar = null,
            bool? styles = null);

        /// <summary>
        /// Generate an HTML attribute by converting the value to JSON 
        /// - but only in edit mode
        /// </summary>
        /// <param name="name">the attribute name, used for ...=</param>
        /// <param name="value">the attribute value, used for ="..."</param>
        /// <returns>A string but as HtmlString, so it can be used with @Attribute(...)</returns>
        HtmlString Attribute(string name, string value);

        /// <summary>
        /// Generate an HTML attribute by converting the value to JSON 
        /// - but only in edit mode
        /// </summary>
        /// <param name="name">the attribute name, used for ...=</param>
        /// <param name="value">the attribute value, used for ="..."</param>
        /// <returns>A string but as HtmlString, so it can be used with @Attribute(...)</returns>
        HtmlString Attribute(string name, object value);
    }
}
