---
uid: HowTo.Razor.Edit
---
# Edit / @Edit Object in Razor / .net


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

Here's an [](xref:Specs.Cms.InnerContent) example:

```html
<div class="app-blog-text sc-content-block-list" @Edit.ContextAttributes(post, field: "DesignedContent")>
    @foreach(var cb in @post.DesignedContent) {
        @cb.Render();
    }
</div>
```

In this example, the `Edit.ContextAttributes` will add some attributes with JSON, which will help the toolbars _inside_ that loop to correctly edit those items, and not the main item around it.

## What's In the Edit Object
The `Edit`-object is always available in all Razor-templates. Read the API: [](xref:ToSic.Sxc.Web.IInPageEditingSystem).

A short summary of what's inside

#### Check or Enable Editing Mode

* `Edit.Enable(...)`  
allows you to enable editing functionality even if it's not available by user permissions, see [more](xref:HowTo.Razor.Edit.Enable)

* `Edit.Enabled` (boolean)  
Tells you if it's edit-mode or not, allowing your code to output other things if edit is enabled.

#### Work with Toolbars

Creates a Toolbar, see [Razor Edit Toolbar](xref:HowTo.Razor.Edit.Toolbar).

* `Edit.TagToolbar(...)` attribute (_2sxc 9.40+_, recommended)
it is used inside a tag like  
`<div @Edit.TagToolbar(Content)>`  
to create best-practice hover toolbars

* `Edit.Toolbar(...)`  (_2sxc 8.04+_)  
is used like a tag (it generates an invisible `<ul>` tag) and is used for non-hover toolbars.

#### Create HTML Attributes if in Edit Mode

`Edit.Attribute` create any attribute on the condition that the user may edit the page, using  
`Edit.Attribute(name, string|object)`  
for use in things like  
`<div class="..." @Edit.Attribute("data-enable-edit", "true") >...</div>`

#### Create Context-Attributes for the UI (advanced use cases)

`Edit.ContextAttributes(...)`  
creates Context-Information for other edit-functionality. This is advanced, and currently only needed for [](xref:Specs.Cms.InnerContent)) - read about it on [Razor Edit.ContextAttributes](xref:Razor.ContextAttributes]



## Demo App and further links

You should find some code examples in this demo App
* [Blog App](xref:App.Blog)

## History

1. Introduced in 2sxc 8.04
2. Enhanced with [Enable(...)](xref:HowTo.Razor.Edit.Enable) method in 9.30
3. Enhanced with [TagToolbar(...)](xref:HowTo.Razor.Edit.Toolbar) in 9.40


