---
uid: WebApi.Content
---

# 2sxc Content REST and CRUD Web API

2sxc provides a full set of REST endpoints for CRUD (Create, Read, Update, Delete) operations like:

1. Read a list of all items/entities
1. Read a single item/entity
1. Create an item/entity
1. Update an item/entity
1. Delete an item/entity

> [!TIP]
> <img src="./assets/admin-ui-data-rest-menu.jpg" width="40%" align="right" >
> Starting in 2sxc 11.10+ the admin-UI has [built-in help](xref:WebApi.Help.AdminUiContentRest) to get started. These UIs also show sample code and everything.
> So you probably don't need to read the stuff here and just go ahead with that. 

## How to Read Content-Items/Entities

Assume you have the [blog-app](xref:App.Blog) installed and your JS would request a JSON from this endpoint (logged in as host, so security is not an issue):

`[root-path]/app/auto/content/BlogPost`

...then your JS would receive a JSON with all BlogPost items. 

Reading `[root-path]/app/auto/content/BlogPost/1050` would give you exactly one BlogPost item (with the id 1050)

> [!TIP]
> To figure out the exact path and the `[root-path]` part, 
> best read about it in the [admin-ui](xref:WebApi.Help.AdminUiContentRest)
> or work your way into the [](xref:WebApi.Specs.UrlSchema)


## How to Create Content-Items/Entities

Doing an http POST to this `[root-path]/app/auto/content/BlogPost` with a POST body of `{ "Title": "changed title"}` would let you create the item. You will get a return message containing ID, GUID etc. of the new item. 

If your POST package also contains an `EntityGuid` then this will be used as the GUID for the new item. 

## How to Update Content-Items/Entities

Doing an http POST to this `[root-path]/app/auto/content/BlogPost/1050` with a POST body of `{ "Title": "changed title"}` would let you update the item 1050.

## How to Delete Content-Items/Entities

Doing an http DELETE to this `[root-path]/app/auto/content/BlogPost/1050` would delete the item 1050.



## JavaScript Helpers

The [$2sxc](xref:Specs.Js.$2sxc) and the [sxc Controller](xref:Specs.Js.Sxc) make it really easy to use this. Best to get familiar with them. 


## About Security / Permissions

> [!WARNING]
> You are usually developing as a super-user, in which case these endpoints automatically work. If you want normal users to do certain things like create items, you need to [configure the permissions](https://azing.org/2sxc/r/k0YbVYXO).

To use these endpoints for normal users, you would need to enable the permissions and then you can access them using the REST URLs. Use this Checklist to get started. 


<iframe src="https://azing.org/2sxc/r/34pAzAF2?embed=1" width="100%" height="400" frameborder="0" allowfullscreen style="box-shadow: 0 1px 3px rgba(60,64,67,.3), 0 4px 8px 3px rgba(60,64,67,.15)"></iframe>


## Recommended Reading

* [General WebApi Specs](xref:Specs.WebApi.Intro)
* [URL Schema](xref:Specs.WebApi.UrlSchema)




## History

1. Introduced Content-REST API in 2sxc 5.x
1. Added [in-admin help](xref:WebApi.Help.AdminUiContentRest) to access it in 11.10
