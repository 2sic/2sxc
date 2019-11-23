# SexyContentWebPage (Razor Templates)
## What are Razor Templates
All razor Templates derive from the SexyContentWebPage, so the following variables and objects are available for you to work with.

## Example
Visit the [App Catalog][app-cat] where almost all apps use Razor. There you can find hundreds of examples. 


## Objects Added by 2sxc in Razor Templates

1. [App](xref:HowTo.DynamicCode.App) - the current App and all it's data
1. Content ([DynamicEntity][DynamicEntity]) - primary and often the only content-item in the [Data][Data] for this template
1. [Data][Data] (IDataSource)- this object gives you all the data which was meant to be used by this Templates
1. [Dnn](Razor-Dnn) - the common Dnn object providing page, module, user information
1. [Edit](Razor-Edit) - helper providing you with various edit-functionality like `Toolbar(...)`
1. [Link](Razor-Link) - helper to generate links, according to the DNN-environment configuration
1. ~~List~~ - old, use `Data["Default"]` instead for more consistant programming style
1. ListContent ([DynamicEntity][DynamicEntity]) - the header data if the template expects to be a list
1. ~~ListPresentation~~ - old - use `ListContent.Presentation` instead for more consistant programming style
1. Permissions - ???
1. ~~Presentation~~ - old - use `Content.Presentation` instead for more consistant programming style

## Helper Commands provided by 2sxc

* AsAdam()
* AsDynamic(...) - takes just about anything (an iEntity, a list of iEntities, a dynamic, ...) and casts it to a [DynamicEntity][DynamicEntity]
* AsEntity(...) - takes just about anything (iEntity, DynamicEntity, list of that) and casts it to an [iEntity](DotNet-Entity)
* ~~CreateSource(...)~~ (IDataSource) - old, better to use the `CreateSource<T>(...)` syntax
* CreateInstance(...) - to create an object of a parsed CSHTML file, for example to then access methods of that code
* CreateSource\<T\>(...) (IDataSource) - more modern, generic, type-proof syntax for create-source

## Customizing Data & Search

* `Overrideable` [CustomizeData](razor-sexycontentwebpage.customizedata) - is like a "before-data-is-used" of the page, used to change what data is delivered to the page - or to the search.  
  Note that this is an older feature and many things this does can also be done using the visual query designer. But sometimes you will need code, and this is the place to do it.
* `Overridable` [CustomizeSearch](razor-sexycontentwebpage.customizesearch)
* `string` [InstancePurpose][InstancePurpose] - tells you if the code is running to render into html, or for another reason like populating the search index - so your code can adapt

[//]: # "Links referenced in this page"
[InstancePurpose]:Razor-SexyContentWebPage.InstancePurpose
[DynamicEntity]:dotnet-dynamicentity
[Data]:Razor-Data
[app-cat]: http://2sxc.org/en/apps