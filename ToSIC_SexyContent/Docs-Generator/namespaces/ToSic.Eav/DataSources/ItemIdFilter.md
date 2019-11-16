# Data Source: ItemIdFilter / EntityIdFilter

## Purpose / Description
The **ItemIdFilter** [DataSource](xref:ToSic.Eav.DataSources.IDataSource) is part of the [Standard EAV Data Sources](xref:Specs.DataSources.ListAll). It will only return the items with the id specified. 

_Warning: You should always use it in combination with a [Content-Type filter](DotNet-DataSource-ContentTypeFilter), as you want to be sure nobody can just crawl any entity you have in your system!_

## How to use with the Visual Query
When using the [Visual Query](xref:Temp.VisualQuery) you can just drag it into your query. This is what it usually looks like:

<img src="/assets/data-sources/item-id-basic.png" width="100%">

You can also use multiple values:

<img src="/assets/data-sources/item-id-multiple.png" width="100%">

Or URL-parameters:

<img src="/assets/data-sources/item-id-url.png" width="100%">


## Programming With The ItemIdFilter DataSource
_Note: We recommend to use the Visual Query where possible, as it's easier to understand and is consistant for C# and JavaScript. It's also better because it separates data-retrieval from visualization._

Because of limited resources we don't have code-examples. It works, but you'll probably never need it so we don't document it. 

FQN: `ToSic.Eav.DataSources.EntitydFilter`

## Read also

* [Source code of the ItemIdFilter](//github.com/2sic/eav-server/blob/master/ToSic.Eav.DataSources/EntityIdFilter.cs)
* [List of all EAV Data Sources](xref:Specs.DataSources.ListAll)

## Demo App and further links
You should find some examples in this demo App
* [Demo App with examples for most DataSources](https://github.com/2sic/app-demo-visual-query/releases/latest)


## History
[//]: # "If possible, tell when it was added or modified strongly"

1. Introduced in EAV 4.x, 2sxc ?




