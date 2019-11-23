---
uid: HowTo.DynamicCode.Edit
---
# Edit / @Edit Object in Razor / .net

## Purpose / Description

Technically the entire Edit-UI is JavaScript based, so all the buttons, events etc. are client side scripts. Writing this JS would be complicated to say the least, so the `Edit` object provides the Razor-Template an intelligent, fast way to generate what's necessary.

## How to use

Here's a quick example of using the `Edit` object in a Razor template:

```razor
<h1 @Edit.TagToolbar(Content)>
    @Content.Title
</h1>
<div>...</div>
```

This example shows the title and will add the standard editing-buttons for the `Content` item.

Here's an @Concepts.InnerContent example:

```html
<div class="app-blog-text sc-content-block-list" @Edit.ContextAttributes(post, field: "DesignedContent")>
    @foreach(var cb in @post.DesignedContent) {
        @cb.Render();
    }
</div>
```

In this example, the `Edit.ContextAttributes` will add some attributes with JSON, which will help the toolbars _inside_ that loop to correctly edit those items, and not the main item around it.

## How it works
The `Edit`-object is always available in all Razor-templates and either...

1. **Enabled State**

    1. `Edit.Enable(...)`  
    allows you to enable editing functionality even if it's not available by user permissions, see [more](razor-edit.enable)

    1. `Edit.Enabled` (boolean)  
    Tells you if it's edit-mode or not, allowing your code to output other things if edit is enabled.

1. **Toolbars** - creates a Toolbar, see [Razor Edit Toolbar](Razor-Edit.Toolbar).

    1. `Edit.TagToolbar(...)` attribute (_2sxc 9.40+_, recommended)
    it is used inside a tag like  
    `<div @Edit.TagToolbar(Content)>`  
    to create best-practice hover toolbars

    1. `Edit.Toolbar(...)`  (_2sxc 8.04+_)  
    is used like a tag (it generates an invisible `<ul>` tag) and is used for non-hover toolbars.

1. **Attributes** which are only added when edit is enabled

    1. `Edit.Attribute` create any attribute on the condition that the user may edit the page, using  
    `Edit.Attribute(name, string|object)`  
    for use in things like  
    `<div class="..." @Edit.Attribute("data-enable-edit", "true") >...</div>`

1. **Edit Context Information** for advanced use cases

    1. `Edit.ContextAttributes(...)`  
    creates Context-Information for other edit-functionality. This is advanced, and currently only needed for @Concepts.InnerContent) - read about it on [Razor Edit.ContextAttributes](Razor-Edit.ContextAttributes)



## Demo App and further links

You should find some code examples in this demo App
* [Blog App](xref:App.Blog)

## History

1. Introduced in 2sxc 8.04
2. Enhanced with [Enable(...)](razor-edit.enable) method in 9.30
3. Enhanced with [TagToolbar(...)](razor-edit.toolbar) in 9.40

(xref:Concepts.InnerContent): http://2sxc.org/en/blog/post/designing-articles-with-inner-content-blocks-new-in-8-4-like-modules-inside-modules
[DynamicEntity]: Dynamic-Entity
[actions-source]: https://github.com/2sic/2sxc/blob/master/src/inpage/2sxc._actions.js
[template-content-data]: http://2sxc.org/en/blog/post/12-differences-when-templating-data-instead-of-content
[float-toolbar]: http://2sxc.org/en/Docs-Manuals/Feature/feature/2875

