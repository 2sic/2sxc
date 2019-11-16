# @Data in Razor Template
## What is _Data_

A 2sxc instance is a running _2sxc-engine_ which is about to execute some code (Razor, Token or a web service) together with some content-items which should be used in this case. These content-items are provided to the code in an object called `Data`.

2sxc will decide what items to deliver, based on the situation.

1. In most cases, a DNN-Module will show a 2sxc-template, and the content-editor will manually add items to it using the normal web-UI. In this case, these items will be tied to this use-case and 2sxc will deliver them in the `Data` object.

1. In other cases, the template (or view) will be configured to get data from a query configered elsewhere. In this case, all data retrieved in the query will be provided in the same `Data` object.

1. There are also cases, where a template needs both queried data (like _all categories_) as well as content-items added manually. There are multiple easy ways of doing this **todo - document later**

1. Note: if you want _all data_ of a type, like "Get me all Tag-items in the system" you need to use [App-Data](Razor-App).


## Most common way to use the Data object
In all Razor-templates and also the web-api files, the `Data` object is already created and ready to access. Here's a code example:

```cs
<div class="app-blog">
    @foreach(var post in AsDynamic(Data["Default"]))
    {
        // do something with the @post here
    }
</div>
```

* the foreach will loop through all items of the data in the _Stream_ **Default** - read more about streams below
* the var post is the inner variable containing the current item
* the AsDynamic will ensure that the items we're working with are easy to code with, because they are [Dynamic Entities](DotNet-DynamicEntity)


## The Streams in a Data Object
The data object can have multiple _Streams_, each containing a list of items. In most cases you'll just have the _Default_ stream, which you can access using `Data["Default"]`. Read more about streams in the [Stream docs](DotNet-DataStream)

But you may also have additional streams, depending on what has been configured in the data-preparation stuff (usually a visual query). For example, if you are working on a view showing one product and a menu with all possible categories, then your query may have these streams:

* Book (the current book, this stream has 1 item with the ID matching the ID in the URL)
* Categories (all categories in the system, sorted A-Z)

The code would then look a bit like this:

```cs
var book = Data["Book"].FirstOrDefault();               // returns an IEntity Object
var cats = Data["Categories"];                          // returns an IEnumerable of iEntity objects
```

Or in most cases, because of the simpler syntax later on:

```cs
var book = AsDynamic(Data["Book"].FirstOrDefault());    // returns one dynamic entity
var cats = AsDynamic(Data["Categories"]);               // returns an IEnumerable of dynamic entities
```  


## Changing what is provided by Data
The main configuration of the template will determine, what data is initially provided to the template.
Afterwards, other mechanisms can override / change this.

1. Initial Sources
    1. Default type/list configuration - this can determine that 0, 1 or many items (of a certain type) are delivered  
    if one or many are delivered, then the selection will be based on what the editor added to the module instance.
    1. Visual Query - if the template has a query attached, this query will deliver the data
2. Modifications to the source for the template
    1. The event [CustomizeData][CustomizeData] can optionally completely reconfigure what Data contains in your code.


## Data Object API
The `Data` object itself is actually a normal [EAV IDataSource][code-eav-datasource]. So if you need to know more about the internals, read it up there.

All other properties of the Data object are very special use only, so you probably shouldn't bother about them - which is why they are not documented here.



[//]: # "Links referenced in this page"
[CustomizeData]:Razor-SexyContentWebPage.CustomizeData
[code-eav-datasource]:https://github.com/2sic/eav-server/blob/master/ToSic.Eav.DataSources/IDataSource.cs