# @Edit.ContextAttributes(...) Method in Razor / .net

## Purpose / Description
Technically the entire Edit-UI is JavaScript based, so all the buttons, events etc. are client side scripts.

These scripts need to know what _Context_ they are in, meaning which DNN-Module, which 2sxc-App, which Zone, permissions etc. 

By default, this context is already provided by the environment, but sometimes a _new context_ must provide overrides. This is rare, but important, for example using [inner-content][inner-content]. 

So the `ContextAttributes` will provide this information in some hidden html.


## How to use

This example shows the title and will add the standard editing-buttons for the `Content` item.

Here's an [inner-content][inner-content] example:

```html
<div class="app-blog-text sc-content-block-list" @Edit.ContextAttributes(post, field: "DesignedContent")>
    @foreach(var cb in @post.DesignedContent) {
        @cb.Render();
    }
</div>
```

In this example, the Edit.ContextAttributes will add some attributes with JSON, which will help the toolbars _inside_ that loop to correctly edit those items, and not the main item around it.


## How it works
[//]: # "Some explanations on the functionality"
The `Edit.ContextAttributes(...)` is always used inside an HTML-tag and will add some attributes with JSON. Any buttons or actions inside that tag will then find this information, and assume that it is has precendence over the global information.

## Using ContextAttributes
These context-attributes enhance an HTML-tag, so that buttons inside that tag can be in a different context than the original context. 

Here's a common example: imagine you have a 2sxc-instance (a module showing 2sxc-data) and all the buttons there know the App-ID, the Zone, the Content-Type etc. Inside this module, you can have multiple items but they all still work well in the original context (all items are in the same app, so an edit-dialog will also know the AppId). 

## The parameters

1. `target` required  
    the content-item for which the new context should be. this item usually has a field (see the next property) which has [inner-content][inner-content]
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

* [About Inner Content][inner-content]

## Demo App and further links

You should find some code examples in this demo App
* [Blog App](http://2sxc.org/en/apps/app/dnn-blog-app-for-dnn-dotnetnuke)


## History

1. Introduced in 2sxc 8.4

[//]: # "The following lines are a list of links used in this page, referenced from above"
[inner-content]: http://2sxc.org/en/blog/post/designing-articles-with-inner-content-blocks-new-in-8-4-like-modules-inside-modules
[DynamicEntity]: Dynamic-Entity
[actions-source]: https://github.com/2sic/2sxc/blob/master/src/inpage/2sxc._actions.js
[template-content-data]: http://2sxc.org/en/blog/post/12-differences-when-templating-data-instead-of-content
