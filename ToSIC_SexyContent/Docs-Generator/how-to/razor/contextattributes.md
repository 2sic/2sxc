---
uid: HowTo.DynamicCode.ContextAttributes
---

# ContextAttributes in Razor-Output

## Purpose / Description
Technically the entire Edit-UI is JavaScript based, so all the buttons, events etc. are client side scripts.

These scripts need to know what _Context_ they are in, meaning which DNN-Module, which 2sxc-App, which Zone, permissions etc. 

## How it works
The `Edit.ContextAttributes(...)` is always used inside an HTML-tag and will add some attributes with JSON. Any buttons or actions inside that tag will then find this information, and assume that it is has precendence over the global information.

## What do You need to do?
By default, this context is already provided by the 2sxc-environment, but sometimes a _new context_ must provide overrides. For example using @Concepts.InnerContent. 

For this you need the `Edit.ContextAttributes` - see [docs here](xref:ToSic.Sxc.Web.IInPageEditingSystem.ContextAttributes(ToSic.Sxc.Data.IDynamicEntity,System.String,System.String,System.String,System.Nullable{System.Guid})).


## How to use

This example shows the title and will add the standard editing-buttons for the `Content` item.

Here's an @Concepts.InnerContent example:

```html
<div class="app-blog-text sc-content-block-list" @Edit.ContextAttributes(post, field: "DesignedContent")>
    @foreach(var cb in @post.DesignedContent) {
        @cb.Render();
    }
</div>
```

In this example, the Edit.ContextAttributes will add some attributes with JSON, which will help the toolbars _inside_ that loop to correctly edit those items, and not the main item around it.



## Using ContextAttributes
These context-attributes enhance an HTML-tag, so that buttons inside that tag can be in a different context than the original context. 

Here's a common example: imagine you have a 2sxc-instance (a module showing 2sxc-data) and all the buttons there know the App-ID, the Zone, the Content-Type etc. Inside this module, you can have multiple items but they all still work well in the original context (all items are in the same app, so an edit-dialog will also know the AppId). 

## The parameters

1. `target` required  
    the content-item for which the new context should be. this item usually has a field (see the next property) which has @Concepts.InnerContent
1. `field` string  
    the field of this content-item, which contains the inner-content-items 
1. `contentType` string  
    this should not be used yet, it's for a future feature

## Notes and Clarifications

### Enforced Parameter Naming
To promote long term API stability, we require all parameters except for the first content-item `target` to be named when used - see [convention](convention-named-parameters).

```html
<!-- this will work -->
@Edit.ContextAttributes(target: postItem, field: "InnerContent")

<!-- this won't work -->
@Edit.ContextAttributes(postItem, "InnerContent")
```

## Read also

* @Concepts.InnerContent

## Demo App and further links

You should find some code examples in this demo App
* [Blog App](xref:App.Blog)


## History

1. Introduced in 2sxc 8.4
