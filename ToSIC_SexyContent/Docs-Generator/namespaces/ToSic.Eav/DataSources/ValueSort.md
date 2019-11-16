# Data Source: ValueSort

## Purpose / Description
The **ValueSort** [DataSource](xref:ToSic.Eav.DataSources.IDataSource) is part of the [Standard EAV Data Sources](xref:Specs.DataSources.ListAll). It will reorder items passing through A-Z or Z-A based on a value of each item. 

## How to use with the Visual Query
When using the [Visual Query](xref:Temp.VisualQuery) you can just drag it into your query. This is what it usually looks like:

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
_Note: We recommend to use the Visual Query where possible, as it's easier to understand and is consistant for C# and JavaScript. It's also better because it separates data-retrieval from visualization._

Because of limited resources we don't have many code-examples: 

```c#
// A source which can filter by Content-Type (EntityType)
var allAuthors = CreateSource<EntityTypeFilter>();
allAuthors.TypeName = "Author";

// Sort by FullName
var sortedAuthors = CreateSource<ValueSort>(allAuthors);
sortedAuthors.Attributes = "FullName";

```
FQN: `ToSic.Eav.DataSources.ValueSort`

## Read also

* [Source code of the ValueSort](//github.com/2sic/eav-server/blob/master/ToSic.Eav.DataSources/ValueSort.cs)
* [List of all EAV Data Sources](xref:Specs.DataSources.ListAll)

## Demo App and further links
You should find some examples in this demo App
* [Demo App with examples for most DataSources](https://github.com/2sic/app-demo-visual-query/releases/latest)


## History
[//]: # "If possible, tell when it was added or modified strongly"

1. Introduced in EAV 3.x, 2sxc ?




