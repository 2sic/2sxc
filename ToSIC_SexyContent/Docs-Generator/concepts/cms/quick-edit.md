---
uid: Concepts.QuickE
---
# Concept: quickE - Quick Edit 2.0

quickE (pronounced _quick-e_) is the quick-edit feature inside 2sxc to quickly add / move modules and inner content blocks. It supports touch and usually is used with a mouseover.

![Example showing quickE](/assets/concepts/inpage-toolbar/example-quicke.png)

In the current version, it allows you to do the following in normal view-mode:

1. add a content or app module on any position in a DNN panes
1. move / delete any DNN module using a copy-paste concept
1. send any DNN module to an empty pane (which couldn't do paste, as the target is invisible)
1. add a content or app content-block to any inner-content area
1. move / delete any content-block inside an inner-content area

Since version 2.0 you can also configure some of this at skin and template level. 

## How to use
By default any page that has _any_ 2sxc module added will automatically load quickE because it needs it to provide buttons/toolbars for inner content blocks.

We also recommend to include the JS in the skin by default, so that quickE is already enabled on empty pages as well. You can find the necessary asp.net web-control in our [Bootstrap Instant Theme](https://github.com/2sic/dnn-theme-bootstrap4-instant/blob/master/controls/2sxc-quickedit.ascx) Here's how:

```ascx
<%@ Register tagprefix="Edit" tagname="QuickEdit" src="~/DesktopModules/ToSIC_SexyContent/DnnWebForms/Skins/QuickEdit.ascx" %>
<Edit:QuickEdit runat="server" />
```

The above lines first tell .net that this control QuickEdit exists, and then adds it to the page using the `<Edit:...>` tag. Note that this tag won't create any HTML, it will just tell DNN that it must load the relevant JavaScripts when a user is logged on.
Once it's included, it will just work automatically.

## Auto-Disabled Module-Quick-Edit on Details Pages
By default quickE will change it's behavior if it finds inner-content blocks. The reason is that often inner-blocks are found on **child** pages of something, for example in blog-post details. If the user could insert both modules and inner-content on a details page, then the user would often _by accident_ insert modules (instead of content-blocks). The user would believe that he did the right thing, when in reality the newly added module would now show up on all other details-pages as well. 

Now there are cases where this auto-disable shouldn't happen - for example in accordeon-style modules which are used on normal pages. This can be configured as explained below:

## Configuring quickE Quick-Edit
Since quickE 2.0 (released in 2sxc 8.7) you can now also configure it a bit. Here are the most important features

1. enable / disable the entire quickE
1. enable / disable module quick-edit
1. enable / disable inner-block quick edit

Read more about this in the [Html Js documentation](Html-Js-$quickE) page


## Read also
* [Inpage Toolbars](xref:Concepts.EditToolbar) - the item-scoped toolbars used for editing
* [Inner Content Blocks](http://2sxc.org/en/blog/post/designing-articles-with-inner-content-blocks-new-in-8-4-like-modules-inside-modules) - blog about inner content-blocks

## Demo App and further links
You should find some code examples in this demo App
* [2sxc blog](xref:App.Blog)

## History
1. quickE 1.0 in 2sxc v08.04
2. quickE 2.0 with move/delete dnn module and configurable in 2sxc 08.07.00
