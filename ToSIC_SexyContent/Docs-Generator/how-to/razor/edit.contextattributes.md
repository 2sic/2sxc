---
uid: HowTo.Razor.ContextAttributes
---

# ContextAttributes in Razor-Output

Technically the entire Edit-UI is JavaScript based, so all the buttons, events etc. are client side scripts.

These scripts need to know what _Context_ they are in, meaning which DNN-Module, which 2sxc-App, which Zone, permissions etc. 

By default, this context is already provided by the environment, but sometimes a _new context_ must provide overrides. This is rare, but important, for example using [](xref:Specs.Cms.InnerContent). 

So the `ContextAttributes` will provide this information in some hidden html.

## How it works
The `Edit.ContextAttributes(...)` is always used inside an HTML-tag and will add some attributes with JSON. Any buttons or actions inside that tag will then find this information, and assume that it is has precendence over the global information.

## What do You need to do?
By default, this context is already provided by the 2sxc-environment, but sometimes a _new context_ must provide overrides. For example using [](xref:Specs.Cms.InnerContent). 

For this you need the `Edit.ContextAttributes` - see [docs here](xref:ToSic.Sxc.Web.IInPageEditingSystem.ContextAttributes*).


## How to use

This example shows the title and will add the standard editing-buttons for the `Content` item.

Here's an [](xref:Specs.Cms.InnerContent) example:

```html
<div class="app-blog-text sc-content-block-list" @Edit.ContextAttributes(post, field: "DesignedContent")>
    @foreach(var cb in @post.DesignedContent) {
        @cb.Render();
    }
</div>
```

In this example, the Edit.ContextAttributes will add some attributes with JSON, which will help the toolbars _inside_ that loop to correctly edit those items, and not the main item around it.

## How it works
The `Edit.ContextAttributes(...)` is always used inside an HTML-tag and will add some attributes with JSON. Any buttons or actions inside that tag will then find this information, and assume that it is has precendence over the global information.


## Using ContextAttributes
These context-attributes enhance an HTML-tag, so that buttons inside that tag can be in a different context than the original context. 

Here's a common example: imagine you have a 2sxc-instance (a module showing 2sxc-data) and all the buttons there know the App-ID, the Zone, the Content-Type etc. Inside this module, you can have multiple items but they all still work well in the original context (all items are in the same app, so an edit-dialog will also know the AppId). 

## Read also

* [](xref:Specs.Cms.InnerContent)

## Demo App and further links

You should find some code examples in this demo App
* [Blog App](xref:App.Blog)


## History

1. Introduced in 2sxc 8.4
