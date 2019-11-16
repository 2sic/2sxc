# Toolbars in Angular (dnn-sxc-angular)
## Purpose / Description
dnn-sxc-angular provides  directives/components which allow to place toolbars in an Angular App. Prerequisite is an Angular App running with [dnn-sxc-angular](https://github.com/2sic/dnn-sxc-angular).

## How to use
1. In your `app.module.ts`, add an import to `ContentManagerModule` from `@2sic.com/dnn-sxc-angular` and add it to the imports array.
2. Place the toolbar directives/components in your templates. There are two different ways to place your toolbars, the tag-toolbar (default) and the inline toolbar, depending on the use case. In most cases, you will want to use the tag-toolbar; the inline toolbar is useful when you want to show the toolbar without the need to hover over the target element.

### Tag-Toolbar
To use the tag-toolbar, insert the [sxc-toolbar] directive where you want to place the toolbar, e.g.:
```html
<div *ngIf="!!reference" [sxc-toolbar]='{toolbar:{entityId:reference.Id}}'>
    ...
</div>
```
The object being passed to the attribute can contain a toolbar and a settings property, according to [HTML Toolbars and Buttons](https://github.com/2sic/2sxc/wiki/html-toolbars-and-buttons).

### Inline-Toolbar
Place the inline toolbar in your template, for example inside of a table cell:
```html
<td>
    <sxc-toolbar [config]="{toolbar:{entityId:reference.Id}}"></sxc-toolbar>
</td>
```
The config object passed to the component follows the same rules as the tag-toolbar (https://github.com/2sic/2sxc/wiki/html-toolbars-and-buttons).