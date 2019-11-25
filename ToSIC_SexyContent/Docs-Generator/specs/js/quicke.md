---
uid: Specs.Js.QuickE
---
# Html & JS: quickE - Quick Edit 2.0

quickE (pronounced quicky) is the quick-edit feature inside 2sxc to quickly add / move modules and inner content blocks.
This page explains the In-Browser things you must know, incl. JSON configuration

## Concept and Background
For this please read [Concept-Quick-Edit](xref:Specs.Cms.QuickE), which also explains how to include it in a Skin etc.

## How to use
Assuming that the $quickE is already on the page and you're logged on, there is not much to do for now. 

Note that the internal API of $quickE is still prone to change, so anything you do which isn't officially documented could brake on a future upgrade. 

## Configuring quickE Quick-Edit in HTML
Since quickE 2.0 (released in 2sxc 8.7) you can now also configure it a bit. Here are the most important features

1. enable / disable the entire quickE
1. enable / disable module quick-edit
1. enable / disable inner-block quick edit

To do this, just add a `quick-edit-config="{...}"` attribute to any DOM node with the desired configuration. Here's what will happen

1. if you add it to a skin-dom-node, you can set "defaults" like "never enable at all"
1. if you add it to a view-template, you can change the behavior if that view is included

## Why include configuration it in a View?
As explained above, quickE changes it's behavior by default if it finds inner-content. This is desired on details-views, but not on templates which just create more complex content on a normal page. So on these templates, you could add a config to continue enabling module-quick-edit.

## The Configuration Object Structure
```JavaScript
config = {
  enable: true, // default
  modules: {
   enable: null // auto-detect, can also be true/false
  },
  innerBlocks: {
   enable: null // auto-detect, can also be true/false
  }
}
```

## A Quick Razor Example
If you want to add this attribute only when the user is editing the page, the best way to do it is like this:
```razor
<div @Edit.Attribute("quick-edit-config", new { modules = new {  enable = true }})>
  ...
</div>
```

## Read also
* [Inner Content Blocks](http://2sxc.org/en/blog/post/designing-articles-with-inner-content-blocks-new-in-8-4-like-modules-inside-modules) - blog about inner content-blocks

## Demo App and further links
You should find some code examples in this demo App
* [Accordion (collapsible sections)](https://github.com/2sic/app-accordion-bootstrap3)
* [2sxc blog](xref:App.Blog)

## History
1. Introduced in 2sxc v08.04
2. Enhanced / made configurable in 2sxc 08.06.01
