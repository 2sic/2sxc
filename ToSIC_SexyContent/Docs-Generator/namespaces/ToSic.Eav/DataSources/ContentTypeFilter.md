---
uid: Todo.ToSic.Eav.DataSources.ContentTypeFilter
---
# Data Source: ContentType EntityType Filter
_Note: this data source used to be called EntityTypeFilter, we renamed it in 2sxc 9.8 for consistency_

## Purpose / Description
The **ContentTypeFilter** [DataSource](xref:Specs.DataSources.DataSource) is part of the [Standard EAV Data Sources](xref:Specs.DataSources.ListAll). It will only let items pass through, which are of a specific type. 

## How to use with the Visual Query
When using the [Visual Query](xref:Temp.VisualQuery) you can just drag it into your query. In the settings you'll specify the type-name. The resulting query will usually be a bit like this:

<img src="/assets/data-sources/content-type-filter-two-examples.png" width="100%">

The above example shows:

1. a two content-type filters, each filtering a different type



## Programming With The ContentTypeFilter DataSource
[!include["simpler-with-vqd"](shared-use-vqd.md)]

```c#
// A source which can filter by Content-Type (EntityType)
var allAuthors = CreateSource<EntityTypeFilter>();
allAuthors.TypeName = "Author";

```

[!include["Read-Also-Section"](shared-read-also.md)]

[!include["Demo-App-Intro"](shared-demo-app.md)]

[!include["Heading-History"](shared-history.md)]

1. Introduced in EAV 3.x, 2sxc ?

[!include["Start-APIs"](shared-api-start.md)]