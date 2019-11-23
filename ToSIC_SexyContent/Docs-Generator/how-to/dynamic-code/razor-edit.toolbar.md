
# @Edit.Toolbar(...) Method in Razor / .net
2sxc has a cool in-page toolbar system - here you'll find a [conceptual overview around the toolbar](xref:Concepts.EditToolbar). These toolbars are usually hover-buttons like this:

![hover inline toolbar example](/assets/concepts/inpage-toolbar/example-hover-toolbar.png)

Technically the entire Edit-UI is JavaScript based, so all the buttons, events etc. are client side scripts. Writing this JS would be complicated to say the least, so the `@Edit.TagToolbar(...)` and `@Edit.Toolbar(...)` are your tools of choice for adding toolbars from Razor.

## How to use

Here's a quick example of using the `Edit` object in a Razor template:

```razor
<h1 @Edit.TagToolbar(Content)>
  @Content.Title
</h1>
```

This example will output the item title in an `h1` tag and add a hidden, appear-on-mouse-over toolbar with editing-buttons for the item named `Content`.

Let's assume you're building the details-page of a news-app and you only want the edit/remove buttons, to improve the UI for your use case. Additionally, you want the mouse-hover to react on the whole article, not just oven the title. Here's how:

```razor
@* this will show an "edit and remove" button for the current item *@
<div @Edit.TagToolbar(Content, actions: "edit,remove")>
  <h1>
    @Content.Title
  </h1>
  @Html.Raw(Content.Body)
</div>
```

Here's a different example, how to create a toolbar with only one button, namely an _add new item_ button to create a new BlogPost-item.

```razor
@* this will show an "add" button if the current user is an editor *@
<h1 @Edit.TagToolbar(actions: "new", contentType: "BlogPost")>
  @Content.Title
</h1>
```

As you can see, the `actions: "new"` tells the toolbar to only show this one button, while the `contentType: "BlogPost"` says what content-type this new item should be. As this toolbar won't have buttons that modify an existing item, it doesn't need that parameter.



## How it works
This command is part of the [Edit](Razor-Edit) object and used in Razor templates. It provides a simple API to generate in-page buttons if the current user is an editor.

It also checks if edit should be enabled (based on security specs) and will then generate some HTML/JavaScript at that location. 

## Common Tasks
Here are a few snippets that you'll typically need, saving you from reading all the docs in common scenarios:

1. `Edit.Toolbar(employee)` creates a default toolbar for the content-item `employee` with all default buttons like edit, change-view, more, etc.
2. `Edit.Toolbar(employee, actions: "edit")` creates a toolbar for the item `employee` but only with the `edit`-button.
3. `Edit.Toolbar(employee, actions: "edit,add,remove")` creates a toolbar with three buttons `edit`, `add`, `remove`
4. `Edit.Toolbar(actions: "new", contentType: "BlogPost")` creates a toolbar with one button, namely `new` which will open a new `BlogPost` form.
5. `@Edit.Toolbar(actions: "new", contentType: "BlogPost",  prefill: new { Title = "Hello", Color = "red" } )` creates a toolbar with one button, namely `new` which will open a new `BlogPost` form, and prefills the `Title` and `Color` field.

TODO TODO MORE SIMPLE EXAMPLES

## Edit.Toolbar API
The `Edit.Toolbar()` has these parameters 

1. `target` a [DynamicEntity][DynamicEntity] content-item, _optional_  
this is a content item which will be affected by the toolbar
2. `actions` string, _optional, must be [named](convention-named-parameters)_  
this is a comma-separated string overriding the default buttons which the toolbar will create, containing [command names](Html-Js-Commands). 
3. `contentType` string, _optional, must be [named](convention-named-parameters)_  
this is necessary if you want a _new_ button which would open the dialog to create a new item, and you don't have a target-item which already tells the system what type to use. This allows you to create a button for a new "Category" and another button for a new "BlogPost" etc.
4. `prefill` object, _optional, must be [named](convention-named-parameters)_  
allows a _new_ dialog to receive values as a prefill, for example to already specify a date, title, category, etc.
1. `toolbar` object, _optional, must be [named](convention-named-parameters)_ (v08.06+)  
a full toolbar configuration object - see the [JS documentation for Toolbar](Html-Toolbars-and-Buttons). If you use this, you cannot also specify _actions_, _contentType_ or _prefill_ as those would want to configure the same information. 
1. `settings` object, _optional, must be [named](convention-named-parameters)_ (v08.06+)  
the settings used by a toolbar like alignment - see the [JS documentation for settings](Html-Js-Toolbar-Settings)

## The Toolbar Actions
_Note:_ at the moment, the buttons are grouped into bundles like

