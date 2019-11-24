# JS: The Module-Instance Sxc Controller

## Purpose / Description
The module-specific `sxc`-controller is the core JavaScript object helping you in these advanced cases:  

1. if you want full control over the edit-experience with custom buttons etc. 
1. when you want to use view-data as an asyc-JS call
1. if you wish to work with WebAPI REST calls - of your own App-WebApi, 2sxc-WebApi or DNN-WebApi

## How to use
First you must ensure that you have the [`$2sxc` manager](JavaScript-$2sxc) on your page, which will get you a module-specific `sxc` controller. Read about the [$2sxc manager](JavaScript-$2sxc) here. 

Here's a simple example (assuming you have the $2sxc manager):

```HTML
<a onclick="var sxc = $2sxc(this); alert(sxc.isEditMode())">
    click me 
</a>
```

The code above shows

1. how the sxc-object is retrieved using the `$2sxc(...)` manager, based on the current context `this`
2. how to ask if we're in edit-mode

Here's another quick example, calling a C# web-api endpoint: 

```JavaScript
var sxc = $2sxc(27);
sxc.webApi.post("Form/ProcessForm")
    .success(function(result) {
        // ....
    });
```

## How to Get the Current Context's `sxc` Controller
Before you continue, make sure you know how to resolve/get your `sxc`-controller, as it is unique for each DNN-Module. This is because each action needs to know which module it belongs to. Read about the 3 ways to get this in the [$2sxc Manager docs](JavaScript-$2sxc).


## The API of an Initialized Module `sxc` Controller

* `.data` _object_: todo...
* `.id` _int_: the id of this sxc-controller, usually the module-id
* ~~`.cbid` _int_: internal use ~~
* `.isEditMode()` _bool_: looks up if this module is editable - this can vary on each module
* `.webApi...` _object_: helpers to call server side WebApis and REST resources
* ~~`.showDetailedHttpError(...)` _void_: internal use ~~
* [`.manage...`][manage] _object_ - to create your own buttons, toolbars or run CMS commands


## Custom Toolbars, Buttons and Commands
Read the [manage][manage] documentation to discover more about the `getToolbar(...)`, `getButton(...)` or `run(...)` methods which let you customize the toolbars, buttons and use other UI elements like links to run commands like "show layout selector". 





## Working with the JSON Data of the Current Module (TODO)

1. The data object and how-to use, probably using the example of [TimeLineJS][TimeLineJS]
2. Using the load-event, reload, and the "one" event
3. The structure of the returned data

Short note: The data is loaded using jQuery and when it's returned and processed, a callback will be executed. This is different from modern promise-implementations but easy to use anyhow. We haven't changed this, because it's trivial and we don't want to break existing code. 

_Till we find time to document more, please consult the [source][source]_

## Working with REST Api to Read/Write content-items
Read about it in the [Sxc WebApi](JavaScript-Sxc-WebApi) page.


## Using App-Queries with $2sxc
Read about it in the [Sxc WebApi](JavaScript-Sxc-WebApi) page.


## Working with Custom C# App WebAPIs
Read about it in the [Sxc WebApi](JavaScript-Sxc-WebApi) page.




## Technical Features Explained
TODO





## Additional Properties of a Module Controller

TODO - must document more about properties like

1. isLoaded, lastRefresh, etc.
2. that these are all internal-use and not guaranteed to stay stable in future versions

_Till we find time to document more, please consult the [source][source]_




## Demo App and further links

You should find some code examples in this demo App
* [TimeLineJS][TimeLineJS]
* all the JS-apps including AngularJS in the [app-catalog][catalog]

More links: [Description of the feature on 2sxc docs](http://2sxc.org/en/Docs-Manuals/Feature/feature/2683)

## History

1. Introduced in 2sxc 04.00

[F12]: http://2sxc.org/en/blog/post/debugging-javascript-errors-with-a-modern-browser-and-f12-(200)
[asset-optimization]:Template-Assets
[content-blocks]: http://2sxc.org/en/blog/post/designing-articles-with-inner-content-blocks-new-in-8-4-like-modules-inside-modules
[TimeLineJS]: http://2sxc.org/en/apps/app/timelinejs-app-for-dnn-dotnetnuke
[catalog]: http://2sxc.org/en/apps
[source]: https://github.com/2sic/2sxc-ui/blob/master/src/js-api/2sxc.api/2sxc.api.js
[manage]: JavaScript-Manage-Controller