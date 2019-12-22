---
uid: HowTo.Razor.Templates
---
# RazorComponent aka Razor Templates

Razor Components contain both normal HTML intermixed with Razor placeholders like `@Content.FirstName` or longer code blocks usually marked with `@{ ...}`.

All razor Templates derive from the RazorComponent, so the following variables and objects are available for you to work with.

> [!TIP]
> Read about Razor Components/Templas in the [Specs](xref:Specs.Cms.Templates.Razor) or the [API docs](xref:ToSic.Sxc.Dnn.RazorComponent).

[!include["Tip Inherits"](shared-tip-inherits.md)]

## Example
Visit the [App Catalog](xref:AppsCatalog) where almost all apps use Razor. There you can find hundreds of examples. 


## Objects Added by 2sxc in Razor Components / Templates

1. [App](xref:HowTo.DynamicCode.App) - the current App and all it's data
1. Content ([DynamicEntity](xref:HowTo.DynamicCode.Entity)) - primary and often the only content-item in the [Data](xref:HowTo.DynamicCode.Data) for this template
1. Content.Presentation ([DynamicEntity](xref:HowTo.DynamicCode.Entity))
1. [Data](xref:HowTo.DynamicCode.Data) (IDataSource)- this object gives you all the data which was meant to be used by this Templates
1. [Dnn](xref:HowTo.DynamicCode.Dnn) - the common Dnn object providing page, module, user information
1. [Edit](xref:HowTo.Razor.Edit) - helper providing you with various edit-functionality like `Toolbar(...)`
1. Header ([DynamicEntity](xref:HowTo.DynamicCode.Entity)) - the header data if the template expects to be a list
1. Header.Presentation ([DynamicEntity](xref:HowTo.DynamicCode.Entity))
1. [Link](xref:HowTo.DynamicCode.Link) - helper to generate links, according to the DNN-environment configuration

## Helper Commands provided by 2sxc

* AsAdam()
* AsDynamic(...) - takes just about anything (an iEntity, a list of iEntities, a dynamic, ...) and casts it to a [DynamicEntity](xref:HowTo.DynamicCode.Entity)
* AsEntity(...) - takes just about anything (iEntity, DynamicEntity, list of that) and casts it to an [iEntity](xref:HowTo.DynamicCode.Entity)
* CreateInstance(...) - to create an object of a parsed CSHTML file, for example to then access methods of that code
* CreateSource\<T\>(...) (IDataSource) - more modern, generic, type-proof syntax for create-source

## Customizing Data & Search

* `Overrideable` [CustomizeData](xref:HowTo.Razor.CustomizeData) - is like a "before-data-is-used" of the page, used to change what data is delivered to the page - or to the search.  
  Note that this is an older feature and many things this does can also be done using the visual query designer. But sometimes you will need code, and this is the place to do it.
* `Overridable` [CustomizeSearch](xref:HowTo.Razor.CustomizeSearch)
* `string` [Purpose](xref:HowTo.Razor.Purpose) - tells you if the code is running to render into html, or for another reason like populating the search index - so your code can adapt


## Migrating from the old Razor (before 10.20) to the new RazorComponent

The RazorComponent was created in v10.20 to provide a newer, cleaner API. To not break existing code, old templates still work, but you must migrate a template to RazorComponent if you wish to use the new features. Here what you need to do:

<iframe src="https://azing.org/2sxc/r/9g6xRMrD?embed=1" width="100%" height="400" frameborder="0" allowfullscreen style="box-shadow: 0 1px 3px rgba(60,64,67,.3), 0 4px 8px 3px rgba(60,64,67,.15)"></iframe>
