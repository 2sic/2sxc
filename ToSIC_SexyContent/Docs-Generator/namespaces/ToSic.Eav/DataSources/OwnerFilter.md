# Data Source: OwnerFilter

## Purpose / Description
The **OwnerFilter** [DataSource][ds] is part of the [Standard EAV Data Sources][eavds]. It will only let items pass through, which a specific user (often the current one) has created initially. 

You will typically use the **OwnerFilter** in scenarios where users create their own data, and should only see/edit items which they own (usually in combination with security settings, which only allow the owner to modify their own items).

## How to use with the Visual Query
When using the [Visual Query][vqd] you can just drag it into your query. You must then edit the settings once - and usually you will use the recommended prefilled-form. But you can also do something different. This is what it usually looks like:

<img src="assets/data-sources/ownerfilter-configured.png" width="100%">

The above example shows:

1. a content-type filter limiting the items to type _Company_
2. an owner-filter which receives 5 items, but only lets 3 pass, because the _Test Settings_ have a demo-value of the user who only created 3 of the 5 items. 



## Programming With The OwnerFilter DataSource
_Note: We recommend to use the Visual Query where possible, as it's easier to understand and is consistant for C# and JavaScript. It's also better because it separates data-retrieval from visualization._

Because of limited resources we don't have code-examples. It works, but you'll probably never need it so we don't document it. 

FQN: `ToSic.Eav.DataSources.OwnerFilter`

## Read also

* [Source code of the OwnerFilter](//github.com/2sic/eav-server/blob/master/ToSic.Eav.DataSources/OwnerFilter.cs)
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