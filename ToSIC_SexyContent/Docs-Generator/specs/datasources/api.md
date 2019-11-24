---
uid: Specs.DataSources.Api
---
# DataSource API

## Purpose / Description
DataSources are a magic, generic system which can generate new data-items or filter / modify other data. This section explains how they work and the API you need to create your own. 

## Basics
DataSources have a few important concepts you must first understand:

1. An item is a data-object of the type `IEntity` - it can be a person, product, file-informatio etc.
1. A list of items is is a `List<IEntity>` which contains zero, one or many items
1. A [stream](xref:ToSic.Eav.DataSources.IDataStream) is an object which has list of items, and a name
1. A correctly built [stream](xref:ToSic.Eav.DataSources.IDataStream) will offer this list, but only build it if it's actually requested
1. Each [DataSource](xref:Specs.DataSources.DataSource) has one or many named `Out` streams
1. Each [DataSource](xref:Specs.DataSources.DataSource) _can_ have one or more named `In` streams coming from other DataSources
1. Each [DataSource](xref:Specs.DataSources.DataSource) has a `ConfigurationProvider`, which gives the DataSource information about the environment (like Portal or Tab information), App-Settings and more
1. Each [DataSource](xref:Specs.DataSources.DataSource) can have custom **Settings**, which the user entered in a dialog. Internally this is also an IEntity object
1. DataSources also have cache-identity mechanism, to inform any up-stream cache what parameters actually caused this result, so that the data could be cached if needed

## Providing Data
To offer data on the `Out` you will usually use the `Provide` method. If you're generating new items, you'll usually use the `AsEntity` method. Docs for these: 

* [DataSource.Provide](xref:Specs.DataSources.Api.Provide)
* [DataSource.AsEntity](xref:Specs.DataSources.Api.AsEntity)

## Receiving Data from In for further processing
if your DataSource performs filtering or similar actions on existing data, then this data comes in on the `In` streams. In such scenarios, you would simply iterate over the `In[streamname].List` and provide the result in your out-stream again. You can find many examples in the EAV DataSources code. 

## Getting Configuration
A DataSource gets the configuration from a configuration provider. To better understand this, you should read:

* [about configuration](xref:Specs.DataSources.Configuration)
* [about tokens](xref:Concepts.Tokens) 
* [ConfigMask(...)](xref:Specs.DataSources.Api.ConfigMask).

## Configuring the UI
There is a special attribute called `VisualQuery` to tell the UI how to show your DataSource and provide help etc. 

* [VisualQuery attribute](dotnet-datasource-api-visualquery)

## History

1. Introduced in 2sxc 4 or 5
2. enhanced / simplified in 2sxc 9.13