1. initial buttons
2. list buttons
3. template / view buttons
4. app buttons


The actions can be a combination of known button-names. Here's the current [JavaScript catalog of commands][actions-source]: 

The following commands all require `target` to be set, or they only make sense in a List-setup - see also [content and not as data][template-content-data]. 

1. `new` open a dialog to create a new item, requires a `target` _or_ a `contentType` parameter
2. `edit` to edit the current item
1. `publish` will optionally show the publish-button, but only if the current item is not published.
1. `add` opens a dialog to create a new item just like new, but will add it below the current item in the [content list][template-content-data]
1. `remove` will remove (not delete) this item from the [content list][template-content-data]
1. `moveup` will move the item up one position in the [content list][template-content-data]
1. `movedown` will move the item down one position in the [content list][template-content-data]
1. `sort` will open the sort dialog of the [content list][template-content-data]
1. `replace` will open a dialog to swap the current item in the 
[content list][template-content-data]

_Note: the command `metadata` - is a bit special, not supported in the `actions` parameter - use the complex  `toolbar:` instead and read the instructions for the [JS Commands][actions-source]._

For many more commands you should check the [JS Commands][actions-source]), which covers many more like `app-import`, `layout`, `develop`, `contenttype`, `contentitems`, `app`, `zone`, `more` etc.  

## More About the Prefill
Basically this is a .net object which will be serialized to JSON and used to prefill a new item. Usually you'll just create a new, anonymous object like `new { Title = "xyz", Date = ... }`.

example
```Razor
@Edit.Toolbar(actions: "new", 
  contentType: "BlogPost", 
  prefill: new { Title = "Hello", Color = "red", Link = "page:" + Dnn.Tab.TabID } )
```

This example will prefill the title, the color and in the link field add a reference to the current page. 

TODO: get more examples with prefilling
* a related entity (like a category)
* a date

## Multiple Entities Prefil 
For this you must simply provide an array of strings, like this:
```Razor
@Edit.Toolbar(actions: "new", contentType: "BlogPost", prefill: new { Tags = new string[] {"08387a10-1aac-494a-9eb5-1383e31490eb","b498a694-047a-4e51-a285-50f9a64edda1"} })
```

## Styling the Toolbar
As of now there are only limited stying functions: 
### Floating the Toolbar
This happens automatically, if a surrounding HTML-tag has a class "sc-element". [more...][float-toolbar]

## What Really Happens with the toolbar
As previously noted, the toolbar actually puts some html/js into the page, which the javascript [$2sxc][$2sxc] object will pick up and work with. Quite a lot happens on the client, and that will be documented some other day. Here just the short version:

1. js runs, picks up Toolbars
2. jc reviews DOM to see what context it's in (either the module-instance or an inner-content)
3. js generates buttons
4. if a button is clicked, an action-manager then executes the correct action on the correct item 

## Notes and Clarifications

### Enforced Parameter Naming
To promote long term API stability, we require all parameters except for the first content-item `target` to be [named](convention-named-parameters) when used.

```html
<!-- this will work -->
@Edit.Toolbar(target: postItem, actions: "add,edit,more")
@Edit.Toolbar(postItem, actions: "add")

<!-- this won't work -->
@Edit.Toolbar(postItem, "add")

<!-- this will work -->
@Edit.Toolbar(actions: "new", contentType: "BlogPost")

<!-- this won't -->
@Edit.Toolbar("new", "BlogPost")
```

### Older @Content.Toolbar Syntax Is Deprecated
_Note_: there was an older `@SomeContentItem.Toolbar` syntax and this still works, but we ran into architecture issues, so we decided to place all advanced functions into the `Edit.Toolbar(...)` method. This is the way to go from now on, the old syntax will continue to work but is not recommended any more. 

## Read also

* todo

## Demo App and further links

You should find some code examples in this demo App
* [FAQ with Categories](http://2sxc.org/en/apps/app/faq-with-categories-and-6-views)

More links: [Description of the feature on 2sxc docs](http://2sxc.org/en/Docs-Manuals/Feature/feature/2683)

## History

1. .Toolbar() Introduced in 2sxc 8.04
2. .TagToolbar() introduced in 2sxc 9.40

(xref:Concepts.InnerContent): http://2sxc.org/en/blog/post/designing-articles-with-inner-content-blocks-new-in-8-4-like-modules-inside-modules
[DynamicEntity]: Dynamic-Entity
[actions-source]: Html-Js-Commands
[template-content-data]: http://2sxc.org/en/blog/post/12-differences-when-templating-data-instead-of-content
[float-toolbar]: http://2sxc.org/en/Docs-Manuals/Feature/feature/2875
[$2sxc]: JavaScript-$2sxc