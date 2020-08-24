---
uid: Specs.Adam.AsAdam
---

# AsAdam(...) Command

When a content-item has a `Library` field (see [](xref:Specs.Data.Inputs.Hyperlink-Library)) you will need to find the files and maybe folders of that field, to show galleries or something. 

This is where `AsAdam(...)` comes in. All RazorComponents and ApiControllers have this command. Here's the [official API docs](xref:ToSic.Sxc.Code.IDynamicCode.AsAdam*).

Basically all you need for `AsAdam(...)` is

1. The IEntity content item 
1. the field name of which you want the Adam objects

As a result you'll get an [](xref:ToSic.Sxc.Adam.IFolder) object with which you can get all files in the folder or subfolders. 

## Also Read

* [](xref:Specs.Adam.Intro)
* [](xref:ToSic.Sxc.Adam)
* check out `AsAdam(...)` in the [](xref:ToSic.Sxc.Dnn.RazorComponent) and [](xref:ToSic.Sxc.Dnn.ApiController)


## History

1. General Tokens introduced in 2sxc 8.0
1. Added extra security switch in 2sxc 9.32
