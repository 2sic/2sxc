---
uid: ToSic.Eav.DataSources
summary: *content
---

This is all about DataSources which are chained together to prepare/process data for views, WebApi or anything else. 

In most cases there will be a VisualQuery which connects all the parts automatically, but when you want to work with them programatically, here you'll find the API. 
Note that we've hidden the constructors for all DataSource objects in these docs, because you should usually use the `CreateSource<T>(...)` command on the Razor template or WebApi, which will auto-configure things behind the scenes. 

_Note: Most DataSources have an Attribute which adds more metadata - as of now, we couldn't figure out how to include it in the Docs-generator, so you can't see them. we're working on it._