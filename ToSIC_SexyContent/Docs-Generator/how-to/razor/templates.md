---
uid: HowTo.Razor.Templates
---
# Razor Templates - RazorComponent

Razor Templates contain both normal HTML intermixed with Razor placeholders like `@Content.FirstName` or longer code blocks usually marked with `@{ ...}`.

All razor Templates derive from the RazorComponent, so the following variables and objects are available for you to work with.

> [!TIP]
> Read about [Razor Templates](xref:Specs.Cms.Templates.Razor) in the specs

## Example
Visit the [App Catalog](xref:AppsCatalog) where almost all apps use Razor. There you can find hundreds of examples. 


## Objects Added by 2sxc in Razor Templates

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
* `string` [InstancePurpose](xref:HowTo.Razor.Purpose) - tells you if the code is running to render into html, or for another reason like populating the search index - so your code can adapt
