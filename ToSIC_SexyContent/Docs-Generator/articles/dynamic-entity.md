---
uid: Articles.DynamicEntity
---
# How to Use a Dynamic Entity

Whenever you create a content-type - like _Person_ - and want to work with the data in your C# Razor templates, you'll be working with a _Dynamic Entity_. The main code for this is the [DynamicEntity.cs][link-main-code] and also the [Entity.cs][link-entity-code].

## Code example using a Dynamic Entity
We'll assume we have a _Content Type_ called *Book* with the following properties:
* Title (text, 1-line)
* Teaser (text, multi-line)
* Description (text, html)
* ReleaseDate (date)
* Author (entity - another content item)

Here's a code example in a C# Razor template:

```html
<!--
  The default variable for the current item is Content, 
  we'll just use another name for this sample
  note that .Title is automatically provided, 
  because the content-type has the property title. 
-->
<h1>@Content.Title</h1>
<div>@Content.Description</div>
<div>Author: @Content.Author[0].FullName</div>
```
So basically all properties of this book can be shown using `[Object].[PropertyName]` - for example `Content.ReleaseDate`.

## What Dynamic Entity really does - and how...
Technically the dynamic entity object is like a read-helper for the more complex @ToSic.Eav.Interfaces.IEntity. So actually the _dynamic entity_ object will keep a reference to the underlying read-only `IEntity` item on a property `Entity`, and whenever your code accesses a property, the dynamic entity will query it from the underlying `Entity`.

The main things that the _dynamic entity_ does for you, are

1. Give you a nice, short syntax to access a property - so `Content.FirstName` instead of `Object.Attributes["FirstName"]["en"]` which would be necessary using the more advanced `IEntity` object
2. Ensure that the language used in retrieving a value is the current user language
3. Give conveniant access to related information like the `Presentation` object
4. Automatically handle some data-not-found errors
5. Automatically do conversions, like convert related entities (like `.Children`) into dynamic objects to make your code more consistant  

## Properties of a Dynamic Entity

Read the API docs in the @ToSic.Sxc.Interfaces.IDynamicEntity.

Additional properties that work (they are dynamic, so don't appear in the code)

1. **EntityType** _string_ - the type name like _Person_
1. **IsPublished** _bool_ - true/false if this item is currently published
1. **_AnyProperty_** _dynamic, but actually bool | string | decimal | datetime | List<DynamicEntity>_ any normal property of the content-item can be accessd directly. It's correctly .net typed (string, etc.)

The following Methods exist on all dynamic entities

1. **Render()** - will render HTML for the current item, if there is a configuration for this. Almost always returns a simple HTML-comment, unless used as inner-content (added in 2sxc 8.3)

## Working with unpublished/draft items
TODO: write something about how-to-check if published/unpublished, navigating it, etc. - or link to such a page

## Appendix
The following properties/methods exist, but shouldn't be used. They are documented here so that you know that they are not meant for public use:

1. Created - the created date
2. Author - the person who created this item
3. Owner - the current owner of the item, usually the author
1. Metadadata - currently use `AsEntity(theObject).Metadata`
4. Permissions - permissions of the current item (if any are defined)

## History

1. Introduced in 2sxc 01.00
1. Changed to use interface IDynamicEntity in 6.x
1. Draft/Published introduced in 2sxc 7.x
1. Presentation introduced in 2sxc 7.x
1. Modified introduced in 2sxc 8.x
1. Implemented .net equality comparer in 2sxc 9.42
1. Parents added in 2sxc 9.42
1. Get added in 2sxc 9.42 and added to interface IDynamicEntity in 10.07
1. Parents introduced in 2sxc 9.42, and added to interface IDynamicEntity in 10.07
1. IsDemoItem property added in 2sxc 10.06


[link-main-code]:https://github.com/2sic/2sxc/blob/master/SexyContent/DynamicEntity.cs
[link-entity-code]:https://github.com/2sic/eav-server/blob/master/ToSic.Eav.Core/Entity.cs