<img src="/assets/logos/2sxc10/2sxcV10.06.png" style="width: 100%">

[//]: # "Documentation Overview"
<details>
    <summary>
        <strong>Documentation Overview</strong>
    </summary>

This is where you can find things like...

* [2sxc.org](https://2sxc.org)
* [2sxc Wiki Home](Home)
* [Learn on 2sxc.org](http://2sxc.org/en/Learn)
* [Blog on 2sxc.org](http://2sxc.org/en/blog)
* [Apps on 2sxc.org](http://2sxc.org/en/Apps)
* [Roadmap](2sxc-roadmap-and-history)
* [Manuals on 2sxc.org](http://2sxc.org/en/docs)

</details>

## Concepts / Features

[//]: # "Basic Concepts / Features"
<details>
    <summary>
        <strong>Basic Concepts / Features</strong>
    </summary>

* [Views](xref:Concepts.Views)
* [Data Entities](xref:Concepts.Entities)
* [Content-Types](xref:Concepts.ContentType)
* [Relationships](xref:Concepts.Relationships)
* [Editing content](xref:Concepts.Edit)
* [Auto-Optimization of JS/CSS Assets](Template-Assets)
* [Hide advanced features](xref:Concepts.HideAdvancedFeatures)
* [Edit Context](xref:Concepts.EditContext)
* [In-Page toolbars](xref:Concepts.EditToolbar)
* [Quick Edit toolbars (quickE)](xref:Concepts.QuickE)
* [Tokens](xref:Concepts.Tokens)
* TinyMCE WYSIWYG [todoc](doc-tags#todoc)
* ADAM Automatic Digital Assets Management [todoc](doc-tags#todoc)

</details>



[//]: # "Advanced Features"
<details>
    <summary>
        <strong>Advanced Concepts / Features</strong>
    </summary>

* [Inner Content](xref:Concepts.InnerContent)
* [Page Publishing](xref:Concepts.PagePublishing)
* [Polymorph Editions](xref:Concepts.Polymorphism)
* [Permissions](xref:Concepts.Permissions)
* [Features Management](xref:Concepts.Features)

</details>



[//]: # "Developer Features"
<details>
    <summary>
        <strong>Developer Concepts / Features</strong>
    </summary>

* [DNN Module and 2sxc Instance](xref:Concepts.DnnModule)
* [File provided content types](xref:Concepts.FileBasedContentTypes)

</details>



[//]: # "Contributing"
<details>
    <summary>
        <strong>For Contributors</strong>
    </summary>

* [2sxc / EAV Architecture](architecture)
* [Setting up a dev solution](xref:Specs.Contribute)
* [How to contribute](xref:Specs.DocsContribute)
* [Internal JSON formats](xref:Specs.Data.Formats.JsonV1)

</details>



## API References

[//]: # "C# API"
<details>
    <summary>
        <strong>C# API</strong>
    </summary>

* [C# API Overview](DotNet-Razor-CSharp-Overview)
* [LINQ tipps](xref:Specs.DataSources.Linq)
* [Using objects from non-2sxc code](dotnet-external-use)
* [WebApi](xref:HowTo.WebApi)
* [Features API](dotnet-features)

</details>

[//]: # "C# API Razor"
<details>
    <summary>
        <strong>C# API in Razor</strong>
    </summary>

* [C# API in Razor Templates](razor-templates)
* Main Objects
  * [App object](xref:HowTo.DynamicCode.App)
  * Content, a [DynamicEntity](dotnet-dynamicentity)
  * [Data](xref:HowTo.DynamicCode.Data), a [DataSource](xref:Specs.DataSources.DataSource)
  * [DNN](xref:HowTo.DynamicCode.Dnn)
  * [Edit](xref:HowTo.DynamicCode.Edit) with [Toolbar](xref:HowTo.Razor.EditToolbar) and [Enable](xref:Razor.EditEnable]
  * [Link](xref:HowTo.DynamicCode.Link)
  * ListContent, a [DynamicEntity](dotnet-dynamicentity)
  * Permissions [todoc](doc-tags#todoc)
* Helper Commands
  * AsAdam(...) [todoc](doc-tags#todoc)
  * AsDynamic(...) [todoc](doc-tags#todoc)
  * AsEntity(...) [todoc](doc-tags#todoc)
  * CreateInstance(...) [todoc](doc-tags#todoc)
  * CreateSource<T>(...) [DataSource](xref:Specs.DataSources.DataSource)
* Customizing Data & Search
  * [InstancePurpose](razor-sexycontentwebpage.instancepurpose)
  * [CustomizeData](xref:HowTo.Razor.CustomizeData)
  * [CustomizeSearch](xref:HowTo.Razor.CustomizeSearch)
* RazorBlade API
  * [RazorBlade Tutorials](https://2sxc.org/dnn-tutorials/en/razor/blade/home)
  * [RazorBlade fluent API](https://2sxc.org/dnn-tutorials/en/razor/blade800/page)
  * [RazorBlade on github](https://github.com/DNN-Connect/razor-blade/)
</details>

[//]: # "C# API WebAPI"
<details>
    <summary>
        <strong>C# API in WebAPI</strong>
    </summary>

* [Overview Custom WebAPI](xref:HowTo.WebApi)
* Main Objects
  * [App object](xref:HowTo.DynamicCode.App)
  * Content, a [DynamicEntity](dotnet-dynamicentity)
  * [Data](xref:HowTo.DynamicCode.Data), a [DataSource](xref:Specs.DataSources.DataSource)
  * [DNN](xref:HowTo.DynamicCode.Dnn)
  * [Edit](xref:HowTo.DynamicCode.Edit) with [Toolbar](xref:HowTo.Razor.EditToolbar)
  * Permissions [todoc](doc-tags#todoc)
* Helper Commands
  * AsAdam(...) [todoc](doc-tags#todoc)
  * AsDynamic(...) [todoc](doc-tags#todoc)
  * AsEntity(...) [todoc](doc-tags#todoc)
  * CreateSource<T>(...) [DataSource](xref:Specs.DataSources.DataSource)
  * [SaveInAdam(...)](dotnet-webapi-saveinadam)

</details>

[//]: # "Data Sources"
<details>
    <summary>
        <strong>DataSources</strong>
    </summary>

* [DataSource Overview](xref:Specs.DataSources.DataSource)
* [List of all sources](xref:Specs.DataSources.ListAll)
* [Create custom DataSources](xref:Specs.DataSources.Custom)
* DataSource Configuration
  * [Concept](xref:Specs.DataSources.Configuration)
  * [Tokens](xref:Concepts.Tokens)
* API for creating Data Sources
  * [API overview](xref:Specs.DataSources.Api)
  * [AsEntity](xref:Specs.DataSources.Api.AsEntity)
  * [EnsureConfiguration...](xref:Specs.DataSources.Api.EnsureConfigurationIsLoaded)
  * [ConfigMask](xref:Specs.DataSources.Api.ConfigMask)
  * [Provide](xref:Specs.DataSources.Api.Provide)
  * [VisualQuery attribute](dotnet-datasource-api-visualquery)

</details>



[//]: # "Field Data Types"
<details>
    <summary>
        <strong>Field Data Types</strong>
    </summary>

* [Data Types Overview](xref:Specs.Data.Type.Overview)
* [Boolean](xref:Specs.Data.Type.Boolean)
* [DateTime](data-type-datetime)
* [Empty (groups/titles)](xref:Specs.Data.Type.Empty)
* [Entity (relationships)](xref:Specs.Data.Type.Entity)
* [Hyperlink (links/files)](xref:Specs.Data.Type.Hyperlink)
* [Number](xref:Specs.Data.Type.Number)
* [String (text)](xref:Specs.Data.Type.String)

</details>



[//]: # "Field Input Types"
<details>
    <summary>
        <strong>Field Input Types</strong>
    </summary>

* [Input Fields Overview](xref:Specs.Data.Inputs.All)
* [All (config for all types)](xref:Specs.Data.Inputs.All)
* [Boolean (overview)](xref:Specs.Data.Inputs.Boolean)
  * [boolean-default](xref:Specs.Data.Inputs.Boolean-Default)
* [DateTime (overview)](xref:Specs.Data.Inputs.Datetime)
  * [datetime-default](xref:Specs.Data.Inputs.Datetime-Default)
* [Empty (overview)](xref:Specs.Data.Inputs.Empty)
  * [empty-default (group/title)](xref:Specs.Data.Inputs.Empty-Default)
* [Entity (overview)](xref:Specs.Data.Inputs.Entity)
  * [entity (default)](xref:Specs.Data.Inputs.Entity-Default)
  * [entity-query](xref:Specs.Data.Inputs.Entity-Query)
* [Number (overview)](xref:Specs.Data.Inputs.Number)
  * [number-default](xref:Specs.Data.Inputs.Number-Default)
* [Hyperlink / Files](xref:Specs.Data.Inputs.Hyperlink)
  * [hyperlink-default](xref:Specs.Data.Inputs.Hyperlink-Default)
  * [hyperlink-library](xref:Specs.Data.Inputs.Hyperlink-Library)
* [String (overview)](xref:Specs.Data.Inputs.String)
  * [string-default](xref:Specs.Data.Inputs.String-Default)
  * [string-dropdown](xref:Specs.Data.Inputs.String-Dropdown)
  * [string-dropdown-query](xref:Specs.Data.Inputs.String-Dropdown-Query)
  * [string-font-icon-picker](xref:Specs.Data.Inputs.String-Font-Icon-Picker)
  * [string-url-path](xref:Specs.Data.Inputs.String-Url-Path)
* string-wysiwyg [todoc](doc-tags#todoc)
* Advanced / Special types
  * GPS Picker [todoc](doc-tags#todoc)
* Custom input types [todoc](doc-tags#todoc)

</details>



[//]: # "Html & JS API"
<details>
    <summary>
        <strong>HTML & JS API</strong>
    </summary>

* [JavaScript API Overview](xref:Specs.Js.Overview)
* [$2sxc Controller](xref:Specs.Js.$2sxc)
* [$2sxc.cms](xref:Specs.Js.$2sxc.Cms)
* [sxc instance](xref:Specs.Js.Sxc)
* [sxc webapi](xref:Specs.Js.Sxc.WebApi)
* [manage controller (edit-api)](xref:Specs.Js.Sxc.Manage)
* [toolbars, buttons, actions](xref:Specs.Js.Toolbar.Js)
  * [button object](xref:Specs.Js.Toolbar.Buttons)
  * [commands](xref:Specs.Js.Commands)
  * [toolbar-settings](xref:Specs.Js.Toolbar.Settings)
* [$quicke quick-edit controller](xref:Specs.Js.QuickE)

</details>

[//]: # "Angular API"
<details>
    <summary>
        <strong>Angular API</strong>
    </summary>

* [Angular 6+ Toolbar API](dnn-sxc-angular)

</details>


[//]: # "REST / Web API"
<details>
    <summary>
        <strong>REST API / WebApi</strong>
    </summary>

* [WebAPI and REST endpoints](xref:HowTo.WebApis)
* [Create your own WebAPI (.net)](xref:HowTo.WebApi)
</details>


[//]: # "Templating"
<details>
    <summary>
        <strong>Templating</strong>
    </summary>

* [Asset Optimization (css, js, etc.)](template-assets)

</details>

