---
uid: Specs.Js.Toolbar.Settings
---
# Html & JS: Toolbar Settings

Each in page toolbar can have some settings which control how it works and how it's shown. 

_note: there is a [blog post on using toolbar settings](http://2sxc.org/en/blog/post/customize-edit-toolbar-hover-alignment-more-button-look-and-feel)._

## How to use
Here's a quick example:

```Html
<div style="height: 100px" class="sc-element">
    100px tall area to show alignments floating left with more to the left
    <ul class="sc-menu" data-toolbar='' settings='{ "hover": "left", "align": "left" }'></ul>
</div>
```

The above example will hover the toolbar when the mouse moves over this DIV but place it to the left `hover: "left"` and place the _more_ button on the left side as well `align: "left"`.

## How it works
Internally the toolbar configuration has a very flexible structure, so internally it will look a bit like

```javascript
{
    groups: [...], // array of groups
    settings: {
        hover: "...",
        autoAddMore: ...,
        show: "..."
    },
    ... // more stuff
}
```

In many cases the `<ul toolbar="...">` tag will be very simple, and adding these _settings_ would be very complex. Because of this, we added the attribute `settings="{json}` which is merged into that object. 

## Settings and Values

1. string `autoAddMore`: **(null)** | "right" | "left"  
will automatically add a "..." (more) button if multiple button groups are detected
1. string `hover`: **"right"** | "left" | "none"  ("center" ist still beta)
determines where the toolbar appears when the mouse hovers over the area (usually a DIV) with the class `sc-element`
1. string `follow` _new in 11.06_ **"none"** | "initial" | "scroll" | "always"  
tells the toolbar to follow the mouse - ideal for large content blocks where you need the toolbar even if otherwise it would be off-screen. _Note: this used to default to `scroll` on the `TagToolbar` but was changed to `none` in 2sxc 11.06 because it caused too many UX issues._
1. string `show`: **"hover"** | "always"  
by default any toolbar inside an element with a `sc-element` class will appear on hover
1. string `classes`: **(null)** | _any kind of string_   

## Read also

* [InstancePurpose](xref:HowTo.Razor.Purpose) - which tells you why the current code is running so you could change the data added
* [CustomizeData](xref:HowTo.Razor.CustomizeData)

## Demo App and further links

You should find some code examples in this demo App
* [FAQ with Categories](http://2sxc.org/en/apps/app/faq-with-categories-and-6-views)

More links: [Description of the feature on 2sxc docs](http://2sxc.org/en/Docs-Manuals/Feature/feature/2683)

## History

1. Introduced in 2sxc ??.??
2. 

