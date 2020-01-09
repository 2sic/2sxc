---
uid: Specs.WebApi.Intro
---

# Introduction to WebAPI and REST APIs

To use 2sxc/EAV data in JavaScript, from Mobile Apps or from other websites you need **endpoints** which will provide the data. 2sxc and EAV have everything prepared to make this easy for you and to ensure security - especially to ensure you only publish data you intend to publish.

A few terms to clarify before we begin:

- [REST](https://en.wikipedia.org/wiki/Representational_state_transfer) means that there is a url convention to access data like `.../blogposts/25`
- [REST](https://en.wikipedia.org/wiki/Representational_state_transfer) also specifies that an _HTTP GET_ is for reading, _HTTP POST_ is for writing, etc.
- All the APIs use [JSON](https://en.wikipedia.org/wiki/JSON) for sending/receiving data. [This is the default schema](https://azing.org/2sxc/r/2zBGrCAd).

> [!TIP]
> ➡ [read about the URLs for using these endpoints](xref:Specs.WebApi.UrlSchema)

## Standard Content/Entity REST Endpoints

These endpoints already exist and allow normal CRUD (Create, Read, Update, Delete) operations. To use them, you would need to enable the permissions and then you can access them using the REST URLs.

## Standard Query REST Endpoints

Every Visual Query you create has a REST URL. If you set the permissions, you can then read from the Query through REST. You can also pass query-parameters in the URL.

## Custom C# WebAPI

You can easily create custom C# WebAPIs, and then access them through the URL. What these endpoints do is completely up to you.







Once you have opened the security on a standard endpoint or once you have created your custom endpoint, you can then access it from JavaScript, Mobile Apps or external systems. 


## For Standard Endpoints: Set Security

<iframe src="https://azing.org/2sxc/r/34pAzAF2?embed=1" width="100%" height="400" frameborder="0" allowfullscreen style="box-shadow: 0 1px 3px rgba(60,64,67,.3), 0 4px 8px 3px rgba(60,64,67,.15)"></iframe>

## For Custom WebAPI: Write the Code

TODO

## Write the Endpoint-Client - usually in JavaScript

TODO



## Read also

- [DotNet WebApi](xref:HowTo.WebApi)
- [Concepts: Polymorphisms](xref:Specs.Cms.Polymorphism)

## Demo App and further links

You should find some code examples in this demo App

- [REST and WebApi Tutorial](http://2sxc.org/en/apps/app/tutorial-javascript-rest-api-using-jquery-and-angularjs)

## History

1. Introduced in 2sxc ca. 5.x
2. Enhanced with Polymorph Editions in 2sxc 9.35 (allowing subfolder/api)
