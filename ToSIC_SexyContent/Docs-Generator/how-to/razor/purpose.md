---
uid: HowTo.Razor.Purpose
---
# Property _Purpose_ / _InstancePurpose_ on a Razor page
## Purpose / Description
To let your code know, if it's running to produce HTML or if it's running to fill the search-index.

## How to use
in most cases you'll use this within the [CustomizeData](xref:HowTo.Razor.CustomizeData) event, in case you want to provide different data to the template than to the search. 

In most cases you'll also want to override [CustomizeSearch](xref:HowTo.Razor.CustomizeSearch).  

Note that it's of the type `Purposes` which is an enum containing
* WebView,
* IndexingForSearch,
* PublishData

```c#
@using ToSic.Sxc.Blocks;
@if(Purpose == Purposes.IndexingForSearch){
    // code which should only run when indexing
}

```

## Not run code, which can't run while indexing

Sometimes you also have code which requires a user to be visiting a page - like a permission check. Since the indexer doesn't have an HTTP session or a user, this will fail. So you could do something like:

```c#
@using ToSic.Sxc.Blocks;
@if(Purpose != Purposes.IndexingForSearch){
    // code which should only run when really viewing 
    // like something if(userIsLoggedIn) { ... }
}

```

## Demo App or further links
* [Docs on 2sxc.org](http://2sxc.org/en/Docs-Manuals/Feature/feature/2687)

## History

* 2sxc 10.20 - changed to `Purpose` from `InstancePurpose` - old code still works
