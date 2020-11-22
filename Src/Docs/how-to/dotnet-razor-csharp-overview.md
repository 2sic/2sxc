---
uid: HowTo.CSharp
---
# DotNet / C# / Razor API Documentation for DNN / 2sxc

This is the C# .net API Documentations, usually meant for Razor-Templates, custom Web-APIs or when accessing 2sxc-instances from external code.

[!include["Tip Inherits"](razor/shared-tip-inherits.md)]

## Working with C# Razor Templates
1. [C# API in a normal Razor Template](xref:HowTo.Razor.Templates)
1. Core Objects:
    1. [App](xref:HowTo.DynamicCode.App) (includes App.Data)
    1. [Data](xref:HowTo.DynamicCode.Data)
    1. [Dnn](xref:HowTo.DynamicCode.Dnn)
    1. [Edit](xref:HowTo.Razor.Edit)
    1. [Link](xref:HowTo.DynamicCode.Link)
1. [LINQ Data Handling examples](xref:Specs.DataSources.Linq) on working with relationships, filtering etc.

## Important APIs when Working with Content-Items/Data

1. [Dynamic Entities](xref:HowTo.DynamicCode.DynamicEntity) (DynamicEntity / AsDynamic)
1. [IEntity](xref:HowTo.DynamicCode.Entity) - the complex data object for advanced use cases
1. [DataSource](xref:Specs.DataSources.DataSource) and [DataStream](xref:ToSic.Eav.DataSources.IDataStream), the core concept for data read/processing/delivery
    1. [List of all DataSource Objects](xref:Specs.DataSources.ListAll)
    2. [Querying Data and Data Sources with code and LINQ](xref:Specs.DataSources.Linq)
    3. [how to create custom data sources](http://2sxc.org/en/blog/post/new-2sxc7-create-your-own-custom-datasource-for-visual-query)

## Advanced APIs

1. [WebService API](xref:HowTo.WebApi) to create your own web services in your apps
1. [External API](xref:HowTo.External) to access 2sxc-instances on the server from WebForms or other MVC components
1. [Content-Blocks API](xref:HowTo.Razor.Blocks) to render inner-content (see also the [concept](xref:Specs.Cms.InnerContent))



