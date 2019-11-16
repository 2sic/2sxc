# Data Source: ContentType EntityType Filter
_Note: this data source used to be called EntityTypeFilter, we renamed it in 2sxc 9.8 for consistency_

## Purpose / Description
The **ContentTypeFilter** [DataSource](xref:ToSic.Eav.DataSources.IDataSource) is part of the [Standard EAV Data Sources][eavds]. It will only let items pass through, which are of a specific type. 

## How to use with the Visual Query
When using the [Visual Query](xref:Temp.VisualQuery) you can just drag it into your query. In the settings you'll specify the type-name. The resulting query will usually be a bit like this:

<img src="/assets/data-sources/content-type-filter-two-examples.png" width="100%">

The above example shows:

1. a two content-type filters, each filtering a different type



## Programming With The ContentTypeFilter DataSource
_Note: We recommend to use the Visual Query where possible, as it's easier to understand and is consistant for C# and JavaScript. It's also better because it separates data-retrieval from visualization._

Because of limited resources we don't have many code-examples. Here's a simple one: 

```c#
// A source which can filter by Content-Type (EntityType)
var allAuthors = CreateSource<EntityTypeFilter>();
allAuthors.TypeName = "Author";

```

FQN: `ToSic.Eav.DataSources.EntityTypeFilter`

## Read also

* [Source code of the ContentTypeFilter (previously EntityTypeFilter)](//github.com/2sic/eav-server/blob/master/ToSic.Eav.DataSources/EntityTypeFilter.cs)
* [List of all EAV Data Sources][eavds]

## Demo App and further links
You should find some examples in this demo App
* [Demo App with examples for most DataSources](https://github.com/2sic/app-demo-visual-query/releases/latest)


## History
[//]: # "If possible, tell when it was added or modified strongly"

1. Introduced in EAV 3.x, 2sxc ?

[//]: # "The following lines are a list of links used in this page, referenced from above"

[eavds]: DotNet-DataSources-All
