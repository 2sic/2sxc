---
uid: Concepts.Edit
---
# Overview: Edit Content or Data in DNN / 2sxc

## Purpose / Description
When users edit content they usually use in-page buttons to access edit-dialogs and more. Here is a short overview and links to what you need to know.

## The Standard Edit Dialogs
Editing mostly happens in stand-alone dialogs which are JavaScript based (using AngularJS). These dialogs are typically dialogs like

1. edit an item
1. edit a combination of items - like a _content_ item and an assigned _presentation-settings_ item

Note that other edit-actions happen in-page directly, like _move-up/down_ in a list etc.

The configuration of these edit-dialogs happens in the **Content Type** configuration, which automatically generates the correct dialog for the user. To understand this better, you may want to research

1. **Content types** which define what fields exist in the edit-dialog
1. **[Field Data Types](xref:Specs.Data.Type.Overview)** and **[Input Types](ui-fields)** which determine what options a field has and how it's stored
1. **[Custom input types](http://2sxc.org/en/Blog/post/custom-input-type-advanced-dynamic-data)** for special input types not provided by default
1. **[Presentation Settings](http://2sxc.org/en/docs/Separate-Presentation-Settings-from-Real-Content)** which tell the view how an item is to be shown, check also the [content/data differences](http://2sxc.org/en/blog/post/12-differences-when-templating-data-instead-of-content/source/dnnsoftware)
1. **View/Template configuration** which assigns certain content-types to Templates - check out this [tutorial](https://2sxc.org/en/Learn/Getting-started-with-creating-stuff/First-Content-Template)
1. **[Difference between Content and Data](http://2sxc.org/en/blog/post/12-differences-when-templating-data-instead-of-content)** and how it affects the in-page editing features


## All about Toolbars and Editing

1. In-Page Item Edit Toolbars
    1. Most of the concept is explained in [Concept InPage Toolbars](xref:Concepts.EditToolbar).
    1. You will usually create such toolbars from the Razor templates - read about [Edit.TagToolbar and Edit.Toolbar](xref:HowTo.Razor.EditToolbar)

2. The hovering insert-modules toolbar-system is called [quickE for quick-edit](xref:Concepts.QuickE). There you will also see how to customize the editing experience. 


## Advanced JavaScript & HTML Based Toolbars and Buttons
If you want to do more than the default toolbars do, you want to read about:

1. [Html toolbars and buttons](xref:Specs.Js.Toolbar.Intro) to customize the toolbars
2. [Toolbar settings](xref:Specs.Js.Toolbar.Settings) to configure alignment, hover etc.
3. [Buttons](xref:Specs.Js.Toolbar.Buttons) to understand in details how buttons work and how to customize them
4. There is also a more technical article if you want to see deeper into the [JavaScript](xref:Specs.Js.Toolbar.Js).
5. [Commands](xref:Specs.Js.Commands) to understand which commands the CMS can run, and how to call them from normal links
6. [Custom Code Buttons](xref:Specs.Js.Commands.Code) to create buttons which run your custom code


## Read also
1. Blog post about [Introducing Shake - Mobile Content Editing just turned sexy](http://2sxc.org/en/blog/post/introducing-shake-mobile-content-editing-just-turned-sexy)
2. Blog post about [Toolbars for Designers and Developers](http://2sxc.org/en/blog/post/toolbar-for-designers-and-devs-in-2sxc-8-6)
3. Blog post about [Customize in-page toolbars](http://2sxc.org/en/blog/post/customize-edit-toolbar-hover-alignment-more-button-look-and-feel)
4. Blog post about [Calling commands from links](http://2sxc.org/en/blog/post/create-links-which-run-cms-commands-new-2sxc-8-6)