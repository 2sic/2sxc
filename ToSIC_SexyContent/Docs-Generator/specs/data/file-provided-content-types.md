---
uid: Specs.Data.FileBasedContentTypes
---

# File-Based Content-Types 

Usually [Content-Types](xref:Specs.Data.ContentTypes) are stored in the database. For a special use case, content-type definitions can also stored in a json-file. This will primarily be used in providing future _shared_ content types which are used across all portals and apps. Examples of such types are:

1. configurations of data-sources (like the SqlDataSource in 2sxc 9.8) 
2. any kind of input-configuration types (like string-dropdown, etc. starting in 2sxc 9.10)
3. any kind of global types like view metadata etc.

## Overview
Basically the app-repository is a folder which contains content-type definitions in a _contenttype_ sub folder. In 2sxc, this is located in:

`/desktopmodules/ToSIC_SexyContent/.data/`

The format is the [json-format V1](xref:Specs.Data.Formats.JsonV1-ContentType)

## Limitation: No GUI
As of now the system will pick up the content-types stored there and everything works. BUT there is no built-in UI to edit these. We (2sic) can easily create content-types in a normal 2sxc and export them to json for this purpose, but as of now there is no GUI to do so. 

This should not affect you, as it's not meant to be managed by anybody else than us as of now. 

## When To Use
You will almost never need these, except for 2 important scenarios:

1. Shared Content-Types across Apps (similar to Ghost-Types)
1. When you create a [custom data-source](xref:Specs.DataSources.Custom), and want to distribute the Configuration Content-Type along with your DLL

## Future Features & Wishes

1. _App level_ content-types. This would dramatically enhance our ability to upgrade existing apps, as it's easier to detect type-changes. 

## History

1. Added in 2sxc 9.7

