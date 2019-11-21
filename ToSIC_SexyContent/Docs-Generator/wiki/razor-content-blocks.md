
# Razor: Rendering Content-Blocks

## Purpose / Description
When using [Inner Content](Concept-Inner-Content) the linked content-blocks (apps or pieces of content) must be rendered into the template. Here's how to do it in Razor. 

## How to use
There are two common use cases

1. Virtual Panes at item level - in this case, the item has it's own pane for placing apps and content-blocks
2. Very Rich Text - where you add content-blocks and apps in the WYSIWYG

## Item-Level Virtual Panes
Here's a quick example which renders an area with all content-blocks: 

```razor
@ToSic.SexyContent.ContentBlocks.Render.All(Content, field: "InnerContent")
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
If you care about doing it manually, read more about the [Edit object](Razor-Edit).

## Very Rich Text / Inner-Content
_this is new in 2sxc 8.9_

Here's an example how to merge content-blocks with a html-text which has placeholders for each content-block, allowing a mix of text/apps. 
```razor
@ToSic.SexyContent.ContentBlocks.Render.All(post, field: "WysiwygContent", merge: post.Body)
```
There is a new parameter merge, where you fill in your WYSIWYG-field that contains the content-block(s).

Here is a [blog tutorial on implementing Very Rich Text](http://2sxc.org/en/blog/post/tutorial-create-very-rich-text-inner-content-2-with-2sxc).


## Notes and Clarifications
### Object and Interfaces
The Edit-Object is of type `ToSic.SexyContent.Edit.InPageEditingSystem.IInPageEditingSystem`.

## Read also
* Read more on [Razor Edit.ContextAttributes](Razor-Edit.ContextAttributes)


## Demo App and further links
[//]: # "Apps which provide sample code using this"

You should find some code examples in this demo App
* [Blog App](http://2sxc.org/en/apps/app/dnn-blog-app-for-dnn-dotnetnuke)

## History
[//]: # "If possible, tell when it was added or modified strongly"

1. Introduced in 2sxc 8.4
2. Clean API and merge capabilitien in 8.9

[inner-content]: http://2sxc.org/en/blog/post/designing-articles-with-inner-content-blocks-new-in-8-4-like-modules-inside-modules
[DynamicEntity]: Dynamic-Entity
[actions-source]: https://github.com/2sic/2sxc/blob/master/src/inpage/2sxc._actions.js
[template-content-data]: http://2sxc.org/en/blog/post/12-differences-when-templating-data-instead-of-content
[float-toolbar]: http://2sxc.org/en/Docs-Manuals/Feature/feature/2875

