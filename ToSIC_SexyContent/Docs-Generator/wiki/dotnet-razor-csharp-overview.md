# DotNet / C# / Razor API Documentation for DNN / 2sxc

This is the C# .net API Documentations, usually meant for Razor-Templates, custom Web-APIs or when accessing 2sxc-instances from external code.

## Working with C# Razor Templates
1. [C# API in a normal Razor Template](Razor-Templates)
1. Core Objects:
    1. [App](Razor-App) (includes App.Data)
    1. [Data](Razor-Data)
    1. [Dnn](Razor-Dnn)
    1. [Edit](Razor-Edit)
    1. [Link](Razor-Link)
1. [LINQ Data Handling examples](DotNet-Query-Linq) on working with relationships, filtering etc.

## Important APIs when Working with Content-Items/Data
1. [Dynamic Entities](DotNet-DynamicEntity) (DynamicEntity / AsDynamic)
1. [IEntity](DotNet-Entity) - the complex data object for advanced use cases
1. [DataSource](DotNet-DataSource) and [DataStream](DotNet-DataStream), the core concept for data read/processing/delivery
    1. [List of all DataSource Objects](DotNet-DataSources-All)
    2. [Querying Data and Data Sources with code and LINQ](DotNet-Query-Linq)
    3. [how to create custom data sources](http://2sxc.org/en/blog/post/new-2sxc7-create-your-own-custom-datasource-for-visual-query)

## Advanced APIs
1. [WebService API](DotNet-WebApi) to create your own web services in your apps
1. [External API](DotNet-External-Use) to access 2sxc-instances on the server from WebForms or other MVC components
1. [Content-Blocks API](Razor-Content-Blocks) to render inner-content (see also the [concept](Concept-Inner-Content))



[CustomizeData]:Razor-SexyContentWebPage.CustomizeData
[InstancePurpose]:Razor-SexyContentWebPage.InstancePurpose
[CustomizeSearch]:Razor-SexyContentWebPage.CustomizeSearch