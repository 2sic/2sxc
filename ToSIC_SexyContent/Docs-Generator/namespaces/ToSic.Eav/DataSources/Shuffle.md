# Data Source: Shuffle

## Purpose / Description
The **Shuffle** [DataSource][ds] is part of the [Standard EAV Data Sources][eavds]. It will randomize the order of items which came in. This is common for components which show "3 random quotes" and similar scenarios. 

## How to use with the Visual Query
When using the [Visual Query][vqd] you can just drag it into your query. This is what it usually looks like:

<img src="assets/data-sources/shuffle-3.png" width="100%">

The above example shows:

1. a content-type filter limiting the items to type _Company_
2. a shuffle which only passes on 3 random companies 



## Programming With The Shuffle DataSource
_Note: We recommend to use the Visual Query where possible, as it's easier to understand and is consistant for C# and JavaScript. It's also better because it separates data-retrieval from visualization._

Because of limited resources we don't have code-examples. It works, but you'll probably never need it so we don't document it. 

FQN: `ToSic.Eav.DataSources.Shuffle`

## Read also

* [Source code of the Shuffle](//github.com/2sic/eav-server/blob/master/ToSic.Eav.DataSources/Shuffle.cs)
* [List of all EAV Data Sources][eavds]

## Demo App and further links
You should find some examples in this demo App
* [Demo App with examples for most DataSources](https://github.com/2sic/app-demo-visual-query/releases/latest)


## History
[//]: # "If possible, tell when it was added or modified strongly"

1. Introduced in EAV 4.x, 2sxc ?

[//]: # "The following lines are a list of links used in this page, referenced from above"
[vqd]: http://2sxc.org/en/Learn/Visual-Query-Designer
[eavds]: DotNet-DataSources-All
[ds]: DotNet-DataSource