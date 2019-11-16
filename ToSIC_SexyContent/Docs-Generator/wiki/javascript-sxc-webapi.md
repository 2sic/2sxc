# JS: The WebApi Helpers on the Sxc Controller

## Purpose / Description
The WebApi object on the module-specific `sxc`-controller is for AJAX calls. It helps you in these advanced cases:  

1. to read/write content-items using REST
1. to access your apps WebApi controllers

## How to use
First you must ensure that you have the [`$2sxc` manager](JavaScript-$2sxc) on your page, which will get you a module-specific `sxc` controller. Read about the [$2sxc manager](JavaScript-$2sxc) here. 

Here's a simple example (assuming you have the $2sxc manager):

```HTML
<a onclick="$2sxc(this).webApi.get('app/auto/content/Category').then(handleResult);">
    get all Categories 
</a>
```

The code above shows

1. how the sxc-object is retrieved using the `$2sxc(...)` manager, based on the current context `this`
2. how all items of type "Category" are requested
3. how the result (promise) is passed on to `handleResults` for updating the view etc.

Here's another quick example, calling a C# web-api endpoint: 

```JavaScript
var sxc = $2sxc(27);
sxc.webApi.post("Form/ProcessForm")
    .success(function(result) {
        // ....
    });
```

## How to Get the Current Context's `sxc` Controller
Before you continue, make sure you know how to resolve/get your `sxc`-controller, as it is unique for each DNN-Module. This is because each action needs to know which module it belongs to. Read about the 3 ways to get this in the [$2sxc Manager docs](JavaScript-$2sxc). Here you'll also find out more about the [sxc-controller](JavaScript-Sxc-Controller).


## Working with REST / HTTP Async Stuff
Short note: the WebAPIs work like classic javascript promises, supporting `.then(...)`, `.error(...)` etc.

The $2sxc(...).webApi has 4 commands
* `.webApi.get(url, ...)` 
* `.webApi.post(url, ...)`
* `.webApi.delete(url, ...)`
* `.webApi.put(url, ...)`

Each of these has the following parameters

1. `url` or `settings` _required_ string|object: a Url for the end-point OR a `{ ... }` settings object  
2. `params` _optional_ object: the url params like `{ id: 27, name: "hello" }`
3. `data` _optional_ object: the data - in case of post / put, like `{ ... }`
4. `preventAutoFail` _optional_ bool: if true, won't automatically show a default message on error, so that you can handle errors differently

Quick examples:

```JavaScript
var sxc = $2sxc(27);
sxc.webApi.post("Form/ProcessForm", {}, data, true)
    .success(function() {
        // ....
    })
    .error(function() {
        // ...
    });
```

This will call the C# WebApi controller `FormController` in the `api` folder and go for its `ProcessForm` command. It will use no url-params, but put a `data` object in the body (as json), and will do error-handling itself. 

_Till we find time to document more, please consult the [source][source]_

## Using App-Queries with $2sxc  (TODO)
Todo: must document more about this

In short: 

1. use `$2sxc(...).webApi.get(...)` or `$2sxc(...).webApi.post(...)` etc.
2. for the path-parameter, use a path starting with `app/auto/query/[queryname]` - 2sxc will take care of all the path resolutions if the path starts with `app/auto/query/` 

_Till we find time to document more, please consult the [source][source]_


## Working with Custom C# App WebAPIs (TODO)
TODO - must document more about what the web-apis are (link to more documentation)

In short: 

1. use `$2sxc(...).webApi.get(...)` or `$2sxc(...).webApi.post(...)` etc.
2. for the path-parameter, use a path starting with `app/auto/api/[controller]/[action]` - 2sxc will take care of all the path resolutions if the path starts with `app/auto/api/` 

You can read more about the [C# WebApi Server Side](DotNet-WebApi)

Short note: the WebAPIs work like classic javascript promises. So not like the data.on(event) implementation which is a bit special, this is very standard jQuery promise. 

_Till we find time to document more, please consult the [source][source]_








## Technical Features Explained
todo





## Additional Properties of a Module Controller

TODO - must document more about properties like

1. isLoaded, lastRefresh, etc.
2. that these are all internal-use and not guaranteed to stay stable in future versions

_Till we find time to document more, please consult the [source][source]_




## Demo App and further links
[//]: # "Apps which provide sample code using this"

You should find some code examples in this demo App
* [TimeLineJS][TimeLineJS]
* all the JS-apps including AngularJS in the [app-catalog][catalog]

More links: [Description of the feature on 2sxc docs](http://2sxc.org/en/Docs-Manuals/Feature/feature/2683)

## History
[//]: # "If possible, tell when it was added or modified strongly"

1. Introduced in 2sxc 04.00

[//]: # "This is a comment - for those who have never seen this"
[//]: # "The following lines are a list of links used in this page, referenced from above"
[F12]: http://2sxc.org/en/blog/post/debugging-javascript-errors-with-a-modern-browser-and-f12-(200)
[asset-optimization]:Template-Assets
[content-blocks]: http://2sxc.org/en/blog/post/designing-articles-with-inner-content-blocks-new-in-8-4-like-modules-inside-modules
[TimeLineJS]: http://2sxc.org/en/apps/app/timelinejs-app-for-dnn-dotnetnuke
[catalog]: http://2sxc.org/en/apps
[source]: https://github.com/2sic/2sxc-ui/blob/master/src/js-api/2sxc.api/2sxc.api.js
[manage]: JavaScript-Manage-Controller