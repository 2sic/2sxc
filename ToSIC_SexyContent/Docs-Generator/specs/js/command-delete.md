---
uid: Specs.Js.Commands.Delete
---
# Html & Js: delete Command

## Purpose / Description 
This button let's a user really delete a content item (since 2sxc 8.9).

## How to use
Here's a basic example showing a 2sxc-toolbar with a custom code:

```razor
@Edit.Toolbar(toolbar: new { 
    action = "delete", 
    entityId = tag.EntityId, 
    entityGuid = tag.EntityGuid, 
    entityTitle = tag.EntityTitle })

```
The previous example just renders a delete-button. Here's an example creating an entire toolbar, incl. the delete-button.

```razor
@Edit.Toolbar(Content, toolbar: new { 
    entityId = Content.EntityId, 
    entityGuid = Content.EntityGuid, 
    entityTitle = Content.EntityTitle })
```
Note that for the delete-button to appear, the following conditions must be met:

1. It is _not_ an module-assigned item (also knows as content-mode), because in this case the item is in use, and it cannot be quick-deleted anyhow
2. It has an entityId - used to show to the user to help him be sure he's deleting the right item
3. It has an entityTitle - also shown to the user to be sure he's deleting the right thing
3. It has an entityGuid - used as the ID when deleting the data as an extra level of security. 


## Notes and Clarifications
requires id, title and guid. otherwise the button won't appear

## Read also

* [commands][commands] 

## Demo App and further links

todo

## History
1. Introduced in 2sxc v08.09

[commands]:Html-Js-Commands