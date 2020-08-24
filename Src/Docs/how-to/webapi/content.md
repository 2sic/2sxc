---
uid: HowTo.WebApi.Content
---

# Content / Entity API in 2sxc

The `content` API endpoint allows your JavaScript code or external code to do basic CRUD operations like

1. Read a list of all items/entities
1. Read a single item/entity
1. Create an item/entity
1. Update an item/entity
1. Delete an item/entity

> [!TIP]
> Before you start, make sure you are familiar with the [Specs](xref:Specs.WebApi.Intro) and the [URL Schema](xref:Specs.WebApi.UrlSchema).

> [!WARNING]
> You are usually developing as a super-user, in which case these endpoints automatically work. If you want normal users to do certain things like create items, you need to [configure the permissions](https://azing.org/2sxc/r/k0YbVYXO).

## Read Content-Items/Entities

Assume you have the [blog-app](xref:App.Blog) installed and your JS would request a JSON from this endpoint (logged in as host, so security is not an issue):

`[root-path]/app/auto/content/BlogPost`

...then your JS would receive a JSON with all BlogPost items. 

Reading `[root-path]/app/auto/content/BlogPost/1050` would give you exactly one BlogPost item (with the id 1050)



## Create Content-Items/Entities

Doing an http POST to this `[root-path]/app/auto/content/BlogPost` with a POST body of `{ "Title": "changed title"}` would let you create the item. You will get a return message containing ID, GUID etc. of the new item. 

If your POST package also contains an `EntityGuid` then this will be used as the GUID for the new item. 

## Update Content-Items/Entities

Doing an http POST to this `[root-path]/app/auto/content/BlogPost/1050` with a POST body of `{ "Title": "changed title"}` would let you update the item 1050.

## Delete Content-Items/Entities

Doing an http DELETE to this `[root-path]/app/auto/content/BlogPost/1050` would delete the item 1050.


## JavaScript Helpers

The [$2sxc](xref:Specs.Js.$2sxc) and the [sxc Controller](xref:Specs.Js.Sxc) make it really easy to use this. Best to get familiar with them. 


## History

1. Introduced in 2sxc ca. 5.x

