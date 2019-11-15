---
uid: Api.DotNet
---

# This is the **2sxc API Documentation**

## Background: Architecture of Eav, Sxc, Dnn

> Before you start, please get familiar with the [architecture](xref:Articles.Architecture) - otherwise you probably won't understand what you see here.

Note also that the real code of EAV/2sxc/DNN has way more stuff, but that's inofficial. 
Please don't use objects that are not documented here. 
That allows us to improve the architecture without worrying about breaking your code. 
Once we're really sure that certain parts are very final, we'll publish the API docs for those parts. 

## What You're Probably Looking for

### APIs in Razor Templates and WebApi

1. If you are creating a **Razor** template and want to know what APIs are available, start with @ToSic.Sxc.Dnn.IRazor which is mostly an @ToSic.Sxc.Dnn.IDynamicCode with an @ToSic.Sxc.Dnn.IHtmlHelper. 
	This is because a Razor Page inherits from that interface, so you'll see all the commannds you get there. 

1. If you're creating a **WebApi** and need to know everything in it, you also want to check the @ToSic.Sxc.Dnn.IDynamicWebApi, because all WebApi classes implement that interface. 

### Working with Entities and ADAM Assets

1. If you're working with `DynamicEntity` objects and want to know more about them, check out @ToSic.Sxc.Data.IDynamicEntity.  
	In very rare cases you also want to know more about the underlying @ToSic.Eav.Data.IEntity.
1. If you're working with `ADAM` Assets, like from the `AsAdam(...)` command on @ToSic.Sxc.Data.IDynamicEntity objects,  
	you'll want to read about @ToSic.Sxc.Adam.IAdamFolder and @ToSic.Sxc.Adam.IAdamFile

### Programming with DataSources and VisualQuery

All the DataSources are based on @ToSic.Eav.DataSources.IDataSource and most of them are also @ToSic.Eav.DataSources.IDataTarget . You can find most of them in @ToSic.Eav.DataSources . 

