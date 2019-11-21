
# Concept: File/Folder Based Content-Types (9.7) 

## Purpose / Description
We're adding support to use content-type definitions stored in a json-file, in addition to the SQL data base. 

This will primarily be used in providing future _shared_ content types which are used across all portals and apps. Examples of such types are
1. configurations of data-sources (like the SqlDataSource in 2sxc 9.8) 
2. any kind of input-configuration types (like string-dropdown, etc. starting in 2sxc 9.10)
3. any kind of global types like view metadata etc.

This will also be used in the near future to provide _app level_ content-types - probably in 2sxc 9.11 or similar. This will dramatically enhance our ability to upgrade existing apps, as it's easier to detect type-changes. 

## Overview
Basically the app-repository is a folder which contains content-type definitions in a _contenttype_ sub folder. As of 2sxc 9.7, this is located in

`/desktopmodules/ToSIC_SexyContent/.data/`

We're using the `.data` folder as this is protected from external browsing. 

The format is the [json-format V1](xref:Specs.Data.Formats.JsonV1-ContentType)

In 2sxc 9.7 you will not find any files or folders in this yet, as v. 9.7 has all the features in place, but doesn't yet distribute any types. This is so we can continue to test and design the new content-types before releasing them "in the wild". 

## Limitations
As of now (2sxc 9.7) the system will pick up the content-types stored there and everything works. BUT there is no built-in mechanism to edit these. We (2sic) can easily create content-types in a normal 2sxc and export them to json for this purpose, but as of now there is no GUI to do so. 

This should not affect you, as it's not meant to be managed by anybody else than us as of now. 

## Read also
[//]: # "Additional links - often within this documentation, but can also go elsewhere"

* (-)

## History
[//]: # "If possible, tell when it was added or modified strongly"

1. Added in 2sxc 9.7

