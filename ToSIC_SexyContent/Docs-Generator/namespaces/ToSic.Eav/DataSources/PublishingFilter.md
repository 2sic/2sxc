# Data Source: PublishingFilter

## Purpose / Description
The **PublishingFilter** [DataSource](xref:ToSic.Eav.DataSources.IDataSource) is part of the [Standard EAV Data Sources][eavds]. It will hide unpublished items for non-editors and provide these drafts if an editor is viewing the results.  

## How to use with the Visual Query
When using the [Visual Query](xref:Temp.VisualQuery) you can just drag it into your query. Here's a picture showing app data with or without publishing filter:  

<img src="/assets/data-sources/app-compare-no-in-with-publishing-filter.png" width="100%">

## Configuring the PublishingFilter DataSource
The PublishingFilter has no configuration. 

## How does it work?
Since the published/unpublished is a very common query, internally the cache actually already provides different streams for this - and the PublishingFilter simply chooses the stream based on the current permissions. 


## Programming With The PublishingDataSource
_Note: We recommend to use the Visual Query where possible, as it's easier to understand and is consistant for C# and JavaScript. It's also better because it separates data-retrieval from visualization._

Because of limited resources we haven't documented this yet. You'll figure it out based on other examples :).  

FQN: `ToSic.Eav.DataSources.PublishingFilter`

## Read also

* [Source code of the PublishingFilter](//github.com/2sic/eav-server/blob/master/ToSic.Eav.DataSources/PublishingFilter.cs)
* [List of all EAV Data Sources][eavds]

## Demo App and further links
You should find some examples in this demo App
* [Demo App with examples for most DataSources](https://github.com/2sic/app-demo-visual-query/releases/latest)


## History
[//]: # "If possible, tell when it was added or modified strongly"

1. Introduced in EAV 3.x, in 2sxc ?

[//]: # "The following lines are a list of links used in this page, referenced from above"

[eavds]: DotNet-DataSources-All
