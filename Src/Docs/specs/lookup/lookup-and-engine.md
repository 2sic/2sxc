---
uid: Specs.LookUp.Intro
---

# LookUp & LookUpEngine

Many things in the EAV and 2sxc require it to look up parameters. 
For example, when a Query is created, it may need to know the current PortalId or the current time, to properly filter/sort something. 

This is achieved through 2 special object types, the [](xref:ToSic.Eav.LookUp).ILookUp and [](xref:ToSic.Eav.LookUp).ILookUpEngine, both in the [](xref:ToSic.Eav.LookUp) namespace. 



## LookUp Objects

[LookUp objects](xref:ToSic.Eav.LookUp.ILookUp) have a `Name` similar to a namespace or a scope. For example, when a Module is being rendered, there are LookUp objects with names like `QueryString`, `Module`, `Portal` etc. 

They also have a list of properties they can look up, like `Id`, `PortalId` or parameters from the url. 

The lookup object is then responsible to retrieve these if requested. Everything is lazy, so these objects are only accessed if the parameter is actually needed. 

All LookUp objects implement the [](xref:ToSic.Eav.LookUp).ILookUp interface and should inherit the [](xref:ToSic.Eav.LookUp).LookUpBase object. 

## LookUpEngine

LookUp Engines will collect a set of LookUp objects and use these to resolve strings like `Module:ModuleId`. For this, they will check which LookUp has the right name (in this case `Module`) and will ask it if it can provide the value (in this case the `ModuleId`). 

LookUp Engines all implement the [](xref:ToSic.Eav.LookUp).ILookUpEngine interface and should inherit the [](xref:ToSic.Eav.LookUp).LookUpEngine object. 

> [!NOTE]
> Usually LookUp Engines will receive a long list (Dictionary) of things to look up, and resolve these in one quick call. This is because often they are attached to a DataSource which requires many configuration values - so they will prepare the list of parameters, pass it to the LookUpEngine and then work with the results as needed.

> [!TIP]
> LookUp Engines can also perform default-fallbacks - so if a LookUp source can't provide the answer needed, the engine may use a static value instead: 
> `[QueryString:PageSize||10]` 

> [!TIP]
> LookUp Engines can also perform lookup-fallbacks if the source can't provide an answer. In this case it may ask another LookUp if it has the answer. This happens when the Token looks like this:  
> `[QueryString:PageSize||[App:Settings:PageSize]]`

LookUpEngine objects are provided with DepedencyInjection. The system that gets the currently valid LookUpEngine inherits the [](xref:ToSic.Eav.LookUp).IGetEngine.

## Examples of LookUp Objects

Just to give you an idea of the power of LookUp objects, here are some in use:

1. [](xref:ToSic.Eav.LookUp).LookUpInEntity - this resolves entity values. 
1. [](xref:ToSic.Eav.LookUp).LookUpInNameValueCollection - this resolves from name/value lists like `Dictionary` or `Request.QueryString` objects
1. [](xref:ToSic.Eav.LookUp).LookUpInMetadata - will get values from Metadata of something
1. [](xref:ToSic.Eav.LookUp).LookUpInLookUps - will look up values in various attached LookUp objects
1. [](xref:ToSic.Sxc.Dnn.LookUp.LookUpInDnnPropertyAccess) - will look up stuff in DNN specific PropertyAccess objects, which are similar to LookUp objects
1. [](xref:ToSic.Sxc.LookUp.LookUpInDynamicEntity) - will look up things in a IDynamicEntity and also provide more information like Count, IsFirst, etc. for the Token Engine


## Also Read

* [](xref:Specs.DataSources.Configuration)
* [](xref:Specs.LookUp.Tokens)
* [](xref:Specs.DataSources.Api.EnsureConfigurationIsLoaded)
* [](xref:ToSic.Eav.LookUp)
* [](xref:ToSic.Sxc.LookUp)
* [](xref:ToSic.Sxc.Dnn.LookUp)


## History

1. General Tokens introduced in 2sxc 1.0
1. Most enhancements were in 2sxc 07.00

