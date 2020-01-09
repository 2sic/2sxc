---
uid: HowTo.Razor.Blocks
---

# Razor: Rendering Content-Blocks

When using [Inner Content](xref:Specs.Cms.InnerContent) the linked content-blocks (apps or pieces of content) must be rendered into the template. Here's how to do it in Razor. 

## How to use
There are two common use cases

1. Virtual Panes at item level - in this case, the item has it's own pane for placing apps and content-blocks
2. Very Rich Text - where you add content-blocks and apps in the WYSIWYG

## Item-Level Virtual Panes
Here's a quick example which renders an area with all content-blocks: 

```razor
@ToSic.Sxc.Block.Render.All(Content, field: "InnerContent")
```

This example creates the area for the content-blocks (important so that the UI appears for editors to add more blocks) and renders all existing content-blocks in the predefined order into that area. 

Here's a more manual example of doing the same thing, but done manually to demonstrate what happens and to allow myself to add more css-classes: 

```html
<div class="some-class sc-content-block-list" @Edit.ContextAttributes(post, field: "DesignedContent")>
    @foreach(var cb in @post.DesignedContent) {
        @cb.Render();
    }
</div>
```
If you care about doing it manually, read more about the [Edit object](xref:HowTo.Razor.Edit).

## Very Rich Text / Inner-Content

Here's an example how to merge content-blocks with a html-text which has placeholders for each content-block, allowing a mix of text/apps. 

```razor
@ToSic.Sxc.Blocks.Render.All(post, field: "WysiwygContent", merge: post.Body)
```

There is a new parameter merge, where you fill in your WYSIWYG-field that contains the content-block(s).

Here is a [blog tutorial on implementing Very Rich Text](http://2sxc.org/en/blog/post/tutorial-create-very-rich-text-inner-content-2-with-2sxc).


## Notes and Clarifications

The Edit-Object is of type [](xref:ToSic.Sxc.Web.IInPageEditingSystem).

## Read also
* Read more on [Razor Edit.ContextAttributes](xref:Razor.ContextAttributes]

## Demo App and further links

You should find some code examples in this demo App

* [Blog App](xref:App.Blog)

## History

1. Introduced in 2sxc 8.4
2. Clean API and merge capabilitien in 8.9


