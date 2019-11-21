# Concept: In-Page Edit-Item Toolbars and Buttons

## Purpose / Description
2sxc provides super-fast in-page buttons and toolbars for the content-editor to manage and edit everything. The system is very advanced, so what at first may seem trivial can become very complex as you go deeper into customizing it. This is why we are providing an overview.

![hover inline toolbar example](./assets/concepts/inpage-toolbar/example-hover-toolbar.png)


## Best Practice: Hover vs. Inline Toolbars

The toolbars for the editor can be hovering above the elements - usually appearing on mouse-over, or directly inline. Hover-toolbars can also be configured to always be visible, but this is a rare use case.

> **Recommendation**  
> Hover-Toolbars are highly recommended. These appear when the mouse moves over the item to be edited. It gives the editor a perfect preview of the page, without cluttering the screen with buttons.
>
> Toolbars which hover and are always visible make sense for buttons which the editor may not discover on his own if they are not always visible, or when the item to hover on would be very small, and hard to discover.
>
> Inline (non-hovering) toolbars which are always visible should be used as rarely as possible. A good use for this are admin-tables where each row should directly show the edit-button in a buttons-column.

## How the System Work

To make the magic work, these parts are involved:

1. Server side code detects that editing is allowed. This automatically adds **context-information** to the module in the form of a hidden JSON attribute. You can see these large attributes in the source-code if you are logged in. It also ensures that the edit-javascripts are loaded by the page.
    1. This is either auto-detected, because the user has edit-rights
    2. Or it was explicitly enabled using [`@Edit.Enable(...)` in razor](razor-edit.enable).

2. Server side code adds stuff to mark where toolbars should appear...
    1. ...either (new in 2sxc 9.40) it adds hidden JSON attributes called `sxc-toolbar='{...}'` to any tag that should have a hover-menu with the desired **configuration** and **settings**, added like this  
    [`<div @Edit.TagToolbar(...)>...</div>` in razor](razor-edit.toolbar)
    1. ...or it adds an empty `<ul toolbar='{...}' settings='{...}>` tag to the page, also with the **configuration** and **settings**, added using [`@Edit.Toolbar(...)` in razor](razor-edit.toolbar) or the `[Content.Toolbar]` equivalent in [tokens](http://2sxc.org/en/Learn/Token-Templates-and-Views). These toolbars can also hover using an old convention of adding an `sc-element` class, but that is deprecated since 2sxc 9.40.

3. JavaScript on the client looks at the HTML and picks up both the special `<ul>` tags as well as all tags having the `sxc-toolbar` attribute, reads the configuration and generates the necessary html-tags with the buttons and hover-effects. Based on the **context-information** and the **configuration** it will choose if advanced button should appear (like edit-template, which only admins should see). Special hover-placement and other visual things are picked up from the **settings**.

4. If an editor interacts with the menu, each click will result in JavaScript looking at the _closest_ **context-information** in the page, and using this information to run a [command](html-js-commands) like `edit`, `new` or `template-develop`.

5. When a command has completed, certain code may reload the view, either using ajax or by reloading the page if ajax is not supported by this particular view.


## Short Example
The toolbar system is 100% JavaScript but offers special helpers to improve the experience in other dev-environments. Here's a simple example using Razor:

```Razor
<h1 @Edit.TagToolbar(Content)>
  @Content.Title
</h1>
```

This creates a invisible toolbar which appears and hovers when the mouse moves over the `<h1>` tag (if the user has edit-permissions). The toolbar has all the buttons for this `Content` item.

The next example is similar, but instead of all default buttons it only shows the edit-button:

```Razor
<h1 @Edit.TagToolbar(Content, actions: "edit")>
  @Content.Title
</h1>
```

## Basic Toolbar Concepts and Functionality
1. each toolbar is specific to a content-item
2. a page can have many toolbars, each for a different element / purpose
3. usually toolbars are invisible until the mouse hovers over the area to be edited (best practice, but configurable)
4. various show and hover/float behaviors
5. each toolbar is fully customizable - both in regards to which buttons are shown as well as how they are grouped, how they look etc.
6. toolbars are multi-language
7. easy to add in your template or to your JS Apps
8. mobile capable with [shake support](http://2sxc.org/en/blog/post/introducing-shake-mobile-content-editing-just-turned-sexy)


## How to Use
We'll try to provide you with full details of the toolbars for advanced use cases. But in most cases you will need the default toolbars minimal or no customizations. For these common cases you should continue on.

* [Razor @Edit.TagToolbar(...) attribute](Razor-Edit.Toolbar) (hovering, recommended)
* [Razor @Edit.Toolbar(...)](Razor-Edit.Toolbar) (non-hovering)
* [Token Toolbars](http://2sxc.org/en/Learn/Token-Templates-and-Views) (using simpler placeholder templates)


## Core JavaScript Architecture Parts

### Commands
This is what is executed when a button is clicked. Commands are things like `edit`. Some commands need additional parameters like `EntityId`, resulting in a command more like `run('edit', {EntityId: 27})`.  Commands can also be run without toolbars, for example from _edit_ links in tables etc.

You can read more about Commands, incl. the full list of current command, parameters and how to create custom commands in the [Commands](Html-Js-Commands) section.


### Buttons
This is a square thingy with an icon, which is will run a _Command_. When the button is created, it is fully configured with icon, commmand and command-parameters.

1. **Button-Group**: This is a set of one or more -Buttons_ which are shown together. Often there will be a `more` button at one end of the set, which will show another button-group when clicked.

1. **Toolbar**: This is a set of one or more _Button-Groups_.

1. **Toolbar Builder**: This is an API-layer which builds the HTML for the _Toolbar_.

1. **Toolbar Bootstrapper**: This will pick up HTML placeholders for toolbars and run the _Toolbar Builder_ for these.

1. **Defaults**: For the entire chain to work properly, various initial configurations


## Read also
* [quickE - the quick-edit hover toolbar](concept-quick-edit) - the quick-edit hover-toolbar for inserting/moving modules
* [Inner Content Blocks](http://2sxc.org/en/blog/post/designing-articles-with-inner-content-blocks-new-in-8-4-like-modules-inside-modules) - blog about inner content-blocks


## Demo App and further links
You should find some code examples in this demo App
* [2sxc blog](http://2sxc.org/en/apps/app/dnn-blog-app-for-dnn-dotnetnuke)


## History
1. Added toolbars in 2sxc 1.0 ca. 2011
1. hundreds of ongoing optimizations
1. Added new feature with the `Edit.TagToolbar` which works using an `sxc-toolbar` attribute instead of an `<ul>` tag in 2sxc 9.40. In this version we also changed the CSS functionality to not use the `sc-element` attribute, but still support it for backward compatibility.
