
# This is the **2sxc API Documentation**.

It's completely work-in-progress (WIP) as of October / November 2019. Do not use this yet!

## Background: Eav vs. Sxc vs. Dnn

The data management system underneath everything is called the **EAV** - which stands for **Entity**, **Attribute**, **Value**. 
Anything in that namespace is about internal data models, reading/saving data etc. 
So anything inside the @ToSic.Eav.Interfaces is all about the internals, which you only need in special scenarios. 
The same applies to @ToSic.Eav.Apps.Interfaces which is the sub-system responsible for combining data into virtual bundles called **Apps**.
You can usually ignore this. 

On top of the _EAV_ layer we have the **Sxc** layer. 
It's responsible for _Content Management_ on top of the _App_ model provided by the _EAV_. 
The _Sxc_ layer provides things like @ToSic.Sxc.Interfaces.IDynamicEntity to let you code like `@Content.Title`. 
This is usually more interesting for you, but still fairly generic, because 2sxc is also meant to work with other 
platforms like NopCommerce, Orchard or Oqtane, but it hasn't been implemented yet.

On top of the _Sxc_ layer we have the **Dnn** layer. It connects 2sxc with Dnn. 
Usually when you're writing code and want to know about the API, you'll typically start here, 
and drill down to the other APIs as needed.

## What You're Probably Looking for

### APIs in Razor Templates and WebApi

1. If you are creating a **Razor** template and want to know what APIs are available, start with @ToSic.Sxc.Dnn.IRazor which is mostly an @ToSic.Sxc.Dnn.IDynamicCode with an @ToSic.Sxc.Dnn.IHtmlHelper. 
	This is because a Razor Page inherits from that interface, so you'll see all the commannds you get there. 

1. If you'le creating a **WebApi** and need to know everything in it, you also want to check the @ToSic.Sxc.Dnn.IDynamicCode, because all WebApi classes implement that interface. 

### Working with Entities and ADAM Assets

1. If you're working with `DynamicEntity` objects and want to know more about them, check out @ToSic.Sxc.Interfaces.IDynamicEntity.  
	In very rare cases you also want to know more about the underlying @ToSic.Eav.Interfaces.IEntity.
1. If you're working with `ADAM` Assets, like from the `AsAdam(...)` command on @ToSic.Sxc.Interfaces.IDynamicEntity objects,  
	you'll want to read about @ToSic.Eav.Apps.Assets.IAdamFolder and @ToSic.Eav.Apps.Assets.IAdamFile

### 

## todo
datasources
