---
uid: Specs.Js.Commands.ContentItems
---
# Html & Js: contentitems Command

This button opens the admin-dialog with all content-items. It has a (beta) feature which also allows you to add filters (since 2sxc 8.7).

## How to use
Here's a basic example showing a 2sxc-toolbar with a custom code:

```razor
@* Example using hover TagToolbar (recommended) *@
<div @Edit.TagToolbar(toolbar: new {
    action = "contentitems",
    contentType= "Tag",
    filters = new {  ManualWeight = 2 })>
...
</div>

@* Example using inline Toolbar (not recommended) *@
<div>
    @Edit.Toolbar(toolbar: new {
        action = "contentitems",
        contentType= "Tag",
        filters = new {  ManualWeight = 2 })
...
</div>
```

This shows a button which opens the table with all **Tag** items and filters for ManualWeight = 2. Let's try a more complex setup:

```razor
@* Example using hover TagToolbar (recommended) *@
<div @Edit.TagToolbar(
    toolbar: new object[] {
        new { action = "contentitems",
            contentType= "Tag",
            filters = new {  ManualWeight = 2 }
        },
        new { action = "contentitems",
            contentType= "BlogPost",
            filters = new {Tags = new[] { tag.Tag } }
        }
    },
    settings: new { show = "always" })>
...
</div>
```

This shows 2sxc-toolbar with 2 buttons, one opening tag-management (filtered by ManualWeight), the other opens all BlogPosts filtering by Tag. It also has some settings which always show it, even if the mouse is not hovering.

## Possible filters on contentitems

The filters-object is a JS-object with properties. We're still working on the format, but for now it's probably

1. NumberPropertyName: ##
2. StringPropertyName: "..."
3. BoolPropertyName: true
4. EntityPropertyName: ["title1", "title2", ...]
5. IsPublished: true
6. IsMetadata: true



## Read also
* [commands](xref:Specs.Js.Commands)

## Demo App and further links
* [Mobius Forms](https://2sxc.org/en/apps/app/mobius-forms) uses the contentitems command to let the admin see the forms-records for his use case only

## History
1. Introduced in 2sxc v08.06
2. Filters introduced in 2sxc v08.08
