# Data Source: Paging

## Purpose / Description
The **Paging** [DataSource](xref:Specs.DataSources.DataSource) is part of the [Standard EAV Data Sources](xref:Specs.DataSources.ListAll). It will limit the items returned to a _page_ containing just a few items, and will also provide a _paging-information_ so that the UI knows what page it's on and how many pages remain.  

## How to use with the Visual Query
When using the [Visual Query](xref:Temp.VisualQuery) you can just drag it into your query. This is what it usually looks like:

<img src="/assets/data-sources/paging-basic.png" width="100%">

The above example shows:

1. a content-type filter limiting the items to type _Company_
2. a Paging which only passes on the first 3 companies of page 1

## Using Url Parameters for Paging
This example shows how you can use the Url Parameter to page through the results:

<img src="/assets/data-sources/paging-page-from-url.png" width="100%">

...you could also set the page size from other tokens like url or app-configuration, like this:

<img src="/assets/data-sources/paging-page-size-app-settings.png" width="100%">

## Programming With The Paging DataSource
_Note: We recommend to use the Visual Query where possible, as it's easier to understand and is consistant for C# and JavaScript. It's also better because it separates data-retrieval from visualization._

Because of limited resources we don't have code-examples. It works, but you'll probably never need it so we don't document it. 

FQN: `ToSic.Eav.DataSources.Paging`

## Read also

* [Source code of the Paging](//github.com/2sic/eav-server/blob/master/ToSic.Eav.DataSources/Paging.cs)
* [List of all EAV Data Sources](xref:Specs.DataSources.ListAll)

## Demo App and further links
You should find some examples in this demo App
* [Demo App with examples for most DataSources](https://github.com/2sic/app-demo-visual-query/releases/latest)


## History
[//]: # "If possible, tell when it was added or modified strongly"

1. Introduced in EAV 4.x, 2sxc ?




