<img src="assets/logos/2sxc10/2sxcV10.06.png" style="width: 100%">

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

* [Views](concept-views)
* [Data Entities](concept-entities)
* [Content-Types](concept-content-types)
* [Relationships](concept-relationships)
* [Editing content](overview-edit)
* [Auto-Optimization of JS/CSS Assets](Template-Assets)
* [Hide advanced features](concept-hide-advnaced-features)
* [Edit Context](concept-edit-context)
* [In-Page toolbars](concept-inpage-toolbars)
* [Quick Edit toolbars (quickE)](concept-quick-edit)
* [Tokens](concept-tokens)
* TinyMCE WYSIWYG [todoc](doc-tags#todoc)
* ADAM Automatic Digital Assets Management [todoc](doc-tags#todoc)

</details>



[//]: # "Advanced Features"
<details>
    <summary>
        <strong>Advanced Concepts / Features</strong>
    </summary>

* [Inner Content](concept-inner-content)
* [Page Publishing](concept-dnn-evoq-page-publishing)
* [Polymorph Editions](concept-polymorph)
* [Permissions](concept-permissions)
* [Features Management](concept-features)

</details>



[//]: # "Developer Features"
<details>
    <summary>
        <strong>Developer Concepts / Features</strong>
    </summary>

* [DNN Module and 2sxc Instance](concept-dnn-module)
* [File provided content types](concept-file-provided-content-types)

</details>



[//]: # "Contributing"
<details>
    <summary>
        <strong>For Contributors</strong>
    </summary>

* [2sxc / EAV Architecture](architecture)
* [Setting up a dev solution](contribute-setup)
* [How to contribute](contribute)
* [Internal JSON formats](format-json-v1)

</details>



## API References

[//]: # "C# API"
<details>
    <summary>
        <strong>C# API</strong>
    </summary>

* [C# API Overview](DotNet-Razor-CSharp-Overview)
* [LINQ tipps](dotnet-query-linq)
* [Using objects from non-2sxc code](dotnet-external-use)
* [WebApi](dotnet-webapi)
* [Features API](dotnet-features)

</details>

[//]: # "C# API Razor"
<details>
    <summary>
        <strong>C# API in Razor</strong>
    </summary>

* [C# API in Razor Templates](razor-templates)
* Main Objects
  * [App object](razor-app)
  * Content, a [DynamicEntity](dotnet-dynamicentity)
  * [Data](razor-data), a [DataSource](dotnet-datasource)
  * [DNN](razor-dnn)
  * [Edit](razor-edit) with [Toolbar](razor-edit.toolbar) and [Enable](razor-edit.enable)
  * [Link](razor-link)
  * ListContent, a [DynamicEntity](dotnet-dynamicentity)
  * Permissions [todoc](doc-tags#todoc)
* Helper Commands
  * AsAdam(...) [todoc](doc-tags#todoc)
  * AsDynamic(...) [todoc](doc-tags#todoc)
  * AsEntity(...) [todoc](doc-tags#todoc)
  * CreateInstance(...) [todoc](doc-tags#todoc)
  * CreateSource<T>(...) [DataSource](dotnet-datasource)
* Customizing Data & Search
  * [InstancePurpose](razor-sexycontentwebpage.instancepurpose)
  * [CustomizeData](razor-sexycontentwebpage.customizedata)
  * [CustomizeSearch](razor-sexycontentwebpage.customizesearch)
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

* [Overview Custom WebAPI](dotnet-webapi)
* Main Objects
  * [App object](razor-app)
  * Content, a [DynamicEntity](dotnet-dynamicentity)
  * [Data](razor-data), a [DataSource](dotnet-datasource)
  * [DNN](razor-dnn)
  * [Edit](razor-edit) with [Toolbar](razor-edit.toolbar)
  * Permissions [todoc](doc-tags#todoc)
* Helper Commands
  * AsAdam(...) [todoc](doc-tags#todoc)
  * AsDynamic(...) [todoc](doc-tags#todoc)
  * AsEntity(...) [todoc](doc-tags#todoc)
  * CreateSource<T>(...) [DataSource](dotnet-datasource)
  * [SaveInAdam(...)](dotnet-webapi-saveinadam)

</details>

[//]: # "Data Sources"
<details>
    <summary>
        <strong>DataSources</strong>
    </summary>

* [DataSource Overview](dotnet-datasource)
* [List of all sources](dotnet-datasources-all)
* [Create custom DataSources](dotnet-datasources-custom)
* DataSource Configuration
  * [Concept](dotnet-datasources-configuration)
  * [Tokens](concept-tokens)
* API for creating Data Sources
  * [API overview](dotnet-datasource-api)
  * [AsEntity](dotnet-datasource-api-asentity)
  * [EnsureConfiguration...](dotnet-datasource-api-ensureconfigurationisloaded)
  * [ConfigMask](dotnet-datasource-api-configmask)
  * [Provide](dotnet-datasource-api-provide)
  * [VisualQuery attribute](dotnet-datasource-api-visualquery)

</details>



[//]: # "Field Data Types"
<details>
    <summary>
        <strong>Field Data Types</strong>
    </summary>

* [Data Types Overview](data-types)
* [Boolean](data-type-boolean)
* [DateTime](data-type-datetime)
* [Empty (groups/titles)](data-type-empty)
* [Entity (relationships)](data-type-entity)
* [Hyperlink (links/files)](data-type-hyperlink)
* [Number](data-type-number)
* [String (text)](data-type-string)

</details>



[//]: # "Field Input Types"
<details>
    <summary>
        <strong>Field Input Types</strong>
    </summary>

* [Input Fields Overview](ui-fields)
* [All (config for all types)](ui-field-all)
* [Boolean (overview)](ui-field-boolean)
  * [boolean-default](ui-field-boolean-default)
* [DateTime (overview)](ui-field-datetime)
  * [datetime-default](ui-field-datetime-default)
* [Empty (overview)](ui-field-empty)
  * [empty-default (group/title)](ui-field-empty-default)
* [Entity (overview)](ui-field-entity)
  * [entity (default)](ui-field-entity-default)
  * [entity-query](ui-field-entity-query)
* [Number (overview)](ui-field-number)
  * [number-default](ui-field-number-default)
* [Hyperlink / Files](ui-field-hyperlink)
  * [hyperlink-default](ui-field-hyperlink-default)
  * [hyperlink-library](ui-field-hyperlink-library)
* [String (overview)](ui-field-string)
  * [string-default](ui-field-string-default)
  * [string-dropdown](ui-field-string-dropdown)
  * [string-dropdown-query](ui-field-string-dropdown-query)
  * [string-font-icon-picker](ui-field-string-font-icon-picker)
  * [string-url-path](ui-field-string-url-path)
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

* [JavaScript API Overview](javascript-overview)
* [$2sxc Controller](javascript-$2sxc)
* [$2sxc.cms](javascript-$2sxc.cms)
* [sxc instance](javascript-sxc-controller)
* [sxc webapi](javascript-sxc-webapi)
* [manage controller (edit-api)](javascript-manage-controller)
* [toolbars, buttons, actions](javascript-toolbars-and-buttons)
  * [button object](html-js-button)
  * [commands](html-js-commands)
  * [toolbar-settings](html-js-toolbar-settings)
* [$quicke quick-edit controller](html-js-$quicke)

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

* [WebAPI and REST endpoints](webapi)
* [Create your own WebAPI (.net)](dotnet-webapi)
</details>


[//]: # "Templating"
<details>
    <summary>
        <strong>Templating</strong>
    </summary>

* [Asset Optimization (css, js, etc.)](template-assets)

</details>

