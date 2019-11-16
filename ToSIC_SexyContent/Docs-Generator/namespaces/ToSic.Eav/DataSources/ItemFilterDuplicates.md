# Data Source: ItemFilterDuplicates

## Purpose / Description
The **ItemFilterDuplicates** [DataSource](xref:ToSic.Eav.DataSources.IDataSource) is part of the [Standard EAV Data Sources](xref:Specs.DataSources.ListAll). It will provide two streams, one with all the items (but without the duplicates) and one with all the duplicates, in case you need these. 

## How to use with the Visual Query
When using the [Visual Query](xref:Temp.VisualQuery) you can just drag it into your query. This is what it usually looks like:

<img src="/assets/data-sources/item-filter-duplicates-basic.png" width="100%">

The above example shows two filters finding items and delivering them on the same stream. 

Because 1 item would have been delivered 2x (2sic is in Switzerland), the default-stream now only contains each item once, and the duplicates can also be retrieved from the _Duplicates_ stream if needed.

## Example Using StreamMerge
A common scenario will also combine this using the [StreamMerge](DotNet-DataSource-StreamMerge). It's demonstrated there. 



## No Settings
The ItemFilterDuplicates DataSource has no settings to configure.

Note that it will automatically merge the streams in A-Z order of the In-Stream names. 

## Programming With The ItemFilterDuplicates DataSource
_Note: We recommend to use the Visual Query where possible, as it's easier to understand and is consistant for C# and JavaScript. It's also better because it separates data-retrieval from visualization._

Because of limited resources we don't have many code-examples: 

FQN: `ToSic.Eav.DataSources.ItemFilterDuplicates`

## Read also

* [Source code of the ItemFilterDuplicates](//github.com/2sic/eav-server/blob/master/ToSic.Eav.DataSources/ItemFilterDuplicates.cs)
* [List of all EAV Data Sources](xref:Specs.DataSources.ListAll)

## Demo App and further links
You should find some examples in this demo App
* [Demo App with examples for most DataSources](https://github.com/2sic/app-demo-visual-query/releases/latest)


## History
[//]: # "If possible, tell when it was added or modified strongly"

1. Introduced in EAV 4.x, 2sxc 9.10




