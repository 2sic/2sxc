---
uid: HowTo.WebApis
---
# Http WebAPI and REST API
_note that this documentation is for 2sxc 8.10+. Previous versions aready support most of these features with a different URL which still works, but those URLs are deprecated for the future._



The WebApi is a set of http APIs which let you access content-items, lists of items, queries or even custom c# APIs. You can find some jQuery examples on the [$2sxc-WebApi page](xref:Specs.Js.$2sxc).

For example, assume you have the [blog-app](xref:App.Blog) installed and your JS would request a JSON from this endpoint (logged in as host, so security is not an issue):

`[root-path]/app/auto/content/BlogPost`

...then your JS would receive a JSON with all BlogPost items. More examples:

1. Reading this: `[root-path]/app/auto/content/BlogPost/1050`  
...would give you exactly one BlogPost item (with the id 1050).
1. Doing an http POST to this `[root-path]/app/auto/content/BlogPost/1050`  
... with a POST body of `{ "Title": "changed title"}`  
...would let you update the item 1050.
1. This kind of a call `[root-path]/app/auto/query/BlogPostsByAuthor?Author=Daniel%20Mettler`  
...would run the pre-defined query and return the Blog Posts of this author

## Understanding URLs for In-Context vs. External Use
In most cases your JS will be part of your App, so it will be running inside a 2sxc-template. In this case, you can rely on auto-detection of the app, like

`.../app/auto/content/...`

There are also cases where you want to access the app REST or WebApi from outside of the app itself. This can be from the skin (for example, to highlight all words to which your glossary-app has definitions), from another app, or from another website. In this case, auto-app detection won't work, so you'll have to specify the app-name (folder-name) in the request-path, like this:

`.../app/Glossary/content/...`

This applies to all endpoints like `content`, `query`, `api`.

## Available Endpoints in 2sxc 8.10+

### Query
The Query endpoint can be accessed on

* `.../app/auto/query/[your-query-name]` when you're accessing a query of the current App (from a dnn-page with this module), as then 2sxc uses auto-detect
* `.../app/[app-folder]/query/[your-query-name]` using this endpoint from external (other module, other page, other website) as then auto-detect can't work. 

Note that Query endpoints only support the http-verb GET.

### C# WebAPI Endpoints
[C# / .net WebApis](xref:HowTo.WebApi) are custom WebAPIs of an app. You can find a good example on the [Mobius Forms App](https://2sxc.org/en/apps/app/mobius-forms). Calling these is as follows:

* `.../app/auto/api/[YourName]` when accessing a WebApi of the current app (from a dnn-page with this module), as then 2sxc uses auto-detect
* `.../app/[app-folder]/api/[YourName]` when using this endpoint from external, as auto-detect can't work then.

#### New in 9.35: Polymorphism / Multi-Edition Controllers
In 2sxc 9.35 we're introducing an experimental feature to publish the **same** api controller in multiple editions, to enable open-heart-surgery. It's called [Polymorph](xref:Specs.Cms.Polymorphism), and if you use it to place controllers in a `subfolder/api` you'll access it as follows:

* `.../app/auto/[edition]/api/[YourName]` when accessing from a 2sxc-module directly with auto-detect
* `.../app/[app-folder]/[edition]/api/[YourName]` when auto-detect can't work (external access).

Read more about [Polymorph](xref:Specs.Cms.Polymorphism) and [C# WebApi](xref:HowTo.WebApi).

#### WebAPI Security / Permissions
Your C# code determines what security is applied, and what http verbs are supported.

### Content Items Services
The content-items services support all common REST verbs

* `.../app/auto/content/[YourContentType]` - GET (retrieve list)
* `.../app/auto/content/[your-content-type]/[item-id]` - GET (get one), POST (create/update), DELETE (delete)
* `.../app/auto/content/[your-content-type]/[item-guid]` - GET (get one), POST (create/update), DELETE (delete)

Note that when using this endpoint from external, you'll have to replace `auto` with the app name as it is used in the folder of the app (not the nice, translatable/visible app-name).

#### Content Security / Permissions
Security / Permissions are configured at the content-type level, in the App-Management UI.


## How it works
todo

## Notes and Clarifications
1. Basically every call to these paths will simply return what you expect, or perform what you want
2. Note that for most actions some security rules will apply, for example:
    1. reading data requires read-permissions on that content-type
    2. writing data requires write-permissions or a content-type or on a specific item (owner-permissions)
    3. querying a query requires read-permissions on that query
    4. querying a web-api requires the web-api to have its own permission check
3. In DNN 9, the root-path for api changed!
    1. DNN 7 & 8: `[portal-root with language]/desktopmodules/2sxc/api/...`
    2. DNN 9: `[portal-root with language]/api/2sxc/...`
    3. This is handled automatically if you are using $2sxc or 2sxc4ng - but you need to be aware of this if you're using your own JS or accessing from external


## Read also

* [DotNet WebApi](xref:HowTo.WebApi)
* [Concepts: Polymorphisms](xref:Specs.Cms.Polymorphism)

## Demo App and further links

You should find some code examples in this demo App
* [REST and WebApi Tutorial](http://2sxc.org/en/apps/app/tutorial-javascript-rest-api-using-jquery-and-angularjs)


## History

1. Introduced in 2sxc ??.??
2. Enhanced with Polymorph Editions in 2sxc 9.35 (allowing subfolder/api)
