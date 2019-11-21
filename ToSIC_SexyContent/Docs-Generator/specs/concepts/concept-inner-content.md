# Concept: Inner Content 2.0

## Purpose / Description
Inner Content is the feature to place content-blocks (apps or common content-items) _inside_ another content-item. 

There are two common scenarios for this:

1. Apps with list/details views, with a **feature rich details-view**. For example: a blog, which lets the editor add galleries and other apps into each blog-post. This is called a [mashup app](https://en.wikipedia.org/wiki/Mashup_(web_application_hybrid)) as it mixes apps together
2. Apps which **wrap** other content/apps, for example an [accordion-app][accordion] which lets the editor add further content-blocks or apps into each collapsing segment.

## Basic Concept
_Inner Content_ links other content-blocks (apps, content) to a _Content Item_, allowing the template to then show this inner content where it wants to. 

## An Item can have Many Sets of Inner-Content
A template can have multiple sets of inner content. For example: 

1. a real-estate app showing a details-page with 2 columns, each containing an variety of content-items, different from house-item to house-item. One house could have a gallery-app in the first column, while the other house would have a 3D walkthrough app and preferr to use the gallery in the second column
2. a Catalog app containing many WYSIWYG text blocks (ProductDescription, ShowCase, ApplicationSuggestions) shown in multiple tabs, each WYSIWYG containing apps inline between the paragraphs. 

To achieve this flexibility, the _Inner Content_ items are linked not only to a _Content Item_ but to a specific _field_ in that item. So each set of related items is stored in one field, and by using multiple fields you can have multiple sets of items. This allows the template to handle each set separately. 

## Standalone Inner-Content - like a DNN Pane just for this item
A common use case is to provide the editor with an area into which he/she can add as many content-blocks/apps as they want to. This feels like a DNN-pane - the editor just adds apps as he needs them. 

This mode is common for mashup-apps and is mostly used on sophisticated details-pages with a clear area containing additional functionality. It's also common in
layout-changing apps like the [accordion app][accordion].

## In WYSIWYG Inner-Content
_this is new in 2sxc 8.9_

Sometimes you may let the editor add inner-content blocks within a wysiwyg-field, so that normal written content can be interspersed with apps. This is common in news-style apps or blogs, where additional features are needed (galleries, code-snippets, etc.) but mixed with the main text and not in a separate area. 

Here is a [blog tutorial on implementing Very Rich Text](http://2sxc.org/en/blog/post/tutorial-create-very-rich-text-inner-content-2-with-2sxc).

## The Parts that Make it Work
For _Inner Content_ to work, the following parts play together:

1. **Data storage**: you need fields in your content-type to link to the external content blocks. Just create entity-fields and use the type _Content Block Items_. In many cases you also want to hide the field because it's not important to the content-editor. Just go to the field-settings and set _Visible in Edit UI_ to off. 
2. **Content-Block rendering** in the templates, using the [Razor API](Razor-Content-Blocks) - it is prepared both for the standalone-area-mode as well as the merge-with-wysiwyg-mode. 
2. **In-Page Editing**: to allow the editor to add / edit content-blocks in the normal view, the UI must support it. This is handeled automatically by [Quick-Edit](Concept-Quick-Edit). 
3. To do the in-page Editing, quickE needs to know some **context information** (what field to store the links in, etc.), provided in an HTML-attribute. It's either rendered in automatically when using the [Razor commands](Razor-Content-Blocks) or you can manually place them using [context attributes](Razor-Edit.ContextAttributes).
4. If you want the **WYSIWYG-integration** the wysiwyg must know which field to use to manage the linked content-blocks. This is simply done by convention: as soon as a content-block field follows right after a wysiwyg-field, they will be linked and the button will appear. This also works it the content-block field is set to invisible. 

## Read also
* [Inner Content Blocks](http://2sxc.org/en/blog/post/designing-articles-with-inner-content-blocks-new-in-8-4-like-modules-inside-modules) - blog about inner content-blocks V1 - the _Area Mode_

## Demo App and further links
You should find some code examples in this demo App
* [2sxc blog](http://2sxc.org/en/apps/app/dnn-blog-app-for-dnn-dotnetnuke)

## History
1. Inner Content 1.0 in 2sxc v08.04
2. Enhanced Razor API in v08.09
3. WYSIWYG mode in v08.09


[accordion]:http://2sxc.org/en/apps/app/accordion-for-collapsible-sections