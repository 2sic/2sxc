---
uid: ToSic.Eav.DataSources.ValueSort
---

The **ValueSort** [DataSource](xref:Specs.DataSources.DataSource) is part of the [Standard EAV Data Sources](xref:Specs.DataSources.ListAll). It will reorder items passing through A-Z or Z-A based on a value of each item. 

## How to use with the Visual Query
When using the [Visual Query](xref:ToSic.Eav.DataSources.Queries.VisualQueryAttribute) you can just drag it into your query. This is what it usually looks like:

<img src="/assets/data-sources/value-sort-standard.png" width="100%">

The above example shows the same items being sorted in two different ways and delivered to the target. 

## Example Using Multi-Sort
You can also sort multiple fields, so "first sort by xyz, then by xyz" and use different sort-orders.:

<img src="/assets/data-sources/value-sort-multiple.png" width="100%">

## Example Using URL Parameters
...and of course you can also use url parameters to specify field-names or sort order:
<img src="/assets/data-sources/value-sort-parameterized.png" width="100%">

## Sorting Direction
For sorting direction you can use either words or numbers
* asc/desc
* 1/0

## Programming With The ValueSort DataSource
[!include["simpler-with-vqd"](shared-use-vqd.md)]

```c#
// A source which can filter by Content-Type (EntityType)
var allAuthors = CreateSource<EntityTypeFilter>();
allAuthors.TypeName = "Author";

// Sort by FullName
var sortedAuthors = CreateSource<ValueSort>(allAuthors);
sortedAuthors.Attributes = "FullName";

```

[!include["Read-Also-Section"](shared-read-also.md)]

[!include["Demo-App-Intro"](shared-demo-app.md)]

[!include["Heading-History"](shared-history.md)]

1. Introduced in EAV 3.x, 2sxc ?


[!include["Start-APIs"](shared-api-start.md)]