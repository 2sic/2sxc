---
uid: Todo.ToSic.Eav.DataSources.xxx
---

# Data Source: Module-Instance DataSource

## Purpose / Description
The **Module-Instance DataSource** [DataSource](xref:Specs.DataSources.DataSource) is part of the [Standard 2sxc/DNN Data Sources](xref:Specs.DataSources.ListAll). It is the default data source as it directly delivers module-instance data and is used for all scenarios which don't explicitly have a query, and it can also be used as part of a query.  

## How to use with the Visual Query
When using the [Visual Query](xref:Temp.VisualQuery) it is already in the default / initial query: 

<img src="/assets/data-sources/module-basic.png" width="100%">

The above example shows:

1. all data start in the cache _ICache_
1. it is then by default passed through the [PublishingFilter](DotNet-DataSource-PublishingFilter)
1. then it enters this Instance/Module, which gets the current ModuleInstance and passes on the data which has been assigned to it


## Using Module/Instance Data For Configuration
Things get really exciting when you use values which the user edited in the module as a setting in your query. Here's an example: 

<img src="/assets/data-sources/module-providing-settings-to-sort.png" width="100%">

As you can see, the _Default_ out of the **ModuleDataSource** is passed into the [ValueSort](DotNet-DataSource-ValueSort) data source with the stream-name _Settings_ and is then used in a token to configure behaviour of the sort. 


## Manually Assigning a Module Instance ID (2sxc 9.9)
In 2sxc 9.9 we added the configuration dialog, so you can specify what module the data should come from. This allows you to have configuration-modules or primary-data-modules, which are re-used in queries:

<img src="/assets/data-sources/module-instance-configured.png" width="100%">



## Programming With The ModuleDataSource DataSource
[!include["simpler-with-vqd"](shared-use-vqd.md)]

The only property you need to set is `InstanceId` (2sxc 9.9+) if you want to provide a different module-id. 

[!include["Read-Also-Section"](shared-read-also.md)]

[!include["Demo-App-Intro"](shared-demo-app.md)]

[!include["Heading-History"](shared-history.md)]

1. Introduced in EAV 4.x, 2sxc ?
1. Added ability to configure in in the visual-query (2sxc 9.9)

[!include["Start-APIs"](shared-api-start.md)]