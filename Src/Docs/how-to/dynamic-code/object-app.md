---
uid: HowTo.DynamicCode.App
---
# App / @App object in Razor

The `App` object gives you full access to everything you need to know about the current App, including Path-info, access to all Data this App has, access to Settings and language Resources and more.

## How to use

Here's are two simple examples taken from the [Blog App](xref:App.Blog):

```html
<link rel="stylesheet" href="@App.Path/assets/style.css"  data-enableoptimizations="true"/>
<script type="text/javascript" src="@App.Path/assets/scripts.js" data-enableoptimizations="true"></script>
@foreach(var tag in AsDynamic(App.Data["Tag"])) {
    <li class='@("app-blog-tag" + tag.ManualWeight)'><a href="@App.Settings.DetailsPage/tag/@tag.Tag" title="@tag.Name">@tag.Name</a></li>
}
```

The `<link...` and `<script...` use the app-path to ensure that the file is correctly loaded, no matter what portal or app-name is currently valid. Here you can discover more about the [optimizations](xref:HowTo.Output.Assets).

The loop iterates through all tags with the `@foreach(var tag in AsDynamic(App.Data["Tag"]))`, creates `<li>` items and links these to a page defined in the `App.Settings`.  

## How it works
Whenever a 2sxc-instance is created to render a page or to deliver JSON data, the `App` object is created and prepared to deliver everything you need. It's very performant, because it doesn't actually get any data or run any queries unless these are accessed. 

## App Properties
The app-object uses the `IApp` interface ([see code](xref:ToSic.Sxc.Apps.IApp)) has the following simple properties:

1. `AppId` number, current App id
2. `AppGuid` guid, internal use global id
3. `Configuration` [DynamicEntity](xref:HowTo.DynamicCode.Entity), contains the [configuration content-item](xref:Feat.AppConfig)
4. `Data` IAppData, to access all App-data (see below)
5. `Folder` string, storage folder name in portal/#/2sxc/...
6. `Hidden` bool, info if the app cannot be selected in the UIs
7. `Name` string, the app name
8. `Path` string, the path as used in URLs in  html
9. `PhysicalPath` string, the path as used on the server C:\...
10. `Query["QueryName"]` dictionary of queries (see below) 
11. `Resources` [DynamicEntity](xref:HowTo.DynamicCode.Entity), all the multi-language labels etc. (see below)
12. `Settings` [DynamicEntity](xref:HowTo.DynamicCode.Entity), all the app-settings (see below)
13. `ZoneId` number, current Zone ID (similar to PortalId)


## Using App Data (App.Data)
The App object gives you immediate acccess to all data in the app, through the `Data` property. Basically you can use it as follows:


### Get All Data Items of a Content Type
`App.Data["ContentTypeName"]` will give you a [stream](xref:ToSic.Eav.DataSources.IDataStream) of all entities of that type. In most cases you'll use an `AsDynamic(...)` to use it efficiently in loops etc. because most of the razor templating will prefer a [DynamicEntity](xref:HowTo.DynamicCode.Entity) to a pure IEntity-object. Here's an example:

```cs
@foreach(var post in AsDynamic(App.Data["BlogPost"]))
{
    @RenderPage("_list-item.cshtml", new { Post = post })
}
```

_note_: this will give you all items, but you'll have to sort it using LINQ or other mechanisms. If you're not familiar with that, you're better of using `App.Query[...]` (see below). 


### Edit App Data Content-Items
In addition to giving access to all entities in this app, you can also create, edit and delete items using the `App.Data` object. The commands provided are:

1. `App.Data.Create(contentTypeName, values, userName)`
1. `App.Data.Update(entityId, values, userName)`
1. `App.Data.Delete(entityId, userName)`

You can read more about this in the [App Data API Feature](xref:Feat.AppDataApi)


## Using App Queries (App.Query)
The queries you create in the app-configuration dialogs can do many things like filter certain items, order them and more. You will often just connect them to a template and visualize the result, by you can also use it in your code. Here's how:

```html
@foreach(var tag in AsDynamic(App.Query["SortedTags"]["Default"])) {
    <li class='@("app-blog-tag" + tag.ManualWeight)'><a href='@Link.To("tag= " tag.Tag)' title="@tag.Name">@tag.Name</a></li>
}
```

Technically the `App.Query` is a `IDictionary<string, IDataSource>`, meaning that it's a dictionary using string identifiers (names), returning an [`IDataSource`](xref:Specs.DataSources.DataSource) object. 

It's important to realize that a [DataSource](xref:Specs.DataSources.DataSource) can deliver multiple [streams of data](xref:ToSic.Eav.DataSources.IDataStream) - a bit like delivering multiple tables. Each stream has a name, and you must specify which stream you want to work with. In the above example, we're using the `Default` stream as defined with `App.Query["SortedTags"]["Default"]`.

## Note about Unpublished / Draft Content-Items
In case you're not aware of the draft/unpublished features in 2sxc, we want to note that each item can be live/draft, and each item could have a corresponding counterpart. So a draft-item _could_ have a live-item (but doesn't have to), and a live-item _could_ have a draft item.

This is important, because the admin/editor will see all the draft items, while the end-user will only see the live ones. So the exact items shown and the item-count can differ if you are logged in. 


## App Settings and Resources
In the App dialogs you can manage `Settings` and `Resources`. Basically both are a content-item with the fields you specify, the only difference is the _purpose_ they have.

* You should put button-labels, standard-texts, decorative images etc. into `Resources` and these will often change from language to language.
* You should put settings like "what page is xxx on" or "the primary color is #53aaff" into `Settings`

You would normally use it like this:

```html

<h1 class='@App.Settings.HeadingsDecorators'>
    @Content.Title
</h1>
<div>
    @Content.Body
</div>
<a href="@App.Settings.DisclaimerPage">
    @App.Resources.Disclaimer
</a>
```

As you can see, the `HeadingsDecorators` or `DisclaimerPage` are best placed in the `Settings`, while the label of the `Disclaimer` link are best handled as a multi-language `Resource`.


## Read also
* If you need to get an App object for other apps, read [External App Use](xref:HowTo.External)
* If you want to use the App object from non 2sxc-code, like other MVC pages, check out [External App Use](xref:HowTo.External)


## Demo App and further links
You should find some code examples in this demo App
* [Blog App](xref:App.Blog) showing many such features

More links:
* [App Data Create/Update/Delete](xref:Feat.AppDataApi)


## History
1. Introduced in 2sxc 05.05
2. Stable since 2sxc 06.00
3. Data-API was introduced in 2sxc 06.05
