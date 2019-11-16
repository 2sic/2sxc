# Data Source: StreamMerge

## Purpose / Description
The **StreamMerge** [DataSource](xref:ToSic.Eav.DataSources.IDataSource) is part of the [Standard EAV Data Sources][eavds]. It will create a new stream containing everything which came into this data source.  

## How to use with the Visual Query
When using the [Visual Query](xref:Temp.VisualQuery) you can just drag it into your query. This is what it usually looks like:

<img src="/assets/data-sources/stream-merge-basic.png" width="100%">

The above example shows two filters finding items and delivering them on the same stream. 

## Example Using ItemFilterDuplicates
A common scenario will also combine this using the [ItemFilterDuplicates](DotNet-DataSource-ItemFilterDuplicates), in case various in-streams deliver the same item:

<img src="/assets/data-sources/stream-merge-with-item-filter-duplicates.png" width="100%">


## No Settings
The StreamMerge DataSource has no settings to configure.

Note that it will automatically merge the streams in A-Z order of the In-Stream names. 

## Programming With The StreamMerge DataSource
_Note: We recommend to use the Visual Query where possible, as it's easier to understand and is consistant for C# and JavaScript. It's also better because it separates data-retrieval from visualization._

Because of limited resources we don't have many code-examples: 

FQN: `ToSic.Eav.DataSources.StreamMerge`

## Read also

* [Source code of the StreamMerge](//github.com/2sic/eav-server/blob/master/ToSic.Eav.DataSources/StreamMerge.cs)
* [List of all EAV Data Sources][eavds]

## Demo App and further links
You should find some examples in this demo App
* [Demo App with examples for most DataSources](https://github.com/2sic/app-demo-visual-query/releases/latest)


## History
[//]: # "If possible, tell when it was added or modified strongly"

1. Introduced in EAV 4.x, 2sxc 9.10

[//]: # "The following lines are a list of links used in this page, referenced from above"

[eavds]: DotNet-DataSources-All
