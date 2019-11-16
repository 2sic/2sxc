# Data Source: StreamFallback

## Purpose / Description
The **StreamFallback** [DataSource][ds] is part of the [Standard EAV Data Sources][eavds]. It will pass on the first stream which has results. The order of evaluation is based on the names of the streams as they enter the StreamFallback. 

## How to use with the Visual Query
When using the [Visual Query][vqd] you can just drag it into your query. This is what it usually looks like:

<img src="assets/data-sources/stream-fallback-4-stream-example.png" width="100%">

The above example shows a query from the [blog app](https://2sxc.org/en/apps/app/dnn-blog-app-for-dnn-dotnetnuke):

1. a stream-fallback with 4 in-streams
2. the first stream which would filter by tag, if the URL had a tag in it - as the test-data has no tag, this stream delivers no results
2. the second stream filters by author - as the url would have an author (specified in the test-values), it does return items

The StreamFallback therefor passes on the items provided by the author-filter. Note that while using the visual query, all other streams are also processed to show what's happening. At runtime it will stop processing other streams once it's found a first match. 


## Programming With The StreamFallback DataSource
_Note: We recommend to use the Visual Query where possible, as it's easier to understand and is consistant for C# and JavaScript. It's also better because it separates data-retrieval from visualization._

Because of limited resources we don't have code-examples. 

FQN: `ToSic.Eav.DataSources.StreamFallback`

## Read also

* [Source code of the StreamFallback](//github.com/2sic/eav-server/blob/master/ToSic.Eav.DataSources/StreamFallback.cs)
* [List of all EAV Data Sources][eavds]

## Demo App and further links
You should find some examples in this demo App
* [Demo App with examples for most DataSources](https://github.com/2sic/app-demo-visual-query/releases/latest)


## History
[//]: # "If possible, tell when it was added or modified strongly"

1. Introduced in EAV 3.x, 2sxc ?

[//]: # "The following lines are a list of links used in this page, referenced from above"
[vqd]: http://2sxc.org/en/Learn/Visual-Query-Designer
[eavds]: DotNet-DataSources-All
[ds]: DotNet-DataSource