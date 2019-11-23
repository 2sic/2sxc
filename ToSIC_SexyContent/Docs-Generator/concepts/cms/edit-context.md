---
uid: Concepts.EditContext
---
# Concept: In-Page Edit Context

To ensure that client-side commands like _edit_ can work (this includes all toolbar functionalities), the in-page scripts must pick up various things like AppId and more. This is provided as an **Edit-Context** and looks a bit like this:

```html
<div data-edit-context='{
  "Environment":{"WebsiteId":0,"WebsiteUrl":"//.../en/","PageId":56,"PageUrl":"http://.../en/","parameters":[{"Key":"TabId","Value":"56"},{"Key":"language","Value":"en-US"}],"InstanceId":421,"SxcVersion":"9.30.0.40333","SxcRootUrl":"/","IsEditable":true},
  "User":{"CanDesign":true,"CanDevelop":true},
  "Language":{"Current":"en-us","Primary":"en-us","All":[]},
  "ContentBlock":{"ShowTemplatePicker":true,"IsEntity":false,"VersioningRequirements":"DraftOptional","Id":421,"ParentFieldName":null,"ParentFieldSortOrder":0,"PartOfPage":true},
  "ContentGroup":{"IsCreated":true,"IsList":false,"TemplateId":3770,"QueryId":null,"ContentTypeName":"e2351b42-87f2-427e-9566-ff271e3e5a9f","AppUrl":"/Portals/0/2sxc/Content","AppSettingsId":null,"AppResourcesId":null,"IsContent":true,"HasContent":true,"SupportsAjax":true,"ZoneId":2,"AppId":2,"Guid":"c238e78b-a6e5-4811-a5c9-51d5ebf48b39","Id":3894},
  "error":{"type":null},
  "Ui":{"AutoToolbar":true}}'>
  <span>more stuff...</span>
</div>

```

## How the Edit-Context is Added

By default, it is automatically added if the system detects that the current user has edit-permissions. In 2sxc 9.30 a feature was added to add the context in code, using the [Edit.Enable(...)](razor-edit.enable) command.

## Inner-Context for Inner Content

When [inner content](concept-inner-content) is used, each block of inner-content will change the context, because it will have different IDs etc. So inner-content blocks will add their own context-attributes. See also [Edit.ContextAttributes](razor-edit.contextattributes).

## How JS Picks up the Context

In most cases the context is picked up automatically - like in such a code:

```html
<a onclick="$2sxc(this).run('edit', ...)">edit</a>
```

This kind of code traverses the HTML to look for the closes context-node, and uses it to figure out everything automatically.

The second auto-pickup method uses the module/content-block ID, like this:

```html
<a onclick="$2sxc(4203).run('edit', ...)">edit</a>
```

This will also find the context, but instead of traversing the DOM upwards, it will check all DOM objects on the page and find the appropriate one for this ID.

## What's in the Context

This information is just conceptual. _Do NOT try to access these values in your code, because they will change, and it's not part of any public API!_.

The concept contains things like:

1. versions (so that the UI can behave as needed and correctly load scripts with cache-breaking)
1. urls so the GUI can perform certain actions correctly
1. language information for the GUI
1. various IDs like the current zone/app, item-IDs, module-IDs etc.

## Using Edit Context in Your Code

You should not use this in your code, as it's not a public api and will change from time to time. 

## History

1. Introduced in 2sxc 1.0
1. constantly modified/extended in future versions
