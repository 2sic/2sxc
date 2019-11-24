---
uid: Specs.AngularJs
---
# AngularJs 1 Overview

## Purpose / Description
2sxc contains a special helper called *2sxc4ng* which takes care of starting your app inside DNN and providing you with toolbars and data from 2sxc.

## How to use

Until we find time to document everything, here's the short version

1. To use 2sxc4ng you must include a JS file in your template, and you must bootstrap your app using this (not using the standard angularjs bootstrapping)  
this is to ensure that multiple Angular apps can run on the same page, and to provide your app with the necessary context so it knows what module it's working on, etc.  
[Here's an introduction to that][bootstrapping].
2. To get your current sxc-controller there are two objects *$2sxc* and *sxc* which you can just include it in your function definition, like  
`module.controller('AppCatalogCtrl', function ($2sxc, sxc, $http, ...) {`
  1. The sxc is the one you want most, it is already set to your current instance, so you can ask it things like `sxc.manage` etc.
  2. The $2sxc is the same as the global [$2sxc](xref:Specs.Js.$2sxc) object you know from jquery. You could of course also do `$2sxc(27).manage` to acces the manage, but that's unnecessarily complicated
3. Additional services provided when bootstrapping with *2sxc4ng* is
  1. `content(typename)` - a service which requests content-data from the current app / context, can also delete / create items etc.
  2. `query(queryname)` - a service which gets data from app-queries
4. Additional directives
  1. `sxcToolbar` - a create-toolbar directive to provide in-app toolbars to edit/manage etc. for `<sxc-toolbar toolbar="...">` tags


Todo: a simple full example right here


## Including All Necessary Files
You need three files + your code
1. `2sxc.min.js` - only necessary, if you intend to work with 2sxc data items & toolbars, must come before _2sxc4ng_
2. Angular - ideally from a CDN
3. `2sxc4ng.min.js` - only necessary, if you intend to work with 2sxc data items & toolbars, must come after _angular_
4. Your code

Example:

```html
<script src="/desktopmodules/tosic_sexycontent/js/2sxc.api.min.js" data-enableoptimizations="100"></script> 
<script src="//ajax.googleapis.com/ajax/libs/angularjs/1.6.1/angular.min.js" data-enableoptimizations="101"></script> 
<script src="/desktopmodules/tosic_sexycontent/js/angularjs/2sxc4ng.min.js" data-enableoptimizations="110"></script> 
<script src="@App.Path/dist/angular-app.min.js" data-enableoptimizations="120"></script> 
```


## Toolbar Directive
Quick example - this requires 2sxc 8.8
```html
// a quick sxcToolbar example
<li ng-repeat="app in apps">
    <sxc-toolbar toolbar='{ "entityId": app.EntityId }' settings='{ "hover": "left", "align": "left" }'></sxc-toolbar>
    ...
</li>
```

## Content Service
This is a quick example of the _content_ service
Todo: you can find some infos till then in the [bootstrapping][bootstrapping] article

Important: you can use the `content` service to 

1. get all of a type
2. get one of a type
3. create one of a type (if permissions have been configured for that type)
4. delete one of a type (permissions...)

quick demo of syntax
```javascript
var cSrv = content("BlogPosts");
var onePromise = cSrv.get(740);
var allPromise = cSrv.get();
var createPromise = cSrv.create({ "Title": "hello", "Body": "great article"});
var deletePromise = cSrv.delete(7740); 
```

## Query Service
This is a quick example of the _query_ service

Todo: you can find some infos till then in the [bootstrapping][bootstrapping] article 

```JavaScript
// this example assumes you added the query service in your constructor
// it also assumes you created a visual-query called "All-blog-items"
var qAll = query("All-blog-items");
qAll.get().then(function (result) {
    ... // some code here
});

// this assumes you want to query the data from the current view
// so the real items assigned to this instance
// or processed inside the view in a PrepareData() method
var qCurrent = query();
qCurrent.get().then(function (result) {
    ... // some code here
});
```

When working with queries that expect parameters, you can pass them in the `get()` call
```
qAll.get({ data: { "sort": "EntityTitle" },  }).then(...)
```

## How it works
[//]: # "Some explanations on the functionality"
todo

## Notes and Clarifications On Bootstrapping
Just fyi: in 2sxc 6.0 till 2sxc 8.8.0 the AngularJS bootstrapping needed to know the module-id. This was done by either

1. providing an attribute like `iid="@Dnn.Module.ModuleId"` or for tokens `iid="[Module:ModuleId]"`
2. providing the mod-id in the app name like `sxc-app="MyApp-@Dnn.Module.ModuleId"`

This is because it needs if for webservice calls. In 2sxc 8.8.1 the bootstrapping will auto-detect the module id, so you don't have to provide it any more. 


## Read also

* [Full explanation of bootstrapping and how/why etc.][bootstrapping] - which tells you why the current code is running so you could change the data added

## Demo App and further links

You should find some code examples in this demo App
* [Various AngularJS based apps](http://2sxc.org/en/Apps/tag/AngularJS)

## History

1. Introduced in 2sxc ??.??
2. sxcToolbar released in 2sxc 8.8.0

[$2sxc]:JavaScript-$2sxc


[bootstrapping]:http://2sxc.org/en/Learn/Simple-AngularJS-in-DNN-with-2sxc4ng
