---
uid: Specs.Architecture.DataFlow
---

# EAV / 2sxc Data Flow

2sxc is simple to use, but to enable that, it's pretty sophisticated on the inside. Let's take a look at how data is accessed, cached, queried, maybe cached again, and provided to the output.

> [!NOTE]
> All the data handling systems come from the [EAV](xref:ToSic.Eav). This is then enhanced and customized by Sxc and Sxc.Dnn.

## Core Paradigms

### Rule #1: Data is Reactive
As you'll soon see, the EAV performs a lot of automatic lookup and caching, but as a first rule, this happens _Reactively_, only when data is actually accessed. 
This is similar to how [ReactiveX](http://reactivex.io/) or [NgRx](https://ngrx.io/) work, just much simpler. 

### Rule #2: Data is Read-Only

Any data the code receives as @ToSic.Eav.Data.IEntity objects is read-only. 
This allows for a much more robust data model and API which in a CMS environment is usually 99% read, 1% write. 

### Rule #3: Data Always Comes from DataSources

Inside the system everything is loosely coupled (think Depedency Injection). 
So anything requesting data will always use a [DataSource](xref:Specs.DataSources.Intro) provided by some magic, and then use what was given. 

### Rule #4: Data is Managed and Cached per App

Once data from an App was requested, the entire app is loaded and cached. 
Within an app a lot of data can be linked together using Content-Types, Relationships, Sub-Lists, Metadata and more. If we would lazy-load this on-demand from the storage system (SQL), it would result in a ping-pong of requests, which would be very inefficient. This unit of cache is called an [AppState](xref:Specs.Data.AppState).

## Overall Flow When Creating Html

Imagine that your custom Razor template in a DNN module is initialized. Here's what happens:

### 1. Preparation

2sxc starts with the ModuleId as it's inside a DNN Module. It will then create a [Block](xref:ToSic.Sxc.Blocks.IBlock) for this InstanceId (which is the ModuleId).

1. The Block internally will create a [CmsBlock](xref:ToSic.Sxc.DataSources.CmsBlock) [DataSource](xref:Specs.DataSources.Intro), pass in the InstanceId and then wait for data to pour in.
1. The [CmsBlock](xref:ToSic.Sxc.DataSources.CmsBlock) knows about DNN, and will use the InstanceId to get the ModuleSettings in DNN. This contains only 2 pieces of information: the `AppId` which it is visualizing, and a `ContentBlockId` which is the full configuration of the Content-Block which will be shown. The CmsBlock also knows if the user is an editor (which would result in draft-items being included). 
1. The CmsBlock DataSource will then ask the underlying EAV for this block configuration (stored as an Entity) which is then loaded from the Cache.  
    1. When the configuration is found, the CmsBlock then knows what [View](xref:ToSic.Sxc.Blocks.IView) will be used. 
    1. The CmsBlock itself is a [DataSource](xref:Specs.DataSources.Intro) and is now configured to deliver the data which the view expects. 

> [!NOTE]
> The CmsBlock doesn't know about the internals of the cache, it just asks for it. The cache will auto-initialize the [AppState](xref:Specs.Data.AppState) if it hasn't been accessed before.

> [!NOTE]
> The CmsBlock will also _not_ load any data yet. It just knows what would be loaded, should it ever be accessed.

> [!TIP]
> When you use views which don't have a content-type configured, then properties like `Data`, `Content` or `Header` will be null. But `App.Data` will still work.

### 2. Execution 
The Block is now ready. 2sxc now consults the View to find out which [Engine](xref:ToSic.Sxc.Engines.IEngine) to use (Razor or Token). It will now load this engine, give it the Block and wait for the resulting Html to be created. 

1. The engine loads the template and lets it do what it should. 
1. If the template has code accessing [Data](xref:ToSic.Sxc.Web.IDynamicCode.Data) then the underlying source will retrieve the necessary data.
    * If it's a normal content-view, then the items provided will be the ones which an editor has manually added, since they were referenced in the ContentBlock. 
    * If the view relies on a Query, then this query is built and will be waiting to execute if the data is accessed.
1. If the code accesses [App](xref:ToSic.Sxc.Web.IDynamicCode.Data).[Data](xref:ToSic.Eav.Apps.App.Data) then this data source will build up everything necessary so it just works.
1. If the code accesses [App](xref:ToSic.Sxc.Web.IDynamicCode.Data).[Query](xref:ToSic.Eav.Apps.App.Query) then the underlying system will prepare the Query as needed. 

> [!NOTE]
> All the data retrieving features like `Data` do not actually retrieve any data unless accessed.  
> This also means that `Content` or `Header` won't use any CPU cycles unless accessed.  
> The same goes for `App.Data` - nothing is processed unless it's accessed.

> [!NOTE]
> The queries are only provided by the App for immediate use. They won't be built or executed unless they are actually read.

## Overall Flow when Creating WebAPIs

This works just like the Html creation, except that we don't need an engine. The code which inherits from [ApiController](xref:ToSic.Sxc.Dnn.ApiController) now works with the data as needed. 

The same lazy-execution rules apply: neither `Data`, `Content`, `App.Data` or `App.Query` use any CPU cycles, unless the data is actually pulled by reading it. 


## Read Also

1. Templates
1. Rendering Engines like Token/Razor
1. Content-Blocks
1. Custom WebApi (which is also kind of a dynamic rendering engine)
