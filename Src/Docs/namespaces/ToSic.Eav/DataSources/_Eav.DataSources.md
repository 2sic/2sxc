---
uid: ToSic.Eav.DataSources
---

DataSources are objects which deliver one or many [DataStreams](xref:ToSic.Eav.DataSources.IDataStream), which contain a bunch of content-items. They are then attached to each other (from one sources `Out` to another ones `In`) creating a `Query`. Queries can be customized at will. The whole system is used to prepare/process data for views, WebApi or anything else.

In most cases there will be a VisualQuery which connects all the parts automatically, but when you want to work with them programatically, here you'll find the API. 

> [!NOTE]
> We've hidden the constructors for all DataSource objects in these docs, because you should usually use the `CreateSource<T>(...)` command on the Razor template or WebApi, which will auto-configure things behind the scenes. 

> [!TIP]
> Read about [DataSources here](xref:Specs.DataSources.DataSource). It also explains how the configuration system works and how to create custom DataSources to deliver your data to EAV/2sxc.
